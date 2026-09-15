using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class AdministrarDirectorioController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));

        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorGeneral objGestorGral;

        // GET: AdministrarDirectorio
        [SecadPlusAutorizador("/AdministrarDirectorio/Index")]
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

                ViewBag.UnidadDSP = ObtenerUnidad();

                ////Tamaño máximo del archivo de carga
                int maxRequestLength = 0;
                HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
                if (section != null)
                {
                    maxRequestLength = section.MaxRequestLength;
                    ViewBag.longMaxArchivo = maxRequestLength * 1024;
                }

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: AdministrarDirectorio/ProcesarArchivoPlano To protect from overposting attacks,
        // please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcesarArchivoPlano(HttpPostedFileBase uploader, string codigoLugar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploader == null)
                    {
                        ViewBag.MENSAJE_EXITOSO = "Debe seleccionar un archivo .txt";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                        return View("Index");
                    }
                    else if (codigoLugar == "")
                    {
                        ViewBag.MENSAJE_EXITOSO = "Debe seleccionar una ciudad o municipio";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                        return View("Index");
                    }
                    else
                    {
                        var fileName = Path.GetFileName(uploader.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Document"), fileName);
                        DataTable dtInfoLog = null;
                        uploader.SaveAs(filePath);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string[] strLineas = System.IO.File.ReadAllLines(Path.Combine(Server.MapPath("~/Document"), fileName));

                            //consulta funcion para traer el resultado del archivo procesado
                            objGestorGral = new GestorGeneral();
                            objGestorGral.strArreglo = strLineas;
                            objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_informacion", "", typeof(string[])));
                            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadlugeCodigo", codigoLugar.Split('-')[0], typeof(decimal)));
                            objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));

                            dtInfoLog = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_cargarDirectorio", strConexion, "Nonquery");
                            CargarLog(dtInfoLog);
                        }
                    }
                }

                return RedirectToAction("Index", "AdministrarDirectorio");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que carga el directorio
        /// </summary>
        /// <param name="cadenaFiltros"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerDirectorio(string cadenaFiltros)
        {
            try
            {
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

                if (cadenaFiltros == "barriosNulls")
                {
                    strQuery += " WHERE BARR_CODIGO IS NULL";
                }

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    parametros.ConsultaValor = parametros.ConsultaValor.TrimStart();
                    blFiltro = true;
                    if (cadenaFiltros != "barriosNulls")
                    {
                        Int64 intParametro = 0;
                        if (Int64.TryParse(parametros.ConsultaValor, out intParametro))
                        {
                            strQuery += " WHERE NUME_TELEFONO = '" + intParametro + "' " +
                              " OR NIT = '" + intParametro + "' " +
                              " OR CADLUGE_CODIGO = '" + intParametro + "' " +
                              " OR BARR_CODIGO = '" + intParametro + "' ";
                        }
                        else
                        {
                            strQuery += " WHERE UPPER(NUME_TELEFONO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(NOMB_PROPIETARIO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(DIRE_PROPIETA) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(BARRIO) = UPPER('" + parametros.ConsultaValor + "') " +
                            " OR UPPER(NIT) = UPPER('" + parametros.ConsultaValor + "') ";
                        }
                    }
                    else
                    {
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
                    }
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
        public JsonResult CargaCiudades(int tab)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";
                if (tab == 1)
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                           "onclick = \"CargarCiudad(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                           " Seleccionar </button>";
                }
                else
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                               "onclick = \"CargarCiudadDirectorio(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                               " Seleccionar </button>";
                }
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

        /// <summary>
        /// Metodo que actualiza los barrios por ciudad
        /// </summary>
        /// <param name="strCodigoLugar"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ProcesarAutomaticoCiudades(string strCodigoLugar)
        {
            try
            {
                string respuesta = string.Empty;
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_dsp_cod_ciudad", strCodigoLugar.Split('-')[0], typeof(decimal)));

                respuesta = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_procesoDirectorioCiudades", strConexion);

                var resultado = Json(new { result = true, proceso = respuesta });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Guarda un nuevo usuario en el directorio
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult GuardarUsuarioDirectorio(string[] datos)
        {
            try
            {

                CADDIRECTORIO cADDIRECTORIO;
                string strNumeTelefono = datos[0].ToString();

                cADDIRECTORIO = (from registro in contexto.CADDIRECTORIOS
                                 where registro.NUME_TELEFONO == strNumeTelefono
                                 select registro).FirstOrDefault();
                int intCantidad = 0;
                if (cADDIRECTORIO != null)
                {
                    // contexto.CADDIRECTORIOS
                    // cADDIRECTORIO.NUME_TELEFONO = datos[0];
                    cADDIRECTORIO.NOMB_PROPIETARIO = datos[1];
                    cADDIRECTORIO.NIT = datos[2];
                    cADDIRECTORIO.DIRE_PROPIETA = datos[3];
                    cADDIRECTORIO.BARRIO = datos[4];
                    if (datos[5] != "")
                        cADDIRECTORIO.CADLUGE_CODIGO = Convert.ToInt32(datos[5]);
                    if (datos[6] != "")
                        cADDIRECTORIO.BARR_CODIGO = Convert.ToInt32(datos[6]);
                    contexto.Entry(cADDIRECTORIO).State = EntityState.Modified;
                    intCantidad = contexto.SaveChanges();
                }
                else
                {
                    cADDIRECTORIO = new CADDIRECTORIO();

                    // contexto.CADDIRECTORIOS
                    cADDIRECTORIO.NUME_TELEFONO = datos[0];
                    cADDIRECTORIO.NOMB_PROPIETARIO = datos[1];
                    cADDIRECTORIO.NIT = datos[2];
                    cADDIRECTORIO.DIRE_PROPIETA = datos[3];
                    cADDIRECTORIO.BARRIO = datos[4];
                    if (datos[5] != "")
                        cADDIRECTORIO.CADLUGE_CODIGO = Convert.ToInt32(datos[5]);
                    if (datos[6] != "")
                        cADDIRECTORIO.BARR_CODIGO = Convert.ToInt32(datos[6]);

                    contexto.CADDIRECTORIOS.Add(cADDIRECTORIO);
                    contexto.SaveChanges();
                }

                //contexto.SaveChanges();

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el numero telefonico : " + cADDIRECTORIO.NUME_TELEFONO.ToString());
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                var resultado = Json(new { result = true, mensajes = mensaje, tipos = tipo });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que actualiza barrios
        /// </summary>
        /// <param name="strBarrio"></param>
        /// <param name="strCodLuge"></param>
        /// <param name="strCodBarrNuevo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult AsignarBarrioManual(string strBarrio, string strCodLuge, string strCodBarrNuevo)
        {
            try
            {
                string strMensaje = "";

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_barrCodigo", strCodBarrNuevo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_lugeCodigo", strCodLuge, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_barrioConsulta", strBarrio, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_lugeCodigoConsulta", strCodLuge, typeof(decimal)));

                strMensaje = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_asignarBarriosManual", strConexion);

                var resultado = Json(new { result = true, mensaje = strMensaje });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que carga la DSP unidad
        /// </summary>
        /// <returns></returns>
        public string ObtenerUnidad()
        {
            string strValor = string.Empty;

            try
            {
                if (Session["USUARIO_SECAD"] != null)
                {
                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                    //consulta funcion para traer la unidad
                    objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));

                    strValor = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_desc_unidad", strConexion);
                }

                return strValor;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }

            return strValor;
        }

        /// <summary>
        /// Metodo que carga el log del archivo plano
        /// </summary>
        /// <param name="dtInfoLog"></param>
        public void CargarLog(DataTable dtInfoLog)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                foreach (DataRow item in dtInfoLog.Rows)
                {
                    sb.AppendLine("Registros Cargados ....................." + item[0].ToString());
                    sb.AppendLine("Registros Dupl/Actualizados ............" + item[1].ToString());
                    sb.AppendLine("Registros Rechazados ..................." + item[2].ToString());
                    sb.AppendLine("Total Registros Leidos ................." + item[3].ToString());
                }

                Response.Clear(); // paso 1
                Response.ClearHeaders(); // paso 2
                Response.ClearContent(); // paso 2
                Response.ContentType = "application/vnd.text";
                Response.AppendHeader("content-disposition", "attachment;filename=LogResultado.txt");
                Response.Write(sb.ToString());
                Response.Flush(); // paso 5
                Response.End(); // paso 6
                Response.Close();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }
    }
}