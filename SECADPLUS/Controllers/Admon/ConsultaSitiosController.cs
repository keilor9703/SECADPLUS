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
    public class ConsultaSitiosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: ConsultaSitios
        [SecadPlusAutorizador("/ConsultaSitios/Index")]
        public ActionResult Index()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View(db.V_FORM_SITIOS_GRABACION.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult VerParqueaderos(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + id + " ", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_ParqueaderosModel> listaParqueaderos = new List<_ParqueaderosModel>();
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
                    strQuery += " AND (SITIO_GRABA = " + id + " AND REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (NOMBRECIUDAD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (NOMBREBARRIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (DIRECCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (TELEFONO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (HORARIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (USUA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (FECHA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i')) ";
                    //}
                    //else
                    //{
                    //strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')";
                    //}
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY NOMBRECIUDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY NOMBREBARRIO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY DIRECCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strOrderna += " ORDER BY TELEFONO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 7:
                        strOrderna += " ORDER BY HORARIO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 8:
                        strOrderna += " ORDER BY USUA_TRANSACCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 9:
                        strOrderna += " ORDER BY FECHA_TRANSACCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaParqueaderos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaParqueaderos.Add(new _ParqueaderosModel("", fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["NOMBRECIUDAD"].ToString(), fila["NOMBREBARRIO"].ToString(),
                                                                     fila["DIRECCION"].ToString(), fila["TELEFONO"].ToString(), fila["HORARIO"].ToString(), fila["USUA_TRANSACCION"].ToString(), fila["FECHA_TRANSACCION"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaParqueaderos });
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
        public JsonResult VerSitiosImportantes(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + id + " ", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                List<_SitiosImportantesModel> listaImportantes = new List<_SitiosImportantesModel>();
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
                    strQuery += " AND (SITIO_GRABA = " + id + " AND REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (CLASESITIODESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (NOMBRECIUDAD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (NOMBREBARRIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (DIRECCION, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (LATITUD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (LONGITUD, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (TELEFONO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + id + " AND REGEXP_LIKE (COMENTARIO, '" + parametros.ConsultaValor + "', 'i')) ";
                    //}
                    //else
                    //{
                    //strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')";
                    //}
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY CLASESITIODESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY NOMBRECIUDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY NOMBREBARRIO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strOrderna += " ORDER BY DIRECCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 7:
                        strOrderna += " ORDER BY LATITUD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 8:
                        strOrderna += " ORDER BY LONGITUD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 9:
                        strOrderna += " ORDER BY TELEFONO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 10:
                        strOrderna += " ORDER BY COMENTARIO " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_sitios_importantes", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    listaImportantes.Add(new _SitiosImportantesModel("", fila["CONSECUTIVO"].ToString(), fila["CLASESITIODESCRIPCION"].ToString(), fila["DESCRIPCION"].ToString(), fila["NOMBRECIUDAD"].ToString(), fila["NOMBREBARRIO"].ToString(),
                                                                     fila["DIRECCION"].ToString(), fila["LATITUD"].ToString(), fila["LONGITUD"].ToString(), fila["TELEFONO"].ToString(), fila["COMENTARIO"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaImportantes });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
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