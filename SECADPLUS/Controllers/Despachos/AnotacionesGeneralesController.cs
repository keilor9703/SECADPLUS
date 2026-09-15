using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class AnotacionesGeneralesController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorAuditoria auditoria = new GestorAuditoria();

        // GET: AdministrarNotificacion
        [SecadPlusAutorizador("/AnotacionesGenerales/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
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

        [Authorize]
        public JsonResult ObtenerAnotacionesGnr()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;

                #region Codigo para funcionalidad extra

                //string strBoton = "<ul class='icons-list'> " +
                //                        "<li class='dropdown'> " +
                //                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                //                                "<i class='icon-menu9'></i> " +
                //                            "</button> " +
                //                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                //                                "<li> " +
                //                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                //                                            "title='' data-placement='bottom' data-original-title='Clic para editar' " +
                //                                            "onclick=\"window.location.href = '/AnotacionesGenerales/Edit?sitioGraba=SITIO_GRABA&consecutivoAnotacion=CONSECUTIVO'\"> " +
                //                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                //                                    "</button> " +
                //                                "</li> " +
                //                                "<li> " +
                //                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                //                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                //                                            "onclick=\"EliminarRegistroGeneral('/AnotacionesGenerales/Eliminar?sitioGraba=SITIO_GRABA&consecutivoAnotacion=CONSECUTIVO')\"> " +
                //                                        "<i class='icon-trash'></i> Eliminar" +
                //                                    "</button>" +
                //                                "</li>" +
                //                            "</ul>" +
                //                        "</li>" +
                //                    "</ul>";

                //string strBotonDestino = "";

                #endregion Codigo para funcionalidad extra

                List<_AnotacionesGnrModel> listaAnotacionesGnr = new List<_AnotacionesGnrModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (USUARIO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (FECHA_GRABACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ANOTACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (MAQUINA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY USUARIO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY SITIO_GRABA " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA_GRABACION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 5:
                        strOrderna += " ORDER BY ANOTACION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 6:
                        strOrderna += " ORDER BY MAQUINA " + parametros.TipoOrdenamiento + " ";

                        break;
                    default:
                        strOrderna += " ORDER BY REGISTRO.FECHA_GRABACION DESC ";
                        break;
                }
                strLimite = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaAnotacionesGenerales", strConexion, "Nonquery");
                string strBoton = "<ul class='icons-list'> " +
                                         "<li class='dropdown'> " +
                                             "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                 "<i class='icon-menu9'></i> " +
                                             "</button> " +
                                             "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                 "<li> " +
                                                     "<a class='btn btn-xs btn-default' href='_Reporte' target='_blank' data-popup='tooltip' " +
                                                             "title='' data-placement='bottom' data-original-title='Clic para Imprimir' " +
                                                              "> " +
                                                         "<i class='glyphicon glyphicon-edit'></i> Imprimir Anotación " +
                                                     "</a> " +
                                                 "</li> " +
                                             "</ul>" +
                                         "</li>" +
                                     "</ul>";

                foreach (DataRow fila in dttLugar.Rows)
                {
                    string strBotonImprime = strBoton.Replace("_Reporte", Url.Action("ReporteAnotacionesGenerales", "Reportes", new { intSitio = fila["SITIO_GRABA"].ToString(), intNumeroLlamada = fila["CONSECUTIVO"].ToString(), dFechaAnotacion = fila["FECHA_GRABACION"].ToString() }));

                    listaAnotacionesGnr.Add(new _AnotacionesGnrModel("", strBotonImprime, fila["USUARIO"].ToString(), fila["CONSECUTIVO"].ToString(), fila["SITIO_GRABA"].ToString(), fila["FECHA_GRABACION"].ToString(), fila["ANOTACION"].ToString(), fila["MAQUINA"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaAnotacionesGnr });
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
        public ActionResult Create()
        {
            if (Session["USUARIO_SECAD"] == null)
            {
                var mensaje = "La sesión a caducado por favor vuelva a iniciar sesión.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }

            CADANOTAGRAL cADANOTAGRAL = new CADANOTAGRAL();
            CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

            cADANOTAGRAL.SITIO_GRABA = usr.SITIO_GRABACION;
            cADANOTAGRAL.MAQUINA = GestorBase.ObtenerDireccionIp() + "-" + ObtenerMaquina();
            cADANOTAGRAL.USUARIO = usr.USUARIO;
            cADANOTAGRAL.CONSECUTIVO = 1;

            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            string strAccion = ControllerContext.RouteData.Values["action"].ToString();
            string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
            ViewBag.NombreUbicacion = "AnotacionesGenerales-Crear";
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////



            return View(cADANOTAGRAL);
        }

        // POST: CADANOTAGRALs/Create Para protegerse de ataques de publicación excesiva, habilite
        // las propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CONDICION,CADCANA_FUERZA_ID,CADCANA_CODIGO,USUARIO,FECHA_GRABACION,ANOTACION,MAQUINA,TITULO")] CADANOTAGRAL cADANOTAGRAL)
        {
            try
            {
                if (ModelState.IsValid && Session["USUARIO_SECAD"] != null)
                {
                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    GestorGeneral gestorGeneral = new GestorGeneral();

                    cADANOTAGRAL.SITIO_GRABA = usr.SITIO_GRABACION;
                    cADANOTAGRAL.USUARIO = usr.USUARIO;
                    cADANOTAGRAL.FECHA_GRABACION = gestorGeneral.ObtenerTiempoServidor;
                    cADANOTAGRAL.CONSECUTIVO = GenerarConsecutivo();
                    cADANOTAGRAL.MAQUINA = GestorBase.ObtenerDireccionIp() + "-" + ObtenerMaquina();

                    db.CADANOTAGRALs.Add(cADANOTAGRAL);
                    await db.SaveChangesAsync();

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADANOTAGRAL.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }
                else if (Session["USUARIO_SECAD"] == null)
                {
                    var mensaje = "La sesión a caducado por favor vuelva a iniciar sesión.";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                return View(cADANOTAGRAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADANOTAGRALs/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? sitioGraba, int? consecutivoAnotacion)
        {
            try
            {
                CADANOTAGRAL cADANOTAGRAL = await db.CADANOTAGRALs.FindAsync(sitioGraba, consecutivoAnotacion);
                if (cADANOTAGRAL == null)
                {
                    return HttpNotFound();
                }
                return View(cADANOTAGRAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADANOTAGRALs/Edit/5 Para protegerse de ataques de publicación excesiva, habilite
        // las propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CONDICION,CADCANA_FUERZA_ID,CADCANA_CODIGO,USUARIO,FECHA_GRABACION,ANOTACION,MAQUINA,TITULO")] CADANOTAGRAL cADANOTAGRAL)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADANOTAGRAL).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    auditoria.RegistrarAuditoria<CADANOTAGRAL>(cADANOTAGRAL, "CADANOTAGRAL", "SITIO_GRABA=" + cADANOTAGRAL.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADANOTAGRAL.CONSECUTIVO, TipoAuditoria.Modificar);
                }

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADANOTAGRAL.SITIO_GRABA.ToString() + "-" + cADANOTAGRAL.CONSECUTIVO.ToString());
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                // return View(cADANOTAGRAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADANOTAGRALs/Delete/5
        [Authorize]
        public async Task<ActionResult> Eliminar(int? sitioGraba, int? consecutivoAnotacion)
        {
            try
            {
                if (sitioGraba == null && consecutivoAnotacion == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADANOTAGRAL cADANOTAGRAL = await db.CADANOTAGRALs.FindAsync(sitioGraba, consecutivoAnotacion);
                if (cADANOTAGRAL == null)
                {
                    return HttpNotFound();
                }

                return View("Delete", null, cADANOTAGRAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADANOTAGRALs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? sitioGraba, int? consecutivoAnotacion)
        {
            try
            {
                CADANOTAGRAL cADANOTAGRAL = await db.CADANOTAGRALs.FindAsync(sitioGraba, consecutivoAnotacion);

                db.CADANOTAGRALs.Remove(cADANOTAGRAL);
                await db.SaveChangesAsync();

                CADANOTAGRAL cADANOTAGRAL_copy = new CADANOTAGRAL();
                cADANOTAGRAL_copy = (CADANOTAGRAL)auditoria.GenerarCopiaAuditoria(cADANOTAGRAL);

                auditoria.RegistrarAuditoria<CADANOTAGRAL>(cADANOTAGRAL_copy, "CADANOTAGRAL", "SITIO_GRABA=" + cADANOTAGRAL.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADANOTAGRAL.CONSECUTIVO, TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Metodo Genera un consecutivo.
        /// </summary>
        /// <returns></returns>
        public int GenerarConsecutivo()
        {
            try
            {
                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                return (int)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_ConsecutivoAnotacionesGrn", strConexion);
            }
            catch (Exception ex)
            {
                throw ex;
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