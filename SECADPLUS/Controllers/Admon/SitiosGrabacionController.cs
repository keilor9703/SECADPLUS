using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class SitiosGrabacionController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        //private string strConexion = ConfigurationManager.ConnectionStrings["SECAD12C"].ConnectionString;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: SitiosGrabacion
        [SecadPlusAutorizador("/SitiosGrabacion/Index")]
        public ActionResult Index(string strMensaje, string strTipo)
        {
            try
            {
                var mensaje = "";
                var tipo = "";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (string.IsNullOrEmpty(strMensaje))
                    mensaje = string.Empty;
                else
                    mensaje = strMensaje;

                if (string.IsNullOrEmpty(strMensaje))
                    tipo = string.Empty;
                else
                    tipo = strTipo;

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;


                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// Permite realizar la consulta de los sitios de grabación existentes en la base de datos
        /// </summary>
        /// <param name="intSitio">id del sitio de grabación a filtrar</param>
        /// <param name="strOpcion">Opción para modificar las acciones de cada tabla</param>
        /// <param name="strConsulta">Opción para extender el compotamiento de los datos resultantes</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarSitiosGrabacion(int? intSitio, string strOpcion = null, string strConsulta = null)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<ExtCadSitioGrabacion> elementos = new List<ExtCadSitioGrabacion>();
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;
                string strBoton = "", strSitio = "";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                if (intSitio > 0)
                {
                    strSitio += " WHERE sg.CONSECUTIVO = " + intSitio.ToString() + " ";
                }
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CONSECUTIVO = '" + parametros.ConsultaValor + "') ";

                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(INTERNO.DESCRIPCION1, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                else
                {

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INTERNO.Departamento " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY INTERNO.LUGE_CIUDAD " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNDE_FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY INTERNO.CADUNDE_CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION1 " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 9:
                        strOrdenamiento += " ORDER BY INTERNO.FECHA_INSTALACION " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 10:
                        strOrdenamiento += " ORDER BY INTERNO.OBSERVACIONES " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consulta_sitios_grabacion", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    switch (strOpcion)
                    {
                        case "IMPORTANCIA":
                            strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                            " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"CargaParqueaderos(" + fila["CONSECUTIVO"].ToString() + ",'" + fila["DESCRIPCION"].ToString() + "');CargaSitiosImportantes(" + fila["CONSECUTIVO"].ToString() + ", '" + fila["DESCRIPCION"].ToString() + "');\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='icon icon-eye'>" +
                            "</i> Ver Parqueaderos y Sitios Importantes </button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + @Url.Action("CreaParqueadero", "SitiosImportancia", new { idFather = fila["CONSECUTIVO"].ToString(), intPadre = fila["PADRE_CODIGO"].ToString(), strNombre = fila["DESCRIPCION"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Eliminar'> <i class='icon icon-add'>" +
                            "</i> Crear Parqueadero </button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + @Url.Action("CreaSitio", "SitiosImportancia", new { idFather = fila["CONSECUTIVO"].ToString(), intPadre = fila["PADRE_CODIGO"].ToString(), strNombre = fila["DESCRIPCION"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='icon icons-add'>" +
                            "</i> Crear Sitio Importante </button> </li>" +
                            "</ul> </li></ul>";
                            break;
                        case "ENTIDADES":
                            strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                            " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + @Url.Action("Index", "Entidades", new { idSitioGrabacion = fila["CONSECUTIVO"].ToString(), nombre = fila["DESCRIPCION"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='icon icons-eye'>" +
                            "</i> Entidades </button> </li>" +
                            "</ul> </li></ul>";
                            break;
                        case "TURNOS":
                            strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                            " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + @Url.Action("Entidades", "Turnos", new { intSitioGrabacion = fila["CONSECUTIVO"].ToString(), strNombre = fila["DESCRIPCION"].ToString(), strConsulta = strConsulta }) + "'\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='icon icons-eye'>" +
                            "</i> Entidades </button> </li>" +
                            "</ul> </li></ul>";
                            break;
                        default:
                            strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                            " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + Url.Action("Edit", new { id = fila["CONSECUTIVO"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='glyphicon glyphicon-edit'>" +
                            "</i> Editar </button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", new { id = fila["CONSECUTIVO"].ToString() }) + "')\" data -placement='bottom' data-original-title='Clic para Eliminar'> <i class='icon icon-trash'>" +
                            "</i> Eliminar </button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + Url.Action("CopiarSitiosGrabacion", new { idDestino = fila["CONSECUTIVO"].ToString(), nomDestino = fila["DESCRIPCION"].ToString(), descCiudad = fila["LUGE_CIUDAD"].ToString(), idCiudad = fila["CODCIUDAD"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Editar'> <i class='icon icons-versions'>" +
                            "</i> Copiar Referencias </button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + Url.Action("Index", "ReferenciasSitioGrabacion", new { id = fila["CONSECUTIVO"].ToString(), nom = fila["DESCRIPCION"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Revisiones'> <i class='icon icon-eye'>" +
                            "</i> Referencias</button> </li>" +
                            "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"window.location.href ='" + Url.Action("Index", "EquiposSitioGrabacion", new { id = fila["CONSECUTIVO"].ToString(), nom = fila["DESCRIPCION"].ToString() }) + "'\" data -placement='bottom' data-original-title='Clic para Revisiones'> <i class='icon icon-eye'>" +
                            "</i> Equipos de Computo </button> </li>" +
                            "</ul> </li></ul>";
                            break;
                    }
                    elementos.Add(new ExtCadSitioGrabacion(fila, strBoton));
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
        // GET: SitiosGrabacion/CopiarSitiosGrabacion
        [SecadPlusAutorizador("/SitiosGrabacion/CopiarSitiosGrabacion")]
        public ActionResult CopiarSitiosGrabacion(int? idDestino, string nomDestino, string descCiudad, int idCiudad, string strMensaje, string strTipo)
        {
            try
            {
                var mensaje = "";
                var tipo = "";

                if (string.IsNullOrEmpty(strMensaje))
                    mensaje = string.Empty;
                else
                    mensaje = strMensaje;

                if (string.IsNullOrEmpty(strMensaje))
                    tipo = string.Empty;
                else
                    tipo = strTipo;

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;

                if (idDestino == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //ViewBag.SITIOORIGEN = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.idDestino = idDestino;
                ViewBag.SITIODESTINO = idDestino + " - " + nomDestino;
                ViewBag.descCiudad = descCiudad;
                ViewBag.idCiudad = idCiudad;

                ViewBag.SITIOORIGEN = new SelectList(db.CADSITIOS_GRABACION.Where(m => m.CONSECUTIVO != idDestino).Select(c => new { CONSECUTIVO = c.CONSECUTIVO, DESCRIPCION = c.CONSECUTIVO + " - " + c.DESCRIPCION }).OrderBy(m => m.CONSECUTIVO), "CONSECUTIVO", "DESCRIPCION");
                //ViewBag.SITIOORIGEN1 = new SelectList(db.CADSITIOS_GRABACION.Where(m => m.CONSECUTIVO != idDestino).OrderBy(m => m.CONSECUTIVO), "CONSECUTIVO", "DESCRIPCION");

                //GestorAdministrador gestor = new GestorAdministrador();
                //var a = gestor.EjecutarCopiarReferencias(1003,115,"BOGOTA", 1228, strConexion);

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult CopiarReferencias(int idDestino, int idOrigen, string descCiudad, int idCiudad)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var a = gestor.EjecutarCopiarReferencias(idOrigen, idDestino, descCiudad, idCiudad, strConexion);

                return Json(new { result = true, datos = "Se copio referencias de forma exitosa" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult GetCiudadesByDepartamento(int id)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var lugares = gestor.ConsultarCiudadesDepartamento(id, strConexion);

                var ciudades = (from DataRow dr in lugares.Rows
                                select new
                                {
                                    CODIGO = dr["CODIGO"],
                                    DESCRIPCION = dr["DESCRIPCION"],
                                }).ToList();

                return Json(ciudades);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult GetCiudades(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "", strFiltro = id.ToString();
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorCiudad(CODIGO, 'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_CiudadesModel> listaCiudades = new List<_CiudadesModel>();
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
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_ciudades", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaCiudades.Add(new _CiudadesModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString(), fila["TIPO"].ToString(), fila["DPTO"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCiudades });
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
        public JsonResult GetUnidades(int id)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "", strFiltro = " WHERE FUERZA = " + id;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorUnidad(CODIGO, 'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_UnidadesModel> listaUnidades = new List<_UnidadesModel>();
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
                    strQuery = " AND (FUERZA = " + id + " AND REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (FUERZA = " + id + " AND REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i')) ";
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
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_unidades", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaUnidades.Add(new _UnidadesModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaUnidades });
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
        public JsonResult GetUnidadesByFuerza(int? id)
        {
            try
            {
                StringBuilder opciones = new StringBuilder();

                //List<CADUNIDADE> unidades = new List<CADUNIDADE>();
                var unidades = db.CADUNIDADES.Where(m => m.FUERZA == id).Select(m => new { CODIGO = m.CODIGO, DESCRIPCION = m.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();

                if (unidades.Count == 0)
                {
                    unidades.Insert(0, new { CODIGO = 0, DESCRIPCION = " " });
                }
                //else
                //{
                //    unidades.Insert(0, new { CODIGO = 0, DESCRIPCION = " " });
                //}

                //var unidad = (from r in unidades
                //              select new
                //              {
                //                  CODIGO = r.CODIGO,
                //                  DESCRIPCION = r.DESCRIPCION
                //              }).ToList();

                for (int i = 0; i < unidades.Count(); i++)
                {
                    opciones.AppendFormat("<option value = '{0}'  > {1} </option>", unidades[i].CODIGO, unidades[i].DESCRIPCION);
                }

                return Json(opciones.ToString());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult GetRegionalByUnidad(int idFuerza, int idCodigo)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var regionales = gestor.ConsultarRegional(idFuerza, idCodigo, strConexion);

                var regional = (from DataRow dr in regionales.Rows
                                select new
                                {
                                    CONSECUTIVO = dr["CONSECUTIVO"],
                                    DESCRIPCION = dr["DESCRIPCION"],
                                }).ToList();

                return Json(regional);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: SitiosGrabacion/Create
        [SecadPlusAutorizador("/SitiosGrabacion/Create")]
        public ActionResult Create()
        {
            try
            {
                List<CADLUGARES_GEOGRAFICOS> listaLugares = new List<CADLUGARES_GEOGRAFICOS>();
                List<CADREGIONALE> listaRegionales = new List<CADREGIONALE>();
                listaLugares.Insert(0, new CADLUGARES_GEOGRAFICOS { CODIGO = 0, DESCRIPCION = " " });
                listaRegionales.Insert(0, new CADREGIONALE { CONSECUTIVO = 0, DESCRIPCION = " " });

                ViewBag.DEPARTAMENTO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                //ViewBag.LUGE_CIUDAD = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                //ViewBag.CADUNDE_CODIGO = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                ViewBag.CONS_REGIONAL = new SelectList(listaRegionales, "CONSECUTIVO", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: SitiosGrabacion/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CONSECUTIVO,DESCRIPCION,LUGE_CIUDAD,CADUNDE_FUERZA,CADUNDE_CODIGO,FECHA_INSTALACION,OBSERVACIONES,CONS_REGIONAL")] CADSITIOS_GRABACION cADSITIOS_GRABACION)
        {
            try
            {
                var DEPARTAMENTO = Request["DEPARTAMENTO"];
                if (string.IsNullOrWhiteSpace(DEPARTAMENTO))
                    ModelState.AddModelError("DEPARTAMENTO", "El campo Departamento es obligatorio.");

                var CIUDAD = Request["CIUDAD"];
                var UNIDAD = Request["UNIDAD"];

                //var existe = db.CADSITIOS_GRABACION.Any(c => c.CONSECUTIVO.Equals(cADSITIOS_GRABACION.CONSECUTIVO));
                var existe = db.CADSITIOS_GRABACION.Find(cADSITIOS_GRABACION.CONSECUTIVO);

                if (existe != null)
                    ModelState.AddModelError("CONSECUTIVO", "Este Consecutivo esta siendo utilizado en otro registro.");

                if (cADSITIOS_GRABACION.CADUNDE_CODIGO == 0)
                    ModelState.AddModelError("CADUNDE_CODIGO", "El campo Unidad es obligatorio.");

                if (ModelState.IsValid)
                {
                    if (cADSITIOS_GRABACION.CONS_REGIONAL == 0)
                    {
                        cADSITIOS_GRABACION.CONS_REGIONAL = null;
                    }

                    db.CADSITIOS_GRABACION.Add(cADSITIOS_GRABACION);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADSITIOS_GRABACION>(cADSITIOS_GRABACION, "CADSITIOS_GRABACION", cADSITIOS_GRABACION.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADSITIOS_GRABACION.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                List<CADLUGARES_GEOGRAFICOS> listaLugares = new List<CADLUGARES_GEOGRAFICOS>();
                List<CADREGIONALE> listaRegionales = new List<CADREGIONALE>();
                listaLugares.Insert(0, new CADLUGARES_GEOGRAFICOS { CODIGO = 0, DESCRIPCION = " " });
                listaRegionales.Insert(0, new CADREGIONALE { CONSECUTIVO = 0, DESCRIPCION = " " });

                //if (string.IsNullOrWhiteSpace(DEPARTAMENTO))
                //    ViewBag.LUGE_CIUDAD = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                //else
                //    ViewBag.LUGE_CIUDAD = new SelectList(db.CADLUGARES_GEOGRAFICOS.SqlQuery("SELECT * FROM CADLUGARES_GEOGRAFICOS CONNECT BY PRIOR CODIGO = LUGE_CODIGO START WITH CODIGO = " + DEPARTAMENTO).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.LUGE_CIUDAD);

                //if (cADSITIOS_GRABACION.CADUNDE_FUERZA == 0)
                //    ViewBag.CADUNDE_CODIGO = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                //else
                //    ViewBag.CADUNDE_CODIGO = new SelectList(db.CADUNIDADES.Where(m => m.FUERZA == cADSITIOS_GRABACION.CADUNDE_FUERZA).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_CODIGO);

                ViewBag.DEPARTAMENTO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", DEPARTAMENTO);
                //ViewBag.LUGE_CIUDAD = cADSITIOS_GRABACION.LUGE_CIUDAD;
                ViewBag.CIUDAD = CIUDAD;
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                //ViewBag.CADUNDE_CODIGO = cADSITIOS_GRABACION.CADUNDE_CODIGO;
                ViewBag.UNIDAD = UNIDAD;
                //ViewBag.CONS_REGIONAL = new SelectList(listaRegionales, "CONSECUTIVO", "DESCRIPCION");
                ViewBag.CONS_REGIONAL = new SelectList(db.CADREGIONALES.Where(c => c.CONSECUTIVO == cADSITIOS_GRABACION.CONS_REGIONAL).Select(c => new { CONSECUTIVO = c.CONSECUTIVO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CONSECUTIVO", "DESCRIPCION", cADSITIOS_GRABACION.CONS_REGIONAL);

                return View(cADSITIOS_GRABACION);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: SitiosGrabacion/Edit/5
        [SecadPlusAutorizador("/SitiosGrabacion/Edit")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CADSITIOS_GRABACION cADSITIOS_GRABACION = db.CADSITIOS_GRABACION.Find(id);

                CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = db.CADLUGARES_GEOGRAFICOS.Find(cADSITIOS_GRABACION.LUGE_CIUDAD);

                GestorAdministrador gestor = new GestorAdministrador();
                var lugares = gestor.ConsultarCiudadesDepartamento((Int32)cADLUGARES_GEOGRAFICOS.LUGE_CODIGO, strConexion);

                var ciudades = (from DataRow dr in lugares.Rows
                                select new
                                {
                                    CODIGO = dr["CODIGO"],
                                    DESCRIPCION = dr["DESCRIPCION"],
                                }).ToList();

                if (cADSITIOS_GRABACION == null)
                {
                    return HttpNotFound();
                }
                ViewBag.antiguoConsecutivo = cADSITIOS_GRABACION.CONSECUTIVO;

                var DEPARTAMENTO = db.CADLUGARES_GEOGRAFICOS.SqlQuery("SELECT * FROM CADLUGARES_GEOGRAFICOS WHERE TIPO = 'DE' CONNECT BY PRIOR LUGE_CODIGO = CODIGO START WITH CODIGO = " + cADSITIOS_GRABACION.LUGE_CIUDAD).Select(c => c.CODIGO).First();
                ViewBag.DEPARTAMENTO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", DEPARTAMENTO);
                //ViewBag.LUGE_CIUDAD = new SelectList(ciudades, "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.LUGE_CIUDAD);
                ViewBag.CIUDAD = cADSITIOS_GRABACION.CADLUGARES_GEOGRAFICOS.DESCRIPCION;
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_FUERZA);
                //ViewBag.CADUNDE_CODIGO = new SelectList(db.CADUNIDADES.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_CODIGO);
                //ViewBag.CADUNDE_CODIGO = cADSITIOS_GRABACION.CADUNDE_CODIGO;
                ViewBag.UNIDAD = cADSITIOS_GRABACION.CADUNIDADE.DESCRIPCION;
                ViewBag.CONS_REGIONAL = new SelectList(db.CADREGIONALES.Where(c => c.CONSECUTIVO == cADSITIOS_GRABACION.CONS_REGIONAL).Select(c => new { CONSECUTIVO = c.CONSECUTIVO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CONSECUTIVO", "DESCRIPCION", cADSITIOS_GRABACION.CONS_REGIONAL);
                return View(cADSITIOS_GRABACION);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: SitiosGrabacion/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CONSECUTIVO,DESCRIPCION,LUGE_CIUDAD,CADUNDE_FUERZA,CADUNDE_CODIGO,FECHA_INSTALACION,OBSERVACIONES,CONS_REGIONAL")] CADSITIOS_GRABACION cADSITIOS_GRABACION)
        {
            try
            {
                var antiguoConsecutivo = int.Parse(Request["antiguoConsecutivo"]);

                var DEPARTAMENTO = Request["DEPARTAMENTO"];
                if (string.IsNullOrWhiteSpace(DEPARTAMENTO))
                    ModelState.AddModelError("DEPARTAMENTO", "El campo Departamento es obligatorio.");

                var CIUDAD = Request["CIUDAD"];
                var UNIDAD = Request["UNIDAD"];

                if (antiguoConsecutivo != cADSITIOS_GRABACION.CONSECUTIVO)
                {
                    var existe = db.CADSITIOS_GRABACION.Find(cADSITIOS_GRABACION.CONSECUTIVO);

                    if (existe != null)
                        ModelState.AddModelError("CONSECUTIVO", "Este Consecutivo esta siendo utilizado en otro registro.");
                }

                if (cADSITIOS_GRABACION.CADUNDE_CODIGO == 0)
                    ModelState.AddModelError("CADUNDE_CODIGO", "El campo Unidad es obligatorio.");

                if (ModelState.IsValid)
                {
                    if (cADSITIOS_GRABACION.CONS_REGIONAL == 0)
                    {
                        cADSITIOS_GRABACION.CONS_REGIONAL = null;
                    }

                    if (antiguoConsecutivo == cADSITIOS_GRABACION.CONSECUTIVO)
                    {
                        db.Entry(cADSITIOS_GRABACION).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADSITIOS_GRABACION>(cADSITIOS_GRABACION, "CADSITIOS_GRABACION", cADSITIOS_GRABACION.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADSITIOS_GRABACION cADSITIOS_GRABACION1 = db.CADSITIOS_GRABACION.Find(antiguoConsecutivo);
                        db.CADSITIOS_GRABACION.Remove(cADSITIOS_GRABACION1);
                        db.SaveChanges();

                        //cADSITIOS_GRABACION.CONSECUTIVO = nuevoConsecutivo;

                        db.CADSITIOS_GRABACION.Add(cADSITIOS_GRABACION);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADSITIOS_GRABACION>(cADSITIOS_GRABACION, "CADSITIOS_GRABACION", cADSITIOS_GRABACION.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADSITIOS_GRABACION.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                List<CADLUGARES_GEOGRAFICOS> listaLugares = new List<CADLUGARES_GEOGRAFICOS>();
                List<CADREGIONALE> listaRegionales = new List<CADREGIONALE>();
                listaLugares.Insert(0, new CADLUGARES_GEOGRAFICOS { CODIGO = 0, DESCRIPCION = " " });
                listaRegionales.Insert(0, new CADREGIONALE { CONSECUTIVO = 0, DESCRIPCION = " " });

                ViewBag.antiguoConsecutivo = antiguoConsecutivo;

                //if (string.IsNullOrWhiteSpace(DEPARTAMENTO))
                //    ViewBag.LUGE_CIUDAD = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                //else
                //    ViewBag.LUGE_CIUDAD = new SelectList(db.CADLUGARES_GEOGRAFICOS.SqlQuery("SELECT * FROM CADLUGARES_GEOGRAFICOS CONNECT BY PRIOR CODIGO = LUGE_CODIGO START WITH CODIGO = " + DEPARTAMENTO).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.LUGE_CIUDAD);

                //if (cADSITIOS_GRABACION.CADUNDE_FUERZA == 0)
                //    ViewBag.CADUNDE_CODIGO = new SelectList(listaLugares, "CODIGO", "DESCRIPCION");
                //else
                //    ViewBag.CADUNDE_CODIGO = new SelectList(db.CADUNIDADES.Where(m => m.FUERZA == cADSITIOS_GRABACION.CADUNDE_FUERZA).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_CODIGO);

                ViewBag.DEPARTAMENTO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", DEPARTAMENTO);
                ViewBag.CIUDAD = CIUDAD;
                ViewBag.CADUNDE_FUERZA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_FUERZA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_FUERZA);
                //ViewBag.CADUNDE_CODIGO = new SelectList(db.CADUNIDADES.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADSITIOS_GRABACION.CADUNDE_CODIGO);
                //ViewBag.CADUNDE_CODIGO = cADSITIOS_GRABACION.CADUNDE_CODIGO;
                ViewBag.UNIDAD = UNIDAD;
                ViewBag.CONS_REGIONAL = new SelectList(db.CADREGIONALES.Select(c => new { CONSECUTIVO = c.CONSECUTIVO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CONSECUTIVO", "DESCRIPCION", cADSITIOS_GRABACION.CONS_REGIONAL);

                return View(cADSITIOS_GRABACION);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/SitiosGrabacion/Eliminar")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADSITIOS_GRABACION cADSITIOS_GRABACION = db.CADSITIOS_GRABACION.Find(id);
                if (cADSITIOS_GRABACION == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADSITIOS_GRABACION);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: SitiosGrabacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                CADSITIOS_GRABACION cADSITIOS_GRABACION = db.CADSITIOS_GRABACION.Find(id);
                db.CADSITIOS_GRABACION.Remove(cADSITIOS_GRABACION);
                db.SaveChanges();



                CADSITIOS_GRABACION cADSITIOS_GRABACION_copy = new CADSITIOS_GRABACION();
                cADSITIOS_GRABACION_copy = (CADSITIOS_GRABACION)auditoria.GenerarCopiaAuditoria(cADSITIOS_GRABACION);

                auditoria.RegistrarAuditoria<CADSITIOS_GRABACION>(cADSITIOS_GRABACION_copy, "CADSITIOS_GRABACION", cADSITIOS_GRABACION.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el sitio de grabación: " + cADSITIOS_GRABACION.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
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