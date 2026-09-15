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
    public class LocalizacionMediosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));

        private GestorGeneral objGestorGral;

        [SecadPlusAutorizador("/LocalizacionMedios/Index")]
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
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// <summary>
        /// Metodo que obtiene localizacion medios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerLocalizacionMedios(string strMedios = null, string strCiudad = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLocalizacionMedios;

                List<Ext_LocalizacionMedios> listaLocalizacionMedios = new List<Ext_LocalizacionMedios>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (strMedios != "" && strMedios != null)
                {
                    blFiltro = true;

                    strQuery = " WHERE (UPPER(DESC_PATRULLA) = UPPER('" + strMedios + "')) ";

                    if (strCiudad != "" && strCiudad != null)
                        strQuery += " AND (UPPER(MUNICIPIO) = UPPER('" + strCiudad + "')) ";
                }

                if (strCiudad != "" && strCiudad != null)
                {
                    blFiltro = true;

                    strQuery = " WHERE (UPPER(MUNICIPIO) = Upper( '" + strCiudad + "')) ";

                    if (strMedios != "" && strMedios != null)
                        strQuery += " AND (UPPER(DESC_PATRULLA) = Upper('" + strMedios + "')) ";
                }

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    parametros.ConsultaValor = parametros.ConsultaValor.TrimStart();

                    if (strQuery != "")
                    {
                        strQuery += " AND REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (FECHA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (DIRE_UBICACION, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (MUNICIPIO, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (X_COORD_UBICA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (Y_COORD_UBICA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (VIGENTE, '" + parametros.ConsultaValor + "', 'i') ";
                    }
                    else
                    {
                        blFiltro = true;

                        strQuery = " WHERE REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (FECHA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (DIRE_UBICACION, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (MUNICIPIO, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (X_COORD_UBICA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (Y_COORD_UBICA, '" + parametros.ConsultaValor + "', 'i') " +
                                   " OR REGEXP_LIKE (VIGENTE, '" + parametros.ConsultaValor + "', 'i') ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY MUNICIPIO ASC ";
                        break;

                    case 1:
                        strOrderna += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY FECHA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DIRE_UBICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY MUNICIPIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY X_COORD_UBICA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY Y_COORD_UBICA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttLocalizacionMedios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaLocalizacionMedios", strConexion, "Nonquery");

                foreach (DataRow fila in dttLocalizacionMedios.Rows)
                {
                    listaLocalizacionMedios.Add(new Ext_LocalizacionMedios(fila, ""));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaLocalizacionMedios });
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
        /// Metodo que obtiene los medios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerMedios()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick=\"CargarMedio(CODIGO,DESCRIPCION)\" title='' data-placement ='bottom' data-original-title='Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttMedios;
                List<_MediosModel> listaMedios = new List<_MediosModel>();
                string turnoA = Convert.ToString(ObtenerTurnoAccesorio());
                string turnoN = Convert.ToString(ObtenerTurnoNormal());
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (ESTADO_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (ESTADO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (TIPO, '" + parametros.ConsultaValor + "', 'i') " +
                             " OR REGEXP_LIKE (CUENTA, '" + parametros.ConsultaValor + "', 'i') ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY REGISTRO.DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabacion", usr.SITIO_GRABACION.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoA", turnoA, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoN", turnoN, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttMedios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaMediosNovedadesPatrulla", strConexion, "Nonquery");

                foreach (DataRow fila in dttMedios.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CONSECUTIVO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESC_PATRULLA"].ToString() + "'");
                    listaMedios.Add(new _MediosModel("", strBotonSelect, fila["DESC_PATRULLA"].ToString(), fila["SITIO_GRABA"].ToString(), fila["CONSECUTIVO"].ToString(), fila["ESTADO_PATRULLA"].ToString(), fila["TIPO"].ToString(), fila["ESTADO"].ToString(), fila["CUENTA"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaMedios });
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
        /// Metodo que obtiene ciudades
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargaCiudades()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";

                strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarCiudad(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca Ciudades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                int? intLugeCodigo = usr.LUGE_CODIGO_DPTO;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_Ciudades> listaCiudades = new List<Ext_Ciudades>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";
                    int codigoBarr;
                    bool result = Int32.TryParse(parametros.ConsultaValor, out codigoBarr);
                    if (result)
                    {
                        strQuery += " OR CODIGO LIKE ('%" + codigoBarr + "%') ";
                    }
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                // strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                if (strQuery == string.Empty)
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                else
                    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaCiudadesAsignarBarrios", strConexion, "Nonquery");

                foreach (DataRow fila in dttCiudades.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaCiudades.Add(new Ext_Ciudades(fila, strBotonSelect));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCiudades });
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
        /// Metodo que obtiene un turno normal
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerTurnoNormal()
        {
            decimal turnoN = 0;

            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                objGestorGral = new GestorGeneral();

                var claseTurno = (from t in db.CADCANALES join a in db.CADFUERZAS on t.CADFUERZ_ID equals a.ID where a.ID == usr.CADCANA_FUERZA_ID && t.CODIGO == usr.CADCANA_CODIGO select t.CLASE_TURNO).FirstOrDefault();

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", objGestorGral.ObtenerTiempoServidor.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clasturno", claseTurno.ToString(), typeof(string)));

                turnoN = (decimal)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_turnoNormal", strConexion);
                return turnoN;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que obtiene turno accesorio
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerTurnoAccesorio()
        {
            decimal turnoA = 0;

            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", objGestorGral.ObtenerTiempoServidor.ToString(), typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clasturno", "03", typeof(string)));

                turnoA = (decimal)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.F_TURNO_ACCESORIO", strConexion);
                return turnoA;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
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
    }
}