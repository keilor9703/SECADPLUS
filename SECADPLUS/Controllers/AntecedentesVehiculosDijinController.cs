using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class AntecedentesVehiculosDijinController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;

        // GET: AntecedentesVehiculosDijin
        [SecadPlusAutorizador("/AntecedentesVehiculosDijin/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr.CADCANA_CODIGO == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_CANAL_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View("Index", "BaseRecepcion");
        }

        /// <summary>
        /// Metodo que carga vehiculos
        /// </summary>
        /// <param name="strTipoBusqueda"></param>
        /// <param name="filtroBusqueda"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerVehiculos(string strTipoBusqueda = null, string filtroBusqueda = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                int intContadorRegistros = 0;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttVehiculos;
                List<Ext_V_Vehiculos> listaVehiculos = new List<Ext_V_Vehiculos>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length == 0 && strTipoBusqueda != null && filtroBusqueda != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoBusqueda", strTipoBusqueda, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_placa", filtroBusqueda, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_motor", filtroBusqueda, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_serie", filtroBusqueda, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_chasis", filtroBusqueda, typeof(string)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttVehiculos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_vehiculosDijinrUnoAuno", strConexion, "Nonquery");

                    foreach (DataRow fila in dttVehiculos.Rows)
                    {
                        intContadorRegistros += 1;
                        listaVehiculos.Add(new Ext_V_Vehiculos(fila));
                    }
                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
                    blFiltro = true;
                }

                parametros.TotalRegistros = intContadorRegistros;
                parametros.TotalRegistroFiltrados = intContadorRegistros;

                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaVehiculos });
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
        /// Metodo que guarda la auditoria dijin
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult DijinAntecedentesVehiculos(string strfiltroBusqueda = null, string strDatosAuditoria = null)
        {
            try
            {
                Dictionary<string, string> datosAntecedentesVehiculos = new Dictionary<string, string>();
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                string strDesFuerza = ObtenerDescFuerza(usr.CADCANA_FUERZA_ID.ToString());
                string strDesCanal = ObtenerCanal(usr.CADCANA_FUERZA_ID.ToString(), usr.CADCANA_CODIGO.ToString());
                string strMaquina = ObtenerMaquina();
                string strBtnTem = "";
                string[] datosBienes = ObtenerBienImplicado(strfiltroBusqueda.ToUpper()).Split(';');
                string[] arrayDatosAuditoria = strDatosAuditoria == null ? null : strDatosAuditoria.Split(';');
                string strConsultaTemporal = "F";
                string mesajeDijin = "";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (datosBienes.Length > 1)
                {
                    strBtnTem = "<a type='button' class='btn btn-xs btn-warning' href='" + (Url.Action("VehiculosDijinNovedades", "AntecedentesVehiculosDijin", new { intIdConsecutivo = datosBienes[0], intSitioGraba = datosBienes[1] })).ToString() + "' target='_blank' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para Temporales' > " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Ir a Temporales  " +
                                 "</a> <label class='bgcolor-amarillo-claro label'> Vehículos con Anotaciones en Temporales </label>  <br />   <br /> ";
                    strConsultaTemporal = "V";
                }

                if (arrayDatosAuditoria != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_placa", arrayDatosAuditoria[0], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_motor", arrayDatosAuditoria[1], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_serie", arrayDatosAuditoria[2], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_chais", arrayDatosAuditoria[3], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultado_consulta", arrayDatosAuditoria[4], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_campo_busqueda", arrayDatosAuditoria[5], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_criterio_busqueda", arrayDatosAuditoria[6], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_local", strConsultaTemporal, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_consulta_dijin", arrayDatosAuditoria[7], typeof(string)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strDesFuerza, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", strDesCanal, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_unde_fuerza", arrayDatosAuditoria[8] == "" ? "0" : arrayDatosAuditoria[8], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_unde_codigo", arrayDatosAuditoria[9] == "" ? "0" : arrayDatosAuditoria[9], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_codi_patrulla", arrayDatosAuditoria[10] == "" ? "0" : arrayDatosAuditoria[10], typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_patrulla", arrayDatosAuditoria[11], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_policial", arrayDatosAuditoria[12], typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", usr.USUARIO, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_maquina", strMaquina, typeof(string)));

                    objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                    objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_consecutivo", null, typeof(decimal)));
                    objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("P_mensaje", null, typeof(string)));

                    objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.p_cadcon_vehiculos", strConexion, "MULTIPLE");

                    foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                    {
                        datosAntecedentesVehiculos.Add(parameter.Item1, parameter.Item2.ToString());
                    }

                    if (datosAntecedentesVehiculos["p_consecutivo"] != "null")
                    {
                        mesajeDijin = "Registro : " + datosAntecedentesVehiculos["p_consecutivo"] + "  " + datosAntecedentesVehiculos["P_mensaje"];
                    }
                }

                var resultados = Json(new { result = true, btnTem = strBtnTem, msDijin = mesajeDijin });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [SecadPlusAutorizador("/AntecedentesVehiculosDijin/VehiculosDijinNovedades")]
        public ActionResult VehiculosDijinNovedades(Int64? intIdConsecutivo, int intSitioGraba)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var maquina = db.CADCONEXIONs.Where(c => c.CADUSUA_USUARIO == usr.USUARIO && c.VIGENTE == "S").OrderByDescending(c => c.SESION_ID).Select(c => c.MAQUINA).First();

                if (string.IsNullOrWhiteSpace(maquina))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                var cADBIENES = db.CADBIENES.Include(c => c.CADCLASES_BIEN).Include(c => c.CADLINEAS_MARCA).Include(c => c.CADSITIOS_GRABACION).Include(c => c.CADCOLORE).Where(c => c.SITIO_GRABA == intSitioGraba && c.CONSECUTIVO == intIdConsecutivo);
                var cADBIENES_IMPLICADOS = db.CADBIENES_IMPLICADOS.Include(c => c.CADBIENE).Include(c => c.CADLUGARES_GEOGRAFICOS).Include(c => c.CADCASO).Include(c => c.CADBARRIO).Where(c => c.CADBIEN_SITIOGRABA == intSitioGraba && c.CADBIEN_CONSECUTIVO == intIdConsecutivo);

                var variosModelos = new Tuple<List<CADBIENE>, List<CADBIENES_IMPLICADOS>>
                    (cADBIENES.ToList(), cADBIENES_IMPLICADOS.ToList())
                { };

                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que obtiene consecutivo y sitio graba de un bien
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <returns></returns>
        public string ObtenerBienImplicado(string strFiltro)
        {
            string datos = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));

                datos = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consecutivoBienImplicado", strConexion);

                return datos;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que ontiene la descripcion de la fuerza
        /// </summary>
        /// <param name="strFuerzaID"></param>
        /// <returns></returns>
        public string ObtenerDescFuerza(string strFuerzaID)
        {
            string mensaje = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerzaID, typeof(decimal)));

                mensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cad.f_desc_fuerza", strConexion);

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que obtiene descripcion canal
        /// </summary>
        /// <param name="strFuerzaID"></param>
        /// <param name="strCanalCodigo"></param>
        /// <returns></returns>
        public string ObtenerCanal(string strFuerzaID, string strCanalCodigo)
        {
            string mensaje = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerzaID, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", strCanalCodigo, typeof(decimal)));

                mensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cad.f_desc_canal", strConexion);

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que obtiene el equipo donde se esta ejecutando la aplicacion.
        /// </summary>
        public string ObtenerMaquina()
        {
            string strNombreEquipo = "";

            try
            {
                IPHostEntry host = new IPHostEntry();
                host = Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]);
                strNombreEquipo = host.HostName;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
            }

            return strNombreEquipo;
        }
    }
}