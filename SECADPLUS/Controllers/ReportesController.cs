using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class ReportesController : Controller
    {
        // private DesRepMinEntities contexto = new
        // DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity,
        // "SecadEntities", "DespachosRecepcionMinuta"));
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));

        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private PermisosEntities dbPermisos = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Reportes
        [SecadPlusAutorizador("/Reportes/ReporteExcepciones")]
        public ActionResult ReporteExcepciones()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite consulta los usuarios Oid existentes
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarUsuariosOid()
        {
            try
            {
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtUsuarioOid> listaUsuarios = new List<ExtUsuarioOid>();
                DataTable dttLugar;

                string strQuery = "", strOrdanamiento = "", strLimites = "";
                bool blFiltro = false;
                GestorGeneral objGestorGral;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"CargarUsuario('_CODIGO', '_NOMBRE')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE ( REGEXP_LIKE(USUARIOOID.\"UserName\", '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(USUARIOSECAD.USUARIO,'" + parametros.ConsultaValor + "', 'i')  ) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdanamiento += " ORDER BY USUARIOOID.\"UserName\" " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdanamiento += " ORDER BY USUARIOSECAD.USUARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdanamiento += " ORDER BY USUARIOSECAD.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "USUARIOSOID", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdanamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");
                parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaUsuarios.Add(new ExtUsuarioOid(fila, strBoton.Replace("_CODIGO", fila["OIDID"].ToString()).Replace("_NOMBRE", fila["OID_USUARIO"].ToString())));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaUsuarios });
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
        /// Carga Reporte Conductas de Ponderación
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteConductasPonderacion")]
        public ActionResult ReporteConductasPonderacion()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                    string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                    string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                    ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                    ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                    ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                    return Redirect("~/ReportesWeb/Reportes.aspx?REPORTE=CONDUCTASPONDERACION");
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de planilla de llamadas
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReportePlanillaLlamadas")]
        public ActionResult ReportePlanillaLlamadas()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                ViewBag.Turno = new SelectList(ObtenerTurnos(), "TURNO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                Dictionary<int, string> listImprimir = new Dictionary<int, string>();
                listImprimir.Add(1, "1 - TODOS CON O SIN MARCA DE TIEMPO");
                listImprimir.Add(2, "2 - CON MARCA DE TIEMPO");
                listImprimir.Add(3, "3 - SIN MARCA DE TIEMPO");
                ViewBag.Imprimir = new SelectList(listImprimir, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.CargoResponsable = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.CargoElaboro = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de resumen de planilla
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteResumenPlanilla")]
        public ActionResult ReporteResumenPlanilla()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                ViewBag.Turno = new SelectList(ObtenerTurnos(), "TURNO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                Dictionary<int, string> listImprimir = new Dictionary<int, string>();
                listImprimir.Add(1, "1 - TODOS CON O SIN MARCA DE TIEMPO");
                listImprimir.Add(2, "2 - CON MARCA DE TIEMPO");
                listImprimir.Add(3, "3 - SIN MARCA DE TIEMPO");
                ViewBag.Imprimir = new SelectList(listImprimir, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.CargoResponsable = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.CargoElaboro = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Trae listado de turnos para usar en un drodownlist
        /// </summary>
        /// <returns></returns>
        private List<dynamic> ObtenerTurnos()
        {
            CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_FUERZA", usr.CADCANA_FUERZA_ID.ToString(), typeof(string)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttTurnos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_cargaTurnos", strConexion, "Nonquery");
            var arrTurnos = (from t in dttTurnos.AsEnumerable() select new { TURNO = t["TURNO"], DESCRIPCION = t["DESCRIPCION"], HORA_INICIA = t["HORA_INICIA"], HORA_TERMINA = t["HORA_TERMINA"], SITIO_GRABA = t["SITIO_GRABA"] });
            return arrTurnos.ToList<dynamic>();
        }

        /// <summary>
        /// Genera el reporte de la planilla de llamadas
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReportePlanillaLlamadas")]
        public void GeneraReportePlanillaLlamadas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=PLANILLATURNO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Genera el reporte del Resumen de Planilla de Llamadas
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteResumenPlanilla")]
        public void GeneraReporteResumenPlanilla()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=RESUMENPLANILLA", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Muestra pantalla de parámetros de los archivos planos
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/GenerarInformacionPlanos")]
        public ActionResult GenerarInformacionPlanos()
        {
            try
            {
                var lstTipoPedido = new List<SelectListItem>();
                lstTipoPedido.Add(new SelectListItem() { Text = "Selecione...", Value = "-1" });
                lstTipoPedido.Add(new SelectListItem() { Text = "Pedido Inicial", Value = "1" });
                lstTipoPedido.Add(new SelectListItem() { Text = "Pedido Secundario", Value = "2" });
                lstTipoPedido.Add(new SelectListItem() { Text = "Pedido Real", Value = "3" });
                ViewBag.TipoPedido = new SelectList(lstTipoPedido, "Value", "Text", "-1");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                var lstCodigoCasos = (from t in db.CADCASOS orderby t.CODIGO select new { t.CODIGO, t.DESCRIPCION }).ToList();
                lstCodigoCasos.Add(new { CODIGO = "-1", DESCRIPCION = "TODOS" });

                var lstPlanos = (from t in contexto.CAD_ARCHIVOSPLANOS_SP select new { Text = t.DESCRIPCION, Value = t.VALOR }).ToList();

                ViewBag.Plano = new SelectList(lstPlanos, "Value", "Text", "-1");

                ViewBag.FiltrarCodiPedido = new SelectList(lstCodigoCasos, "CODIGO", "DESCRIPCION", "-1");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Genera archivos planos según la especificación en la tabla cad_archivosplanos_sp
        /// </summary>
        /// <param name="intTipoPlano"></param>
        /// <param name="strFecha1"></param>
        /// <param name="strFecha2"></param>
        /// <param name="intTipoPedido"></param>
        /// <returns></returns>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/GenerarInformacionPlanos")]
        public JsonResult CargaDatosPlanos(int intTipoPlano, string strFecha1, string strFecha2, string intTipoPedido)
        {
            try
            {
                GestorGeneral objGestorGral = new GestorGeneral();
                DataTable dttDatosPlano = new DataTable();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                CAD_ARCHIVOSPLANOS_SP objTipoPlano = contexto.CAD_ARCHIVOSPLANOS_SP.Find(intTipoPlano);
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFecha1, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", strFecha2, typeof(string)));
                if (objTipoPlano.PARAM_TIPO_PEDIDO == "S")
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipo_pedido", intTipoPedido, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttDatosPlano = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + objTipoPlano.PROCEDIMIENTO_ALMACENADO, strConexion, "Nonquery");

                var resultados = Json(new { archivo = dttDatosPlano.Rows[0][0].ToString().Replace("CRLF", "\r\n|") });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(string.Empty);
            }
        }

        /// <summary>
        /// Genera la página de parámetros para el reporte casos por unidad de tiempo
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteCasosUnidadTiempo")]
        public ActionResult ReporteCasosUnidadTiempo()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ViewBag.Unidad = new SelectList((from t in BuscaUnidades() select new { t.DESCRIPCION, t.CODIGO }), "CODIGO", "DESCRIPCION");
                ViewBag.Firma = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera lista de unidades para usarse en un dropdownlist
        /// </summary>
        /// <returns></returns>
        private List<dynamic> BuscaUnidades()
        {
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttUnidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_unidades_por_usuario", strConexion, "Nonquery");
            var arrUnidades = (from t in dttUnidades.AsEnumerable() select new { CODIGO = t["CODIGO"], DESCRIPCION = t["DESCRIPCION"] });
            return arrUnidades.ToList<dynamic>();
        }

        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteCasosUnidadTiempo")]
        public void GeneraReporteCasosUnidadTiempo()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=CASOSUNIDADTIEMPO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de planilla de llamadas
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReportePlanillaLlamadasPeriodos")]
        public ActionResult ReportePlanillaLlamadasPeriodos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                Dictionary<int, string> listImprimir = new Dictionary<int, string>();
                listImprimir.Add(1, "1 - TODOS CON O SIN MARCA DE TIEMPO");
                listImprimir.Add(2, "2 - CON MARCA DE TIEMPO");
                listImprimir.Add(3, "3 - SIN MARCA DE TIEMPO");
                ViewBag.Imprimir = new SelectList(listImprimir, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReportePlanillaLlamadasPeriodos")]
        public void GeneraReportePlanillaLlamadasPeriodos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=PLANILLALLAMADASPERIODOS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Retorna la vista para ingresar los parámetros para el reporte.
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReportePlanillaLlamadasPeriodos")]
        public ActionResult ReporteResumenLlamadasPeriodos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                Dictionary<int, string> listImprimir = new Dictionary<int, string>();
                listImprimir.Add(1, "1 - TODOS CON O SIN MARCA DE TIEMPO");
                listImprimir.Add(2, "2 - CON MARCA DE TIEMPO");
                listImprimir.Add(3, "3 - SIN MARCA DE TIEMPO");
                ViewBag.Imprimir = new SelectList(listImprimir, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el resumén de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteResumenLlamadasPeriodos")]
        public void GeneraReporteResumenLlamadasPeriodos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=RESUMENLLAMADASPERIODOS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de resumen de llamadas sitio grabacion
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteResumenLlamadasSitioGraba")]
        public ActionResult ReporteResumenLlamadasSitioGraba()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.SITIO_GRABA = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ViewBag.FIRMA = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.ENTIDAD_FUERZA = new SelectList((from t in db.CADFUERZAS select new { t.ID, DESCRIPCION = t.ID + " - " + t.DESCRIPCION }), "ID", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Envia los parametros al reporte de resumen llamadas sitio de grabacion
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteResumenLlamadasSitioGraba")]
        public void GeneraResumenLlamadasSitioGraba()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=RESUMENSITIOGRABA", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de seguimiento casos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SecadPlusAutorizador("/Reportes/ReporteSeguimientoCasos")]
        public ActionResult ReporteSeguimientoCasos(string strNumeroLLamada = null, string strFechaInicial = null)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.SITIO_GRABA = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ViewBag.FIRMA = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.CODIGO_CASO = new SelectList((from t in db.CADCASOS orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.CALIDAD = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_PEDIDO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.nroLlamada = strNumeroLLamada == null ? "0" : strNumeroLLamada;
                ViewBag.txtFechaInicial = strFechaInicial == null ? DateTime.Now.ToShortDateString() : strFechaInicial;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Envia los parametros al reporte de seguimiento casos
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteSeguimientoCasos")]
        public void GeneraReporteSeguimientoCasos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=SEGUIMIENTOCASOS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de antecedentes por usuario
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SecadPlusAutorizador("/Reportes/ReporteAntecedentesUsuario")]
        public ActionResult ReporteAntecedentesUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.PATRULLAS = new SelectList((from t in db.CADPATRULLAS orderby t.DESC_PATRULLA select new { CODIGO = t.CADUNID_FUERZA + "-" + t.CADUNID_CODIGO + "-" + t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESC_PATRULLA }), "CODIGO", "DESCRIPCION");
                ViewBag.FIRMA = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga pantalla de parámetros para el reporte de antecedentes por usuario
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteAntecedentesUsuario")]
        public void GeneraReporteAntecedentesUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=ANTECEDENTESUSUARIO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="intSitioGraba"></param>
        /// <param name="intEntidad"></param>
        /// <param name="intConsecutivo"></param>
        /// <returns></returns>
        [HttpGet]
        [SecadPlusAutorizador("/Reportes/ReporteRevistas")]
        public ActionResult ReporteRevistas(int? intSitioGraba = null, Int64? intEntidad = null, int? intConsecutivo = null)
        {
            try
            {
                //intSitioGraba = 3947;
                //intEntidad = 776677501;
                //intConsecutivo = 6;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.SITIOS_GRABA = new SelectList((from t in db.CADSITIOS_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION", intSitioGraba);
                ViewBag.ENTIDADES = new SelectList((from t in db.CADFUERZAS orderby t.DESCRIPCION select new { ID = t.ID, DESCRIPCION = t.ID + " - " + t.DESCRIPCION }), "ID", "DESCRIPCION", intEntidad);
                ViewBag.CARGO_FIRMA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.FIRMA = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.CONSECUTIVO = intConsecutivo;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera reporte de revistas
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteRevistas")]
        public void GeneraReporteRevistas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=REVISTAS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Retorna la vista para ingresar los parámetros para el reporte.
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReportePlanillaControlTiempos")]
        public ActionResult ReportePlanillaControlTiempos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                Dictionary<int, string> listTipoPromedio = new Dictionary<int, string>();
                listTipoPromedio.Add(1, "1 - LLAMADA VS LLEGADA");
                listTipoPromedio.Add(2, "2 - ASIGNACION VS LLEGADA");
                listTipoPromedio.Add(3, "3 - LLAMADA VS TERMINO");
                listTipoPromedio.Add(4, "4 - ASIGNACION VS TERMINO");
                ViewBag.TipoPromedio = new SelectList(listTipoPromedio, "Key", "Value");

                Dictionary<int, string> listCondicionRegistro = new Dictionary<int, string>();
                listCondicionRegistro.Add(1, "1 - TODOS LOS REGISTROS");
                listCondicionRegistro.Add(2, "2 - ENTRE TIEMPOS ESTABLECIDOS");
                listCondicionRegistro.Add(3, "3 - MAYOR QUE TIEMPO INICIAL");
                listCondicionRegistro.Add(4, "4 - MENOR QUE TIEMPO INICIAL");
                ViewBag.CondicionRegistro = new SelectList(listCondicionRegistro, "Key", "Value");

                ViewBag.Unidad = new SelectList((from t in BuscaUnidades() select new { t.DESCRIPCION, t.CODIGO }), "CODIGO", "DESCRIPCION");

                Dictionary<int, string> listIntegrantesMedios = new Dictionary<int, string>();
                listIntegrantesMedios.Add(1, "1 - INCLUIR PERSONAS");
                listIntegrantesMedios.Add(2, "2 - SOLO DESCRIPCION UNIDAD");
                ViewBag.IntegrantesMedios = new SelectList(listIntegrantesMedios, "Key", "Value");

                Dictionary<int, string> listNivelInformacion = new Dictionary<int, string>();
                listNivelInformacion.Add(1, "1 - RESUMÉN POR CANALES");
                listNivelInformacion.Add(2, "2 - RESUMÉN POR UNIDADES");
                listNivelInformacion.Add(3, "3 - RESUMÉN POR PATRULLAS");
                ViewBag.NivelInformacion = new SelectList(listNivelInformacion, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el resumén de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReportePlanillaControlTiempos")]
        public void GeneraReportePlanillaControlTiempos()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=PLANILLACONTROLTIEMPOS", true);


                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Retorna la vista para ingresar los parámetros para el reporte.
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteBarriosYCodigo")]
        public ActionResult ReporteBarriosYCodigo()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                Dictionary<string, string> listTipoCodigo = new Dictionary<string, string>();
                listTipoCodigo.Add("C1", "C1 - POR CÓDIGO PRIMARIO REPORTADO");
                listTipoCodigo.Add("C2", "C2 - POR CÓDIGO SECUNDARIO REPORTADO");
                listTipoCodigo.Add("C3", "CR - POR CÓDIGO REAL ASIGNADO");
                ViewBag.PorTipoCodigo = new SelectList(listTipoCodigo, "Key", "Value");

                Dictionary<int, string> listNivelInformacion = new Dictionary<int, string>();
                listNivelInformacion.Add(1, "1 - NIVEL DE BARRIOS");
                listNivelInformacion.Add(2, "2 - NIVEL DE COMUNAS");
                listNivelInformacion.Add(3, "3 - NIVEL DE CIUDADES");
                ViewBag.NivelInformacion = new SelectList(listNivelInformacion, "Key", "Value");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el resumén de llamadas por barrios y código según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteBarriosYCodigo")]
        public void GeneraReporteBarriosYCodigo()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=LLAMADASBARRIOSYCODIGO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Retorna la vista para ingresar los parámetros para el reporte.
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReportePromedioAtencionDias")]
        public ActionResult ReportePromedioAtencionDias()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                Dictionary<int, string> listDetalle = new Dictionary<int, string>();
                listDetalle.Add(0, "0 - DETALLE DIARIO");
                listDetalle.Add(1, "1 - RESUMEN GENERAL");
                ViewBag.Detalle = new SelectList(listDetalle, "Key", "Value");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el resumén de llamadas por barrios y código según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReportePromedioAtencionDias")]
        public void GeneraReportePromedioAtencionDias()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=PROMEDIOATENCIONDIAS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de planilla de llamadas
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteLlamadasDiaHora")]
        public ActionResult ReporteLlamadasDiaHora()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                Dictionary<int, string> listImprimir = new Dictionary<int, string>();
                listImprimir.Add(1, "1 - TODOS CON O SIN MARCA DE TIEMPO");
                listImprimir.Add(2, "2 - CON MARCA DE TIEMPO");
                listImprimir.Add(3, "3 - SIN MARCA DE TIEMPO");
                ViewBag.Imprimir = new SelectList(listImprimir, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteLlamadasDiaHora")]
        public void GeneraReporteLlamadasDiaHora()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=LLAMADASDIAHORA", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de relación de llamadas por teléfono
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteRelacionLlamadasTelefono")]
        public ActionResult ReporteRelacionLlamadasTelefono()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                Dictionary<string, string> listGrupoLLamadas = new Dictionary<string, string>();
                listGrupoLLamadas.Add("01", "01 - LLAMADAS EFECTIVAS");
                listGrupoLLamadas.Add("02", "02 - LLAMADAS NO EFECTIVAS");
                listGrupoLLamadas.Add("03", "03 - OTRAS LLAMADAS");
                listGrupoLLamadas.Add("REMITIDA", "REMITIDA - OTRAS LLAMADAS");
                listGrupoLLamadas.Add("113", "113 - OTRAS LLAMADAS");
                listGrupoLLamadas.Add("NO PROC", "NO PROC - OTRAS LLAMADAS");
                ViewBag.Abreviatura = new SelectList(listGrupoLLamadas, "Key", "Value");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteRelacionLlamadasTelefono")]
        public void GeneraReporteRelacionLlamadasTelefono()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=LLAMADASTELEFONO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte planilla de llamadas por USUARIO
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteLlamadasPorUsuario")]
        public ActionResult ReporteLlamadasPorUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteLlamadasPorUsuario")]
        public void GeneraReporteLlamadasPorUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=LLAMADASUSUARIO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte planilla de llamadas por USUARIO
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteEventosPorUsuario")]
        public ActionResult ReporteEventosPorUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                var calidad = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CALI_PEDIDO" orderby t.CODIGO select new { t.CODIGO, DESCRIPCION = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                calidad.Add(new { CODIGO = "  ", DESCRIPCION = " - TODOS" });

                ViewBag.Calidad = new SelectList(calidad, "CODIGO", "DESCRIPCION", "  ");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteEventosPorUsuario")]
        public void GeneraReporteEventosPorUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=EVENTOSUSUARIO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte planilla de llamadas por USUARIO
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteCasosPorPatrullas")]
        public ActionResult ReporteCasosPorPatrullas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");

                ViewBag.Unidad = new SelectList(BuscaUnidadesPorUsuario(usr), "CODIGO", "DESCRIPCION");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                Dictionary<int, string> listIntegrantesMedios = new Dictionary<int, string>();
                listIntegrantesMedios.Add(1, "1 - INCLUIR PERSONAS");
                listIntegrantesMedios.Add(2, "2 - SOLO DESCRIPCION UNIDAD");
                ViewBag.IntegrantesMedios = new SelectList(listIntegrantesMedios, "Key", "Value", 1);

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteCasosPorPatrullas")]
        public void GeneraReporteCasosPorPatrullas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=CASOSPATRULLAS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Busca Unidades por Usuario para pantalla de parametros del reporte de eventos por patrulla
        /// </summary>
        /// <returns></returns>
        private List<dynamic> BuscaUnidadesPorUsuario(CADUSUARIO usr)
        {
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unde_fuerza", usr.CADUNDE_FUERZA.ToString(), typeof(int)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unid_codigo", usr.CADUNDE_CODIGO.ToString(), typeof(int)));
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttUnidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaUnidadesPorUsuario", strConexion, "Nonquery");
            var arrUnidades = (from t in dttUnidades.AsEnumerable() select new { CODIGO = t["CODIGO"], DESCRIPCION = t["CODIGO"] + " - " + t["DESCRIPCION"] });
            return arrUnidades.ToList<dynamic>();
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte planilla de llamadas por USUARIO
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteEventosEntidadCanal")]
        public ActionResult ReporteEventosEntidadCanal()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Unidad = new SelectList(BuscaUnidadesPorUsuario(usr), "CODIGO", "DESCRIPCION");
                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" });
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.EntidadFuerza = new SelectList(listEntidadFuerza, "Key", "Value", 0);

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por períodos según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteEventosEntidadCanal")]
        public void GeneraReporteEventosEntidadCanal()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=EVENTOSENTIDADCANAL", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte Evaluación Unidad
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteEvaluacionUnidad")]
        public ActionResult ReporteEvaluacionUnidad()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Unidad = new SelectList(BuscaUnidadesPorUsuario(usr), "CODIGO", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por Evaluación Servicio según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteEvaluacionUnidad")]
        public void GeneraReporteEvaluacionUnidad()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=EVALUACIONUNIDAD", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte Evaluación Personal
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteEvaluacionPersonal")]
        public ActionResult ReporteEvaluacionPersonal()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Unidad = new SelectList(BuscaUnidadesPorUsuario(usr), "CODIGO", "DESCRIPCION");

                Dictionary<string, string> listDetalle = new Dictionary<string, string>();
                listDetalle.Add("SI", "01 - VER DETALLE");
                listDetalle.Add("NO", "02 - NO MOSTRAR DETALLES");
                ViewBag.Detalle = new SelectList(listDetalle, "Key", "Value", "SI");

                Dictionary<string, string> listVerTurno = new Dictionary<string, string>();
                listVerTurno.Add("NO", "NO HACER ROMPIMIENTO POR TURNO");
                listVerTurno.Add("SI", "SÍ HACER ROMPIMIENTO POR TURNO");
                ViewBag.VerTurno = new SelectList(listVerTurno, "Key", "Value", "NO");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de llamadas por Evaluación Personal según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteEvaluacionPersonal")]
        public void GeneraReporteEvaluacionPersonal()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=EVALUACIONPERSONAL", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte Última Evaluación
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteUltimaEvaluacion")]
        public ActionResult ReporteUltimaEvaluacion()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte el reporte Última Evaluación según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteUltimaEvaluacion")]
        public void GeneraReporteUltimaEvaluacion()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=RESUMENEVALUACION", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Cargar unidades antecedentes por usuario
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ObtenerUnidadesAjax()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                DatatableForm parametros = new DatatableForm(Request.Form);
                int intControl = 0;
                string strQuery = "";
                double dblControl = 0;
                bool blControl = false;
                List<ExtUnidad> datos = new List<ExtUnidad>();
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"cargarUnidad(_CODIGO, 'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " AND (NIV.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " AND (NIV.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' OR PADRE.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' OR NIV.SIGLA = '" + parametros.ConsultaValor + "') ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strQuery += " ORDER BY NIV.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strQuery += " ORDER BY NIV.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strQuery += " ORDER BY PADRE.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strQuery += " ORDER BY NIV.SIGLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strQuery += " ORDER BY NIV.OPERATIVA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strQuery += " ORDER BY NIV.VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strQuery += " ORDER BY LUGAR.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_FUERZA", usr.CADUNDE_FUERZA.ToString(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.P_OBTENER_UNIDADES", strConexion, "Nonquery");

                parametros.TotalRegistroFiltrados = dttLugar.Rows.Count;
                foreach (DataRow fila in dttLugar.Rows)
                {
                    //Control del páginado de registros
                    if (intControl >= parametros.Inicio && intControl <= parametros.Final)
                    {
                        datos.Add(new ExtUnidad(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", fila["DESCRIPCION"].ToString())));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    }
                    intControl++;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = datos });
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
        /// Carga la página de parámetros para el reporte DataMart
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteCargarDataMart")]
        public ActionResult ReporteCargarDataMart()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.DSP_SIGLA = usr.USUARIO.Split('_')[0];

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte datamart según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteCargarDataMart")]
        public void GeneraReporteCargarDataMart()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=DATAMART", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte de auditoria
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteAuditoria")]
        public ActionResult ReporteAuditoria()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var listTablas = (from t in BuscaTablasAuditoria() select t).ToList();
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Tabla = new SelectList(listTablas, "CODIGO", "DESCRIPCION", 0);

                Dictionary<string, string> listTipoEvento = new Dictionary<string, string>();
                listTipoEvento.Add("TODOS", "1 - TODOS LOS TIPOS");
                listTipoEvento.Add("MODIFICA", "2 - MODIFICA");
                listTipoEvento.Add("BORRA", "3 - BORRA");
                ViewBag.TipoEvento = new SelectList(listTipoEvento, "Key", "Value");

                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de auditoría según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteAuditoria")]
        public void GeneraReporteAuditoria()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=REPORTEAUDITORIA", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Busca tablas de auditoria para cargar en un dropdownlist
        /// </summary>
        /// <returns></returns>
        private List<dynamic> BuscaTablasAuditoria()
        {
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttTablas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaTablasAuditoria", strConexion, "Nonquery");
            var arrTablas = (from t in dttTablas.AsEnumerable() select new { CODIGO = t["DESCRIPCION"], DESCRIPCION = t["DESCRIPCION"] });
            return arrTablas.ToList<dynamic>();
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte estadístico de encuestas
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteEstadisticoEncuestas")]
        public ActionResult ReporteEstadisticoEncuestas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION");
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                ViewBag.Elaboro = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de estadístico de encuestas según los parámetros
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteEstadisticoEncuestas")]
        public void GeneraReporteEstadisticoEncuestas()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=RESTADISTICOENCUESTAS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte listado de usuarios
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteListadoUsuarios")]
        public ActionResult ReporteListadoUsuarios()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var listSitios = (from t in db.CADSITIOS_GRABACION select new { Key = t.CONSECUTIVO, Value = t.CONSECUTIVO + " - " + t.DESCRIPCION }).ToList();
                listSitios.Add(new { Key = 0, Value = "0 - TODOS LOS SITIOS" });
                listSitios.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Sitio = new SelectList(listSitios, "Key", "Value", 0);

                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" });
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Entidad = new SelectList(listEntidadFuerza, "Key", "Value", 0);

                var listCondicion = (from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CONDICION_USUARIO" select new { Key = t.CODIGO, Value = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                listCondicion.Add(new { Key = "X", Value = "X - TODAS LAS CONDICIONES" });
                listCondicion.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Condicion = new SelectList(listCondicion, "Key", "Value", 0);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de administración de usuarios -&gt; listado usuarios
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteListadoUsuarios")]
        public void GeneraReporteListadoUsuarios()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=LISTADOUSUARIOS", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte acta usuario
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteActaUsuario")]
        public ActionResult ReporteActaUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.Cedula = new SelectList(BuscaUsuariosFuerzasEmpleados(), "CEDU_EMPLEADO", "DESCRIPCION", 0);

                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" });
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Fuerza = new SelectList(listEntidadFuerza, "Key", "Value", 0);

                var listCondicion = (from t in db.CADREFERENCIASSECADs
                                     where t.NOMBRE == "CONDICION_USUARIO"
                                     select new { Key = t.CODIGO, Value = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                listCondicion.Add(new { Key = "X", Value = "X - TODAS LAS CONDICIONES" });
                listCondicion.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Condicion = new SelectList(listCondicion, "Key", "Value", 0);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de administración de usuarios -&gt; acta usuario
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteActaUsuario")]
        public void GeneraReporteActaUsuario()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=ACTAUSUARIO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        private List<dynamic> BuscaUsuariosFuerzasEmpleados()
        {
            GestorGeneral objGestorGral = new GestorGeneral();
            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
            string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(Oracle.ManagedDataAccess.Client.OracleDbType)));
            DataTable dttUsuarios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listadoUsuariosEmpleados", strConexion, "Nonquery");
            var arrUsuarios = (from t in dttUsuarios.AsEnumerable()
                               select new
                               {
                                   CEDU_EMPLEADO = t["CEDU_EMPLEADO"],
                                   DESCRIPCION = t["CEDU_EMPLEADO"] + " - " + t["NOMB_EMPLEADO"] + " " + t["GRAD_ALFABETICO"] + " " + t["DESCRIPCION"]
                               });
            return arrUsuarios.ToList<dynamic>();
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte acta general
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteActaGeneral")]
        public ActionResult ReporteActaGeneral()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.Cedula = new SelectList(BuscaUsuariosFuerzasEmpleados(), "CEDU_EMPLEADO", "DESCRIPCION", 0);

                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" });
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Fuerza = new SelectList(listEntidadFuerza, "Key", "Value", 0);

                var listCondicion = (from t in db.CADREFERENCIASSECADs
                                     where t.NOMBRE == "CONDICION_USUARIO"
                                     select new { Key = t.CODIGO, Value = t.CODIGO + " - " + t.DESCRIPCION }).ToList();
                listCondicion.Add(new { Key = "X", Value = "X - TODAS LAS CONDICIONES" });
                listCondicion.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Condicion = new SelectList(listCondicion, "Key", "Value", 0);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de administración de usuarios -&gt; acta general
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteActaGeneral")]
        public void GeneraReporteActaGeneral()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=ACTAUSUARIO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte geonumérico
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteParteGeonumerico")]
        public ActionResult ReporteParteGeonumerico(string strFechaInicio = null, string strFechaFinal = null)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                //listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" }); /*ESTA OPCIÓN NO TRAE DATOS EN SECAD POR ELLO SE DESACTIVA*/
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Fuerza = new SelectList(listEntidadFuerza, "Key", "Value", 0);

                Dictionary<string, string> listCondicion = new Dictionary<string, string>();
                listCondicion.Add("01", "01 - VER LISTO DE PERSONAL");
                listCondicion.Add("02", "02 - PARTE GEONUMERICO POR UNIDAD");
                ViewBag.Condicion = new SelectList(listCondicion, "Key", "Value", "01");
                ViewBag.FechaInicio = strFechaInicio;
                ViewBag.FechaFinal = strFechaFinal;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de geonumérico
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteParteGeonumerico")]
        public void GeneraReporteParteGeonumerico()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=PARTEGEONUMERICO", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte minuta de servicio
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteMinutaServicioAtencion")]
        public ActionResult ReporteMinutaServicioAtencion(int intSitio, int intEntidad, int? intTurno)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION", intSitio);
                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Fuerza = new SelectList(listEntidadFuerza, "Key", "Value", intEntidad);
                ViewBag.Turno = intTurno;
                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.CargoResponsable = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");



                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { intSitio = intSitio, intEntidad = intEntidad, intTurno = intTurno });
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte minuta de servicio
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteMinutaServicioAtencion")]
        public void GeneraReporteMinutaServicioAtencion()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=MINUTASERVICIOATENCION", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }

        /// <summary>
        /// Carga la página de parámetros para el reporte anotaciones generales
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Reportes/ReporteAnotacionesGenerales")]
        public ActionResult ReporteAnotacionesGenerales(int intSitio, int intNumeroLlamada, string dFechaAnotacion)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.Sitio = new SelectList((from t in db.CADSITIOS_GRABACION where t.CONSECUTIVO == usr.SITIO_GRABACION orderby t.DESCRIPCION select new { t.CONSECUTIVO, DESCRIPCION = t.CONSECUTIVO + " - " + t.DESCRIPCION }), "CONSECUTIVO", "DESCRIPCION", intSitio);
                var listEntidadFuerza = (from t in db.CADFUERZAS select new { Key = t.ID, Value = t.ID + " - " + t.DESCRIPCION }).ToList();
                listEntidadFuerza.Add(new { Key = 0, Value = "0 - TODAS LAS ENTIDADES" });
                listEntidadFuerza.Sort((obj1, obj2) => obj1.Key.CompareTo(obj2.Key));
                ViewBag.Entidad = new SelectList(listEntidadFuerza, "Key", "Value", "0");
                ViewBag.Consecutivo = intNumeroLlamada;
                ViewBag.Responsable = new SelectList((from t in db.CADREFERENCIAS where t.SITIO_GRABA == usr.SITIO_GRABACION && t.NOMBRE == "FIRMANTES" select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.CargoResponsable = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "CARGO_FIRMA" orderby t.CODIGO select new { t.DESCRIPCION }), "DESCRIPCION", "DESCRIPCION");
                ViewBag.FechaInicio = dFechaAnotacion;
                ViewBag.FechaFinal = DateTime.Parse(dFechaAnotacion).AddDays(1).ToString("dd/MM/yyyy hh:mm:ss");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Genera el reporte de anotaciones generales
        /// </summary>
        [HttpPost]
        [SecadPlusAutorizador("/Reportes/ReporteAnotacionesGenerales")]
        public void GeneraReporteAnotacionesGenerales()
        {
            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    Server.TransferRequest("~/ReportesWeb/Reportes.aspx?REPORTE=ANOTACIONESGENERALES", true);
                }
                else
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    Server.TransferRequest("~/Controllers/Home/Index");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                Server.TransferRequest("~/Controllers/Home/Index");
            }
        }
    }
}