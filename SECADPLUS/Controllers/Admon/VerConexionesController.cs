using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class VerConexionesController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: VerConexiones
        [SecadPlusAutorizador("/VerConexiones/Index")]
        public ActionResult Index(string strMensaje = null)
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
                int tiempoControl = 0;

                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" orderby condicion.DESCRIPCION select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion");
                var tiempoRefresco = db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "TIEMPO_CONTROL_USUARIOS").Select(c => c.DESCRIPCION).FirstOrDefault();

                if (!int.TryParse(tiempoRefresco, out tiempoControl))
                    ViewBag.tiempoRefresco = 15000; // 15 segundos por defecto
                else
                    ViewBag.tiempoRefresco = int.Parse(tiempoRefresco);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult ObtenerConexiones(string strTipoConexion = null, string strCondicion = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttConexiones;

                List<_VerConexionesModel> listaConexiones = new List<_VerConexionesModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros

                if (strTipoConexion != "")
                {
                    blFiltro = true;

                    strQuery = " WHERE (UPPER(VIGENTE) = UPPER('" + strTipoConexion + "')) ";

                    if (strCondicion != "")
                        strQuery += " AND (UPPER(CONDICION) = UPPER('" + strCondicion + "')) ";
                }

                if (strCondicion != "")
                {
                    blFiltro = true;

                    strQuery = " WHERE (UPPER(CONDICION) = Upper( '" + strCondicion + "')) ";

                    if (strTipoConexion != "")
                        strQuery += " AND (UPPER(VIGENTE) = Upper('" + strTipoConexion + "')) ";
                }

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    parametros.ConsultaValor = parametros.ConsultaValor.TrimStart();

                    strQuery += " AND UPPER(DESC_CONDICION) = UPPER('" + parametros.ConsultaValor + "') " +
                    " OR UPPER(GRAD_ALFABETICO) = UPPER('" + parametros.ConsultaValor + "') " +
                    " OR UPPER(NOMB_EMPLEADO) = UPPER('" + parametros.ConsultaValor + "') " +
                    " OR UPPER(SESION_ID) = UPPER('" + parametros.ConsultaValor + "') " +
                    " OR UPPER(MAQUINA) = UPPER('" + parametros.ConsultaValor + "') " +
                    " OR UPPER(CADUSUA_USUARIO) = UPPER('" + parametros.ConsultaValor + "')" +
                    " OR REGEXP_LIKE(FECHA_INICIO, '" + parametros.ConsultaValor + "', 'i') " +
                    " OR REGEXP_LIKE(FECHA_HASTA, '" + parametros.ConsultaValor + "', 'i') ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY DESC_CONDICION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY GRAD_ALFABETICO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY NOMB_EMPLEADO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA_INICIO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY FECHA_HASTA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strOrderna += " ORDER BY SESION_ID " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 7:
                        strOrderna += " ORDER BY MAQUINA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 8:
                        strOrderna += " ORDER BY CADUSUA_USUARIO " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttConexiones = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaConexiones", strConexion, "Nonquery");

                foreach (DataRow fila in dttConexiones.Rows)
                {
                    listaConexiones.Add(new _VerConexionesModel("", "", fila["DESC_CONDICION"].ToString(), fila["GRAD_ALFABETICO"].ToString(), fila["NOMB_EMPLEADO"].ToString(), fila["FECHA_INICIO"].ToString(),
                        fila["FECHA_HASTA"].ToString(), fila["SESION_ID"].ToString(), fila["MAQUINA"].ToString(), fila["CADUSUA_USUARIO"].ToString(), fila["CADCANA_FUERZA_ID"].ToString(), fila["CADCANA_CODIGO"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaConexiones });
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
        public JsonResult ObtenerDatosFuerzaCanal(string idfuerza, string idCanal)
        {
            try
            {
                string strFuerza = string.Empty;
                string strCanal = string.Empty;

                GestorGeneral objGestorGral;

                //consulta funcion para traer la descripcion de fuerza por id
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", idfuerza, typeof(int)));
                strFuerza = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_desc_fuerza", strConexion);

                //consulta funcion para traer la descripcion de canal por fuerza y codigo
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", idfuerza, typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", idCanal, typeof(int)));
                strCanal = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_desc_canal", strConexion);

                var resultado = Json(new { result = true, fuerza = strFuerza, canal = strCanal });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult CerrarSesiones()
        {
            try
            {
                decimal decValor = 0;

                GestorGeneral objGestorGral;

                //consulta funcion para traer las sesiones cerradas
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                decValor = objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_cerrarSesionesActivas", strConexion);

                var resultado = Json(new { result = true, cerradas = decValor });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}