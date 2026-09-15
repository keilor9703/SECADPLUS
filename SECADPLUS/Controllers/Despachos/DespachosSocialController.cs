using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers.Despachos
{
    public class DespachosSocialController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: DespachosSocial
        [SecadPlusAutorizador("/DespachosSocial/Index")]
        public ActionResult Index(string strMensaje = null, string strTipo = null, string strSitioGrabaEvento = null, string strEvento = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    if (strTipo != null)
                        ViewBag.TIPO_MENSAJE = strTipo;
                    else
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { strSitioGrabaEvento = strSitioGrabaEvento, strEvento = strEvento });
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                if (strSitioGrabaEvento == null || strEvento == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = "No se encuentra un evento para social.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.TIPO_REMISION = new SelectList((from t in contexto.CADTIPOSREMISIONES where t.OPCIONSOCIAL == "S" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_SERVICIO = new SelectList((from t in contexto.CADTIPOSERVICIOs where t.OPCIONSOCIAL == "S" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                ViewBag.TIPO_IDENTIFICACION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_DOCUMENTO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.CODI_GENERO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.CODI_RAZA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_RAZA" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                //ViewBag.EVEN_NUMEEVENTO = 216894;
                //ViewBag.EVEN_SITIOGRABACION = 3947;

                ViewBag.EVEN_NUMEEVENTO = strEvento;
                ViewBag.EVEN_SITIOGRABACION = strSitioGrabaEvento;

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View("Index", "BaseRecepcion");
        }

        /// <summary>
        /// Metodo que obtiene ficha social
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObteneFichaSocial(string strSitioGrabaEvento, string strEvento)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_FichaSocial> listaFichaSocial = new List<Ext_FichaSocial>();
                GestorGeneral objGestorGral = new GestorGeneral();
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                string strBotonDestino = "";
                string strBoton = "<ul class='icons-list'> " +
                            "<li class='dropdown'> " +
                                "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                    "<i class='icon-menu9'></i> " +
                                "</button> " +
                                "<ul class='dropdown-menu dropdown-menu-left'> " +
                                    "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                "onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", "DespachosSocial") + "?_ELIMINAR')\"> " +
                                            "<i class='icon-trash'></i> Eliminar" +
                                        "</button>" +
                                    "</li>" +
                                "</ul>" +
                            "</li>" +
                        "</ul>";

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (NRO_FICHA, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(DESP_MPIO_ATENCION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(DESP_BARRIO_ATENCION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(DIRECCION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(TELEFONO, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(FECHA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(CADUSUARIO, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(CODI_EVENTO, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY NRO_FICHA ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY NRO_FICHA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY FICHA_REMITIDA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY DESP_MPIO_ATENCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY DESP_BARRIO_ATENCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY DIRECCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY CODI_EVENTO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY OBSERVACION1 " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 10:
                        strOrderna += " ORDER BY CADUSUARIO " + parametros.TipoOrdenamiento + " ";
                        break;
                    case 11:
                        strOrderna += " ORDER BY FECHA_TRANSACCION " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", strEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaFichaSocialEvento", strConexion, "Nonquery");

                foreach (DataRow fila in dttPerRelacionada.Rows)
                {
                    strBotonDestino = strBoton.Replace("_ELIMINAR", "intSitioGraba=" + fila["EVEN_SITIOGRABACION"].ToString() + "&intNumeroEvento=" + fila["EVEN_NUMEEVENTO"].ToString() +
                                                                            "&strNroFicha=" + fila["NRO_FICHA"].ToString());

                    listaFichaSocial.Add(new Ext_FichaSocial(fila, strBotonDestino));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaFichaSocial });
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
        public JsonResult ObteneFichaSocialTipos(string strSitioGrabaEvento = null, string strEvento = null, bool blTs = false, string strNroFicha = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttTipos;
                List<Ext_SocialFichaTipos> listaFichaSocialTipos = new List<Ext_SocialFichaTipos>();
                GestorGeneral objGestorGral = new GestorGeneral();
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                string strBotonDestino = "";
                string strBoton = "";

                if (strNroFicha != null && strNroFicha != "")
                {
                    if (blTs)
                    {
                        strBoton = "<ul class='icons-list'> " +
                                "<li class='dropdown'> " +
                                    "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                        "<i class='icon-menu9'></i> " +
                                    "</button> " +
                                    "<ul class='dropdown-menu dropdown-menu-left'> " +
                                        "<li> " +
                                             "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                    "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                    "onclick=\"EliminarTipoServicio(_ELIMINAR)\"> " +
                                                "<i class='icon-trash'></i> Eliminar" +
                                            "</button>" +
                                        "</li>" +
                                    "</ul>" +
                                "</li>" +
                            "</ul>";

                    }
                    else
                    {
                        strBoton = "<ul class='icons-list'> " +
                              "<li class='dropdown'> " +
                                  "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                      "<i class='icon-menu9'></i> " +
                                  "</button> " +
                                  "<ul class='dropdown-menu dropdown-menu-left'> " +
                                      "<li> " +
                                           "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                  "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                  "onclick=\"EliminarRemisiones(_ELIMINAR)\"> " +
                                              "<i class='icon-trash'></i> Eliminar" +
                                          "</button>" +
                                      "</li>" +
                                  "</ul>" +
                              "</li>" +
                          "</ul>";
                    }


                    //Verifica si se debe realizar búsqueda de registros
                    if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                    {
                        blFiltro = true;

                        if (blTs)
                            strQuery += " WHERE REGEXP_LIKE (DESP_TIPO_SERVICIO, '" + parametros.ConsultaValor + "', 'i') ";
                        else
                            strQuery += " WHERE REGEXP_LIKE (DESP_TIPO_REMISION, '" + parametros.ConsultaValor + "', 'i') ";


                    }
                    //Verifica si se deben realizar ordenamientos
                    switch (parametros.IndiceOrdenamiento)
                    {
                        default:
                            if (blTs)
                                strOrderna += " ORDER BY DESP_TIPO_SERVICIO ";
                            else
                                strOrderna += " ORDER BY DESP_TIPO_REMISION " + parametros.TipoOrdenamiento + " ";

                            break;

                        case 2:
                            if (blTs)
                                strOrderna += " ORDER BY DESP_TIPO_SERVICIO " + parametros.TipoOrdenamiento + " ";
                            else
                                strOrderna += " ORDER BY DESP_TIPO_REMISION " + parametros.TipoOrdenamiento + " ";


                            break;

                    }




                    strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;


                    if (blTs)
                    {

                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", strEvento, typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nroFicha", strNroFicha, typeof(string)));

                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                        dttTipos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaFichaSocialTS", strConexion, "Nonquery");

                        foreach (DataRow fila in dttTipos.Rows)
                        {
                            strBotonDestino = strBoton.Replace("_ELIMINAR", fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," +
                                                                                    fila["NRO_FICHA"].ToString() + ",'" + fila["TIPO_SERVICIO"].ToString() + "','" + fila["DESP_TIPO_SERVICIO"].ToString() + "'");

                            listaFichaSocialTipos.Add(new Ext_SocialFichaTipos(fila, strBotonDestino, true));
                            parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                            parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                        }
                    }
                    else
                    {

                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", strEvento, typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nroFicha", strNroFicha, typeof(string)));

                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                        dttTipos = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaFichaSocialTR", strConexion, "Nonquery");

                        foreach (DataRow fila in dttTipos.Rows)
                        {
                            strBotonDestino = strBoton.Replace("_ELIMINAR", fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," +
                                                                                    fila["NRO_FICHA"].ToString() + ",'" + fila["TIPO_REMISION"].ToString() + "','" + fila["DESP_TIPO_REMISION"].ToString() + "'");

                            listaFichaSocialTipos.Add(new Ext_SocialFichaTipos(fila, strBotonDestino, false));
                            parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                            parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                        }
                    }

                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
                }



                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaFichaSocialTipos });
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
        public JsonResult ObteneFichaSocialPersonas(string strSitioGrabaEvento = null, string strEvento = null, string strNroFicha = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_FichaSocialPersona> listaFichaSocial = new List<Ext_FichaSocialPersona>();
                GestorGeneral objGestorGral = new GestorGeneral();
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                string strBotonDestino = "";
                string strBoton = "<ul class='icons-list'> " +
                            "<li class='dropdown'> " +
                                "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                    "<i class='icon-menu9'></i> " +
                                "</button> " +
                                "<ul class='dropdown-menu dropdown-menu-left'> " +
                                    "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                  "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                  "onclick=\"EliminarPersonaSocial(_ELIMINAR)\"> " +
                                              "<i class='icon-trash'></i> Eliminar" +
                                          "</button>" +
                                    "</li>" +
                                "</ul>" +
                            "</li>" +
                        "</ul>";

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(TIPO_IDENTIFICACION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(IDENTIFICACION, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(NOMBRES, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(APELLIDOS, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(FECHA_NACE, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY TIPO_IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY NOMBRES " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY APELLIDOS " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY FECHA_NACE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY EDAD_PERSONA " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                if (strNroFicha != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", strEvento, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nroFicha", strNroFicha, typeof(string)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaFichaSocialPersonas", strConexion, "Nonquery");

                    foreach (DataRow fila in dttPerRelacionada.Rows)
                    {
                        strBotonDestino = strBoton.Replace("_ELIMINAR", fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," +
                                                                                        fila["NRO_FICHA"].ToString() + "," + fila["CONSECUTIVO"].ToString() + "," + fila["IDENTIFICACION"].ToString());


                        listaFichaSocial.Add(new Ext_FichaSocialPersona(fila, strBotonDestino));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
                }

                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaFichaSocial });
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
        public JsonResult CrearFichaSocial([Bind(Include = "NRO_FICHA,FICHA_REMITIDA,MPIO_ATENCION,BARRIO_CODIGO,BARRIO_LUGECODIGO,DIRECCION,TELEFONO,CODI_EVENTO,OBSERVACION1,EVEN_SITIOGRABACION,EVEN_NUMEEVENTO,OBSERVACION2")] CADEVENTO_FICHASSOCIAL cADEVENTO_FICHASSOCIAL)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string mensaje = string.Empty;
                if (ModelState.IsValid)
                {
                    objGestorGral = new GestorGeneral();

                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                    if (cADEVENTO_FICHASSOCIAL.NRO_FICHA == null)
                    {
                        if (cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA == null)
                            cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA = "NO";
                        else
                            cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA = "SI";

                        cADEVENTO_FICHASSOCIAL.CADUSUARIO = usr.USUARIO;
                        cADEVENTO_FICHASSOCIAL.NRO_FICHA = SecuenciaFichaSocial();

                        cADEVENTO_FICHASSOCIAL.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;

                        contexto.CADEVENTO_FICHASSOCIAL.Add(cADEVENTO_FICHASSOCIAL);
                        contexto.SaveChanges();

                        mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADEVENTO_FICHASSOCIAL.NRO_FICHA.ToString());
                    }
                    else
                    {
                        if (cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA == null)
                            cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA = "NO";
                        else
                            cADEVENTO_FICHASSOCIAL.FICHA_REMITIDA = "SI";

                        cADEVENTO_FICHASSOCIAL.CADUSUARIO = usr.USUARIO;

                        cADEVENTO_FICHASSOCIAL.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;


                        contexto.Entry(cADEVENTO_FICHASSOCIAL).State = EntityState.Modified;
                        contexto.SaveChanges();
                        auditoria.RegistrarAuditoria<CADEVENTO_FICHASSOCIAL>(cADEVENTO_FICHASSOCIAL, "CADEVENTO_FICHASSOCIAL", "EVEN_SITIOGRABACION=" + cADEVENTO_FICHASSOCIAL.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTO_FICHASSOCIAL.EVEN_NUMEEVENTO
                            + ",NRO_FICHA=" + cADEVENTO_FICHASSOCIAL.NRO_FICHA, TipoAuditoria.Modificar);

                        mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADEVENTO_FICHASSOCIAL.NRO_FICHA.ToString());
                    }



                }
                resultados = Json(new { result = true, datos = mensaje });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var name = ve.PropertyName;
                        var mensaje = ve.ErrorMessage;
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo para crear personas social
        /// </summary>
        /// <param name="cADEVENTOFICSOC_PERSONAS"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearPersonaSocial([Bind(Include = "NRO_FICHA,CONSECUTIVO,EVEN_SITIOGRABACION,EVEN_NUMEEVENTO,TIPO_IDENTIFICACION,IDENTIFICACION,NOMBRES,APELLIDOS,FECHA_NACE,EDAD_PERSONA,CODI_GENERO,CODI_RAZA,COND_LGTBI,COND_DISCAPACIDAD,COND_DESPLAZADO,OBSERVACIONES")] CADEVENTOFICSOC_PERSONAS cADEVENTOFICSOC_PERSONAS)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                string mensaje = string.Empty;

                objGestorGral = new GestorGeneral();

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                if (cADEVENTOFICSOC_PERSONAS.CONSECUTIVO == 0)
                {
                    if (cADEVENTOFICSOC_PERSONAS.COND_LGTBI == null)
                        cADEVENTOFICSOC_PERSONAS.COND_LGTBI = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_LGTBI = "S";

                    if (cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO == null)
                        cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO = "S";

                    if (cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD == null)
                        cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD = "S";

                    cADEVENTOFICSOC_PERSONAS.CADUSUARIO = usr.USUARIO;


                    var consecutivo = (from t in contexto.CADEVENTOFICSOC_PERSONAS
                                       where t.EVEN_SITIOGRABACION == cADEVENTOFICSOC_PERSONAS.EVEN_SITIOGRABACION && t.EVEN_NUMEEVENTO == cADEVENTOFICSOC_PERSONAS.EVEN_NUMEEVENTO
                                       && t.NRO_FICHA == cADEVENTOFICSOC_PERSONAS.NRO_FICHA
                                       orderby t.CONSECUTIVO descending
                                       select t.CONSECUTIVO).FirstOrDefault();
                    int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                    cADEVENTOFICSOC_PERSONAS.CONSECUTIVO = intConsecutivo;

                    cADEVENTOFICSOC_PERSONAS.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;

                    contexto.CADEVENTOFICSOC_PERSONAS.Add(cADEVENTOFICSOC_PERSONAS);
                    contexto.SaveChanges();

                    mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADEVENTOFICSOC_PERSONAS.CONSECUTIVO.ToString());
                }
                else
                {

                    if (cADEVENTOFICSOC_PERSONAS.COND_LGTBI == null)
                        cADEVENTOFICSOC_PERSONAS.COND_LGTBI = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_LGTBI = "S";

                    if (cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO == null)
                        cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_DESPLAZADO = "S";

                    if (cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD == null)
                        cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD = "N";
                    else
                        cADEVENTOFICSOC_PERSONAS.COND_DISCAPACIDAD = "S";

                    cADEVENTOFICSOC_PERSONAS.CADUSUARIO = usr.USUARIO;


                    cADEVENTOFICSOC_PERSONAS.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;


                    contexto.Entry(cADEVENTOFICSOC_PERSONAS).State = EntityState.Modified;
                    contexto.SaveChanges();
                    auditoria.RegistrarAuditoria<CADEVENTOFICSOC_PERSONAS>(cADEVENTOFICSOC_PERSONAS, "CADEVENTOFICSOC_PERSONAS", "EVEN_SITIOGRABACION=" + cADEVENTOFICSOC_PERSONAS.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTOFICSOC_PERSONAS.EVEN_NUMEEVENTO
                        + ",NRO_FICHA=" + cADEVENTOFICSOC_PERSONAS.NRO_FICHA + ",CONSECUTIVO=" + cADEVENTOFICSOC_PERSONAS.CONSECUTIVO, TipoAuditoria.Modificar);


                    mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADEVENTOFICSOC_PERSONAS.CONSECUTIVO.ToString());
                }




                resultados = Json(new { result = true, datos = mensaje });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var name = ve.PropertyName;
                        var mensaje = ve.ErrorMessage;
                    }
                }
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo para crear ficha ts
        /// </summary>
        /// <param name="strDatos"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearFichaTS(string[] strDatos)
        {
            try
            {
                CADEVENTOFICSOC_TS cADEVENTOFICSOC_TS = new CADEVENTOFICSOC_TS();
                objGestorGral = new GestorGeneral();
                string strMensaje = "";
                int intEventoSitioGraba = Convert.ToInt32(strDatos[0]);
                Int64 intEvento = Convert.ToInt64(strDatos[1]);
                string strNroFicha = strDatos[2];
                string strTipo = strDatos[3];


                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                int intExiste = contexto.CADEVENTOFICSOC_TS.Where(t => t.EVEN_SITIOGRABACION == intEventoSitioGraba && t.EVEN_NUMEEVENTO == intEvento && t.NRO_FICHA == strNroFicha && t.TIPO_SERVICIO == strTipo).Count();

                if (intExiste == 0)
                {
                    cADEVENTOFICSOC_TS.EVEN_SITIOGRABACION = intEventoSitioGraba;
                    cADEVENTOFICSOC_TS.EVEN_NUMEEVENTO = intEvento;
                    cADEVENTOFICSOC_TS.NRO_FICHA = strNroFicha;
                    cADEVENTOFICSOC_TS.TIPO_SERVICIO = strTipo;

                    cADEVENTOFICSOC_TS.CADUSUARIO = usr.USUARIO;
                    cADEVENTOFICSOC_TS.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;


                    contexto.CADEVENTOFICSOC_TS.Add(cADEVENTOFICSOC_TS);
                    contexto.SaveChanges();
                    strMensaje = "Registro creado de manera exitosa.";
                }
                else
                {
                    strMensaje = "El tipo de servicio ya existe";
                }


                var resultados = Json(new { result = true, datos = strMensaje });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo para crear ficha tr
        /// </summary>
        /// <param name="strDatos"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearFichaTR(string[] strDatos)
        {
            try
            {
                CADEVENTOFICSOC_TR cADEVENTOFICSOC_Tr = new CADEVENTOFICSOC_TR();
                objGestorGral = new GestorGeneral();
                string strMensaje = "";
                int intEventoSitioGraba = Convert.ToInt32(strDatos[0]);
                Int64 intEvento = Convert.ToInt64(strDatos[1]);
                string strNroFicha = strDatos[2];
                string strTipo = strDatos[3];


                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                int intExiste = contexto.CADEVENTOFICSOC_TR.Where(t => t.EVEN_SITIOGRABACION == intEventoSitioGraba && t.EVEN_NUMEEVENTO == intEvento && t.NRO_FICHA == strNroFicha && t.TIPO_REMISION == strTipo).Count();

                if (intExiste == 0)
                {
                    cADEVENTOFICSOC_Tr.EVEN_SITIOGRABACION = intEventoSitioGraba;
                    cADEVENTOFICSOC_Tr.EVEN_NUMEEVENTO = intEvento;
                    cADEVENTOFICSOC_Tr.NRO_FICHA = strNroFicha;
                    cADEVENTOFICSOC_Tr.TIPO_REMISION = strTipo;

                    cADEVENTOFICSOC_Tr.CADUSUARIO = usr.USUARIO;
                    cADEVENTOFICSOC_Tr.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;


                    contexto.CADEVENTOFICSOC_TR.Add(cADEVENTOFICSOC_Tr);
                    contexto.SaveChanges();
                    strMensaje = "Registro creado de manera exitosa.";

                }
                else
                {
                    strMensaje = "El tipo de remisión ya existe";
                }


                var resultados = Json(new { result = true, datos = strMensaje });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que elimina un registro
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Eliminar(int? intSitioGraba, int? intNumeroEvento, string strNroFicha)
        {
            try
            {
                if (intSitioGraba == null && intNumeroEvento == null && strNroFicha == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADEVENTO_FICHASSOCIAL cADEVENTO_FICHASSOCIAL = await contexto.CADEVENTO_FICHASSOCIAL.FindAsync(intSitioGraba, intNumeroEvento, strNroFicha);
                if (cADEVENTO_FICHASSOCIAL == null)
                {
                    return HttpNotFound();
                }

                return View("Delete", null, cADEVENTO_FICHASSOCIAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que elimina un registro
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? intSitioGraba, int? intNumeroEvento, string strNroFicha)
        {
            try
            {
                CADEVENTO_FICHASSOCIAL cADEVENTO_FICHASSOCIAL = await contexto.CADEVENTO_FICHASSOCIAL.FindAsync(intSitioGraba, intNumeroEvento, strNroFicha);

                contexto.CADEVENTO_FICHASSOCIAL.Remove(cADEVENTO_FICHASSOCIAL);
                await contexto.SaveChangesAsync();


                CADEVENTO_FICHASSOCIAL cADEVENTO_FICHASSOCIAL_copy = new CADEVENTO_FICHASSOCIAL();
                cADEVENTO_FICHASSOCIAL_copy = (CADEVENTO_FICHASSOCIAL)auditoria.GenerarCopiaAuditoria(cADEVENTO_FICHASSOCIAL);

                auditoria.RegistrarAuditoria<CADEVENTO_FICHASSOCIAL>(cADEVENTO_FICHASSOCIAL_copy, "CADEVENTO_FICHASSOCIAL", "EVEN_SITIOGRABACION=" + cADEVENTO_FICHASSOCIAL.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTO_FICHASSOCIAL.EVEN_NUMEEVENTO
                       + ",NRO_FICHA=" + cADEVENTO_FICHASSOCIAL.NRO_FICHA, TipoAuditoria.Eliminar);


                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                return RedirectToAction("Index", new { strMensaje = mensaje, strSitioGrabaEvento = intSitioGraba, strEvento = intNumeroEvento });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo, strSitioGrabaEvento = intSitioGraba, strEvento = intNumeroEvento });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intSitioGraba"></param>
        /// <param name="intNumeroEvento"></param>
        /// <param name="strNroFicha"></param>
        /// <param name="strTipoServicio"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult EliminarTS(int? intSitioGraba, int? intNumeroEvento, string strNroFicha, string strTipoServicio)
        {
            try
            {
                CADEVENTOFICSOC_TS cADEVENTOFICSOC_TS = contexto.CADEVENTOFICSOC_TS.Where(t => t.EVEN_SITIOGRABACION == intSitioGraba && t.EVEN_NUMEEVENTO == intNumeroEvento && t.NRO_FICHA == strNroFicha && t.TIPO_SERVICIO == strTipoServicio).FirstOrDefault();

                contexto.CADEVENTOFICSOC_TS.Remove(cADEVENTOFICSOC_TS);
                contexto.SaveChanges();


                CADEVENTOFICSOC_TS cADEVENTOFICSOC_TS_copy = new CADEVENTOFICSOC_TS();
                cADEVENTOFICSOC_TS_copy = (CADEVENTOFICSOC_TS)auditoria.GenerarCopiaAuditoria(cADEVENTOFICSOC_TS);

                auditoria.RegistrarAuditoria<CADEVENTOFICSOC_TS>(cADEVENTOFICSOC_TS_copy, "CADEVENTOFICSOC_TS", "EVEN_SITIOGRABACION=" + cADEVENTOFICSOC_TS.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTOFICSOC_TS.EVEN_NUMEEVENTO
                    + ",NRO_FICHA=" + cADEVENTOFICSOC_TS.NRO_FICHA + ",TIPO_SERVICIO=" + cADEVENTOFICSOC_TS.TIPO_SERVICIO, TipoAuditoria.Eliminar);




                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var resultados = Json(new { result = true, datos = mensaje });

                return resultados;
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return Json(new { result = false, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intSitioGraba"></param>
        /// <param name="intNumeroEvento"></param>
        /// <param name="strNroFicha"></param>
        /// <param name="strTipoRemision"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult EliminarTR(int? intSitioGraba, int? intNumeroEvento, string strNroFicha, string strTipoRemision)
        {
            try
            {
                CADEVENTOFICSOC_TR cADEVENTOFICSOC_TR = contexto.CADEVENTOFICSOC_TR.Where(t => t.EVEN_SITIOGRABACION == intSitioGraba && t.EVEN_NUMEEVENTO == intNumeroEvento && t.NRO_FICHA == strNroFicha && t.TIPO_REMISION == strTipoRemision).FirstOrDefault();

                contexto.CADEVENTOFICSOC_TR.Remove(cADEVENTOFICSOC_TR);
                contexto.SaveChanges();

                CADEVENTOFICSOC_TR cADEVENTOFICSOC_TR_copy = new CADEVENTOFICSOC_TR();
                cADEVENTOFICSOC_TR_copy = (CADEVENTOFICSOC_TR)auditoria.GenerarCopiaAuditoria(cADEVENTOFICSOC_TR);

                auditoria.RegistrarAuditoria<CADEVENTOFICSOC_TS>(cADEVENTOFICSOC_TR_copy, "CADEVENTOFICSOC_TR", "EVEN_SITIOGRABACION=" + cADEVENTOFICSOC_TR.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTOFICSOC_TR.EVEN_NUMEEVENTO
                    + ",NRO_FICHA=" + cADEVENTOFICSOC_TR.NRO_FICHA + ",TIPO_REMISION=" + cADEVENTOFICSOC_TR.TIPO_REMISION, TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var resultados = Json(new { result = true, datos = mensaje });

                return resultados;
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return Json(new { result = false, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Elimina persona social
        /// </summary>
        /// <param name="intSitioGraba"></param>
        /// <param name="intNumeroEvento"></param>
        /// <param name="strNroFicha"></param>
        /// <param name="intConsecutivo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult EliminarPersonaSocial(int? intSitioGraba, int? intNumeroEvento, string strNroFicha, int intConsecutivo)
        {
            try
            {
                CADEVENTOFICSOC_PERSONAS cADEVENTOFICSOC_PERSONAS = contexto.CADEVENTOFICSOC_PERSONAS.Where(t => t.EVEN_SITIOGRABACION == intSitioGraba && t.EVEN_NUMEEVENTO == intNumeroEvento && t.NRO_FICHA == strNroFicha && t.CONSECUTIVO == intConsecutivo).FirstOrDefault();

                contexto.CADEVENTOFICSOC_PERSONAS.Remove(cADEVENTOFICSOC_PERSONAS);
                contexto.SaveChanges();


                CADEVENTOFICSOC_PERSONAS cADEVENTOFICSOC_PERSONAS_copy = new CADEVENTOFICSOC_PERSONAS();
                cADEVENTOFICSOC_PERSONAS_copy = (CADEVENTOFICSOC_PERSONAS)auditoria.GenerarCopiaAuditoria(cADEVENTOFICSOC_PERSONAS);


                auditoria.RegistrarAuditoria<CADEVENTOFICSOC_PERSONAS>(cADEVENTOFICSOC_PERSONAS_copy, "CADEVENTOFICSOC_PERSONAS", "EVEN_SITIOGRABACION=" + cADEVENTOFICSOC_PERSONAS.EVEN_SITIOGRABACION.ToString() + ",EVEN_NUMEEVENTO=" + cADEVENTOFICSOC_PERSONAS.EVEN_NUMEEVENTO
                         + ",NRO_FICHA=" + cADEVENTOFICSOC_PERSONAS.NRO_FICHA + ",CONSECUTIVO=" + cADEVENTOFICSOC_PERSONAS.CONSECUTIVO, TipoAuditoria.Eliminar);


                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var resultados = Json(new { result = true, datos = mensaje });

                return resultados;
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return Json(new { result = false, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }



        /// <summary>
        /// metodo que trae datos de ciudad
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosCiudadEnter(string strFiltro)
        {
            try
            {
                int codigo = 0;
                JsonResult resultados = new JsonResult();
                string strResultado = string.Empty;
                string strResultadoCodigo = string.Empty;

                if (int.TryParse(strFiltro, out codigo))
                {


                    GestorAdministrador gestor;
                    DataTable dttCiudades;

                    string strQuery = string.Empty;
                    string strOrdenamiento = string.Empty;
                    string strLimites = "";
                    int? intLugeCodigo = 10;
                    objGestorGral = new GestorGeneral();
                    gestor = new GestorAdministrador();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();


                    strQuery = "WHERE CODIGO = '" + strFiltro + "' ";


                    strLimites = " AND DATOS.FILA BETWEEN " + 0 + " AND " + 1;
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(Int32)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                    dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaCiudadesAsignarBarrios", strConexion, "Nonquery");

                    foreach (DataRow fila in dttCiudades.Rows)
                    {
                        strResultado = fila["DESCRIPCION"].ToString();
                        strResultadoCodigo = fila["CODIGO"].ToString();
                    }

                }

                resultados = Json(new { result = true, datosCiudad = strResultado, codigoCiudad = strResultadoCodigo });
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
        /// Metodo que carga barrios
        /// </summary>
        /// <param name="cozoLugeCodigo"></param>
        /// <param name="cozoCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargaComunasBarrios(int? intLugeCodigo)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";


                strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                 "onclick = \"CargarBarriosComuna(luge_codigo,CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                 " Seleccionar </button>";


                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca barrios sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_BARRIOS> listaBarrios = new List<Ext_BARRIOS>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') ";
                    int codigoBarr;
                    bool result = Int32.TryParse(parametros.ConsultaValor, out codigoBarr);
                    if (result)
                    {
                        strQuery += " OR CODIGO LIKE ('%" + codigoBarr + "%') ";
                    }
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESCRIPCION";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }


                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();


                if (intLugeCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosSocial", strConexion, "Nonquery");

                    foreach (DataRow fila in dttCiudades.Rows)
                    {
                        string strBotonSelect = strBoton.Replace("luge_codigo", fila["luge_codigo"].ToString()).Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                        listaBarrios.Add(new Ext_BARRIOS(fila, strBotonSelect, true));
                        parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                        parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                    }
                }
                else
                {
                    parametros.TotalRegistros = 0;
                    parametros.TotalRegistroFiltrados = 0;
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
        /// Metodo que trae barrios con comuna
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <param name="lugeCodigo"></param>
        /// <returns></returns>
        public JsonResult DatosBarrioComunasEnter(string strFiltro, int? intLugeCodigo)
        {
            try
            {
                GestorAdministrador gestor;
                JsonResult resultados = new JsonResult();
                DataTable dttComunas;
                string strResultado = string.Empty;
                string strLugueCodigo = string.Empty;
                string strCodigo = string.Empty;

                string strQuery = string.Empty;
                string strOrdenamiento = string.Empty;
                string strLimites = "";
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();


                strQuery = " WHERE upper(DESCRIPCION) = upper('" + strFiltro + "')";
                int esNumero = 0;
                if (int.TryParse(strFiltro, out esNumero))
                    strQuery += " OR CODIGO = '" + strFiltro + "'";

                strOrdenamiento = " ORDER BY CODIGO";
                strLimites = " WHERE DATOS.FILA BETWEEN " + 0 + " AND " + 1;
                if (intLugeCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttComunas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosSocial", strConexion, "Nonquery");

                    foreach (DataRow fila in dttComunas.Rows)
                    {
                        strResultado = fila["DESCRIPCION"].ToString();
                        strLugueCodigo = fila["LUGE_CODIGO"].ToString();
                        strCodigo = fila["CODIGO"].ToString();
                    }

                    if (strLugueCodigo != "" && strCodigo != "")
                    {
                        strResultado = DescripcionBarrioConComuna(strCodigo, strLugueCodigo);

                    }
                }
                resultados = Json(new { result = true, datosBarrioComuna = strResultado, lugueCodigo = strLugueCodigo, codigo = strCodigo });
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
        /// Metodo que obtiene datos del pedido
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strNumeroEvento"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerDatosPedido(string strSitioGrabaEvento = null, string strNumeroEvento = null)
        {
            try
            {
                Dictionary<string, string> dcDatosHechos = new Dictionary<string, string>();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeroEvento", strNumeroEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_MPIO_SOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DSP_MPIOSOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_BARRIO_CODIGO_SOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_BARRIO_LUGECODIGO_SOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DSP_BARRIOSOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DIRECCION_SOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_TELEFONO_SOCIAL", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_OBSERVACIONES_SOCIAL", null, typeof(string)));


                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_datosSocialPedido", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    dcDatosHechos.Add(parameter.Item1, parameter.Item2.ToString());
                }

                var resultados = Json(new { result = true, datosHechos = dcDatosHechos });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que carga datos de registraduria 
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strIdentificacion"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosRegistraduria(string strTipoIdentificacion, string strIdentificacion)
        {
            try
            {
                string respuesta = "";
                Int64 intIdentificacion = 0;

                if (Int64.TryParse(strIdentificacion, out intIdentificacion))
                {
                    GestorGeneral objGestorGral = new GestorGeneral();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_tipoIdentificacion", strTipoIdentificacion, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_identificacion", strIdentificacion, typeof(string)));

                    respuesta = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_datosRegistraduriaMj", strConexion);
                    respuesta = respuesta == "null" ? "" : respuesta;
                }

                var resultados = Json(new { result = true, datosReg = respuesta });

                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que obtiene descripcion de comuna y barrio
        /// </summary>
        /// <param name="strBarrioCodigo"></param>
        /// <param name="strBarrioLugueCodigo"></param>
        /// <returns></returns>
        public string DescripcionBarrioConComuna(string strBarrioCodigo, string strBarrioLugueCodigo)
        {
            try
            {
                string respuesta = string.Empty;
                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_barrio_codigo", strBarrioCodigo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_barrio_lugecodigo", strBarrioLugueCodigo, typeof(decimal)));

                respuesta = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_despBarriosComuna", strConexion);


                return respuesta;


            }
            catch (Exception ex)
            {

                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que consulta el consecutivo
        /// </summary>
        /// <returns></returns>
        public string SecuenciaFichaSocial()
        {
            string strConsecutivo = "0";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));

                strConsecutivo = objGestorGral.EjecutarFuncion<int>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consecutivoFichaSocial", strConexion).ToString();

                return strConsecutivo;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

    }
}