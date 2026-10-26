# Integración SECAD - VMS ISS SecurOS — Caso de uso 1

**Desde un caso en despacho, el operador ve las cámaras más cercanas a la ubicación del caso y
abre el video en vivo dentro de SECAD.** Solo visualización: SECAD no mueve ni controla las cámaras.

Fuente técnica: *SecurOS REST API Programming Guide* (`SECADPLUS/SecurOS_REST_API_Programming_Guide_v27-Dec-21.pdf`)
y pruebas sobre el servidor SecurOS del proyecto.

## Cómo funciona

```
Despachador ──1. botón "Cámaras cercanas"──► SECAD (IntegracionVmsController)
                                               │ coordenadas del pedido (CADPEDIDO.LATITUD_CASO / LONGITUD_CASO)
                                               │ censo local CADVMS_CAMARA (se refresca desde SecurOS cada N minutos)
                                               ▼
                          lista de cámaras con distancia y miniatura (GET /api/v2/cameras/{id}/image)
Despachador ──2. clic en una cámara──► SECAD registra la visualización (CADVMS_VISUALIZACION)
                                       y entrega un token temporal (HMAC, 60 s, una sola cámara)
Navegador ──3. WebRTC (WHEP) + token──► Gateway MediaMTX ──pregunta a SECAD si el token vale──► SECAD
                                              │ RTSP :554 (solo mientras alguien mira)
                                              ▼
                                          SecurOS rtsp://…/live?id=<cámara>
```

- El video **no pasa por SECAD**: va de SecurOS al gateway y del gateway al navegador.
- Las credenciales de SecurOS están solo en el servidor de SECAD (REST API) y en el gateway (RTSP).
  Al navegador solo llega un token que sirve para una cámara durante 60 segundos.
- Si SecurOS no responde, SECAD sigue mostrando las cámaras del último censo guardado.

## Archivos

| Archivo | Qué hace |
|---|---|
| `SECADPLUS/Models/Vms/ClienteSecurOS.cs` | Cliente de la REST API de SecurOS (catálogo y miniaturas). Lee solo id, nombre, coordenadas, estado y si es PTZ |
| `SECADPLUS/Models/Vms/GestorIntegracionVms.cs` | Censo en Oracle, cámaras cercanas, auditoría y tokens de video |
| `SECADPLUS/Models/Vms/ModelosVms.cs` | Modelos de la integración |
| `SECADPLUS/Controllers/IntegracionVms/IntegracionVmsController.cs` | `CamarasCercanas`, `Miniatura`, `AbrirVideo`, `Sincronizar` y `AutorizarStream` (llamado por el gateway) |
| `SECADPLUS/Scripts/custom/IntegracionVms.js` | Ventana de cámaras cercanas y reproductor WebRTC (WHEP) |
| `SECADPLUS/Views/PendientesDespacho/Index.cshtml` | Botón de cámara junto al visor geográfico (solo si la integración está habilitada para el sitio) |
| `docs/sql/INTEGRACION_VMS_SECUROS.sql` | Tablas `CADVMS_CAMARA`, `CADVMS_VISUALIZACION` y su secuencia |
| `docs/vms/mediamtx.yml` | Configuración del gateway de video |

## Instalación

### 1. Base de datos
Ejecutar `docs/sql/INTEGRACION_VMS_SECUROS.sql` en el esquema `USR_CAD`.

### 2. SecurOS (lo configura el contratista)
1. Objeto **REST API** con HTTPS en el puerto 8888 (certificado asociado con `netsh http add sslcert`, sección 3 de la guía).
2. Objeto **RTSP Server** activo en el puerto 554, que devuelva la IP real del servidor (no `127.0.0.1`).
3. Objeto **Image Processor** (requerido para las miniaturas).
4. **Usuario técnico para SECAD** (no superusuario) con permiso **solo de ver** (*View*) sobre las cámaras.
5. Coordenadas (`latitude` / `longitude`) diligenciadas en cada cámara. Las que no las tengan no aparecen como cercanas.

### 3. Gateway de video (MediaMTX)
1. Servidor Windows o Linux en la red de SECAD, con ruta al puerto 554 de SecurOS y alcanzable por los puestos de los operadores.
2. Descargar MediaMTX v1.9 o superior y copiar `docs/vms/mediamtx.yml`, reemplazando los valores en mayúsculas.
3. Certificado TLS válido para los puestos (`server.crt` / `server.key`): los navegadores solo permiten WebRTC desde páginas seguras.
4. Firewall: TCP 8889 (WHEP) y UDP/TCP 8189 (media WebRTC) desde la red de operadores; TCP 554 hacia SecurOS.
5. `authHTTPAddress` debe apuntar a la ruta completa de SECAD, incluido el directorio virtual si existe
   (por ejemplo `https://secad/SECADPLUS/IntegracionVms/AutorizarStream?clave=…`).

### 4. Web.config de SECAD (solo en el servidor; las claves no se suben al repositorio)

| Clave | Valor |
|---|---|
| `VMS_Habilitado` | `SI` para activar; `NO` apaga el botón y las acciones |
| `VMS_SitioGraba` | Sitio de grabación al que pertenecen las cámaras |
| `VMS_SecurOS_Url` | `https://<servidor SecurOS>:8888` |
| `VMS_SecurOS_Usuario` / `VMS_SecurOS_Clave` | Usuario técnico de solo visualización |
| `VMS_MinutosSincronizacion` | Cada cuánto se refresca el censo (por defecto 10) |
| `VMS_RadioBusquedaMetros` / `VMS_CantidadCamaras` | Radio de búsqueda (1000 m) y cantidad máxima de cámaras (6) |
| `VMS_Gateway_Url` | `https://<gateway>:8889` |
| `VMS_Gateway_ClaveAutorizacion` | Clave compartida con `authHTTPAddress` del gateway (mínimo 16 caracteres) |
| `VMS_Gateway_IpPermitidas` | IP del gateway (recomendado), separadas por coma |
| `VMS_Token_Clave` | Clave aleatoria para firmar tokens (mínimo 32 caracteres) |
| `VMS_TokenSegundos` | Vigencia del token (60) |

### 5. Permisos en SECAD (Seguridad → Elementos y Perfiles)
Crear los elementos con estas rutas y asignarlos a los perfiles que correspondan:

| Ruta | Permite |
|---|---|
| `/IntegracionVms/CamarasCercanas` | Ver la lista de cámaras cercanas y sus miniaturas |
| `/IntegracionVms/AbrirVideo` | Abrir el video en vivo |
| `/IntegracionVms/Sincronizar` | Forzar la actualización del censo (administradores) |

`/IntegracionVms/AutorizarStream` no lleva permiso de usuario: solo lo llama el gateway, con la clave compartida.

## Pruebas de aceptación

1. Con `VMS_Habilitado=NO` el botón no aparece.
2. Un caso sin coordenadas muestra el aviso "El caso no tiene coordenadas".
3. Un caso con coordenadas lista las cámaras del radio ordenadas por distancia, con miniatura las que tienen señal.
4. Al hacer clic, el video arranca en menos de 3 segundos y queda una fila en `CADVMS_VISUALIZACION`.
5. Copiar la URL WHEP con el token y abrirla 2 minutos después: el gateway responde 401.
6. Cambiar el id de cámara en la URL con un token válido de otra cámara: 401.
7. Cerrar la ventana: en el log del gateway la lectura RTSP de esa cámara se cierra a los 15 segundos.
8. Apagar SecurOS: la lista sigue saliendo del censo guardado; las miniaturas y el video fallan con mensaje.

## Pendiente de confirmar con ISS

- Si el RTSP del puerto 554 exige usuario y clave (se configuran en `source` de `mediamtx.yml`).
- Sintaxis de `rtsp://…/archive?id=N` para video grabado por hora (fase posterior).
- Que la versión instalada (SecurOS 2026) mantiene los endpoints de la guía de 2021 usados aquí:
  `GET /api/v1/cameras`, `GET /api/v2/cameras/{id}/image`, `GET /api/v2/cameras/{id}/rtsp/live`.
