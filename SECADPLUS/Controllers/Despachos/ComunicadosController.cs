using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ComunicadosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private DesRepMinEntities db = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: Comunicados
        [SecadPlusAutorizador("/Comunicados/Index")]
        public ActionResult Index(string strMensaje, string strTipo)
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
                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.globalSitio = usr.SITIO_GRABACION;
                ViewBag.globalFuerza = usr.CADCANA_FUERZA_ID;

                var mensaje = "";
                var tipo = "";

                if (string.IsNullOrEmpty(strMensaje))
                {
                    mensaje = string.Empty;
                    tipo = string.Empty;
                }
                else
                {
                    mensaje = strMensaje;
                    tipo = strTipo;
                }

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

        [Authorize]
        public JsonResult VerComunicados(int idSitio, int idFuerza)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = " WHERE SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " ";
                string strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<ul class='icons-list'>" +
                                        "<li class='dropdown'>" +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'>" +
                                                "<i class='icon-menu9'></i>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu dropdown-menu-left'>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar'" +
                                                            "onclick=\"window.location.href = '../Comunicados/Edit?idSitio=SITIO_GRABA&idFuerza=CADFUERZ_ID&cod=CONSECUTIVO'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar" +
                                                    "</button>" +
                                                "</li>" +
                                                "<li>" +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar'" +
                                                            "onclick=\"EliminarRegistroGeneral('../Comunicados/Eliminar?idSitio=SITIO_GRABA&idFuerza=CADFUERZ_ID&cod=CONSECUTIVO')\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                                "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";
                string strBotonDestino = "";
                List<Comunicados> listaComunicados = new List<Comunicados>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " AND (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (USUARIO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (NUMERO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (FECHA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (AÑO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (ORIGEN, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (DESTINO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (TEXTO, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (FIRMA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (CARGO_FIRMA, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (TRANSMITE, '" + parametros.ConsultaValor + "', 'i')) " +
                               " OR (SITIO_GRABA = " + idSitio + " AND CADFUERZ_ID = " + idFuerza + " AND REGEXP_LIKE (RECIBE, '" + parametros.ConsultaValor + "', 'i')) ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY USUARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY NUMERO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY FECHA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY AÑO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY ORIGEN " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY DESTINO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 10:
                        strOrderna += " ORDER BY TEXTO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 11:
                        strOrderna += " ORDER BY FIRMA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 12:
                        strOrderna += " ORDER BY CARGO_FIRMA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 13:
                        strOrderna += " ORDER BY TRANSMITE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 14:
                        strOrderna += " ORDER BY RECIBE " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_comunicados", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("SITIO_GRABA", fila["SITIO_GRABA"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CADFUERZ_ID", fila["CADFUERZ_ID"].ToString());
                    strBotonDestino = strBotonDestino.Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());

                    listaComunicados.Add(new Comunicados(strBotonDestino, fila));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaComunicados });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: Comunicados/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.SITIO_GRABA = usr.SITIO_GRABACION;
                ViewBag.CADFUERZ_ID = usr.CADCANA_FUERZA_ID;
                ViewBag.USUARIO = usr.USUARIO;

                var idSitio = usr.SITIO_GRABACION;
                var idFuerza = usr.CADCANA_FUERZA_ID;
                var usuario = usr.USUARIO;

                var existe = db.CADPOLIGRAMAS.Where(c => c.SITIO_GRABA == idSitio && c.CADFUERZ_ID == idFuerza).OrderByDescending(c => c.CONSECUTIVO).Select(c => c.CONSECUTIVO).FirstOrDefault();

                ViewBag.CONSECUTIVO = existe + 1;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Comunicados/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADFUERZ_ID,NUMERO,ORIGEN,AÑO,DESTINO,FECHA,TEXTO,FIRMA,CARGO_FIRMA,TRANSMITE,RECIBE,USUARIO")] CADPOLIGRAMA cADPOLIGRAMA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existe = db.CADPOLIGRAMAS.Where(c => c.SITIO_GRABA == cADPOLIGRAMA.SITIO_GRABA && c.CADFUERZ_ID == cADPOLIGRAMA.CADFUERZ_ID).OrderByDescending(c => c.CONSECUTIVO).Select(c => c.CONSECUTIVO).FirstOrDefault();

                    cADPOLIGRAMA.CONSECUTIVO = existe + 1;

                    db.CADPOLIGRAMAS.Add(cADPOLIGRAMA);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADPOLIGRAMA>(cADPOLIGRAMA, "CADPOLIGRAMAS", cADPOLIGRAMA.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADPOLIGRAMA.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                return View(cADPOLIGRAMA);
            }
            catch (DbUpdateException ex)
            {
                var mensaje = "No se puede crear." + ex;
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Comunicados/Edit/5
        [Authorize]
        public ActionResult Edit(int idSitio, int idFuerza, int? cod)
        {
            try
            {
                if (cod == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPOLIGRAMA cADPOLIGRAMA = db.CADPOLIGRAMAS.Find(idSitio, cod, idFuerza);
                if (cADPOLIGRAMA == null)
                {
                    return HttpNotFound();
                }
                return View(cADPOLIGRAMA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Comunicados/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADFUERZ_ID,NUMERO,ORIGEN,AÑO,DESTINO,FECHA,TEXTO,FIRMA,CARGO_FIRMA,TRANSMITE,RECIBE,USUARIO")] CADPOLIGRAMA cADPOLIGRAMA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADPOLIGRAMA).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADPOLIGRAMA>(cADPOLIGRAMA, "CADPOLIGRAMAS", cADPOLIGRAMA.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADPOLIGRAMA.CONSECUTIVO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }
                return View(cADPOLIGRAMA);
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

        // GET: Comunicados/Delete/5
        [Authorize]
        public ActionResult Eliminar(int idSitio, int idFuerza, int? cod)
        {
            try
            {
                if (cod == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADPOLIGRAMA cADPOLIGRAMA = db.CADPOLIGRAMAS.Find(idSitio, cod, idFuerza);
                if (cADPOLIGRAMA == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADPOLIGRAMA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Comunicados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idSitio, int idFuerza, int? cod)
        {
            try
            {
                CADPOLIGRAMA cADPOLIGRAMA = db.CADPOLIGRAMAS.Find(idSitio, cod, idFuerza);
                db.CADPOLIGRAMAS.Remove(cADPOLIGRAMA);
                db.SaveChanges();

                CADPOLIGRAMA cADPOLIGRAMA_copy = new CADPOLIGRAMA();
                cADPOLIGRAMA_copy = (CADPOLIGRAMA)auditoria.GenerarCopiaAuditoria(cADPOLIGRAMA);

                auditoria.RegistrarAuditoria<CADPOLIGRAMA>(cADPOLIGRAMA_copy, "CADPOLIGRAMAS", cADPOLIGRAMA.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO.Replace("NUMERO", cADPOLIGRAMA.CONSECUTIVO.ToString());
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
    }
}