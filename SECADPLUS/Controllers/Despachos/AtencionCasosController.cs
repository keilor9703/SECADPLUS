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
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class AtencionCasosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities dbSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private EntitiesCasos db = new EntitiesCasos(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ModelCasos", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: AtencionCasos
        [SecadPlusAutorizador("/AtencionCasos/Index")]
        public ActionResult Index(int idFuerza, int idCanal, string strMensaje, string strTipo)
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

                ViewBag.idFuerza = idFuerza;
                ViewBag.idCanal = idCanal;
                ViewBag.CANAL = dbSecad.CADCANALES.Find(idFuerza, idCanal).DESCRIPCION;

                GestorAdministrador gestor = new GestorAdministrador();
                var mensajesTextoUsuarios = gestor.ConsultarCasosCanal(idFuerza, idCanal, strConexion);

                List<ExtCasosCanal> cADCASOS_CANAL = new List<ExtCasosCanal>();

                foreach (DataRow fila in mensajesTextoUsuarios.Rows)
                {
                    cADCASOS_CANAL.Add(new ExtCasosCanal(fila));
                }

                List<CADCASO> listaLugares = new List<CADCASO>();
                listaLugares.Insert(0, new CADCASO { CODIGO = "0", DESCRIPCION = " " });

                return View(cADCASOS_CANAL.OrderBy(c => c.CADCASO_CODIGO).ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetCasos(string opcion)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorCaso('CODIGO','DESCRIPCION','" + opcion + "')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_AtencionCasosModel> listaCasos = new List<_AtencionCasosModel>();
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
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_casos", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaCasos.Add(new _AtencionCasosModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCasos });
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
        public JsonResult ProcesarCasosCanal(int idFuerza, int idCanal, string casoDesde, string casoHasta)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var a = gestor.EjecutarCasosCanal(idFuerza, idCanal, casoDesde, casoHasta, strConexion);

                return Json(new { result = true, datos = "Proceso automatico exitoso: Caso desde: " + casoDesde + " - Caso hasta: " + casoHasta });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: AtencionCasos/Create
        [Authorize]
        public ActionResult Create(int idFuerza, int idCanal)
        {
            try
            {
                ViewBag.CADCANA_FUERZ_ID = idFuerza;
                ViewBag.CADCANA_CODIGO = idCanal;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: AtencionCasos/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CADCASO_CODIGO,CADCANA_FUERZ_ID,CADCANA_CODIGO")] CADCASOS_CANAL cADCASOS_CANAL)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cADCASOS_CANAL.CADCASO_CODIGO))
                    ModelState.AddModelError("CADCASO_CODIGO", "El campo es obligatorio.");

                var existe = db.CADCASOS_CANAL.Find(cADCASOS_CANAL.CADCASO_CODIGO, cADCASOS_CANAL.CADCANA_FUERZ_ID, cADCASOS_CANAL.CADCANA_CODIGO);
                if (existe != null)
                    ModelState.AddModelError("CADCASO_CODIGO", "Este Caso esta siendo utilizado.");

                if (ModelState.IsValid)
                {
                    db.CADCASOS_CANAL.Add(cADCASOS_CANAL);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCASOS_CANAL>(cADCASOS_CANAL, "CADCASOS_CANAL", cADCASOS_CANAL.CADCANA_CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADCASOS_CANAL.CADCASO_CODIGO);
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idFuerza = cADCASOS_CANAL.CADCANA_FUERZ_ID, idCanal = cADCASOS_CANAL.CADCANA_CODIGO, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.CADCANA_FUERZ_ID = cADCASOS_CANAL.CADCANA_FUERZ_ID;
                ViewBag.CADCANA_CODIGO = cADCASOS_CANAL.CADCANA_CODIGO;

                return View(cADCASOS_CANAL);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idFuerza = cADCASOS_CANAL.CADCANA_FUERZ_ID, idCanal = cADCASOS_CANAL.CADCANA_CODIGO, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: AtencionCasos/Edit/5
        [Authorize]
        public ActionResult Edit(string id, int idFuerza, int idCanal)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCASOS_CANAL cADCASOS_CANAL = db.CADCASOS_CANAL.Find(id, idFuerza, idCanal);
                if (cADCASOS_CANAL == null)
                {
                    return HttpNotFound();
                }

                ViewBag.antiguoCodigo = cADCASOS_CANAL.CADCASO_CODIGO;
                ViewBag.DESCRIPCION = dbSecad.CADCASOS.Where(c => c.CODIGO == cADCASOS_CANAL.CADCASO_CODIGO).Select(c => c.DESCRIPCION).FirstOrDefault();

                return View(cADCASOS_CANAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: AtencionCasos/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CADCASO_CODIGO,CADCANA_FUERZ_ID,CADCANA_CODIGO")] CADCASOS_CANAL cADCASOS_CANAL)
        {
            try
            {
                var antiguoCodigo = Request["antiguoCodigo"];
                var DESCRIPCION = Request["DESCRIPCION"];

                if (string.IsNullOrWhiteSpace(cADCASOS_CANAL.CADCASO_CODIGO))
                    ModelState.AddModelError("CADCASO_CODIGO", "El campo es obligatorio.");

                if (antiguoCodigo != cADCASOS_CANAL.CADCASO_CODIGO)
                {
                    var existe = db.CADCASOS_CANAL.Find(cADCASOS_CANAL.CADCASO_CODIGO, cADCASOS_CANAL.CADCANA_FUERZ_ID, cADCASOS_CANAL.CADCANA_CODIGO);
                    if (existe != null)
                        ModelState.AddModelError("CADCASO_CODIGO", "Este Caso esta siendo utilizado.");
                }

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADCASOS_CANAL.CADCASO_CODIGO)
                    {
                        db.Entry(cADCASOS_CANAL).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCASOS_CANAL>(cADCASOS_CANAL, "CADCASOS_CANAL", cADCASOS_CANAL.CADCANA_CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADCASOS_CANAL cADCASOS_CANAL1 = db.CADCASOS_CANAL.Find(antiguoCodigo, cADCASOS_CANAL.CADCANA_FUERZ_ID, cADCASOS_CANAL.CADCANA_CODIGO);
                        db.CADCASOS_CANAL.Remove(cADCASOS_CANAL1);
                        db.SaveChanges();

                        db.CADCASOS_CANAL.Add(cADCASOS_CANAL);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCASOS_CANAL>(cADCASOS_CANAL, "CADCASOS_CANAL", cADCASOS_CANAL.CADCANA_CODIGO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCASOS_CANAL.CADCASO_CODIGO);
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idFuerza = cADCASOS_CANAL.CADCANA_FUERZ_ID, idCanal = cADCASOS_CANAL.CADCANA_CODIGO, strMensaje = mensaje, strTipo = tipo });
                }

                ViewBag.antiguoCodigo = antiguoCodigo;
                ViewBag.DESCRIPCION = DESCRIPCION;

                return View(cADCASOS_CANAL);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idFuerza = cADCASOS_CANAL.CADCANA_FUERZ_ID, idCanal = cADCASOS_CANAL.CADCANA_CODIGO, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Eliminar(string id, int idFuerza, int idCanal)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCASOS_CANAL cADCASOS_CANAL = db.CADCASOS_CANAL.Find(id, idFuerza, idCanal);
                if (cADCASOS_CANAL == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADCASOS_CANAL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: AtencionCasos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, int idFuerza, int idCanal)
        {
            try
            {
                CADCASOS_CANAL cADCASOS_CANAL = db.CADCASOS_CANAL.Find(id, idFuerza, idCanal);
                db.CADCASOS_CANAL.Remove(cADCASOS_CANAL);
                db.SaveChanges();


                CADCASOS_CANAL cADCASOS_CANAL_copy = new CADCASOS_CANAL();
                cADCASOS_CANAL_copy = (CADCASOS_CANAL)auditoria.GenerarCopiaAuditoria(cADCASOS_CANAL);


                auditoria.RegistrarAuditoria<CADCASOS_CANAL>(cADCASOS_CANAL_copy, "CADCASOS_CANAL", cADCASOS_CANAL.CADCANA_CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idFuerza = cADCASOS_CANAL.CADCANA_FUERZ_ID, idCanal = cADCASOS_CANAL.CADCANA_CODIGO, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idFuerza = idFuerza, idCanal = idCanal, strMensaje = mensaje, strTipo = tipo });
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