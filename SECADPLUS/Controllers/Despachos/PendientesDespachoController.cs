using Oracle.ManagedDataAccess.Client;
using SecadDatos.Areas.Despacho;
using SecadDatos.Areas.MinutaSivicc;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class PendientesDespachoController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private CADUSUARIO usuarioActual;
        private GestorDespachos despacho = new GestorDespachos();
        private Validador validar = new Validador();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities contextoDespacho = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private CADUSUARIO usr;
        // GET: PendientesDespacho
        [SecadPlusAutorizador("/PendientesDespacho/Index")]
        public ActionResult Index(string strCerrado = null)
        {
            try
            {
                //Cargar DropDownList IJ. DANIEL RUIZ

                var list = new DbMinuta().F_GetModalidad().ToList();
                list = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.ModalidadId = new SelectList(new DbMinuta().F_GetModalidad(), "ReferenciaId", "Descripcion");

                var listC = new DbMinuta().F_GetConductas().ToList();
                listC = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.ConductaId = new SelectList(new DbMinuta().F_GetConductas(), "ReferenciaId", "Descripcion");

                var listG = new DbMinuta().F_GetTipoArma().ToList();
                listG = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.TipoArmId = new SelectList(new DbMinuta().F_GetTipoArma(), "ReferenciaId", "Descripcion");

                var listT = new DbMinuta().F_GetTipoMoviliza().ToList();
                listT = list.OrderBy(r => r.Descripcion).ToList();
                ViewBag.TipoMovilizaId = new SelectList(new DbMinuta().F_GetTipoMoviliza(), "ReferenciaId", "Descripcion");

                string strAbreviatura = "";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();
                if (usr == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validar.IdentificarCliente(Request, Session))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")";
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (strCerrado != null)
                {
                    ViewBag.MENSAJE_EXITOSO = strCerrado;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }


                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.

                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalIdentificacion = usr.IDENTIFICACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;
                ViewBag.globalCanal = usr.CADCANA_CODIGO;
                ViewBag.globalCondicion = usr.CONDICION;
                ViewBag.globalUsuario = usr.USUARIO;
                ViewBag.ES_MUJER = 0;
                ViewBag.SALUD = 0;

                #region Modificaciones Daniel

                var sitioGraba = from e in db.CADSITIOS_GRABACION
                                 where e.CONSECUTIVO == usr.SITIO_GRABACION
                                 select new
                                 {
                                     DESCRIPCION = e.DESCRIPCION
                                 };

                ViewBag.DescSitioGraba = sitioGraba.Select(c => c.DESCRIPCION).FirstOrDefault().ToString();

                #endregion Modificaciones Daniel

                var idFuerza = usr.CADCANA_FUERZA_ID;
                var idCanal = usr.CADCANA_CODIGO;

                //Revisar si es necesario canal asignado
                if (idCanal == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_CANAL_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.idFuerza = idFuerza;
                ViewBag.idCanal = idCanal;

                var fuerza = from e in db.CADFUERZAS
                             where e.ID == idFuerza
                             select new
                             {
                                 DESCRIPCION = e.DESCRIPCION,
                                 ABREVIATURA = e.ABREVIATURA
                             };

                ViewBag.FUERZA_DESCRIPCION = fuerza.Select(c => c.DESCRIPCION);
                ViewBag.FUERZA_ABREVIATURA = fuerza.Select(c => c.ABREVIATURA);

                ViewBag.DescFuerza = fuerza.Select(c => c.DESCRIPCION).FirstOrDefault().ToString();

                var canales = from e in db.CADCANALES
                              where e.CADFUERZ_ID == idFuerza && e.CODIGO == idCanal
                              select new
                              {
                                  CLASE_TURNO = e.CLASE_TURNO,
                                  DESCRIPCION = e.DESCRIPCION,
                                  ABREVIATURA = e.ABREVIATURA
                              };
                ViewBag.DescCanal = canales.Select(c => c.DESCRIPCION).FirstOrDefault().ToString();
                strAbreviatura = canales.Select(c => c.ABREVIATURA).FirstOrDefault();
                switch (strAbreviatura)
                {
                    case "FD2": //Eventos Mujer
                        ViewBag.ES_MUJER = 1;
                        break;

                    case "FD3"://Eventos Salud
                        ViewBag.SALUD = 1;
                        break;

                    case "FD4"://Eventos Salud
                        ViewBag.SOCIAL = 1;
                        break;
                }

                if (canales != null)
                {
                    ViewBag.CANAL_DESCRIPCION = canales.Select(c => c.DESCRIPCION);
                    ViewBag.globalClaseTurno = canales.Select(c => c.CLASE_TURNO).FirstOrDefault();
                }
                else
                {
                    ViewBag.CLASE_TURNO = "99"; // Se asigna como administrador
                    ViewBag.globalClaseTurno = "99";
                }
                string strFuerza = Convert.ToString(idFuerza);
                var tiempoRefresco = db.CADREFERENCIAS.Where(c => c.NOMBRE == "TIEMPO_REFRESCO_EVENTOS" && c.CODIGO == strFuerza).Select(c => c.ABREVIATURA).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoRefresco))
                {
                    tiempoRefresco = "15000"; // 15 segundos por defecto

                    ViewBag.MENSAJE_EXITOSO = "Revise en cadreferenciassecad el valor para TIEMPO_REFRESCO_EVENTOS.. Se establecen por defecto 15 segundos..";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }

                ViewBag.tiempoRefresco = int.Parse(tiempoRefresco);
                ViewBag.FECHA_GRABACION = DateTime.Now;
                ViewBag.USUA_USUARIO = usr.USUARIO;
                ViewBag.CONSECUTIVO = 0;
                ViewBag.globalnumeEvento = contextoDespacho.CADEVENTOACTIVOes.Where(c => c.SITIO_GRABA == usr.SITIO_GRABACION && c.USUA_USUARIO == usr.USUARIO).Select(c => c.NUME_EVENTO).FirstOrDefault();

                ViewBag.CADCLCI_CODIGO = new SelectList(contextoDespacho.CADCLASIFICACION_CIERRE.Where(c => c.ID_FUERZA == idFuerza).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.NOVEDAD = new SelectList(new[] { new { value = "S" , text = "TIENE NOVEDAD" },
                                                         new { value = "N" , text = "SIN NOVEDAD" },
                                                         new { value = "D" , text = "DESASIGNACION"} }, "value", "text");

                var tiempoLlega = db.CADREFERENCIAS.Where(c => c.NOMBRE == "MINUTOS_LLEGA" && c.CODIGO == "01").Select(c => c.DESCRIPCION).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tiempoLlega))
                    ViewBag.tiempoLlega = 5; // 5 Minutos por defecto
                else
                    ViewBag.tiempoLlega = tiempoLlega;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]

        public ActionResult MapaXcrime()
        {

            usr = validar.VerificarUsuario();
            if (usr == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var idFuerza = usr.CADCANA_FUERZA_ID;
            var idCanal = usr.CADCANA_CODIGO;
            var GetHash = new DbDespacho().F_HashMapa(Convert.ToInt64(idFuerza), Convert.ToInt64(idCanal)).ToList();

            ViewBag.filePath = GetHash[0].Hash;

            return View();
        }

        [Authorize]
        public JsonResult AsignarHoraServidor()
        {
            try
            {
                GestorGeneral gestorGeneral = new GestorGeneral();
                var fecha = gestorGeneral.ObtenerTiempoServidor?? DateTime.Now;

                var hora = fecha.ToString("HH:mm:ss");

                var resultados = Json(new { result = true, datos = hora });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerRecibidas(int idFuerza, int idCanal, string opcion)
        {
            try
            {
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();

                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    validar.IdentificarCliente(Request, Session);

                }

                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") ";
                string strLimite = "";
                string strBoton = "<button class='btn bg-teal btn-mixed' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para despachar' " +
                                         " onclick='CargarEventos(NUME_LLAMADA, \"ESTADO_CANAL\", \"CODI_PEDIDO\")'>" +
                                        "<i class='icon-power2'></i>" +
                                  "</button>";
                string strBotonDestino = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                GestorDespachos gestorDespachos = new GestorDespachos();
                DataTable dttLugar;
                List<_PedidosPendientesCanalModel> listaRecibidas = new List<_PedidosPendientesCanalModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (NUME_LLAMADA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (HORA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (D_ESTADO_CANAL, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (DSP_RECURSO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (CNT_EVENTOS, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (CNT_RECURSOS, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (CODIGO1, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (MPIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZ_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND ESTADO_CANAL IN (" + opcion + ") AND REGEXP_LIKE (PRIORIDAD, '" + parametros.ConsultaValor + "', 'i')) ";
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedidospend_canal", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("NUME_LLAMADA", fila["NUME_LLAMADA"].ToString());
                    strBotonDestino = strBotonDestino.Replace("ESTADO_CANAL", fila["ESTADO_CANAL"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CODI_PEDIDO", fila["CODI_PEDIDO"].ToString());

                    listaRecibidas.Add(new _PedidosPendientesCanalModel("", fila["NUME_LLAMADA"].ToString(), strBotonDestino, fila["HORA"].ToString(), fila["TIEMPO_DESPACHO"].ToString(), fila["NUME_TELEFONO"].ToString(),
                                                                        fila["ESTADO_CANAL"].ToString(), fila["D_ESTADO_CANAL"].ToString(), fila["DSP_RECURSO"].ToString(), fila["CNT_EVENTOS"].ToString(),
                                                                        fila["CNT_RECURSOS"].ToString(), fila["CODIGO1"].ToString(), fila["MPIO"].ToString(),
                                                                        fila["DIRE_CASO"].ToString(), fila["BARRIO"].ToString(), fila["PRIORIDAD"].ToString(), fila["COMENTARIO"].ToString(), fila["RECEPTOR"].ToString(), fila["MINUTOS_DESPACHO"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRecibidas });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerRecepcion()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "";
                string strLimite = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_PedidosEfectivosRecepcionModel> listaRecepcion = new List<_PedidosEfectivosRecepcionModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " WHERE REGEXP_LIKE (NUME_LLAMADA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (HORA_CASO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (MPIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CODIGO1, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (COMUNA, '" + parametros.ConsultaValor + "', 'i') ";
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedido_enrecepcion", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaRecepcion.Add(new _PedidosEfectivosRecepcionModel(fila["NUME_LLAMADA"].ToString(), fila["HORA_CASO"].ToString(), fila["NUME_TELEFONO"].ToString(), fila["MPIO"].ToString(),
                                                                           fila["DIRE_CASO"].ToString(), fila["CODIGO1"].ToString(), fila["BARRIO"].ToString(), fila["COMUNA"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRecepcion });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarEvento(string[] parametros)
        {
            try
            {
                var idSitio = int.Parse(parametros[0]);
                var usuario = parametros[7];

                GestorDespachos gestor = new GestorDespachos();
                var resultados = gestor.EjecutarCrearEvento(int.Parse(parametros[0]), parametros[1], decimal.Parse(parametros[2]), int.Parse(parametros[3]), int.Parse(parametros[4]), (parametros[5]), parametros[6], parametros[7]);

                var evento = resultados.Item1;
                var controlCrear = resultados.Item2;
                var mensaje = resultados.Item3;

                if (string.IsNullOrWhiteSpace(mensaje) || mensaje.ToUpper() == "NULL")
                {
                    //CADPEDIDOS_CANALES, CADPEDIDOS mappear
                    CADPEDIDOS_CANALES cADPEDIDOS_CANALES = new CADPEDIDOS_CANALES();

                    cADPEDIDOS_CANALES.CADEVEN_SITIO_GRABA = idSitio;
                    cADPEDIDOS_CANALES.CADEVEN_NUME_EVENTO = evento;
                    cADPEDIDOS_CANALES.ESTADO = "E";
                    cADPEDIDOS_CANALES.CADPEDI_SITIOGRABA = idSitio;
                    cADPEDIDOS_CANALES.CADPEDI_NUMELLAMADA = int.Parse(parametros[2]);
                    cADPEDIDOS_CANALES.CADCANA_FUERZ_ID = int.Parse(parametros[3]);
                    cADPEDIDOS_CANALES.CADCANA_CODIGO = int.Parse(parametros[4]);

                    auditoria.RegistrarAuditoria<CADPEDIDOS_CANALES>(cADPEDIDOS_CANALES, "CADPEDIDOS_CANALES", cADPEDIDOS_CANALES.CADPEDI_NUMELLAMADA.ToString(), TipoAuditoria.Modificar);

                    CADPEDIDO cADPEDIDO = new CADPEDIDO();

                    cADPEDIDO.ESTADO = "E";
                    cADPEDIDO.SITIO_GRABA = idSitio;
                    cADPEDIDO.NUME_LLAMADA = long.Parse(parametros[2]);

                    auditoria.RegistrarAuditoria<CADPEDIDO>(cADPEDIDO, "CADPEDIDO", cADPEDIDO.NUME_LLAMADA.ToString(), TipoAuditoria.Modificar);
                }

                else if (mensaje == "Transaccion no exitosa al Insertar registro en la tabla de Evento Activo..Contactese con su Amon.")
                {
                    CADEVENTOACTIVO cADEVENTOACTIVO = new CADEVENTOACTIVO();

                    cADEVENTOACTIVO.NUME_EVENTO = evento;
                    cADEVENTOACTIVO.USUA_USUARIO = usuario;
                    cADEVENTOACTIVO.SITIO_GRABA = idSitio;

                    auditoria.RegistrarAuditoria<CADEVENTOACTIVO>(cADEVENTOACTIVO, "CADEVENTOACTIVO", cADEVENTOACTIVO.NUME_EVENTO.ToString(), TipoAuditoria.Modificar);
                }

                if (controlCrear == 0)
                {
                    //ir a cambio de evento
                    //var numeEvento = gestor.ConsultarNumeroEvento(int.Parse(parametros[0]), parametros[7]);

                    var numeEvento = contextoDespacho.CADEVENTOACTIVOes.Where(c => c.SITIO_GRABA == idSitio && c.USUA_USUARIO == usuario).Select(c => c.NUME_EVENTO).FirstOrDefault();

                    if (numeEvento == 0)
                    {
                        return Json(new { result = false, datos = "No hay eventos activos para: " + parametros[0] + " - " + parametros[7] });
                    }

                    return Json(new { result = true, datos = numeEvento });
                }
                else
                {
                    return Json(new { result = false, datos = mensaje });
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ConsultarTurno(int idSitio, int idTurno)
        {
            try
            {
                var turno = db.CADREFERENCIASSECADs.SqlQuery("Select b.* " +
                                                             "From CADREFERENCIASSECAD b " +
                                                             "Inner Join CADTURNOS a " +
                                                             "On b.CODIGO = a.TIPO_TURNO " +
                                                             "Where a.sitio_graba = " + idSitio + " " +
                                                             "And a.turno = " + idTurno + " " +
                                                             "And b.nombre = 'TURNO_VIGILANCIA'").Select(c => c.DESCRIPCION).FirstOrDefault();

                var resultados = Json(new { result = true, datos = turno });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerEventos(int idSitio, long numeEvento, int idFuerza, int idCanal)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " ";
                string strLimite = "";
                string strBoton = "<button class='btn bg-teal btn-mixed' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para ver' " +
                                         " onclick='CargarCamposPedido(NUME_EVENTO, CADPEDCA_NUMELLAMADA, \"ESTADO_EVENTO\")'>" +
                                        "<i class='icon-eye'></i>" +
                                  "</button>";
                string strBotonDestino = "";
                bool blFiltro = false, blControl = false;
                double dblControl = 0;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_EventosModel> listaEventos = new List<_EventosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);

                    if (blControl)
                    {
                        strQuery += " AND (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (NUME_EVENTO, '" + parametros.ConsultaValor + "', 'i')) ";
                    }
                    else
                    {
                        strQuery += " AND (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (ESTADO_EVENTO, SUBSTR('" + parametros.ConsultaValor + "', 1, 1), 'i')) " +
                                   " OR (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (FECH_CREA, '" + parametros.ConsultaValor + "', 'i')) " +
                                   " OR (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (CADUSUA_EVEN_GENERA, '" + parametros.ConsultaValor + "', 'i')) ";
                    }

                    //strQuery += " AND (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (NUME_EVENTO, '" + parametros.ConsultaValor + "', 'i')) " +
                    //           " OR (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (ESTADO_EVENTO, SUBSTR('" + parametros.ConsultaValor + "', 1, 1), 'i')) " +
                    //           " OR (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (FECH_CREA, '" + parametros.ConsultaValor + "', 'i')) " +
                    //           " OR (SITIO_GRABA = " + idSitio + " AND NUME_EVENTO = DECODE(" + numeEvento + ", 0, NUME_EVENTO, " + numeEvento + ") AND CADPEDCA_FUERZA_ID = " + idFuerza + " and CADPEDCA_CANAL = " + idCanal + " AND REGEXP_LIKE (CADUSUA_EVEN_GENERA, '" + parametros.ConsultaValor + "', 'i')) ";
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_eventos", strConexion, "Nonquery");
                string nro_evento = "";
                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("NUME_EVENTO", fila["NUME_EVENTO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CADPEDCA_NUMELLAMADA", fila["CADPEDCA_NUMELLAMADA"].ToString());
                    strBotonDestino = strBotonDestino.Replace("ESTADO_EVENTO", fila["ESTADO_EVENTO"].ToString());

                    listaEventos.Add(new _EventosModel(strBotonDestino, fila["NUME_EVENTO"].ToString(), fila["ESTADO_EVENTO"].ToString(), fila["FECH_CREA"].ToString(),
                                                                fila["CADUSUA_EVEN_GENERA"].ToString(), fila["CADTURN_TURNO"].ToString(), fila["CADPEDCA_NUMELLAMADA"].ToString(), fila["CADCLCI_CODIGO"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    nro_evento = fila["NUME_EVENTO"].ToString();
                    Session["EVENTO_RMR"] = nro_evento;
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        //public JsonResult VerPedidoEvento(int idSitio, int numeLlamada) cambio rmr 08-10-2020 08:50 am
        public JsonResult VerPedidoEvento(int idSitio, int numeLlamada, int numeEvento)
        {
            try
            {
                string strQuery = " WHERE t.sitio_graba = " + idSitio + " AND t.nume_llamada = " + numeLlamada + " ";
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_PedidosLlamadaModel> listaPedidosLlamada = new List<_PedidosLlamadaModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_pedidos_llamada", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaPedidosLlamada.Add(new _PedidosLlamadaModel(fila["SITIO_GRABA"].ToString(), fila["HORA_CASO"].ToString(), fila["NUME_LLAMADA"].ToString(), fila["CADUSUA_USUARIO"].ToString(),
                                                                     fila["NUME_TELEFONO"].ToString(), fila["DISP_TELEFONICO"].ToString(), fila["PROP_TELEFONO"].ToString(),
                                                                     fila["DIRE_TELEFONO"].ToString(), fila["LUGE_TELEFONO"].ToString(), fila["BARR_TELEFONO"].ToString(),
                                                                     fila["NOMB_LLAMANTE"].ToString(), fila["DIRE_LLAMANTE"].ToString(), fila["TIPO_INFORMACION"].ToString(),
                                                                     fila["CODIGO_COOPERANTE"].ToString(), fila["CADPEDI_NUME_LLAMADA"].ToString(), fila["COMENTARIO"].ToString(),
                                                                     fila["CODI_PEDIDO"].ToString(), fila["DESC_CODI_PEDIDO"].ToString(), fila["CODI_PEDIDO2"].ToString(),
                                                                     fila["DESC_CODI_PEDIDO2"].ToString(), fila["DIRE_CASO"].ToString(), fila["DSP_BARRIO"].ToString(),
                                                                     fila["DSP_CIUDAD"].ToString(), fila["LUGE_CODIGO"].ToString(), fila["CODI_PEDIDO_REAL"].ToString(), fila["DESC_CODI_PEDIDO_REAL"].ToString()));
                }

                //rmr 02-10-2020

                string strQuery2 = " WHERE CD.CADPEDI_SITIOGRABA = " + idSitio + " AND CD.CADPEDI_NUMELLAMADA = " + numeLlamada + " ";
                GestorAdministrador gestor2;
                DataTable dttLugar2;
                List<_PedidosDatosAdicModel> listaPedidosLlamada2 = new List<_PedidosDatosAdicModel>();
                GestorGeneral objGestorGral2 = new GestorGeneral();
                gestor2 = new GestorAdministrador();

                objGestorGral2.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery2, typeof(string)));
                objGestorGral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar2 = objGestorGral2.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.P_LISTA_PEDIDOS_DATOS_ADIC", strConexion, "Nonquery");

                foreach (DataRow fila2 in dttLugar2.Rows)
                {
                    listaPedidosLlamada2.Add(new _PedidosDatosAdicModel(fila2["CADPEDI_SITIOGRABA"].ToString(), fila2["CADPEDI_NUMELLAMADA"].ToString(), fila2["X_COORD"].ToString(), fila2["Y_COORD"].ToString(),
                                                                     fila2["MODALIDADCASO"].ToString(), fila2["CONDUCTAESPECIAL"].ToString(), fila2["ARMAMEDIO"].ToString(),
                                                                     fila2["TIPOMOVILIZAGRESOR"].ToString()));
                }

                // fin rmr 02-10-2020

                //rmr 08-10-2020
                GestorAdministrador gestor3;
                DataTable dttLugar3;
                List<_PedidosDatoseventos> listaPedidosLlamada3 = new List<_PedidosDatoseventos>();
                GestorGeneral objGestorGral3 = new GestorGeneral();
                gestor3 = new GestorAdministrador();
                objGestorGral3.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", idSitio.ToString(), typeof(int)));
                objGestorGral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", numeEvento.ToString(), typeof(int)));
                objGestorGral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttLugar3 = objGestorGral3.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.P_EST_EVENTO", strConexion, "Nonquery");
                foreach (DataRow fila3 in dttLugar3.Rows)
                {
                    listaPedidosLlamada3.Add(new _PedidosDatoseventos(fila3["SITIO_GRABA"].ToString(), fila3["NUME_EVENTO"].ToString(), fila3["CADCODI_FINAL"].ToString(),
                                                                     fila3["CADCLCI_CODIGO"].ToString()));
                }
                // fin rmr 08-10-2020
                // //var resultados = Json(new { result = true, lista = listaPedidosLlamada }); antes cambio RMR
                var resultados = Json(new { result = true, lista = listaPedidosLlamada, lista2 = listaPedidosLlamada2, lista3 = listaPedidosLlamada3 });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult VerComentariosAdicionales(int idSitio, int numeLlamada)
        {
            try
            {
                var cADPEDIDOS_INFORMACION = contextoDespacho.CADPEDIDOS_INFORMACION.Where(c => c.CADPEDI_SITIOGRABA == idSitio && c.CADPEDI_NUMELLAMADA == numeLlamada);
                return PartialView("_ComentariosAdicionales", cADPEDIDOS_INFORMACION.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult CreacionComentariosAdicionales([Bind(Include = "CADPEDI_SITIOGRABA,CADPEDI_NUMELLAMADA,CONSECUTIVO,CODI_PREGUNTA,RTA_PREGUNTA,ANOTACION,FECHA_GRABACION,USUA_USUARIO")] CADPEDIDOS_INFORMACION cADPEDIDOS_INFORMACION)
        {
            try
            {
                int CODIGO;
                var existe = contextoDespacho.CADPEDIDOS_INFORMACION.Where(c => c.CADPEDI_SITIOGRABA == cADPEDIDOS_INFORMACION.CADPEDI_SITIOGRABA && c.CADPEDI_NUMELLAMADA == cADPEDIDOS_INFORMACION.CADPEDI_NUMELLAMADA).FirstOrDefault();

                if (existe == null)
                    CODIGO = 1;
                else
                {
                    CODIGO = contextoDespacho.CADPEDIDOS_INFORMACION.Where(c => c.CADPEDI_SITIOGRABA == cADPEDIDOS_INFORMACION.CADPEDI_SITIOGRABA && c.CADPEDI_NUMELLAMADA == cADPEDIDOS_INFORMACION.CADPEDI_NUMELLAMADA).Select(c => c.CONSECUTIVO).ToList().Max(c => c);
                    CODIGO += 1;
                }

                cADPEDIDOS_INFORMACION.CONSECUTIVO = CODIGO;

                GestorGeneral gestorGeneral = new GestorGeneral();
                cADPEDIDOS_INFORMACION.FECHA_GRABACION = gestorGeneral.ObtenerTiempoServidor?? DateTime.Now;

                if (ModelState.IsValid)
                {
                    contextoDespacho.CADPEDIDOS_INFORMACION.Add(cADPEDIDOS_INFORMACION);
                    contextoDespacho.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADPEDIDOS_INFORMACION>(cADPEDIDOS_INFORMACION, "CADPEDIDOS_INFORMACION", cADPEDIDOS_INFORMACION.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    return Json(new { result = true, datos = new { MENSAJE = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "AGREGADO") } });
                }
                return Json(new { result = false, datos = "NO FUNCIONO" });
            }
            catch (DbUpdateException ex)
            {
                var m = ex.Message;
                var mensaje = "No se puede crear, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult CargarPreguntasAdicionales(int idSitio, int numeLlamada, string codiCaso, string vigente, string usuario)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var valido = gestor.IngresarComentarioAdicionales(idSitio, numeLlamada, codiCaso, vigente, usuario);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "AGREGADO") });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerBarriosComunas(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "WHERE CIUDAD_BARRIO = " + id, strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorBarrio(CODIGO, 'DESCRIPCION', CIUDAD_BARRIO)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_BarriosComunasModel> listaBarrios = new List<_BarriosComunasModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (CIUDAD_BARRIO = " + id + " AND REGEXP_LIKE(CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CIUDAD_BARRIO = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CIUDAD_BARRIO = " + id + " AND REGEXP_LIKE (CIUDAD_BARRIO, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY CIUDAD_BARRIO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_barrios_comunas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CIUDAD_BARRIO", fila["CIUDAD_BARRIO"].ToString());

                    listaBarrios.Add(new _BarriosComunasModel(strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["CIUDAD_BARRIO"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaBarrios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarDireccionBarrio(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var valido = gestor.ActualizarDireccionBarrio(parametros);

                CADPEDIDO cADPEDIDO = new CADPEDIDO();

                cADPEDIDO.DIRE_CASO = parametros[3];
                cADPEDIDO.SITIO_GRABA = int.Parse(parametros[0]);
                cADPEDIDO.NUME_LLAMADA = int.Parse(parametros[1]);
                cADPEDIDO.LUGE_BARRIO = int.Parse(parametros[5]);
                cADPEDIDO.CODI_BARRIO = int.Parse(parametros[6]);

                auditoria.RegistrarAuditoria<CADPEDIDO>(cADPEDIDO, "CADPEDIDO", cADPEDIDO.NUME_LLAMADA.ToString(), TipoAuditoria.Modificar);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "ACTUALIZADO") });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerMediosDisponibles(int idSitio, long numeTurno, int idFuerza, int idCanal)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE CADUNITUR_SITIOGRABA = " + idSitio + " AND CADUNITUR_TURNO = " + numeTurno + " AND CADCANA_FUERZ_ID = " + idFuerza + " and CADCANA_CODIGO = " + idCanal + " AND CADEVEN_SITIOGRABA = " + idSitio + " AND CADEVEN_NUMEEVENTO = " + Session["EVENTO_RMR"];
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title='' data-placement='bottom' data-original-title='Clic para adicionar medio al caso' " +
                                         " onclick='AdicionarMedioCaso(CONSECUTIVO)'>" +
                                        "<i class='icon-arrow-right14'></i>" +
                                  "</button>";
                string strBotonDestino = "";
                List<_MediosXcrimeDisponiblesModel> listaMedios = new List<_MediosXcrimeDisponiblesModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND CADEVEN_SITIOGRABA = " + idSitio + " AND CADEVEN_NUMEEVENTO = " + Session["EVENTO_RMR"] + " AND (CADUNITUR_SITIOGRABA = " + idSitio + " AND CADUNITUR_TURNO = " + numeTurno + " AND CADCANA_FUERZ_ID = " + idFuerza + " and CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE(DESC_UNIDAD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADEVEN_SITIOGRABA = " + idSitio + " AND CADEVEN_NUMEEVENTO = " + Session["EVENTO_RMR"] + " AND CADUNITUR_SITIOGRABA = " + idSitio + " AND CADUNITUR_TURNO = " + numeTurno + " AND CADCANA_FUERZ_ID = " + idFuerza + " and CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADEVEN_SITIOGRABA = " + idSitio + " AND CADEVEN_NUMEEVENTO = " + Session["EVENTO_RMR"] + " AND CADUNITUR_SITIOGRABA = " + idSitio + " AND CADUNITUR_TURNO = " + numeTurno + " AND CADCANA_FUERZ_ID = " + idFuerza + " and CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (ID_RADIO, '" + parametros.ConsultaValor + "', 'i'))";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY DESC_UNIDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY ID_RADIO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_medios_disponibles", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());

                    listaMedios.Add(new _MediosXcrimeDisponiblesModel(fila["DESC_UNIDAD"].ToString(), fila["DESC_PATRULLA"].ToString(), fila["ID_RADIO"].ToString(), fila["CUENTA"].ToString(), strBotonDestino,
                                                                fila["CADUNITUR_UNDEFUERZA"].ToString(), fila["CADUNITUR_UNDECODIGO"].ToString(), fila["CADPATRU_CODIGO"].ToString(), fila["CONSECUTIVO"].ToString(), fila["ETA_SEG"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaMedios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarIdUbicacion(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var valido = gestor.ActualizarIdUbicacion(parametros);

                CADPATRULLA cADPATRULLA = new CADPATRULLA();

                cADPATRULLA.ID_RADIO = parametros[0];
                cADPATRULLA.CADUNID_FUERZA = int.Parse(parametros[1]);
                cADPATRULLA.CADUNID_CODIGO = int.Parse(parametros[2]);
                cADPATRULLA.CODIGO = int.Parse(parametros[3]);

                auditoria.RegistrarAuditoria<CADPATRULLA>(cADPATRULLA, "CADPATRULLA", cADPATRULLA.CODIGO.ToString(), TipoAuditoria.Modificar);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "ACTUALIZADO") });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarMedioCaso(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var resultados = gestor.EjecutarMedioCaso(parametros);

                var mensaje = resultados;

                if (mensaje == "Ingresado")
                    return Json(new { result = true });
                else
                    return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerEventoMedios(int idSitio, int numeEvento)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " ";
                string strLimite = "";
                string strBoton = "<ul class='icons-list'>" +
                                        "<li class='dropdown'>" +
                                            "<button type='button' class='btn btn-xs border-teal text-teal btn-flat dropdown-toggle'" +
                                                    "data-toggle='dropdown' aria-expanded='true'>" +
                                                "<i class='icon-menu9'></i>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu dropdown-menu-left'>" +
                                                "<li>" +
                                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                                       "title='' data-placement='right' data-original-title='Clic para seleccionar'" +
                                                       "onclick='EventoMedioSeleccionado()'>" +
                                                        "<i class='icon-shield-check position-left'></i> Seleccionar" +
                                                    "</a>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                                       "title='' data-placement='right' data-original-title='Clic para actualizar'" +
                                                       "onclick='ModalEventoMedio()'>" +
                                                        "<i class='glyphicon glyphicon-edit position-left'></i> Actualizar" +
                                                    "</a>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                                       "title='' data-placement='right' data-original-title='Clic para eliminar'" +
                                                       "onclick='EliminarEventoMedio(CADMEDISP_CONSECUTIVO, \"HORA_LLEGA\", \"HORA_TERMINA\")'>" +
                                                        "<i class='icon-trash position-left'></i> Eliminar" +
                                                    "</a>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                                       "title='' data-placement='right' data-original-title='Clic para comunicarse'" +
                                                       "onclick='ModalHuawei(CADMEDISP_CONSECUTIVO)'>" +
                                                        "<i class='icon-phone position-left'></i> Comunicarse" +
                                                    "</a>" +
                                                "</li>" +
                                            "</ul>";
                string strBotonDestino = "", strConsecutivo = "", strSitio = "", strEventoSitio = "", strEventoNumero = "";
                bool blFiltro = false;
                GestorAdministrador gestor;

                DataTable dttLugar;


                List<_Eventos_EventosMediosModel> listaEventoMedios = new List<_Eventos_EventosMediosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (DSP_DESC_UNIDAD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (DSP_MEDIODISPO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (TIPO_RESPONSABLE, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_ASIGNA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_RUTA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_LLEGA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (HORA_TERMINA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " AND REGEXP_LIKE (NOVEDAD, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));

                //Obtiene la infomación en base de datos
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_eventos_medios", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CADMEDISP_CONSECUTIVO", fila["CADMEDISP_CONSECUTIVO"].ToString());
                    strConsecutivo = fila["CADMEDISP_CONSECUTIVO"].ToString();
                    strSitio = fila["CADMEDISP_SITIOGRABA"].ToString();
                    strEventoSitio = fila["CADEVEN_SITIOGRABA"].ToString();
                    strEventoNumero = fila["CADEVEN_NUME_EVENTO"].ToString();
                    if (string.IsNullOrWhiteSpace(fila["HORA_LLEGA"].ToString()))
                        strBotonDestino = strBotonDestino.Replace("HORA_LLEGA", "VACIO");
                    else
                        strBotonDestino = strBotonDestino.Replace("HORA_LLEGA", fila["HORA_LLEGA"].ToString());

                    if (string.IsNullOrWhiteSpace(fila["HORA_TERMINA"].ToString()))
                        strBotonDestino = strBotonDestino.Replace("HORA_TERMINA", "VACIO");
                    else
                        strBotonDestino = strBotonDestino.Replace("HORA_TERMINA", fila["HORA_TERMINA"].ToString());

                    if (fila["HORA_RUTA"].ToString() == "")
                    {

                        fila["HORA_RUTA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTabla(1, " + strConsecutivo + ") ' > " +
                                                "<i class='icon-alarm-add icon_size-xs'></i></button>";

                        //var HoraRuta = GetTimerRuta(numeEvento);

                        //if (HoraRuta=="")
                        //{
                        //     fila["HORA_RUTA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTabla(1, " + strConsecutivo + ") ' > " +
                        //                        "<i class='icon-alarm-add icon_size-xs'></i></button>";
                        //}
                        //else
                        //{
                        //    fila["HORA_RUTA"] = HoraRuta;
                        //}

                    }

                    if (fila["HORA_LLEGA"].ToString() == "")
                    {

                        fila["HORA_LLEGA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTabla(2, " + strConsecutivo + ") ' > " +
                                               "<i class='icon-alarm-add icon_size-xs'></i></button>";

                        //var HoraLlega = GetTimerLlegada(numeEvento);

                        //if (HoraLlega == "")
                        //{
                        //    fila["HORA_LLEGA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTabla(2, " + strConsecutivo + ") ' > " +
                        //                       "<i class='icon-alarm-add icon_size-xs'></i></button>";
                        //}
                        //else
                        //{
                        //    fila["HORA_LLEGA"] = HoraLlega;
                        //}

                    }
                    if (fila["HORA_TERMINA"].ToString() == "")
                    {
                        fila["HORA_TERMINA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTabla(3, " + strConsecutivo + ") ' > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }
                    if (fila["HORA_BCONTROL"].ToString() == "")
                    {
                        fila["HORA_BCONTROL"] = "<button type='button'  class='btn bg-teal btn-xs' onclick=\"AsignarHorasTablaAdic('HORA_BCONTROL', " + strConsecutivo + "," + strSitio + "," + strEventoSitio + "," + strEventoNumero + ") \" > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }
                    if (fila["HORA_SREFERENCIA"].ToString() == "")
                    {
                        fila["HORA_SREFERENCIA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick=\"AsignarHorasTablaAdic('HORA_SREFERENCIA', " + strConsecutivo + "," + strSitio + "," + strEventoSitio + "," + strEventoNumero + ") \" > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }
                    if (fila["HORA_PREGULADO"].ToString() == "")
                    {
                        fila["HORA_PREGULADO"] = "<button type='button'  class='btn bg-teal btn-xs' onclick=\"AsignarHorasTablaAdic('HORA_PREGULADO', " + strConsecutivo + "," + strSitio + "," + strEventoSitio + "," + strEventoNumero + ") \" > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }
                    if (fila["HORA_LLIPS"].ToString() == "")
                    {
                        fila["HORA_LLIPS"] = "<button type='button'  class='btn bg-teal btn-xs' onclick=\"AsignarHorasTablaAdic('HORA_LLIPS', " + strConsecutivo + "," + strSitio + "," + strEventoSitio + "," + strEventoNumero + ") \" > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }
                    if (fila["HORA_PRECIBIDO"].ToString() == "")
                    {
                        fila["HORA_PRECIBIDO"] = "<button type='button'  class='btn bg-teal btn-xs' onclick=\"AsignarHorasTablaAdic('HORA_PRECIBIDO', " + strConsecutivo + "," + strSitio + "," + strEventoSitio + "," + strEventoNumero + ") \" > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }

                    listaEventoMedios.Add(new _Eventos_EventosMediosModel(fila, "", strBotonDestino));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventoMedios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarHorasEventoMedios(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var resultados = gestor.EjecutarHorasMedioCaso(parametros);

                var hora = resultados.Item1;
                var mensaje = resultados.Item2;

                if (mensaje == "Ingresado")
                {
                    CADEVENTOMEDIO cADEVENTOMEDIO = new CADEVENTOMEDIO();

                    if (parametros[2] == "HORA_ASIGNA")
                        cADEVENTOMEDIO.HORA_ASIGNA = DateTime.Parse(hora);

                    if (parametros[2] == "HORA_RUTA" || parametros[2] == "HORA_RUTA_CONTROL")
                        cADEVENTOMEDIO.HORA_RUTA = DateTime.Parse(hora);

                    if (parametros[2] == "HORA_LLEGA" || parametros[2] == "HORA_LLEGA_CONTROL")
                        cADEVENTOMEDIO.HORA_LLEGA = DateTime.Parse(hora);

                    if (parametros[2] == "HORA_TERMINA")
                        cADEVENTOMEDIO.HORA_TERMINA = DateTime.Parse(hora);

                    cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO = long.Parse(parametros[0]);
                    cADEVENTOMEDIO.CADEVEN_NUME_EVENTO = long.Parse(parametros[1]);

                    auditoria.RegistrarAuditoria<CADEVENTOMEDIO>(cADEVENTOMEDIO, "CADEVENTOMEDIO", "LLAVE COMPUESTA: CADMEDISP_CONSECUTIVO:" + cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO + " | CADEVEN_NUME_EVENTO:" + cADEVENTOMEDIO.CADEVEN_NUME_EVENTO, TipoAuditoria.Modificar);

                    return Json(new { result = true, datos = hora });
                }
                else
                    return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ProcesarNovedad(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var valido = gestor.ActualizarNovedad(parametros);

                CADEVENTOMEDIO cADEVENTOMEDIO = new CADEVENTOMEDIO();

                cADEVENTOMEDIO.NOVEDAD = parametros[2];
                cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO = long.Parse(parametros[0]);
                cADEVENTOMEDIO.CADEVEN_NUME_EVENTO = long.Parse(parametros[1]);

                auditoria.RegistrarAuditoria<CADEVENTOMEDIO>(cADEVENTOMEDIO, "CADEVENTOMEDIO", cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "ACTUALIZADO") });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ValidarEventosMediosRuta(string[] parametros)
        {
            try
            {
                string existe;
                GestorGeneral gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_sitio_graba", parametros[0], typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_consecutivo", parametros[1], typeof(long)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_sitio_graba", parametros[2], typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[3], typeof(long)));
                existe = gestorGeneral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_existe_evento_medio_ruta", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                if (!string.IsNullOrEmpty(existe))
                    return Json(new { result = false, datos = "No es posible suprimir el registro principal al existir registros detallados asociados." });
                else
                    return Json(new { result = true });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult EliminarEventosMedios(string[] parametros)
        {
            try
            {
                CADEVENTOMEDIO cADEVENTOMEDIO = contextoDespacho.CADEVENTOMEDIOS.Find(int.Parse(parametros[0]), long.Parse(parametros[1]), int.Parse(parametros[2]), long.Parse(parametros[3]));

                contextoDespacho.CADEVENTOMEDIOS.Remove(cADEVENTOMEDIO);
                contextoDespacho.SaveChanges();

                CADEVENTOMEDIO cADEVENTOMEDIO_copy = new CADEVENTOMEDIO();
                cADEVENTOMEDIO_copy = (CADEVENTOMEDIO)auditoria.GenerarCopiaAuditoria(cADEVENTOMEDIO);


                auditoria.RegistrarAuditoria<CADEVENTOMEDIO>(cADEVENTOMEDIO_copy, "CADEVENTOMEDIOS", cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public ActionResult VerAsociarLlamadas(string[] parametros)
        {
            try
            {
                //string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                //                  "onclick = \"cargarValorAsignarLlamada(NUME_LLAMADA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                //                  " Seleccionar </button>";

                //string strBotonDestino = "";

                //GestorDespachos gestor = new GestorDespachos();
                //var AsignarLlamadas = gestor.ConsultarAsignarLlamadas(int.Parse(parametros[0]), int.Parse(parametros[1]), parametros[2], int.Parse(parametros[3]));

                //List<_AsociarLlamadaModel> listaLlamadas = new List<_AsociarLlamadaModel>();

                //foreach (DataRow fila in AsignarLlamadas.Rows)
                //{
                //    listaLlamadas.Add(new _AsociarLlamadaModel("", "", fila["NUME_LLAMADA"].ToString(), fila["HORA_CASO"].ToString(), fila["NUME_TELEFONO"].ToString(), fila["CALI_PEDIDO"].ToString(),
                //                                                   fila["CIUDAD"].ToString(), fila["NOMB_LLAMANTE"].ToString(), fila["DIRE_CASO"].ToString(), fila["CODI_PEDIDO"].ToString(), fila["ESTADO"].ToString(),
                //                                                   fila["SITIO_GRABA"].ToString()));
                //}

                DataTable dttLugar;
                List<_AsociarLlamadaModel> listaLlamadas = new List<_AsociarLlamadaModel>();

                DateTime Fecha = DateTime.Parse(parametros[2]);

                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", parametros[0], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_gsitio_graba", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_caso", Fecha.ToString("dd/MM/yyyy HH:mm:ss"), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nume_llamada", parametros[3], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_asignar_llamadas", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaLlamadas.Add(new _AsociarLlamadaModel("", "", fila["NUME_LLAMADA"].ToString(), fila["HORA_CASO"].ToString(), fila["NUME_TELEFONO"].ToString(), fila["CALI_PEDIDO"].ToString(),
                                                                   fila["CIUDAD"].ToString(), fila["NOMB_LLAMANTE"].ToString(), fila["DIRE_CASO"].ToString(), fila["CODI_PEDIDO"].ToString(), fila["ESTADO"].ToString(),
                                                                   fila["SITIO_GRABA"].ToString()));
                }

                return PartialView("_AsociarLlamada", listaLlamadas);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerCasos()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorCaso('CODIGO','DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_AtencionCasosModel> listaCasos = new List<_AtencionCasosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    //blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    //if (blControl)
                    //{
                    strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_casos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaCasos.Add(new _AtencionCasosModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCasos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
        public ActionResult VerAnotacionesEvento(string strSitio, string strLlamada)
        {
            try
            {
                ViewBag.Sitio = strSitio;
                ViewBag.Llamada = strLlamada;
                return View("VerAnotaciones", "BaseRecepcion");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize]
        public JsonResult VerAnotaciones(int idSitio, int numeLlamada)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = ""; // " WHERE cadeven_sitiograba = " + idSitio + " AND cadeven_numeevento = " + numeEvento + " ";
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_AnotacionesModel> listaAnotaciones = new List<_AnotacionesModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (REGEXP_LIKE (ANOTACION, '" + parametros.ConsultaValor + "', 'i')) " +

                               " OR ( REGEXP_LIKE (CADUSUA_GRABA, '" + parametros.ConsultaValor + "', 'i')) " +

                               " OR ( REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY INTERNO.FECHA_GRABACION DESC";
                        break;

                    case 1:
                        strOrderna += " ORDER BY INTERNO.ANOTACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY INTERNO.FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY INTERNO.CANAL " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY INTERNO.CADUSUA_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY INTERNO.FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_pecaNumLlamada", numeLlamada.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", idSitio.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_anotaciones", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaAnotaciones.Add(new _AnotacionesModel(fila["ANOTACION"].ToString(), fila["FUERZA"].ToString(), fila["CANAL"].ToString(),
                                                               fila["CADUSUA_GRABA"].ToString(), fila["FECHA_GRABACION"].ToString(), fila["CONSECUTIVO"].ToString(), ""));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaAnotaciones });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult VerEventosMedioControl(int idFuerza, int idCanal)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = ""; // " WHERE CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " ";
                string strLimite = "", strOrderna = "";
                string strBoton1 = "<ul class='icons-list'>" +
                        "<li class='dropdown'>" +
                            "<button type='button' class='btn btn-xs border-teal text-teal btn-flat dropdown-toggle'" +
                                    "data-toggle='dropdown' aria-expanded='true'>" +
                                "<i class='icon-menu9'></i>" +
                            "</button>" +
                            "<ul class='dropdown-menu dropdown-menu-left'>" +
                                "<li>" +
                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                       "title='' data-placement='right' data-original-title='Clic para seleccionar'" +
                                       "onclick='CargarTabEventos1(EVENTO_NUMEEVENTO)'>" +
                                        "<i class='icon-shield-check position-left'></i> Seleccionar" +
                                    "</a>" +
                                "</li>" +
                                "<li>" +
                                    "<a class='btn btn-xs btn-default' data-popup='tooltip'" +
                                       "title='' data-placement='right' data-original-title='Clic para actualizar'" +
                                       "onclick='ModalEventoMedioControl(this)'>" +
                                        "<i class='glyphicon glyphicon-edit position-left'></i> Actualizar" +
                                    "</a>" +
                                "</li>" +
                            "</ul>";

                string strBoton2 = "<button class='btn bg-teal btn-xs' type='button' data-popup='tooltip' title=''" +
                                            "onclick = \"CargarTabEventos2(EVENTO_NUMEEVENTO)\" data-placement='bottom' data-original-title='Clic para ver'>" +
                                        "<i class='icon-arrow-left13'></i>" +
                                    "</button>";
                string strBotonDestino1 = "";
                string strBotonDestino2 = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_EventosMediosControlModel> listaEventos = new List<_EventosMediosControlModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " WHERE (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (CODI_PEDIDO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (CODI_PEDIDO2, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (HORA_ASIGNA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (HORA_RUTA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (HORA_LLEGA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (CADCANA_FUERZA_ID = " + idFuerza + " AND CADCANA_CODIGO = " + idCanal + " AND REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY DESC_PATRULLA";
                        break;

                    case 2:
                        strOrderna += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY CODI_PEDIDO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY CODI_PEDIDO2 " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY HORA_ASIGNA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY HORA_RUTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY HORA_LLEGA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY DIRE_CASO " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", idFuerza.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", idCanal.ToString(), typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_eventos_medios_control", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    if (fila["HORA_RUTA"].ToString() == "")
                    {
                        fila["HORA_RUTA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTablaPorEvento(1, " + fila["CADMEDISP_CONSECUTIVO"].ToString() + "," + fila["EVENTO_NUMEEVENTO"].ToString() + ") ' > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }

                    if (fila["HORA_LLEGA"].ToString() == "SIN HORA")
                    {
                        fila["HORA_LLEGA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTablaPorEvento(2, " + fila["CADMEDISP_CONSECUTIVO"].ToString() + "," + fila["EVENTO_NUMEEVENTO"].ToString() + ") ' > " +
                                             "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    }

                    //if (fila["HORA_TERMINA"].ToString() == "")
                    //{
                    //    fila["HORA_TERMINA"] = "<button type='button'  class='btn bg-teal btn-xs' onclick='AsignarHorasTablaPorEvento(3, " + fila["CADMEDISP_CONSECUTIVO"].ToString() + "," + fila["EVENTO_NUMEEVENTO"].ToString() + ") ' > " +
                    //                         "<i class='icon-alarm-add icon_size-xs'></i></button>";
                    //}

                    strBotonDestino1 = strBoton1.Replace("EVENTO_NUMEEVENTO", fila["EVENTO_NUMEEVENTO"].ToString());
                    strBotonDestino2 = strBoton2.Replace("EVENTO_NUMEEVENTO", fila["EVENTO_NUMEEVENTO"].ToString());

                    listaEventos.Add(new _EventosMediosControlModel("", fila["DESC_PATRULLA"].ToString(), strBotonDestino2, fila["CODI_PEDIDO"].ToString(), fila["CODI_PEDIDO2"].ToString(), fila["HORA_ASIGNA"].ToString(),
                                                                    fila["HORA_RUTA"].ToString(), fila["HORA_LLEGA"].ToString(), fila["DIRE_CASO"].ToString(), fila["MINUTOS"].ToString(), fila["CADMEDISP_CONSECUTIVO"].ToString(), fila["EVENTO_NUMEEVENTO"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ProcesarSeguimiento(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var valido = gestor.ActualizarEventoEnSeguimiento(parametros);

                CADEVENTO cADEVENTO = new CADEVENTO();

                cADEVENTO.SITIO_GRABA = int.Parse(parametros[0]);
                cADEVENTO.NUME_EVENTO = long.Parse(parametros[1]);
                cADEVENTO.ESTADO_EVENTO = parametros[2];

                auditoria.RegistrarAuditoria<CADEVENTO>(cADEVENTO, "CADEVENTO", cADEVENTO.NUME_EVENTO.ToString(), TipoAuditoria.Modificar);

                CADPEDIDOS_CANALES cADPEDIDOS_CANALES = new CADPEDIDOS_CANALES();

                cADPEDIDOS_CANALES.CADEVEN_SITIO_GRABA = int.Parse(parametros[0]);
                cADPEDIDOS_CANALES.CADEVEN_NUME_EVENTO = long.Parse(parametros[1]);
                cADPEDIDOS_CANALES.ESTADO = parametros[2];

                auditoria.RegistrarAuditoria<CADPEDIDOS_CANALES>(cADPEDIDOS_CANALES, "CADPEDIDOS_CANALES", cADPEDIDOS_CANALES.CADPEDI_NUMELLAMADA.ToString(), TipoAuditoria.Modificar);

                var mensaje = "El evento (" + parametros[1] + ") ha sido actualizado EN SEGUIMIENTO";

                return Json(new { result = true, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ProcesarCerrarEvento(string[] parametros)
        {
            try
            {
                GestorDespachos gestor = new GestorDespachos();
                var resultados = gestor.EjecutarCerrarEvento(parametros);

                var SitioGraba = Int32.Parse(parametros[1]);
                if (Convert.ToString(parametros[3]) == "")
                {
                    parametros[3] = "0.0";
                }
                var V_Latitud = Convert.ToDouble(parametros[3].Replace(".", ","));
                if (Convert.ToString(parametros[4]) == "")
                {
                    parametros[4] = "0.0";
                }
                var V_Longitud = Convert.ToDouble(parametros[4].Replace(".", ","));
                var IdLlamada = Int32.Parse(parametros[5]);
                var V_Modalidad = parametros[6];
                var V_Conducta = parametros[7];
                var V_TipoArma = parametros[8];
                var V_TipoMoviliza = parametros[9];
                var Coordenadas = new DbMinuta().Ins_Coordenadas(SitioGraba, IdLlamada, V_Latitud, V_Longitud, V_Modalidad, V_Conducta, V_TipoArma, V_TipoMoviliza);
                int upd = new DbMinuta().update_evento(SitioGraba, Convert.ToInt32(parametros[2]), Convert.ToString(parametros[0]), Convert.ToString(parametros[10]));

                var mensaje = resultados;

                if (mensaje.Contains("Cerrada") || mensaje.Contains("Cerrado"))
                {
                    CADEVENTO cADEVENTO = new CADEVENTO();

                    cADEVENTO.SITIO_GRABA = int.Parse(parametros[1]);
                    cADEVENTO.NUME_EVENTO = long.Parse(parametros[2]);
                    cADEVENTO.ESTADO_EVENTO = "C";

                    auditoria.RegistrarAuditoria<CADEVENTO>(cADEVENTO, "CADEVENTO", cADEVENTO.NUME_EVENTO.ToString(), TipoAuditoria.Modificar);

                    CADEVENTOMEDIO cADEVENTOMEDIO = new CADEVENTOMEDIO();

                    cADEVENTOMEDIO.CADEVEN_SITIOGRABA = int.Parse(parametros[1]);
                    cADEVENTOMEDIO.CADEVEN_NUME_EVENTO = long.Parse(parametros[2]);

                    auditoria.RegistrarAuditoria<CADEVENTOMEDIO>(cADEVENTOMEDIO, "CADEVENTOMEDIO", cADEVENTOMEDIO.CADMEDISP_CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    CADPEDIDOS_CANALES cADPEDIDOS_CANALES = new CADPEDIDOS_CANALES();

                    cADPEDIDOS_CANALES.CADEVEN_SITIO_GRABA = int.Parse(parametros[1]);
                    cADPEDIDOS_CANALES.CADEVEN_NUME_EVENTO = long.Parse(parametros[2]);
                    cADPEDIDOS_CANALES.ESTADO = "C";

                    auditoria.RegistrarAuditoria<CADPEDIDOS_CANALES>(cADPEDIDOS_CANALES, "CADPEDIDOS_CANALES", cADPEDIDOS_CANALES.CADPEDI_NUMELLAMADA.ToString(), TipoAuditoria.Modificar);

                    CADPEDIDO cADPEDIDO = new CADPEDIDO();

                    cADPEDIDO.ESTADO = "C";
                    cADPEDIDO.SITIO_GRABA = int.Parse(parametros[1]);
                    cADPEDIDO.NUME_LLAMADA = int.Parse(parametros[2]);

                    auditoria.RegistrarAuditoria<CADPEDIDO>(cADPEDIDO, "CADPEDIDO", cADPEDIDO.NUME_LLAMADA.ToString(), TipoAuditoria.Modificar);

                    //Guardar Coordenadas

                    //var Coordenadas = new DbMinuta().Ins_Coordenadas(parametros);

                    //Coordenadas. = int.Parse(parametros[0]);
                    //Coordenadas.Longitud = int.Parse(parametros[1]);
                    //Coordenadas.SITIO_GRABA = int.Parse(parametros[2]);
                    //Coordenadas.NUME_LLAMADA = int.Parse(parametros[3]);


                    return Json(new { result = true, datos = mensaje });
                }
                else
                {
                    CADEVENTO cADEVENTO = new CADEVENTO();

                    cADEVENTO.SITIO_GRABA = int.Parse(parametros[1]);
                    cADEVENTO.NUME_EVENTO = long.Parse(parametros[2]);
                    cADEVENTO.ESTADO_EVENTO = "A";

                    auditoria.RegistrarAuditoria<CADEVENTO>(cADEVENTO, "CADEVENTO", cADEVENTO.NUME_EVENTO.ToString(), TipoAuditoria.Modificar);

                    return Json(new { result = false, datos = mensaje });
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarCasos(string strCodigo = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCasosCanal> elementos = new List<ExtCasosCanal>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "", strNombre = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarCaso('_CODIGO' ,'_NOMBRE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = '" + parametros.ConsultaValor + "') ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(strCodigo))
                {
                    strQuery = " WHERE (INTERNO.CODIGO = '" + strCodigo + "') ";
                    strLimites = "";
                }
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_casos_despacho", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new ExtCasosCanal(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    if (!string.IsNullOrEmpty(strCodigo))
                    {
                        strNombre = fila["DESCRIPCION"].ToString();
                    }
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                if (!string.IsNullOrEmpty(strCodigo))
                {
                    resultados = Json(new { result = true, datos = strNombre });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult AsignarCasoReal(int intSitio, long lgLlamada, string strCasoReal)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADUSUARIO usuarioActual;
                CADPEDIDO pedido = (from registro in contextoDespacho.CADPEDIDOS
                                    where registro.SITIO_GRABA == intSitio
                                    && registro.NUME_LLAMADA == lgLlamada
                                    select registro).FirstOrDefault();
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                }
                if (pedido == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE });
                }
                else
                {
                    pedido.COMENTARIO += ". Observación: Caso inicialmente reportado como " + pedido.CODI_PEDIDO +
                        ". Al ser atendido se verifica como " + strCasoReal + ". Usuario " + usuarioActual.USUARIO + ", fecha " + ((DateTime)auditoria.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                    pedido.CODI_PEDIDO = strCasoReal;
                    contextoDespacho.SaveChanges();
                    resultados = Json(new { result = true, datos = "Caso real asociado." });
                }
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Premite guarda una anotación asociada a una patrulla Los parametros son capturados de la
        /// variable Request.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult GuardarAnotacionPatrulla()
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADUSUARIO usuarioActual;
                CADANOTACIONESRECURSO anotacion;
                //Verifica que el usuario de secad se encuentre autenticado
                usr = validar.VerificarUsuario();

                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    validar.IdentificarCliente(Request, Session);
                }
                int intFuerza = 0, intUnidad = 0;
                short srCodigo = 0;
                string strAnotacion;
                long lgConsecutivo = 0;
                //Verifica que el usuario de secad se encuentre autenticado
                resultados.MaxJsonLength = int.MaxValue;
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                    return resultados;
                }
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                    return resultados;
                }
                lgConsecutivo = Convert.ToInt64(Request.Params["CONSECUTIVO"].ToString());
                intFuerza = Convert.ToInt32(Request.Params["CADPATRU_FUERZA"].ToString());
                intUnidad = Convert.ToInt32(Request.Params["CADPATRU_UNIDAD"].ToString());
                srCodigo = Convert.ToInt16(Request.Params["CADPATRU_CODIGO"].ToString());
                strAnotacion = Request.Params["ANOTACION"].ToString();
                //verifica si el registro existe
                anotacion = (from registro in contextoDespacho.CADANOTACIONESRECURSOS
                             where registro.CONSECUTIVO == lgConsecutivo
                             && registro.CADPATRU_FUERZA == intFuerza
                             && registro.CADPATRU_UNIDAD == intUnidad
                             && registro.CADPATRU_CODIGO == srCodigo
                             select registro).FirstOrDefault();
                if (anotacion != null)
                {
                    //actualzia el registro
                    anotacion.ANOTACION = strAnotacion;
                    anotacion.FECHA = (DateTime)validar.ObtenerTiempoServidor;
                    anotacion.MAQUINA = Session["MAQUINA"].ToString();
                    anotacion.USUARIO = usuarioActual.USUARIO;
                    contextoDespacho.Entry(anotacion).State = EntityState.Modified;
                }
                else
                {
                    //Crea el nuevo registro
                    anotacion = new CADANOTACIONESRECURSO();
                    anotacion.ANOTACION = strAnotacion;
                    anotacion.CADPATRU_CODIGO = srCodigo;
                    anotacion.CADPATRU_FUERZA = intFuerza;
                    anotacion.CADPATRU_UNIDAD = intUnidad;
                    anotacion.CONSECUTIVO = despacho.ConsultaConsecAnotacionPatrulla();
                    anotacion.FECHA = (DateTime)validar.ObtenerTiempoServidor;
                    anotacion.MAQUINA = Session["MAQUINA"].ToString();
                    anotacion.USUARIO = usuarioActual.USUARIO;
                    contextoDespacho.CADANOTACIONESRECURSOS.Add(anotacion);
                }

                contextoDespacho.SaveChanges();
                resultados = Json(new { result = true, datos = "Anotación generada." });
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarAnotacionesPatrulla(string strFuerza, string strUnidad, string strCodigo)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<_AnotacionPatrulla> elementos = new List<_AnotacionPatrulla>();
                _AnotacionPatrulla anota;
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";

                bool blFiltro = false;

                string strBoton = "<button class='btn btn-xs btn-teal' type='button' data-popup='tooltip'" +
                "onclick = \"_EDITAR\" title = '' data-placement = 'bottom' data-original-title = 'Clic para Editar' >" +
                " <i class='icon icon-pencil'></i> </button>";
                strBoton += "<button class='btn btn-xs btn-danger' type='button' data-popup='tooltip'" +
                "onclick = \"_ELIMINAR\" title = '' data-placement = 'bottom' data-original-title = 'Clic para Eliminar' >" +
                " <i class='icon icon-cross'></i> </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE (REGEXP_LIKE(INTERNO.ANOTACION, '" + parametros.ConsultaValor + "', 'i') )";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.ANOTACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.ANOTACION " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerza, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", strUnidad, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", strCodigo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_anotapatrulla", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    anota = new _AnotacionPatrulla();
                    anota.Anotacion = fila["ANOTACION"].ToString();
                    anota.Boton = strBoton.Replace("_EDITAR", "EditarAnotacion(" + Convert.ToInt64(fila["CONSECUTIVO"].ToString()) + ",'" + anota.Anotacion + "')");
                    anota.Boton = anota.Boton.Replace("_ELIMINAR", "ElminarAnotacion(" + Convert.ToInt64(fila["CONSECUTIVO"].ToString()) + ",'" + anota.Anotacion + "')");
                    elementos.Add(anota);
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ActualizarHoraAdic(int intSitio, int intEventoSitio, long lgConsec, long lgEventoNum, string strCampo)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                bool blControl = false;
                CADUSUARIO usuarioActual;
                CADEVENTOMEDIOS_HORASADIC horasAdic;
                CADEVENTOMEDIO medio;
                resultados.MaxJsonLength = int.MaxValue;
                //Verifica que el usuario de secad se encuentre autenticado
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                    return resultados;
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validar.IdentificarCliente(Request, Session))
                    {
                        resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                        return resultados;
                    }
                }
                medio = (from registro in contextoDespacho.CADEVENTOMEDIOS
                         where registro.CADEVEN_NUME_EVENTO == lgEventoNum
                         && registro.CADEVEN_SITIOGRABA == intEventoSitio
                         && registro.CADMEDISP_CONSECUTIVO == lgConsec
                         && registro.CADMEDISP_SITIOGRABA == intSitio
                         select registro).FirstOrDefault();
                //verifica si el registro existe
                horasAdic = (from registro in contextoDespacho.CADEVENTOMEDIOS_HORASADIC
                             where registro.CADEVEN_NUME_EVENTO == lgEventoNum
                             && registro.CADEVEN_SITIOGRABA == intEventoSitio
                             && registro.CADMEDISP_CONSECUTIVO == lgConsec
                             && registro.CADMEDISP_SITIOGRABA == intSitio
                             select registro).FirstOrDefault();
                if (horasAdic == null)
                {
                    //Crea el nuevo registro
                    horasAdic = new CADEVENTOMEDIOS_HORASADIC();
                    horasAdic.CADEVEN_NUME_EVENTO = lgEventoNum;
                    horasAdic.CADEVEN_SITIOGRABA = intEventoSitio;
                    horasAdic.CADMEDISP_CONSECUTIVO = lgConsec;
                    horasAdic.CADMEDISP_SITIOGRABA = intSitio;
                    blControl = true;
                }
                switch (strCampo)
                {
                    case "HORA_BCONTROL":
                        if (medio.HORA_LLEGA == null)
                        {
                            resultados = Json(new { result = false, datos = "Debe asignar primero la hora de LLegada.." });
                            return resultados;
                        }
                        if (horasAdic.HORA_BCONTROL == null)
                        {
                            horasAdic.HORA_BCONTROL = despacho.ObtenerTiempoServidor;
                        }
                        break;

                    case "HORA_SREFERENCIA":
                        if (horasAdic.HORA_BCONTROL == null)
                        {
                            resultados = Json(new { result = false, datos = "Debe asignar primero la hora bajo control.." });
                            return resultados;
                        }
                        if (horasAdic.HORA_SREFERENCIA == null)
                        {
                            horasAdic.HORA_SREFERENCIA = despacho.ObtenerTiempoServidor;
                        }
                        break;

                    case "HORA_PREGULADO":
                        if (horasAdic.HORA_SREFERENCIA == null)
                        {
                            resultados = Json(new { result = false, datos = "Debe asignar primero la hora Solicitud de referencia.." });
                            return resultados;
                        }
                        if (horasAdic.HORA_PREGULADO == null)
                        {
                            horasAdic.HORA_PREGULADO = despacho.ObtenerTiempoServidor;
                        }

                        break;

                    case "HORA_LLIPS":
                        if (horasAdic.HORA_PREGULADO == null)
                        {
                            resultados = Json(new { result = false, datos = "Debe asignar primero la hora Solicitud de paciente regulado.." });
                            return resultados;
                        }
                        horasAdic.HORA_LLIPS = despacho.ObtenerTiempoServidor;
                        break;

                    case "HORA_PRECIBIDO":
                        if (horasAdic.HORA_LLIPS == null)
                        {
                            resultados = Json(new { result = false, datos = "Debe asignar primero la hora Solicitud de llegada a IPS.." });
                            return resultados;
                        }
                        if (horasAdic.HORA_PRECIBIDO == null)
                        {
                            horasAdic.HORA_PRECIBIDO = despacho.ObtenerTiempoServidor;
                        }
                        break;
                }
                if (blControl)
                {
                    contextoDespacho.CADEVENTOMEDIOS_HORASADIC.Add(horasAdic);
                }
                else
                {
                    contextoDespacho.Entry(horasAdic).State = EntityState.Modified;
                }
                contextoDespacho.SaveChanges();

                resultados = Json(new { result = true, datos = "Hora asignada." });
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EliminarAnotaPatrulla()
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADUSUARIO usuarioActual;
                CADANOTACIONESRECURSO anotacion;
                int intFuerza = 0, intUnidad = 0;
                short srCodigo = 0;
                string strMensaje = "";
                long lgConsecutivo = 0;
                //Verifica que el usuario de secad se encuentre autenticado
                resultados.MaxJsonLength = int.MaxValue;
                usuarioActual = validar.VerificarUsuario();
                if (usuarioActual == null)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                    return resultados;
                }
                //Verifica si la máquina ha sido identificada.
                if (Session["MAQUINA"] == null)
                {
                    Session["MAQUINA"] = "";
                    if (!validar.IdentificarCliente(Request, Session))
                    {
                        resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                        return resultados;
                    }
                }
                lgConsecutivo = Convert.ToInt64(Request.Params["CONSECUTIVO"].ToString());
                intFuerza = Convert.ToInt32(Request.Params["CADPATRU_FUERZA"].ToString());
                intUnidad = Convert.ToInt32(Request.Params["CADPATRU_UNIDAD"].ToString());
                srCodigo = Convert.ToInt16(Request.Params["CADPATRU_CODIGO"].ToString());
                //verifica si el registro existe
                anotacion = (from registro in contextoDespacho.CADANOTACIONESRECURSOS
                             where registro.CONSECUTIVO == lgConsecutivo
                             && registro.CADPATRU_FUERZA == intFuerza
                             && registro.CADPATRU_UNIDAD == intUnidad
                             && registro.CADPATRU_CODIGO == srCodigo
                             select registro).FirstOrDefault();
                if (anotacion != null)
                {
                    contextoDespacho.CADANOTACIONESRECURSOS.Remove(anotacion);
                    contextoDespacho.SaveChanges();
                    //Almacena la auditoría
                    //  auditoria.RegistrarAuditoria<CADCONDTURNOPER>(persona, "CADCONDTURNOPER", "LLAVE COMPUESTA: SITIO:" + persona.SITIO_GRABA.ToString() + " | CONSECUTIVO:" + persona.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                    resultados = Json(new { result = true, datos = strMensaje });
                }
                else
                {
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    resultados = Json(new { result = false, datos = strMensaje });
                }

                return resultados;
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Anotacion seleccionada") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [HttpPost]
        public ActionResult ConsultarCuadrantePorIdRadio(decimal _CADUNITUR_UNDECODIGO, decimal _CADPATRU_CODIGO, decimal _turno)
        {
            //consulta de cuadrante y cuenta RMR 09-06-2020
            //return Json(new { result = false, datos = new { Cuadrante = "MNVCC202019", Cuenta = 45212 } });
            try
            {
                string cuadrante1 = "";
                string cuenta1 = "";
                DataTable dttLugar;
                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadunitur_undecodigo", _CADUNITUR_UNDECODIGO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadpatru_codigo", _CADPATRU_CODIGO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turno", _turno.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_SECAD_MINUTAS.p_MediosDisponibles", strConexion, "Nonquery");
                foreach (DataRow fila in dttLugar.Rows)
                {
                    cuadrante1 = fila["CUADRANTE"].ToString();
                    cuenta1 = fila["CUENTA"].ToString();
                }
                var resultados = Json(new { result = false, datos = new { Cuadrante = cuadrante1, Cuenta = cuenta1 } });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }



        }

        //Metodo para traer las coordenadas del caso..

        public JsonResult TraerCoordenadas(int PSitioGraba, Int64 PMedioConsecutivo)
        {
            var retorno = new DbMinuta().F_GetCoordenada(PSitioGraba, PMedioConsecutivo);

            if (retorno.Count > 0)
            {
                return Json(new { success = true, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, Dato = retorno }, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetTimerRuta(Int64 IdNumEvento)
        {
            var GetTimer = new DbDespacho().F_GetTime(IdNumEvento).ToList();
            var TimeRuta = GetTimer[0].HoraRuta;
            return TimeRuta;
        }
        public string GetTimerLlegada(Int64 IdNumEvento)
        {
            var GetTimer = new DbDespacho().F_GetTime(IdNumEvento).ToList();
            var TimeLlega = GetTimer[0].HoraLLega;
            return TimeLlega;
        }
    }
}