using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class DirectorioController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorGeneral objGestorGral;

        // GET: Directorio
        [SecadPlusAutorizador("/Directorio/Index")]
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
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult ObtenerDirectorio(string cadenaFiltros)
        {
            try
            {
                string[] arrayFiltros = cadenaFiltros.Split(';');
                string strPrimeraFiltro = "";
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttDirectorio;

                List<_DirectorioModel> listaDirectorio = new List<_DirectorioModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros

                for (int i = 0; i < arrayFiltros.Length; i++)
                {
                    string[] filtro = arrayFiltros[i].Split('=');
                    if (filtro[1] != "")
                    {
                        switch (filtro[0])
                        {
                            case "telefono":
                                strQuery += " WHERE REGEXP_LIKE (NUME_TELEFONO, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "propietario":
                                strQuery += " WHERE REGEXP_LIKE (NOMB_PROPIETARIO, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "nit":
                                strQuery += " WHERE REGEXP_LIKE (NIT, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "direccion":
                                strQuery += " WHERE REGEXP_LIKE (DIRE_PROPIETA, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "barrio":
                                strQuery += " WHERE REGEXP_LIKE (BARRIO, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "codigoMunicipio":
                                strQuery += " WHERE REGEXP_LIKE (CADLUGE_CODIGO, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "codigoBarrAntencion":
                                strQuery += " WHERE REGEXP_LIKE (BARR_CODIGO, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;

                            case "barrAtencion":
                                strQuery += " WHERE REGEXP_LIKE (BAR.DESCRIPCION, '" + filtro[1] + "', 'i') ";
                                strPrimeraFiltro = filtro[0];
                                break;
                        }
                    }

                    if (strQuery != "")
                        break;
                }

                for (int j = 0; j < arrayFiltros.Length; j++)
                {
                    string[] filtro = arrayFiltros[j].Split('=');
                    if (filtro[1] != "")
                    {
                        switch (filtro[0])
                        {
                            case "telefono":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (NUME_TELEFONO, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "propietario":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (NOMB_PROPIETARIO, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "nit":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (NIT, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "direccion":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (DIRE_PROPIETA, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "codigoMunicipio":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (CADLUGE_CODIGO, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "barrio":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (BARRIO, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "codigoBarrAntencion":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (BARR_CODIGO, '" + filtro[1] + "', 'i') " : "";
                                break;

                            case "barrAtencion":
                                strQuery += strPrimeraFiltro != filtro[0] ? " AND REGEXP_LIKE (BAR.DESCRIPCION, '" + filtro[1] + "', 'i') " : "";
                                break;
                        }
                    }
                }

                if (strQuery != "")
                {
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        parametros.ConsultaValor = parametros.ConsultaValor.TrimStart();
                        blFiltro = true;

                        Int64 intParametro = 0;
                        if (Int64.TryParse(parametros.ConsultaValor, out intParametro))
                        {
                            strQuery += " AND NUME_TELEFONO = '" + intParametro + "' " +
                              " OR NIT = '" + intParametro + "' " +
                              " OR CADLUGE_CODIGO = '" + intParametro + "' " +
                              " OR BARR_CODIGO = '" + intParametro + "' ";
                        }
                        else
                        {
                            strQuery += " AND UPPER(NUME_TELEFONO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(NOMB_PROPIETARIO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(DIRE_PROPIETA) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(BARRIO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(NIT) = UPPER('" + parametros.ConsultaValor + "') ";
                        }

                        //blFiltro = true;

                        //strQuery = " WHERE REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (NOMB_PROPIETARIO, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (DIRE_PROPIETA, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (NIT, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (CADLUGE_CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                        //           " OR REGEXP_LIKE (BARR_CODIGO, '" + parametros.ConsultaValor + "', 'i') ";
                    }
                    //Verifica si se deben realizar ordenamientos
                    switch (parametros.IndiceOrdenamiento)
                    {
                        case 1:
                            strOrderna += " ORDER BY NUME_TELEFONO " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 2:
                            strOrderna += " ORDER BY NOMB_PROPIETARIO " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 3:
                            strOrderna += " ORDER BY NIT " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 4:
                            strOrderna += " ORDER BY DIRE_PROPIETA " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 5:
                            strOrderna += " ORDER BY BARRIO " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 6:
                            strOrderna += " ORDER BY CADLUGE_CODIGO " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 7:
                            strOrderna += " ORDER BY LG.DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 8:
                            strOrderna += " ORDER BY BARR_CODIGO " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 9:
                            strOrderna += " ORDER BY BAR.DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                            break;
                    }
                    strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttDirectorio = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaDirectorio", strConexion, "Nonquery");

                    foreach (DataRow fila in dttDirectorio.Rows)
                    {
                        listaDirectorio.Add(new _DirectorioModel("", "", fila["NUME_TELEFONO"].ToString(), fila["NOMB_PROPIETARIO"].ToString(), fila["DIRE_PROPIETA"].ToString(), fila["BARRIO"].ToString(), fila["NIT"].ToString(), fila["CADLUGE_CODIGO"].ToString(), fila["CADLUGE"].ToString(), fila["BARR_CODIGO"].ToString(), fila["BARRIO_ASIGNADO"].ToString()));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                    if (!blFiltro)
                    {
                        parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                    }
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaDirectorio });
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
        /// Busca ciudades para seleccionar
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargaCiudades(int intLugar)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarCiudad(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";
                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                int? intLugeCodigo = 0;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca Ciudades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                intLugeCodigo = usr.LUGE_CODIGO_DPTO;
                if (intLugar != 0)
                {
                    intLugeCodigo = intLugar;
                }

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
        /// Busca barrios por ciudad
        /// </summary>
        /// <param name="lugeCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargarTodosLosBarrios(string lugeCodigo = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarBarrio(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttBarrios;
                List<_BarriosModel> listaBarrios = new List<_BarriosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                /// strQuery = " WHERE (LUGE_CODIGO = " + lugeCodigo + ") ";

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " WHERE (upper(CODIGO) LIKE upper('%" + parametros.ConsultaValor + "%') ) " +
                                " OR (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESCRIPCION ASC ";

                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_lugar", lugeCodigo, typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosLugarBienes", strConexion, "Nonquery");

                foreach (DataRow fila in dttBarrios.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaBarrios.Add(new _BarriosModel("", strBotonSelect, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaBarrios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}