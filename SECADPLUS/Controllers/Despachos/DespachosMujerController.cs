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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers.Despachos
{
    public class DespachosMujerController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;
        private GestorAuditoria auditoria = new GestorAuditoria();


        // GET: DespachosMujer
        [SecadPlusAutorizador("/DespachosMujer/Index")]
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

                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                if (strSitioGrabaEvento == null || strEvento == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = "No se encuentra un evento para mujer.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.TIPO_IDENTIFICACION = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_DOCUMENTO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.CODI_GENERO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_SEXO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.CODI_RAZA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_RAZA" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.MEDIO_CONOCIMIENTO = new SelectList((from t in contexto.CADTIPOSERVICIOs where t.OPCIONMUJER == "S" && t.TIPORECURSO == "RCN" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_ESPECTATIVA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_MOTIVO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_DISCAPACIDAD = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_DISCAPACIDAD" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_VICT_CONFLICTO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_VICTIMA_CONFLICTO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.NIVEL_EDUCATIVO = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "NIVEL_EDUCATIVO" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.ACTIVIDAD_ECONOMICA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "ACTIVIDAD_ECONOMICA" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.REGIMEN_SALUD = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_REGIMEN_SALUD" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.RELACION_FAMILIAR_AGRESOR = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_AGRESOR_FAMILIAR" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.RELACION_PARTICULAR_AGRESOR = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_AGRESOR_PARTICULAR" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.GRUPO_AGRESOR = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_GRUPOS_ARM1" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.MECANISMO_ARMA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_ARMA_AGRESION" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");
                ViewBag.LUGAR_OCURRENCIA = new SelectList((from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_LUGAR_OCURRE" orderby t.DESCRIPCION select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                ViewBag.TIPO_SERV_RAE = new SelectList((from t in contexto.CADTIPOSERVICIOs where t.OPCIONMUJER == "S" && t.TIPORECURSO == "RAE" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                //CAMBIAR
                ViewBag.TIPO_SERV_RM = new SelectList((from t in contexto.CADTIPOSREMISIONES where t.OPCIONMUJER == "S" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                ViewBag.TIPO_SERV_ROI = new SelectList((from t in contexto.CADTIPOSERVICIOs where t.OPCIONMUJER == "S" && t.TIPORECURSO == "ROI" orderby t.DESCRIPCION descending select new { t.CODIGO, DESCRIPCION = t.DESCRIPCION.TrimEnd() }), "CODIGO", "DESCRIPCION");

                ViewBag.EventoGlobal = strEvento;
                ViewBag.SitioGrabaEventoGlobal = strSitioGrabaEvento;


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { strSitioGrabaEvento = strSitioGrabaEvento, strEvento = strEvento });

                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////



                //ViewBag.EventoGlobal = "216894";
                //ViewBag.SitioGrabaEventoGlobal = "3947";
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            return View("Index", "BaseRecepcion");
            ///  return View();
        }

        [Authorize]
        public JsonResult ObtenerPerRelacionadasMujer(string strSitioGrabaEvento, string strEvento)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_PerRelacionesMujer> listaPerRelacionadas = new List<Ext_PerRelacionesMujer>();
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
                                                "onclick=\"EliminarRegistroGeneral('" + Url.Action("Eliminar", "DespachosMujer") + "?_ELIMINAR')\"> " +
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

                    strQuery += " WHERE REGEXP_LIKE (PERSATEN_CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
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
                        strOrderna += " ORDER BY PERSATEN_CONSECUTIVO ASC ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY PERSATEN_CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY TIPO_IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY NOMBRES " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY APELLIDOS " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY FECHA_NACE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY CODI_GENERO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY CODI_RAZA " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento", strEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaPersonasRelacionadasMj", strConexion, "Nonquery");

                foreach (DataRow fila in dttPerRelacionada.Rows)
                {
                    strBotonDestino = strBoton.Replace("_ELIMINAR", "strPasatenSitioGraba=" + fila["PERSATEN_SITIOGRABACION"].ToString() + "&strPasatenConsecutivo=" + fila["PERSATEN_CONSECUTIVO"].ToString() +
                                                                            "&strEventoSitioGraba=" + fila["EVEN_SITIOGRABACION"].ToString() + "&strEvento=" + fila["EVEN_NUMEEVENTO"].ToString());

                    listaPerRelacionadas.Add(new Ext_PerRelacionesMujer(fila, strBotonDestino));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPerRelacionadas });
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
        /// Metodo que elimina un registro
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> Eliminar(int? strPasatenSitioGraba, int? strPasatenConsecutivo, int? strEventoSitioGraba, int? strEvento)
        {
            try
            {
                if (strPasatenSitioGraba == null && strPasatenConsecutivo == null && strEventoSitioGraba == null && strEvento == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPERSATEND_REMEVENMUJ cADPERSATEND_REMEVENMUJ = await contexto.CADPERSATEND_REMEVENMUJ.FindAsync(strPasatenSitioGraba, strPasatenConsecutivo, strEventoSitioGraba, strEvento);
                if (cADPERSATEND_REMEVENMUJ == null)
                {
                    return HttpNotFound();
                }

                return View("Delete", null, cADPERSATEND_REMEVENMUJ);
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
        public async Task<ActionResult> DeleteConfirmed(int? strPasatenSitioGraba, int? strPasatenConsecutivo, int? strEventoSitioGraba, int? strEvento)
        {
            try
            {
                CADPERSATEND_REMEVENMUJ cADPERSATEND_REMEVENMUJ = await contexto.CADPERSATEND_REMEVENMUJ.FindAsync(strPasatenSitioGraba, strPasatenConsecutivo, strEventoSitioGraba, strEvento);

                contexto.CADPERSATEND_REMEVENMUJ.Remove(cADPERSATEND_REMEVENMUJ);
                await contexto.SaveChangesAsync();

                CADPERSATEND_REMEVENMUJ cADPERSATEND_REMEVENMUJ_copy = new CADPERSATEND_REMEVENMUJ();
                cADPERSATEND_REMEVENMUJ_copy = (CADPERSATEND_REMEVENMUJ)auditoria.GenerarCopiaAuditoria(cADPERSATEND_REMEVENMUJ);

                auditoria.RegistrarAuditoria<CADPERSATEND_REMEVENMUJ>(cADPERSATEND_REMEVENMUJ_copy, "CADPERSATEND_REMEVENMUJ", "PERSATEN_SITIOGRABACION=" + cADPERSATEND_REMEVENMUJ.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO
                     + ",EVEN_SITIOGRABACION=" + cADPERSATEND_REMEVENMUJ.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPERSATEND_REMEVENMUJ.EVEN_NUMEEVENTO, TipoAuditoria.Eliminar);



                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                return RedirectToAction("Index", new { strMensaje = mensaje, strSitioGrabaEvento = strEventoSitioGraba, strEvento = strEvento });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que obtiene RAE mujer
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerRAEMujer(string strPersatenSitiograbacion = null, string strPersatenConsecutivo = null, string strSitioGrabaEvento = null, string strEvento = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_RAE_Mujer> listaRAEMujer = new List<Ext_RAE_Mujer>();
                GestorGeneral objGestorGral = new GestorGeneral();
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
                                                "onclick=\"EliminarRAEMujer(_paramEliminar)\"> " +
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
                                       "REGEXP_LIKE (DESC_TIPO_SERVICIO_RAE, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO";
                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY DESC_TIPO_SERVICIO_RAE " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                if (strPersatenSitiograbacion != null && strPersatenConsecutivo != null && strSitioGrabaEvento != null && strEvento != null)
                {
                    strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenSitiograbacion", strPersatenSitiograbacion, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenConsecutivo", strPersatenConsecutivo, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenSitiograbacion", strSitioGrabaEvento, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenNumeevento", strEvento, typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaDatosRAEMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttPerRelacionada.Rows)
                    {
                        strBotonDestino = strBoton.Replace("_paramEliminar", fila["PERSATEN_SITIOGRABACION"].ToString() + "," + fila["PERSATEN_CONSECUTIVO"].ToString() +
                                                                                "," + fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," + fila["CONSECUTIVO"].ToString());

                        listaRAEMujer.Add(new Ext_RAE_Mujer(fila, strBotonDestino));
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

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRAEMujer });
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
        /// Metodo que obtiene RM mujer
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerRMMujer(string strPersatenSitiograbacion = null, string strPersatenConsecutivo = null, string strSitioGrabaEvento = null, string strEvento = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_RM_Mujer> listaRMMujer = new List<Ext_RM_Mujer>();
                GestorGeneral objGestorGral = new GestorGeneral();
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
                                                "onclick=\"EliminarRMMujer(_paramEliminar)\"> " +
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
                                       "REGEXP_LIKE (DESC_TIPO_SERVICIO_RAE, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO";
                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY DESC_TIPO_SERVICIO_RM " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                if (strPersatenSitiograbacion != null && strPersatenConsecutivo != null && strSitioGrabaEvento != null && strEvento != null)
                {
                    strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenSitiograbacion", strPersatenSitiograbacion, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenConsecutivo", strPersatenConsecutivo, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenSitiograbacion", strSitioGrabaEvento, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenNumeevento", strEvento, typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaDatosRMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttPerRelacionada.Rows)
                    {
                        strBotonDestino = strBoton.Replace("_paramEliminar", fila["PERSATEN_SITIOGRABACION"].ToString() + "," + fila["PERSATEN_CONSECUTIVO"].ToString() +
                                                                                "," + fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," + fila["CONSECUTIVO"].ToString());

                        listaRMMujer.Add(new Ext_RM_Mujer(fila, strBotonDestino));
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

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaRMMujer });
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
        /// Metodo que obtiene RM mujer
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerROIMujer(string strPersatenSitiograbacion = null, string strPersatenConsecutivo = null, string strSitioGrabaEvento = null, string strEvento = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_ROI_Mujer> listaROIMujer = new List<Ext_ROI_Mujer>();
                GestorGeneral objGestorGral = new GestorGeneral();
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
                                                "onclick=\"EliminarROIMujer(_paramEliminar)\"> " +
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
                                       "REGEXP_LIKE (DESC_TIPO_SERVICIO_RAE, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO";
                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY DESC_TIPO_SERVICIO_ROI " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                if (strPersatenSitiograbacion != null && strPersatenConsecutivo != null && strSitioGrabaEvento != null && strEvento != null)
                {
                    strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenSitiograbacion", strPersatenSitiograbacion, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_persatenConsecutivo", strPersatenConsecutivo, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenSitiograbacion", strSitioGrabaEvento, typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evenNumeevento", strEvento, typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaDatosROIMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttPerRelacionada.Rows)
                    {
                        strBotonDestino = strBoton.Replace("_paramEliminar", fila["PERSATEN_SITIOGRABACION"].ToString() + "," + fila["PERSATEN_CONSECUTIVO"].ToString() +
                                                                                "," + fila["EVEN_SITIOGRABACION"].ToString() + "," + fila["EVEN_NUMEEVENTO"].ToString() + "," + fila["CONSECUTIVO"].ToString());

                        listaROIMujer.Add(new Ext_ROI_Mujer(fila, strBotonDestino));
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

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaROIMujer });
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
        /// Metodo que obtiene datos pedido
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <param name="strNumeroEvento"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerDatosHechos(string strSitioGrabaEvento = null, string strNumeroEvento = null)
        {
            try
            {
                Dictionary<string, string> dcDatosHechos = new Dictionary<string, string>();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeroEvento", strNumeroEvento, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGrabaEvento", strSitioGrabaEvento, typeof(decimal)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_MPIO_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DSP_MPIOHECOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_BARRIO_CODIGO_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_BARRIO_LUGECODIGO_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DSP_BARRIOHECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DIRECCION_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_COZO_LUGECODIGO_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_COZO_CODIGO_HECHOS", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_DSP_COMUNAHECHOS", null, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_datosHechosMujer", strConexion, "MULTIPLE");

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
        /// Crea datos RAE de mujer
        /// </summary>
        /// <param name="strDatosNuevoRAE"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearRAEMujer(string[] strDatosNuevoRAE)
        {
            try
            {
                CADPEATREEV_DATASOCMUJ_RAE cADPEATREEV_DATASOCMUJ_RAE = new CADPEATREEV_DATASOCMUJ_RAE();
                objGestorGral = new GestorGeneral();
                string strMensaje = "";
                int intPersatenSitio = 0;
                int intPersatenConsecutivo = 0;
                int inteventSitioGraba = 0;
                int intevent = 0;

                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                if (strDatosNuevoRAE[4] == "")
                {
                    intPersatenSitio = Convert.ToInt32(strDatosNuevoRAE[0]);
                    intPersatenConsecutivo = Convert.ToInt32(strDatosNuevoRAE[1]);
                    inteventSitioGraba = Convert.ToInt32(strDatosNuevoRAE[2]);
                    intevent = Convert.ToInt32(strDatosNuevoRAE[3]);

                    var consecutivo = (from t in contexto.CADPEATREEV_DATASOCMUJ_RAE
                                       where
                    t.PERSATEN_SITIOGRABACION == intPersatenSitio
                    && t.PERSATEN_CONSECUTIVO == intPersatenConsecutivo
                    && t.EVEN_SITIOGRABACION == inteventSitioGraba
                    && t.EVEN_NUMEEVENTO == intevent
                                       orderby t.CONSECUTIVO descending
                                       select t.CONSECUTIVO).FirstOrDefault();
                    Int16 intConsecutivo = Convert.ToInt16(consecutivo == 0 ? 1 : (consecutivo + 1));

                    cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRAE[0]);
                    cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoRAE[1]);
                    cADPEATREEV_DATASOCMUJ_RAE.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRAE[2]);
                    cADPEATREEV_DATASOCMUJ_RAE.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoRAE[3]);
                    cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO = intConsecutivo;
                    cADPEATREEV_DATASOCMUJ_RAE.TIPO_SERV_RAE = strDatosNuevoRAE[5];
                    cADPEATREEV_DATASOCMUJ_RAE.OBSERVACION = strDatosNuevoRAE[6];

                    cADPEATREEV_DATASOCMUJ_RAE.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_RAE.CADUSUARIO = usr.USUARIO;

                    contexto.CADPEATREEV_DATASOCMUJ_RAE.Add(cADPEATREEV_DATASOCMUJ_RAE);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO.ToString());
                }
                else
                {
                    cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRAE[0]);
                    cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoRAE[1]);
                    cADPEATREEV_DATASOCMUJ_RAE.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRAE[2]);
                    cADPEATREEV_DATASOCMUJ_RAE.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoRAE[3]);
                    cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO = Convert.ToInt16(strDatosNuevoRAE[4]);
                    cADPEATREEV_DATASOCMUJ_RAE.TIPO_SERV_RAE = strDatosNuevoRAE[5];
                    cADPEATREEV_DATASOCMUJ_RAE.OBSERVACION = strDatosNuevoRAE[6];

                    cADPEATREEV_DATASOCMUJ_RAE.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_RAE.CADUSUARIO = usr.USUARIO;

                    contexto.Entry(cADPEATREEV_DATASOCMUJ_RAE).State = EntityState.Modified;
                    contexto.SaveChanges();
                    auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_RAE>(cADPEATREEV_DATASOCMUJ_RAE, "CADPEATREEV_DATASOCMUJ_RAE", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_CONSECUTIVO
                         + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RAE.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_RAE.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO, TipoAuditoria.Modificar);

                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO.ToString());
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
        /// Elimina datos RAE de mujer
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <param name="strConsecutivoRAE"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult DeleteRAE(int? strPasatenSitioGraba, int? strPasatenConsecutivo, int? strEventoSitioGraba, int? strEvento, Int16? strConsecutivoRAE)
        {
            try
            {
                CADPEATREEV_DATASOCMUJ_RAE cADPEATREEV_DATASOCMUJ_RAE = contexto.CADPEATREEV_DATASOCMUJ_RAE.Find(strPasatenSitioGraba, strPasatenConsecutivo, strEventoSitioGraba, strEvento, strConsecutivoRAE);

                contexto.CADPEATREEV_DATASOCMUJ_RAE.Remove(cADPEATREEV_DATASOCMUJ_RAE);
                contexto.SaveChanges();



                CADPEATREEV_DATASOCMUJ_RAE cADPEATREEV_DATASOCMUJ_RAE_copy = new CADPEATREEV_DATASOCMUJ_RAE();
                cADPEATREEV_DATASOCMUJ_RAE_copy = (CADPEATREEV_DATASOCMUJ_RAE)auditoria.GenerarCopiaAuditoria(cADPEATREEV_DATASOCMUJ_RAE);

                auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_RAE>(cADPEATREEV_DATASOCMUJ_RAE_copy, "CADPEATREEV_DATASOCMUJ_RAE", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RAE.PERSATEN_CONSECUTIVO
                        + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RAE.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_RAE.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RAE.CONSECUTIVO, TipoAuditoria.Eliminar);


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
        /// Crea datos RM de mujer
        /// </summary>
        /// <param name="strDatosNuevoRM"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearRMMujer(string[] strDatosNuevoRM)
        {
            try
            {
                CADPEATREEV_DATASOCMUJ_RM cADPEATREEV_DATASOCMUJ_RM = new CADPEATREEV_DATASOCMUJ_RM();
                objGestorGral = new GestorGeneral();
                string strMensaje = "";
                int intPersatenSitio = 0;
                int intPersatenConsecutivo = 0;
                int inteventSitioGraba = 0;
                int intevent = 0;

                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                if (strDatosNuevoRM[4] == "")
                {
                    intPersatenSitio = Convert.ToInt32(strDatosNuevoRM[0]);
                    intPersatenConsecutivo = Convert.ToInt32(strDatosNuevoRM[1]);
                    inteventSitioGraba = Convert.ToInt32(strDatosNuevoRM[2]);
                    intevent = Convert.ToInt32(strDatosNuevoRM[3]);

                    var consecutivo = (from t in contexto.CADPEATREEV_DATASOCMUJ_RM
                                       where
                        t.PERSATEN_SITIOGRABACION == intPersatenSitio
                        && t.PERSATEN_CONSECUTIVO == intPersatenConsecutivo
                        && t.EVEN_SITIOGRABACION == inteventSitioGraba
                        && t.EVEN_NUMEEVENTO == intevent
                                       orderby t.CONSECUTIVO descending
                                       select t.CONSECUTIVO).FirstOrDefault();
                    Int16 intConsecutivo = Convert.ToInt16(consecutivo == 0 ? 1 : (consecutivo + 1));

                    cADPEATREEV_DATASOCMUJ_RM.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRM[0]);
                    cADPEATREEV_DATASOCMUJ_RM.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoRM[1]);
                    cADPEATREEV_DATASOCMUJ_RM.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRM[2]);
                    cADPEATREEV_DATASOCMUJ_RM.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoRM[3]);
                    cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO = intConsecutivo;
                    cADPEATREEV_DATASOCMUJ_RM.TIPO_SERV_RM = strDatosNuevoRM[5];
                    cADPEATREEV_DATASOCMUJ_RM.OBSERVACION = strDatosNuevoRM[6];

                    cADPEATREEV_DATASOCMUJ_RM.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_RM.CADUSUARIO = usr.USUARIO;

                    contexto.CADPEATREEV_DATASOCMUJ_RM.Add(cADPEATREEV_DATASOCMUJ_RM);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO.ToString());
                }
                else
                {
                    cADPEATREEV_DATASOCMUJ_RM.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRM[0]);
                    cADPEATREEV_DATASOCMUJ_RM.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoRM[1]);
                    cADPEATREEV_DATASOCMUJ_RM.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoRM[2]);
                    cADPEATREEV_DATASOCMUJ_RM.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoRM[3]);
                    cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO = Convert.ToInt16(strDatosNuevoRM[4]);
                    cADPEATREEV_DATASOCMUJ_RM.TIPO_SERV_RM = strDatosNuevoRM[5];
                    cADPEATREEV_DATASOCMUJ_RM.OBSERVACION = strDatosNuevoRM[6];

                    cADPEATREEV_DATASOCMUJ_RM.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_RM.CADUSUARIO = usr.USUARIO;

                    contexto.Entry(cADPEATREEV_DATASOCMUJ_RM).State = EntityState.Modified;
                    contexto.SaveChanges();
                    auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_RM>(cADPEATREEV_DATASOCMUJ_RM, "CADPEATREEV_DATASOCMUJ_RM", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RM.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RM.PERSATEN_CONSECUTIVO
                        + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RM.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_RM.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO, TipoAuditoria.Modificar);

                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO.ToString());
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
        /// Metodo que elimina RM mujer
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <param name="strConsecutivoRM"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult DeleteRM(int? strPasatenSitioGraba, int? strPasatenConsecutivo, int? strEventoSitioGraba, int? strEvento, Int16? strConsecutivoRM)
        {
            try
            {
                CADPEATREEV_DATASOCMUJ_RM cADPEATREEV_DATASOCMUJ_RM = contexto.CADPEATREEV_DATASOCMUJ_RM.Find(strPasatenSitioGraba, strPasatenConsecutivo, strEventoSitioGraba, strEvento, strConsecutivoRM);

                contexto.CADPEATREEV_DATASOCMUJ_RM.Remove(cADPEATREEV_DATASOCMUJ_RM);
                contexto.SaveChanges();

                CADPEATREEV_DATASOCMUJ_RM cADPEATREEV_DATASOCMUJ_RM_copy = new CADPEATREEV_DATASOCMUJ_RM();
                cADPEATREEV_DATASOCMUJ_RM_copy = (CADPEATREEV_DATASOCMUJ_RM)auditoria.GenerarCopiaAuditoria(cADPEATREEV_DATASOCMUJ_RM);


                auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_RM>(cADPEATREEV_DATASOCMUJ_RM_copy, "CADPEATREEV_DATASOCMUJ_RM", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RM.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RM.PERSATEN_CONSECUTIVO
                       + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_RM.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_RM.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_RM.CONSECUTIVO, TipoAuditoria.Eliminar);


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
        /// Metodo que crea ROI mujer
        /// </summary>
        /// <param name="strDatosNuevoROI"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearROIMujer(string[] strDatosNuevoROI)
        {
            try
            {
                CADPEATREEV_DATASOCMUJ_ROI cADPEATREEV_DATASOCMUJ_ROI = new CADPEATREEV_DATASOCMUJ_ROI();
                objGestorGral = new GestorGeneral();
                string strMensaje = "";
                int intPersatenSitio = 0;
                int intPersatenConsecutivo = 0;
                int inteventSitioGraba = 0;
                int intevent = 0;

                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                if (strDatosNuevoROI[4] == "")
                {
                    intPersatenSitio = Convert.ToInt32(strDatosNuevoROI[0]);
                    intPersatenConsecutivo = Convert.ToInt32(strDatosNuevoROI[1]);
                    inteventSitioGraba = Convert.ToInt32(strDatosNuevoROI[2]);
                    intevent = Convert.ToInt32(strDatosNuevoROI[3]);

                    var consecutivo = (from t in contexto.CADPEATREEV_DATASOCMUJ_ROI
                                       where
                                        t.PERSATEN_SITIOGRABACION == intPersatenSitio
                                        && t.PERSATEN_CONSECUTIVO == intPersatenConsecutivo
                                        && t.EVEN_SITIOGRABACION == inteventSitioGraba
                                        && t.EVEN_NUMEEVENTO == intevent
                                       orderby t.CONSECUTIVO descending
                                       select t.CONSECUTIVO).FirstOrDefault();
                    Int16 intConsecutivo = Convert.ToInt16(consecutivo == 0 ? 1 : (consecutivo + 1));

                    cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoROI[0]);
                    cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoROI[1]);
                    cADPEATREEV_DATASOCMUJ_ROI.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoROI[2]);
                    cADPEATREEV_DATASOCMUJ_ROI.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoROI[3]);
                    cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO = intConsecutivo;
                    cADPEATREEV_DATASOCMUJ_ROI.TIPO_SERV_ROI = strDatosNuevoROI[5];
                    cADPEATREEV_DATASOCMUJ_ROI.OBSERVACION = strDatosNuevoROI[6];

                    cADPEATREEV_DATASOCMUJ_ROI.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_ROI.CADUSUARIO = usr.USUARIO;

                    contexto.CADPEATREEV_DATASOCMUJ_ROI.Add(cADPEATREEV_DATASOCMUJ_ROI);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO.ToString());
                }
                else
                {
                    cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoROI[0]);
                    cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosNuevoROI[1]);
                    cADPEATREEV_DATASOCMUJ_ROI.EVEN_SITIOGRABACION = Convert.ToInt32(strDatosNuevoROI[2]);
                    cADPEATREEV_DATASOCMUJ_ROI.EVEN_NUMEEVENTO = Convert.ToInt32(strDatosNuevoROI[3]);
                    cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO = Convert.ToInt16(strDatosNuevoROI[4]);
                    cADPEATREEV_DATASOCMUJ_ROI.TIPO_SERV_ROI = strDatosNuevoROI[5];
                    cADPEATREEV_DATASOCMUJ_ROI.OBSERVACION = strDatosNuevoROI[6];

                    cADPEATREEV_DATASOCMUJ_ROI.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPEATREEV_DATASOCMUJ_ROI.CADUSUARIO = usr.USUARIO;

                    contexto.Entry(cADPEATREEV_DATASOCMUJ_ROI).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_ROI>(cADPEATREEV_DATASOCMUJ_ROI, "cADPEATREEV_DATASOCMUJ_ROI", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_CONSECUTIVO
                    + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_ROI.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_ROI.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO, TipoAuditoria.Modificar);



                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO.ToString());
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
        /// Metodo que elimina ROI mujer
        /// </summary>
        /// <param name="strPasatenSitioGraba"></param>
        /// <param name="strPasatenConsecutivo"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <param name="strConsecutivoROI"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult DeleteROI(int? strPasatenSitioGraba, int? strPasatenConsecutivo, int? strEventoSitioGraba, int? strEvento, Int16? strConsecutivoROI)
        {
            try
            {

                CADPEATREEV_DATASOCMUJ_ROI cADPEATREEV_DATASOCMUJ_ROI = contexto.CADPEATREEV_DATASOCMUJ_ROI.Find(strPasatenSitioGraba, strPasatenConsecutivo, strEventoSitioGraba, strEvento, strConsecutivoROI);

                contexto.CADPEATREEV_DATASOCMUJ_ROI.Remove(cADPEATREEV_DATASOCMUJ_ROI);
                contexto.SaveChanges();


                CADPEATREEV_DATASOCMUJ_ROI cADPEATREEV_DATASOCMUJ_ROI_copy = new CADPEATREEV_DATASOCMUJ_ROI();
                cADPEATREEV_DATASOCMUJ_ROI_copy = (CADPEATREEV_DATASOCMUJ_ROI)auditoria.GenerarCopiaAuditoria(cADPEATREEV_DATASOCMUJ_ROI);

                auditoria.RegistrarAuditoria<CADPEATREEV_DATASOCMUJ_ROI>(cADPEATREEV_DATASOCMUJ_ROI_copy, "cADPEATREEV_DATASOCMUJ_ROI", "PERSATEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_ROI.PERSATEN_CONSECUTIVO
                + ",EVEN_SITIOGRABACION=" + cADPEATREEV_DATASOCMUJ_ROI.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPEATREEV_DATASOCMUJ_ROI.EVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADPEATREEV_DATASOCMUJ_ROI.CONSECUTIVO, TipoAuditoria.Eliminar);


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
        /// Metodo que carga ciudades
        /// </summary>
        /// <param name="tab"></param>
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
                               "onclick = \"CargarCiudadHechos(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
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
                int? intLugeCodigo = 10;
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
        /// Metodo que caega comunas
        /// </summary>
        /// <param name="lugeCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult CargaComunas(int? lugeCodigo, int? tab)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";
                if (tab == 1)
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                          "onclick = \"CargarComunaResidencia(luge_codigo,CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                          " Seleccionar </button>";
                }
                else
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                         "onclick = \"CargarComunaHecho(luge_codigo,CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                         " Seleccionar </button>";
                }

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca comunas sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_COMUNAS> listaComunas = new List<Ext_COMUNAS>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%')  ";
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
                        strOrdenamiento += " ORDER BY CODIGO";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                //  if (strQuery == string.Empty)
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                //else
                //    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();

                if (lugeCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", lugeCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaComunasPorCiudadMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttCiudades.Rows)
                    {
                        string strBotonSelect = strBoton.Replace("luge_codigo", fila["luge_codigo"].ToString()).Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                        listaComunas.Add(new Ext_COMUNAS(fila, strBotonSelect, true));
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

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaComunas });
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
        public JsonResult CargaComunasBarrios(int? cozoLugeCodigo, int? cozoCodigo, int? tab)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";

                if (tab == 1)
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                     "onclick = \"CargarBarriosComunaResidencia(luge_codigo,CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                     " Seleccionar </button>";
                }
                else
                {
                    strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                    "onclick = \"CargarBarriosComunaHecho(luge_codigo,CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                    " Seleccionar </button>";
                }

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
                        strOrdenamiento += " ORDER BY CODIGO";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                //if (strQuery == string.Empty)
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                //else
                //    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();

                if (cozoLugeCodigo != null && cozoCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cozo_luge_codigo", cozoLugeCodigo.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cozo_codigo", cozoCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosComunasMj", strConexion, "Nonquery");

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
        /// Metodo que crea persona relacionado
        /// </summary>
        /// <param name="strDatosPersonaRelacion"></param>
        /// <param name="strEventoSitioGraba"></param>
        /// <param name="strEvento"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult CrearPersonaRelacionada(string[] strDatosPersonaRelacion, string strEventoSitioGraba, string strEvento)
        {
            try
            {
                CADPERSATEND_REMEVENMUJ cADPERSATEND_REMEVENMUJ = new CADPERSATEND_REMEVENMUJ();

                objGestorGral = new GestorGeneral();
                string strMensaje = "";

                if (Session["USUARIO_SECAD"] == null)
                {
                    strMensaje = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    var resultadosError = Json(new { result = false, datos = strMensaje });
                    return resultadosError;
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                cADPERSATEND_REMEVENMUJ.TIPO_IDENTIFICACION = strDatosPersonaRelacion[1].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.IDENTIFICACION = strDatosPersonaRelacion[2].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.NOMBRES = strDatosPersonaRelacion[3].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.APELLIDOS = strDatosPersonaRelacion[4].Split('=')[1];
                if (strDatosPersonaRelacion[5].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.FECHA_NACE = Convert.ToDateTime(strDatosPersonaRelacion[5].ToString().Split('=')[1]);
                cADPERSATEND_REMEVENMUJ.CODI_GENERO = strDatosPersonaRelacion[6].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.CODI_RAZA = strDatosPersonaRelacion[7].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TIPO_INTERVINIENTE = strDatosPersonaRelacion[8].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.CALIDAD_REPORTANTE = strDatosPersonaRelacion[9].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.MEDIO_CONOCIMIENTO = strDatosPersonaRelacion[10].Split('=')[1];
                if (strDatosPersonaRelacion[11].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.NRO_FICHA = Convert.ToDecimal(strDatosPersonaRelacion[11].ToString().Split('=')[1]);
                else
                    cADPERSATEND_REMEVENMUJ.NRO_FICHA = Convert.ToInt32(strEvento);

                cADPERSATEND_REMEVENMUJ.COND_LGTBI = strDatosPersonaRelacion[12].Split('=')[1];
                if (strDatosPersonaRelacion[13].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.EDAD_PERSONA = Convert.ToInt16(strDatosPersonaRelacion[13].Split('=')[1]);
                cADPERSATEND_REMEVENMUJ.COND_DISCAPACIDAD = strDatosPersonaRelacion[14].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TIPO_DISCAPACIDAD = strDatosPersonaRelacion[15].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.COND_DESPLAZADO = strDatosPersonaRelacion[16].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TIPO_VICT_CONFLICTO = strDatosPersonaRelacion[17].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.NIVEL_EDUCATIVO = strDatosPersonaRelacion[18].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.ACTIVIDAD_ECONOMICA = strDatosPersonaRelacion[19].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.PARTICIP_ORGANIZACION = strDatosPersonaRelacion[20].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.NOMBRE_ORGANIZACION = strDatosPersonaRelacion[21].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.REGIMEN_SALUD = strDatosPersonaRelacion[22].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.NOMBRE_IPS = strDatosPersonaRelacion[23].Split('=')[1];
                if (strDatosPersonaRelacion[24].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.MPIO_RESIDENCIA = Convert.ToInt32(strDatosPersonaRelacion[24].Split('=')[1]);
                if (strDatosPersonaRelacion[25].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.COZO_LUGECODIGO_RESIDENCIA = Convert.ToInt32(strDatosPersonaRelacion[25].Split('=')[1]);
                if (strDatosPersonaRelacion[26].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.COZO_CODIGO_RESIDENCIA = Convert.ToInt32(strDatosPersonaRelacion[26].Split('=')[1]);
                if (strDatosPersonaRelacion[27].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.BARRIO_CODIGO_RESIDENCIA = Convert.ToInt32(strDatosPersonaRelacion[27].Split('=')[1]);
                if (strDatosPersonaRelacion[28].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.BARRIO_LUGECODIGO_RESIDENCIA = Convert.ToInt32(strDatosPersonaRelacion[28].Split('=')[1]);
                cADPERSATEND_REMEVENMUJ.DIRECCION_RESIDENCIA = strDatosPersonaRelacion[29].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TELEFONO1 = strDatosPersonaRelacion[30].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TELEFONO2 = strDatosPersonaRelacion[31].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TIPO_RELACION_AGRESOR = strDatosPersonaRelacion[32].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.RELACION_FAMILIAR_AGRESOR = strDatosPersonaRelacion[33].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.RELACION_PARTICULAR_AGRESOR = strDatosPersonaRelacion[34].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.GRUPO_AGRESOR = strDatosPersonaRelacion[35].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.DESCRIPCION_GRUPO = strDatosPersonaRelacion[36].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.MECANISMO_ARMA = strDatosPersonaRelacion[37].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.TIPO_ESPECTATIVA = strDatosPersonaRelacion[38].Split('=')[1];
                if (strDatosPersonaRelacion[39].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.MPIO_HECHOS = Convert.ToInt32(strDatosPersonaRelacion[39].Split('=')[1]);
                if (strDatosPersonaRelacion[40].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.COZO_LUGECODIGO_HECHOS = Convert.ToInt32(strDatosPersonaRelacion[40].Split('=')[1]);
                if (strDatosPersonaRelacion[41].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.COZO_CODIGO_HECHOS = Convert.ToInt32(strDatosPersonaRelacion[41].Split('=')[1]);
                if (strDatosPersonaRelacion[42].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.BARRIO_CODIGO_HECHOS = Convert.ToInt32(strDatosPersonaRelacion[42].Split('=')[1]);
                if (strDatosPersonaRelacion[43].ToString().Split('=')[1] != "")
                    cADPERSATEND_REMEVENMUJ.BARRIO_LUGECODIGO_HECHOS = Convert.ToInt32(strDatosPersonaRelacion[43].Split('=')[1]);
                cADPERSATEND_REMEVENMUJ.DIRECCION_HECHOS = strDatosPersonaRelacion[44].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.LUGAR_OCURRENCIA = strDatosPersonaRelacion[45].Split('=')[1];
                cADPERSATEND_REMEVENMUJ.NOMBRE_SITIO = strDatosPersonaRelacion[46].Split('=')[1];

                if (strDatosPersonaRelacion[0].Split('=')[1] == "")
                {
                    Int64 intConsecutivo = 0;
                    string strTipoIden = strDatosPersonaRelacion[1].Split('=')[1];
                    string strIden = strDatosPersonaRelacion[2].Split('=')[1];

                    CADPERSONAS_ATENDIDAS cADPERSONAS_ATENDIDAS_eXISTE = contexto.CADPERSONAS_ATENDIDAS.Where(t => t.TIPO_IDENTIFICACION == strTipoIden && t.IDENTIFICACION == strIden).FirstOrDefault();
                    if (cADPERSONAS_ATENDIDAS_eXISTE == null)
                    {
                        intConsecutivo = SecuenciaPerMujer();
                        CADPERSONAS_ATENDIDAS cADPERSONAS_ATENDIDAS = new CADPERSONAS_ATENDIDAS();

                        cADPERSONAS_ATENDIDAS.SITIO_GRABACION = usr.SITIO_GRABACION;
                        cADPERSONAS_ATENDIDAS.CONSECUTIVO = intConsecutivo;
                        cADPERSONAS_ATENDIDAS.TIPO_IDENTIFICACION = strDatosPersonaRelacion[1].Split('=')[1];
                        cADPERSONAS_ATENDIDAS.IDENTIFICACION = strDatosPersonaRelacion[2].Split('=')[1];
                        cADPERSONAS_ATENDIDAS.NOMBRES = strDatosPersonaRelacion[3].Split('=')[1];
                        cADPERSONAS_ATENDIDAS.APELLIDOS = strDatosPersonaRelacion[4].Split('=')[1];
                        if (strDatosPersonaRelacion[5].ToString().Split('=')[1] != "")
                            cADPERSONAS_ATENDIDAS.FECHA_NACE = Convert.ToDateTime(strDatosPersonaRelacion[5].ToString().Split('=')[1]);
                        cADPERSONAS_ATENDIDAS.CODI_GENERO = strDatosPersonaRelacion[6].Split('=')[1];
                        cADPERSONAS_ATENDIDAS.CODI_RAZA = strDatosPersonaRelacion[7].Split('=')[1];
                        cADPERSONAS_ATENDIDAS.CADUSUARIO = usr.USUARIO;
                        cADPERSONAS_ATENDIDAS.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;

                        contexto.CADPERSONAS_ATENDIDAS.Add(cADPERSONAS_ATENDIDAS);
                    }
                    else
                    {
                        intConsecutivo = cADPERSONAS_ATENDIDAS_eXISTE.CONSECUTIVO;
                    }

                    // contexto.SaveChanges();

                    int inteventSitioGraba = Convert.ToInt32(strEventoSitioGraba);
                    int intevent = Convert.ToInt32(strEvento);

                    cADPERSATEND_REMEVENMUJ.EVEN_NUMEEVENTO = intevent;
                    cADPERSATEND_REMEVENMUJ.EVEN_SITIOGRABACION = inteventSitioGraba;
                    cADPERSATEND_REMEVENMUJ.PERSATEN_SITIOGRABACION = usr.SITIO_GRABACION;

                    cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO = intConsecutivo;
                    cADPERSATEND_REMEVENMUJ.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPERSATEND_REMEVENMUJ.CADUSUARIO = usr.USUARIO;

                    contexto.CADPERSATEND_REMEVENMUJ.Add(cADPERSATEND_REMEVENMUJ);
                    contexto.SaveChanges();
                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO.ToString());
                }
                else
                {
                    int inteventSitioGraba = Convert.ToInt32(strEventoSitioGraba);
                    int intevent = Convert.ToInt32(strEvento);

                    cADPERSATEND_REMEVENMUJ.EVEN_NUMEEVENTO = intevent;
                    cADPERSATEND_REMEVENMUJ.EVEN_SITIOGRABACION = inteventSitioGraba;
                    cADPERSATEND_REMEVENMUJ.PERSATEN_SITIOGRABACION = usr.SITIO_GRABACION;

                    cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO = Convert.ToInt32(strDatosPersonaRelacion[0].Split('=')[1]);
                    cADPERSATEND_REMEVENMUJ.FECHA_TRANSACCION = (DateTime)objGestorGral.ObtenerTiempoServidor;
                    cADPERSATEND_REMEVENMUJ.CADUSUARIO = usr.USUARIO;

                    contexto.Entry(cADPERSATEND_REMEVENMUJ).State = EntityState.Modified;
                    contexto.SaveChanges();

                    auditoria.RegistrarAuditoria<CADPERSATEND_REMEVENMUJ>(cADPERSATEND_REMEVENMUJ, "CADPERSATEND_REMEVENMUJ", "PERSATEN_SITIOGRABACION=" + cADPERSATEND_REMEVENMUJ.PERSATEN_SITIOGRABACION.ToString() + ",PERSATEN_CONSECUTIVO=" + cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO
                         + ",EVEN_SITIOGRABACION=" + cADPERSATEND_REMEVENMUJ.EVEN_SITIOGRABACION + ",EVEN_NUMEEVENTO=" + cADPERSATEND_REMEVENMUJ.EVEN_NUMEEVENTO, TipoAuditoria.Modificar);


                    strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADPERSATEND_REMEVENMUJ.PERSATEN_CONSECUTIVO.ToString());
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
        /// Metodo que consulta el consecutivo
        /// </summary>
        /// <returns></returns>
        public int SecuenciaPerMujer()
        {
            int consecutivo = 0;

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));

                consecutivo = objGestorGral.EjecutarFuncion<int>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consecutivoPersatenmujer", strConexion);

                return consecutivo;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que carga datos de registraduria 
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strIdentificacion"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosRegistraduriaMj(string strTipoIdentificacion, string strIdentificacion)
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
        /// Metodo que obtiene datos de personas atendiddas
        /// </summary>
        /// <param name="strTipoIdentificacion"></param>
        /// <param name="strIdentificacion"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosRegisAtendidosMj(string strTipoIdentificacion, string strIdentificacion)
        {
            try
            {

                JsonResult resultados = new JsonResult();
                Dictionary<string, string> datosAntecedentes = new Dictionary<string, string>();


                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_tipoIdentificacion", strTipoIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_identificacion", strIdentificacion, typeof(string)));


                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_PERSATEN_SITIOGRABACION", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_PERSATEN_CONSECUTIVO", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_apellidos", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_nombres", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_fecha_nace", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_codi_genero", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_codi_raza", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_resultado", null, typeof(string)));



                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_datosRegisAtendidoSMj", strConexion, "MULTIPLE");

                foreach (Tuple<string, object, Type> parameter in objGestorGral.ParamsSalida)
                {
                    datosAntecedentes.Add(parameter.Item1, parameter.Item2.ToString());
                }


                resultados = Json(new { result = true, datosAtendidos = datosAntecedentes });
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
        /// 
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosCiudadEnter(string strFiltro)
        {
            try
            {
                GestorAdministrador gestor;
                JsonResult resultados = new JsonResult();
                DataTable dttCiudades;
                string strResultado = string.Empty;
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
                }



                resultados = Json(new { result = true, datosCiudad = strResultado });
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
        /// 
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <param name="lugeCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosComunasEnter(string strFiltro, string lugeCodigo)
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


                strQuery = " WHERE upper(DESCRIPCION) = upper('" + strFiltro + "')  ";
                strOrdenamiento = " ORDER BY CODIGO";
                strLimites = " WHERE DATOS.FILA BETWEEN " + 0 + " AND " + 1;
                if (lugeCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", lugeCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttComunas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaComunasPorCiudadMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttComunas.Rows)
                    {
                        strResultado = fila["DESCRIPCION"].ToString();
                        strLugueCodigo = fila["luge_codigo"].ToString();
                        strCodigo = fila["CODIGO"].ToString();
                    }

                }

                resultados = Json(new { result = true, datosComuna = strResultado, lugueCodigo = strLugueCodigo, codigo = strCodigo });
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
        /// 
        /// </summary>
        /// <param name="strFiltro"></param>
        /// <param name="cozoLugeCodigo"></param>
        /// <param name="cozoCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DatosBarriosEnter(string strFiltro, int? cozoLugeCodigo, int? cozoCodigo)
        {
            try
            {
                GestorAdministrador gestor;
                JsonResult resultados = new JsonResult();
                DataTable dttBarrios;
                string strResultado = string.Empty;
                string strLugueCodigo = string.Empty;
                string strCodigo = string.Empty;

                string strQuery = string.Empty;
                string strOrdenamiento = string.Empty;
                string strLimites = "";
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();


                strQuery = " WHERE upper(DESCRIPCION) = upper('" + strFiltro + "')  ";
                strOrdenamiento = " ORDER BY CODIGO";
                strLimites = " WHERE DATOS.FILA BETWEEN " + 0 + " AND " + 1;


                if (cozoLugeCodigo != null && cozoCodigo != null)
                {
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cozo_luge_codigo", cozoLugeCodigo.ToString(), typeof(decimal)));
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cozo_codigo", cozoCodigo.ToString(), typeof(decimal)));

                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosComunasMj", strConexion, "Nonquery");

                    foreach (DataRow fila in dttBarrios.Rows)
                    {
                        strResultado = fila["DESCRIPCION"].ToString();
                        strLugueCodigo = fila["luge_codigo"].ToString();
                        strCodigo = fila["CODIGO"].ToString();
                    }
                }

                resultados = Json(new { result = true, datosBarrio = strResultado, lugueCodigo = strLugueCodigo, codigo = strCodigo });
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