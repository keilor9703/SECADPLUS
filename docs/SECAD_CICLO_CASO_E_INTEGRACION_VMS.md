# SECADPLUS — Ciclo de vida del caso y puntos de integración del VMS ISS SecurOS

> Documento técnico de referencia. Describe cómo opera SECADPLUS (el SECAD que
> corre en la Policía Metropolitana de Pereira) desde que entra una llamada
> hasta que se cierra el caso, y dónde se deben hacer los cambios de código para
> integrar el VMS ISS SecurOS (Contrato 4363-2026, Oficio 009 UT Pereira Segura).
>
> Fuente: lectura del código de este repositorio (rama `main`, commit 86f12fe).
> La lógica de negocio pesada vive en paquetes Oracle (`PK_CADPLUS`, `PK_CAD`,
> `PK_CADINTEGMINUTASECADSIVICC`, `PK_CADVISOR`) cuyo código **no está en el
> repositorio**; lo que se dice de ellos se infiere de sus parámetros y de cómo los
> usa la aplicación.

---

## 1. Arquitectura en una línea

ASP.NET MVC 5 sobre .NET Framework 4.5, Oracle (EF6 + `GestorGeneral` que
ejecuta procedimientos almacenados), vistas Razor + jQuery, mapas con **ArcGIS
API for JavaScript 3.x** (3.20 a 3.36), notificaciones con **SignalR 2.2.2**.
**No hay Web API** (`System.Web.Http` no está configurado): todas las acciones son
controladores MVC que devuelven vistas o `JsonResult`.

| Capa | Dónde |
|---|---|
| Controladores de operación | `SECADPLUS/Controllers/Recepcion/`, `Controllers/Despachos/`, `Controllers/Minuta/` |
| "Gestores" (lógica de aplicación, llaman procedimientos) | `SECADPLUS/Models/Gestor*.cs` (`GestorRecepcion`, `GestorDespachos`, `GestorGeneral`, `GestorAuditoria`) |
| Entidades EF | `SECADEntities/SECADEntities/SecadEntities/` (`CADPEDIDO`, `CADPEDIDOS_CANALES`, `CADEVENTO`, `CADEVENTOMEDIO`, …) |
| JS de cada pantalla | `SECADPLUS/Scripts/custom/*.js` (`PendientesDespacho.js`, `ControlRecepcion.js`, …) |
| Visor geográfico | `Controllers/IngresarMapaController.cs` + `Views/IngresarMapa/Index.cshtml` + `Mapas/js/` |
| Seguridad (menús y permisos) | `Controllers/Seguridad/` + `Models/SecadPlusAutorizador.cs` |

---

## 2. Vocabulario del modelo de datos

| Concepto | Tabla | Llave | Qué es |
|---|---|---|---|
| **Pedido** (la llamada / el caso tal como se recibe) | `CADPEDIDO` | `SITIO_GRABA` + `NUME_LLAMADA` | Lo que crea Recepción. `NUME_LLAMADA` sale de `PK_CADPLUS.f_consultar_seq_pedido` |
| **Canal asignado al pedido** | `CADPEDIDOS_CANALES` | pedido + `CADCANA_FUERZ_ID` + `CADCANA_CODIGO` | A qué canal(es) de atención (fuerza + canal) va el pedido. Un pedido puede ir a varios canales |
| **Evento** | `CADEVENTO` | `SITIO_GRABA` + `NUME_EVENTO` | Lo que crea el despachador al tomar el pedido. Agrupa uno o más pedidos |
| **Evento activo del usuario** | `CADEVENTOACTIVO` | `SITIO_GRABA` + `USUA_USUARIO` | El evento que el despachador tiene abierto en su pantalla |
| **Medio en el evento** | `CADEVENTOMEDIOS` (+ `_HORASADIC`) | evento + medio | Patrulla/recurso asignado y sus horas |
| **Usuario SECAD** | `CADUSUARIO` | `USUARIO` | Trae `SITIO_GRABACION`, `CADCANA_FUERZA_ID`, `CADCANA_CODIGO` (canal del puesto), `CONDICION` |
| **Sitio de grabación** | `CADSITIOS_GRABACION` | `CONSECUTIVO` | La sala / CAD (Pereira es un sitio) |

Campos clave de `CADPEDIDO`: `CODI_PEDIDO` (tipificación del caso), `CODI_PEDIDO_REAL`,
`TIPO_PEDIDO`, `CALI_PEDIDO`, `PRIORIDAD`, `IMPORTANCIA`, `DIRE_CASO`, `LUGE_CODIGO`
(municipio), `CODI_BARRIO`/`LUGE_BARRIO`, `PUCA_CODIGO` (cuadrante),
`LATITUD_CASO`/`LONGITUD_CASO` (texto), `COMENTARIO`, `NUME_TELEFONO`,
`NOMB_LLAMANTE`, `ESTADO`, `ENVIAR`, `CADUSUA_USUARIO`.

Estados observados:

| Objeto | Valor | Significado (por uso en el código) |
|---|---|---|
| Pedido / pedido-canal | `S` | Recién asignado al canal, sin enviar |
| | `P` | Enviado a despacho (pendiente) |
| | `E` | En evento (el despachador lo tomó) |
| | `T` | En seguimiento |
| | `I` | Informativo |
| | `R` | Pendiente de revisión |
| | `C` | Cerrado |
| Evento | `C` | Cerrado |

---

## 3. Ciclo de vida del caso, paso a paso

```
RECEPCIÓN                                   DESPACHO                                  CIERRE
─────────                                   ────────                                  ──────
1 Abrir pantalla ─► consecutivo           5 Bandeja del canal (cada 20 s)          9 Cerrar evento
2 Ubicar y tipificar ─► CrearPedido       6 Despachar ─► p_crear_evento              (codificación final,
3 Asignar canal(es) ─► AsignarCanal       7 Asignar patrulla ─► p_insertar_evento_medio  minuta, coordenadas)
4 Enviar ─► EnviarLlamada                 8 Horas ruta/llegada/termina, anotaciones
```

### 3.1 Recepción (`RecepcionController`, `GestorRecepcion`)

1. **Abrir la recepción.** `Recepcion()` arma el modelo con el usuario (`SITIO_GRABA`,
   ACD, máquina identificada) y pide el consecutivo `PK_CADPLUS.f_consultar_seq_pedido`
   (`GestorRecepcion.ObtenerConsecutivoLlamada`). Datos del abonado:
   `PK_CAD.P_DATOS_TELEFONO`. Ubicación por geocodificación: `PK_CADVISOR.P_GETXYGMLCF`.
   Opcional: ubicación del celular por RapidSOS (`ObtenerUbicacionCelular`).
2. **Crear el pedido.** `CrearPedido` (POST, `[Authorize]`) inserta `CADPEDIDO` con EF
   (`contextoRecepcion.CADPEDIDOS.Add` + `SaveChanges`). Fija `FECHA_GRABACION` con la hora
   del servidor. `ActualizarPedido` lo modifica y deja auditoría (`GestorAuditoria`).
3. **Asignar canales.** El operador elige el/los canales de atención en una lista
   (`ConsultarCanales` → `PK_CADPLUS.p_canales_atencion` / `p_canales_ponal`) y
   `AsignarCanal` inserta `CADPEDIDOS_CANALES` con `ESTADO = 'S'`. **La elección del canal
   es manual; no hay enrutamiento geográfico automático en el código.**
4. **Enviar a despacho.** `EnviarPedido` → `GestorRecepcion.EnviarLlamada`:
   - Si `CODI_PEDIDO = '900'` (informativo) el caso se cierra de una vez:
     `PK_CADINTEGMINUTASECADSIVICC.p_cambio_estado_caso_new` con estado `C`, enviar `N`.
   - Si no: mismo procedimiento con estado `P`, enviar `S`.
   - Siempre: `PK_CAD.p_cambio_estado_pedido_canal` (`P`/`S`) para los canales asignados.
   - Consulta la fuerza destino (`p_obtener_fuerza`). **Si la fuerza es `64570007`, SECAD
     llama a la PIP** (`https://catalogoservicioweb.policia.gov.co/WsIntegracionSecadMeper/api/Secad/ValEvento`)
     y deja traza en `PK_INTEGRACION_SALUD.P_CADLOGHISTWS`. Es la integración externa que
     ya existe para la MEPER y el precedente directo de la integración del VMS.

### 3.2 Despacho (`PendientesDespachoController`, `GestorDespachos`, `Scripts/custom/PendientesDespacho.js`)

5. **Bandeja del canal.** El despachador solo ve los pedidos de **su** canal
   (`usr.CADCANA_FUERZA_ID` + `usr.CADCANA_CODIGO`). `VerRecibidas` →
   `PK_CADPLUS.p_lista_pedidospend_canal` con filtro por estado (`'P','S','E'`, `'T'`, `'I'`,
   `'R'`). La pantalla refresca con `setInterval(ActivarTimer, 20000)` (cada 20 s).
6. **Despachar (crear evento).** `ProcesarEvento` → `GestorDespachos.EjecutarCrearEvento`
   → `PK_CADPLUS.p_crear_evento(sitio, identificación, nume_llamada, fuerza, canal,
   codi_pedido, clase_turno, usuario)` → devuelve `p_evento`, `p_control_crear`, `p_texto`.
   El pedido y el pedido-canal pasan a `E`; queda el evento en `CADEVENTOACTIVO` del usuario.
   Asociar otras llamadas al mismo evento: `VerAsociarLlamadas` / `p_lista_asignar_llamadas`.
7. **Asignar medios (patrullas).** `VerMediosDisponibles` (`p_lista_medios_disponibles`) y
   `ProcesarMedioCaso` → `PK_CADPLUS.p_insertar_evento_medio`.
8. **Atención.** Horas por medio: `ProcesarHorasEventoMedios` →
   `p_insertar_horas_evento_medio` con `HORA_RUTA`, `HORA_LLEGA`, `HORA_TERMINA`
   (y `_CONTROL`). Anotaciones (`CADANOTACIONESRECURSOS`, `p_anotapatrulla`), novedades
   (`p_actualizar_novedad`), corrección de dirección/barrio (`p_actualizar_dire_barrio`),
   seguimiento (`p_actualizar_en_seguimiento`), **reclasificación** del caso
   (`AsignarCasoReal`: cambia `CODI_PEDIDO` y deja la observación en `COMENTARIO`).

### 3.3 Cierre

9. **Cerrar.** `ProcesarCerrarEvento` → `PK_CADPLUS.p_cerrar_evento(caso_final, sitio,
   evento)`; luego `DbMinuta.Ins_Coordenadas` (lat/long, modalidad, conducta, tipo de arma,
   tipo de movilización) y `DbMinuta.update_evento`. Evento, medios, pedido-canal y pedido
   quedan en `C`, con auditoría de cada uno.

### 3.4 Transversales

- **Permisos.** Cada acción protegida usa `[SecadPlusAutorizador("/Controlador/Accion")]`
  (211 usos), que consulta `PK_CADPLUS.p_obtener_permisos(ruta, usuario, permiso)`.
  Los menús se arman en `HomeController` con elementos `MODULO` → `MENU` → `OPCION`
  (`CADNETELEMENTO`, administrados en `Controllers/Seguridad/`: Elementos, Perfiles, Roles).
- **Interruptores por sitio.** `validar.VerificarConexionesPedidos("CONEXION_MAPA_RECEPCION",
  sitio, "01")` y similares leen referencias SECAD por sitio. Sirve para activar o apagar
  un módulo por CAD sin desplegar.
- **Auditoría.** `GestorAuditoria.RegistrarAuditoria<T>(entidad, tabla, llave, tipo)`.
- **Mapa.** `IngresarMapaController` es el visor ArcGIS 3.x: Despacho lo abre con
  `Index(strOrigen = "DESPACHOS")`, Recepción usa `MapaRecepcion` y el mapa general `Index`;
  `ConsultarCasosMapa` / `ConsultarPedidoMapa` traen los casos.

---

## 4. Integración VMS SecurOS: qué cambia y dónde

Principio: **el VMS nunca toca la base**. Los casos que nacen de una alarma del VMS deben
recorrer el mismo camino que un pedido de Recepción (pasos 2 a 4), para que aparezcan en
la bandeja del canal y se despachen como cualquier otro.

### 4.1 Entrada de alarmas (VMS → SECAD), vía PIP

| Pieza | Dónde | Qué hace |
|---|---|---|
| Servicio publicado en la PIP | Servicio nuevo, mismo modelo que `WsIntegracionSecadMeper` | Recibe la alarma (LPR / reconocimiento), autentica al middleware, valida e invoca el caso de uso de SECAD |
| Caso de uso "crear pedido desde alarma" | Nuevo `Models/GestorIntegracionVms.cs` | Reproduce los pasos 2-4: consecutivo (`f_consultar_seq_pedido`) → `CADPEDIDO` (tipificación homologada, `DIRE_CASO`, `LATITUD_CASO`/`LONGITUD_CASO` de la cámara, `COMENTARIO` con placa, confianza, cámara, enlace a evidencia) → `CADPEDIDOS_CANALES` con el **canal de la cámara** → `p_cambio_estado_caso_new` (`P`,`S`) + `p_cambio_estado_pedido_canal` |
| Idempotencia y traza | Tabla nueva `CADVMS_ALARMA` (id alarma VMS, cámara, fecha, sitio, `NUME_LLAMADA` creado, resultado) | Un reintento con el mismo id devuelve el mismo pedido |
| Homologación | Referencias SECAD (`CADREFERENCIASSECAD`) o tabla nueva | Tipo de alarma → `CODI_PEDIDO`, `PRIORIDAD`, `TIPO_PEDIDO` |
| Usuario técnico | `CADUSUARIO` de servicio (p. ej. `VMS_SECUROS`) | Va en `CADUSUA_USUARIO` para trazabilidad |

Pendiente de decidir con OFTIC: si el servicio de la PIP llama a SECADPLUS (que hoy no
tiene Web API; habría que agregar un controlador JSON con autenticación por credencial
de servicio) o si la PIP invoca directamente el procedimiento homologado por OFTIC.
En ambos casos **el contratista solo ve el servicio de la PIP**.

### 4.2 Cámaras y video (SECAD → VMS)

> **Implementado (caso de uso 1):** ver `docs/vms/INTEGRACION_VMS_SECUROS.md`. El video se entrega por
> RTSP de SecurOS convertido a WebRTC por un gateway MediaMTX; el censo vive en `CADVMS_CAMARA`.

| Pieza | Dónde |
|---|---|
| Driver SecurOS (cliente HTTP de la API de ISS) | Nuevo `Models/Vms/DriverSecurOS.cs` detrás de una interfaz `IVmsDriver` (catálogo, URL de video, PTZ, grabación) |
| Censo de cámaras | Tabla nueva `CADVMS_CAMARA` (id VMS, nombre, lat, long, estado, PTZ, **fuerza + canal de despacho**, sitio, activa) + sincronización periódica |
| Cámaras cercanas al caso | Nuevo `Controllers/IntegracionVmsController.cs` → `CamarasCercanas(sitio, nume_llamada, n)` usando `LATITUD_CASO`/`LONGITUD_CASO` |
| Enlace temporal de video + bitácora | Mismo controlador → `AbrirVideo(idCamara, nume_llamada)`; registra quién, qué cámara, cuándo, qué caso (tabla `CADVMS_VISUALIZACION`) antes de devolver la URL |
| Capa de cámaras en el mapa | `Views/IngresarMapa/Index.cshtml` + `Mapas/js/` (ArcGIS 3.x `GraphicsLayer`) en `Index` (despacho y mapa general) y `MapaRecepcion` |
| Botón "Cámaras cercanas" en el caso | `Views/PendientesDespacho/Index.cshtml` + `Scripts/custom/PendientesDespacho.js` (y la consulta de pedidos en Recepción) |
| Permisos | Elementos nuevos en Seguridad (`MENU`/`OPCION`) + `[SecadPlusAutorizador("/IntegracionVms/…")]`: ver mapa general, ver cámaras de su canal, ver sin PTZ, ver con PTZ |
| Interruptor | Referencia `CONEXION_VMS` por sitio, leída con `VerificarConexionesPedidos` |
| Configuración y secretos | `Web.config` (`appSettings` del VMS); credenciales fuera del repositorio |

### 4.3 Riesgos a tener en cuenta en el código actual

- Las consultas armadas concatenando texto (`REGEXP_LIKE(..., '" + valor + "')`) no deben
  replicarse en el código nuevo: usar parámetros.
- `EnviarPedido` y `ObtenerUbicacionCelular` tienen `[AllowAnonymous]`.
- `Web.config` versionado con cadenas de conexión y secretos reales.
- La lógica de `p_crear_evento`, `p_cerrar_evento` y `p_cambio_estado_caso_new` está en
  Oracle; antes de construir la entrada de alarmas hay que obtener de OFTIC el código de
  esos paquetes para confirmar los efectos laterales (bitácoras, notificaciones, minuta).
