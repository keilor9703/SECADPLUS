using SECADEntities.SECAD;
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
    public class MaestraConductasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: MaestraConductas
        [SecadPlusAutorizador("/MaestraConductas/Index")]
        public ActionResult Index(string idTipo, string nomTipo, int? idTitu, string nomTitu, string strMensaje, string strTipo)
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

                if (string.IsNullOrEmpty(idTipo))
                {
                    idTipo = "SP";
                    nomTipo = "SERVICIOS DE POLICIA";
                }

                if (idTitu == null)
                {
                    idTitu = 0;
                    nomTitu = "";
                }

                //var idTitulo = int.Parse(idTitu);

                var cADCONDUCTA_T = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(c => c.CODIGO);
                var cADCONDUCTA_C = db.CADCONDUCTAS.Where(c => c.COND_CODIGO == idTitu && c.NIVEL == 2).OrderBy(c => c.CODIGO);

                var variosModelos = new Tuple<List<CADCONDUCTA>, List<CADCONDUCTA>>
                    (cADCONDUCTA_T.ToList(), cADCONDUCTA_C.ToList())
                { };

                ViewBag.TIPO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_CONDUCTA").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", idTipo);
                ViewBag.NOM_CONDUCTA = nomTitu;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.nomTipo = nomTipo;

                //return View(cADCONDUCTA.ToList());
                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetTitulos(string idTipo, string nomTipo)
        {
            try
            {
                ViewBag.nomTipo = nomTipo;
                var cADCONDUCTA = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(c => c.CODIGO);
                return PartialView("_titulos", cADCONDUCTA.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetConductas(int idTitu, string nomTitu)
        {
            try
            {
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                var cADCONDUCTA = db.CADCONDUCTAS.Where(c => c.COND_CODIGO == idTitu && c.NIVEL == 2).OrderBy(c => c.CODIGO);
                return PartialView("_conductas", cADCONDUCTA.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MaestraConductas/Create

        public ActionResult Create(string idTipo, string nomTipo)
        {
            try
            {
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CODIGO = idTipo;
                ViewBag.TIPO_NOMBRE = nomTipo;

                ViewBag.TIPO = idTipo;
                ViewBag.NIVEL = 1;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string idTipo, string nomTipo, [Bind(Include = "CODIGO,DESCRIPCION,ABREVIATURA,TIPO,COND_CODIGO,NIVEL,VIGENTE,LICO_CODIGO,LICO_VIGENCIA,PERSONAS,BIENES,COMPARENDOS")] CADCONDUCTA cADCONDUCTA)
        {
            try
            {
                var existe = db.CADCONDUCTAS.Find(cADCONDUCTA.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                if (cADCONDUCTA.VIGENTE == "true")
                    cADCONDUCTA.VIGENTE = "S";
                else
                    cADCONDUCTA.VIGENTE = "N";

                if (ModelState.IsValid)
                {
                    db.CADCONDUCTAS.Add(cADCONDUCTA);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADCONDUCTA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo });
                }
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CODIGO = idTipo;
                ViewBag.TIPO_NOMBRE = nomTipo;

                ViewBag.TIPO = idTipo;
                ViewBag.NIVEL = 1;
                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE;

                return View(cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MaestraConductas/CreateC
        public ActionResult CreateC(string idTipo, string nomTipo, int idTitu, string nomTitu)
        {
            try
            {
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.COND_CODIGO = idTitu;
                ViewBag.TITULO_NOMBRE = nomTitu;

                ViewBag.TIPO = idTipo;
                ViewBag.NIVEL = 2;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/Createc To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateC(string idTipo, string nomTipo, int idTitu, string nomTitu, [Bind(Include = "CODIGO,DESCRIPCION,ABREVIATURA,TIPO,COND_CODIGO,NIVEL,VIGENTE,LICO_CODIGO,LICO_VIGENCIA,PERSONAS,BIENES,COMPARENDOS")] CADCONDUCTA cADCONDUCTA)
        {
            try
            {
                var existe = db.CADCONDUCTAS.Find(cADCONDUCTA.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                if (cADCONDUCTA.VIGENTE == "true")
                    cADCONDUCTA.VIGENTE = "S";
                else
                    cADCONDUCTA.VIGENTE = "N";

                if (cADCONDUCTA.PERSONAS == "true")
                    cADCONDUCTA.PERSONAS = "S";
                else
                    cADCONDUCTA.PERSONAS = "N";

                if (cADCONDUCTA.BIENES == "true")
                    cADCONDUCTA.BIENES = "S";
                else
                    cADCONDUCTA.BIENES = "N";

                if (cADCONDUCTA.COMPARENDOS == "true")
                    cADCONDUCTA.COMPARENDOS = "S";
                else
                    cADCONDUCTA.COMPARENDOS = "N";

                if (ModelState.IsValid)
                {
                    db.CADCONDUCTAS.Add(cADCONDUCTA);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADCONDUCTA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu });
                }
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.COND_CODIGO = idTitu;
                ViewBag.TITULO_NOMBRE = nomTitu;

                ViewBag.TIPO = idTipo;
                ViewBag.NIVEL = 2;

                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE;
                ViewBag.PERSONAS = cADCONDUCTA.PERSONAS;
                ViewBag.BIENES = cADCONDUCTA.BIENES;
                ViewBag.COMPARENDOS = cADCONDUCTA.COMPARENDOS;

                return View(cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MaestraConductas/Edit/5
        public ActionResult Edit(int? id, string idTipo, string nomTipo)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                if (cADCONDUCTA == null)
                {
                    return HttpNotFound();
                }

                ViewBag.antiguoCodigo = cADCONDUCTA.CODIGO;
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CODIGO = idTipo;
                ViewBag.TIPO_NOMBRE = nomTipo;

                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE == "S" ? true : false;
                return View(cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string idTipo, string nomTipo, [Bind(Include = "CODIGO,DESCRIPCION,ABREVIATURA,TIPO,COND_CODIGO,NIVEL,VIGENTE,LICO_CODIGO,LICO_VIGENCIA,PERSONAS,BIENES,COMPARENDOS")] CADCONDUCTA cADCONDUCTA)
        {
            try
            {
                var antiguoCodigo = int.Parse(Request["antiguoCodigo"]);

                if (antiguoCodigo != cADCONDUCTA.CODIGO)
                {
                    var existe = db.CADCONDUCTAS.Find(cADCONDUCTA.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                    var existeHijos = db.CADCONDUCTAS.Any(c => c.COND_CODIGO == antiguoCodigo);

                    if (existeHijos)
                        ModelState.AddModelError("CODIGO", "No se puede editar, restricción de integridad ya que se encuentran registros asociados.");
                }

                if (cADCONDUCTA.VIGENTE == "true")
                    cADCONDUCTA.VIGENTE = "S";
                else
                    cADCONDUCTA.VIGENTE = "N";

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADCONDUCTA.CODIGO)
                    {
                        db.Entry(cADCONDUCTA).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADCONDUCTA cADCONDUCTA1 = db.CADCONDUCTAS.Find(cADCONDUCTA.CODIGO);
                        db.CADCONDUCTAS.Remove(cADCONDUCTA1);
                        db.SaveChanges();

                        //cADCONDUCTA.CODIGO = nuevoCodigo;

                        db.CADCONDUCTAS.Add(cADCONDUCTA);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADCONDUCTA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo });
                }

                ViewBag.antiguoCodigo = antiguoCodigo;
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CODIGO = idTipo;
                ViewBag.TIPO_NOMBRE = nomTipo;

                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE;

                return View(cADCONDUCTA);
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

        // GET: MaestraConductas/Edit/5
        public ActionResult EditC(int? id, string idTipo, string nomTipo, int idTitu, string nomTitu)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                if (cADCONDUCTA == null)
                {
                    return HttpNotFound();
                }

                ViewBag.antiguoCodigo = id;

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.TITULO_CODIGO = idTitu;
                ViewBag.TITULO_NOMBRE = nomTitu;

                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE == "S" ? true : false;
                ViewBag.PERSONAS = cADCONDUCTA.PERSONAS == "S" ? true : false;
                ViewBag.BIENES = cADCONDUCTA.BIENES == "S" ? true : false;
                ViewBag.COMPARENDOS = cADCONDUCTA.COMPARENDOS == "S" ? true : false;

                return View(cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditC(string idTipo, string nomTipo, int idTitu, string nomTitu, [Bind(Include = "CODIGO,DESCRIPCION,ABREVIATURA,TIPO,COND_CODIGO,NIVEL,VIGENTE,LICO_CODIGO,LICO_VIGENCIA,PERSONAS,BIENES,COMPARENDOS")] CADCONDUCTA cADCONDUCTA)
        {
            try
            {
                var antiguoCodigo = int.Parse(Request["antiguoCodigo"]);

                if (antiguoCodigo != cADCONDUCTA.CODIGO)
                {
                    var existe = db.CADCONDUCTAS.Find(cADCONDUCTA.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");
                }

                if (cADCONDUCTA.VIGENTE == "true")
                    cADCONDUCTA.VIGENTE = "S";
                else
                    cADCONDUCTA.VIGENTE = "N";

                if (cADCONDUCTA.PERSONAS == "true")
                    cADCONDUCTA.PERSONAS = "S";
                else
                    cADCONDUCTA.PERSONAS = "N";

                if (cADCONDUCTA.BIENES == "true")
                    cADCONDUCTA.BIENES = "S";
                else
                    cADCONDUCTA.BIENES = "N";

                if (cADCONDUCTA.COMPARENDOS == "true")
                    cADCONDUCTA.COMPARENDOS = "S";
                else
                    cADCONDUCTA.COMPARENDOS = "N";

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADCONDUCTA.CODIGO)
                    {
                        db.Entry(cADCONDUCTA).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADCONDUCTA cADCONDUCTA1 = db.CADCONDUCTAS.Find(antiguoCodigo);
                        db.CADCONDUCTAS.Remove(cADCONDUCTA1);
                        db.SaveChanges();

                        //cADCONDUCTA.CODIGO = nuevoCodigo;

                        db.CADCONDUCTAS.Add(cADCONDUCTA);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    var mensaje = "Ha sido editado el registro: " + cADCONDUCTA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu });
                }

                ViewBag.antiguoCodigo = antiguoCodigo;
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.TITULO_CODIGO = idTitu;
                ViewBag.TITULO_NOMBRE = nomTitu;

                ViewBag.VIGENTE = cADCONDUCTA.VIGENTE;
                ViewBag.PERSONAS = cADCONDUCTA.PERSONAS;
                ViewBag.BIENES = cADCONDUCTA.BIENES;
                ViewBag.COMPARENDOS = cADCONDUCTA.COMPARENDOS;

                return View(cADCONDUCTA);
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

        public ActionResult Eliminar(int? id, string idTipo, string nomTipo)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                if (cADCONDUCTA == null)
                {
                    return HttpNotFound();
                }

                ViewBag.nuevoCodigo = cADCONDUCTA.CODIGO;
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CODIGO = idTipo;
                ViewBag.TIPO_NOMBRE = nomTipo;

                return View("Delete", null, cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string idTipo, string nomTipo)
        {
            try
            {
                var mensaje = "";
                var tipo = "";

                var existeHijos = db.CADCONDUCTAS.Any(c => c.COND_CODIGO == id);

                if (existeHijos)
                {
                    mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                    tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, strMensaje = mensaje, strTipo = tipo });
                }

                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                db.CADCONDUCTAS.Remove(cADCONDUCTA);
                db.SaveChanges();



                CADCONDUCTA cADCONDUCTA_copy = new CADCONDUCTA();
                cADCONDUCTA_copy = (CADCONDUCTA)auditoria.GenerarCopiaAuditoria(cADCONDUCTA);

                auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA_copy, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Eliminar);

                //return RedirectToAction("Index");
                mensaje = "Ha sido eliminado el registro: " + cADCONDUCTA.DESCRIPCION.ToString();
                tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo });
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

        public ActionResult EliminarC(int? id, string idTipo, string nomTipo, int idTitu, string nomTitu)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                if (cADCONDUCTA == null)
                {
                    return HttpNotFound();
                }

                ViewBag.nuevoCodigo = cADCONDUCTA.CODIGO;
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.TITULO_CODIGO = idTitu;
                ViewBag.TITULO_NOMBRE = nomTitu;

                ViewBag.TIPO = idTipo;

                return View("DeleteC", null, cADCONDUCTA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MaestraConductas/DeleteC/5
        [HttpPost, ActionName("DeleteC")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedC(int id, string idTipo, string nomTipo, int idTitu, string nomTitu)
        {
            try
            {
                CADCONDUCTA cADCONDUCTA = db.CADCONDUCTAS.Find(id);
                db.CADCONDUCTAS.Remove(cADCONDUCTA);
                db.SaveChanges();

                //auditoria.RegistrarAuditoria<CADCONDUCTA>(cADCONDUCTA, "CADCONDUCTAS", cADCONDUCTA.CODIGO.ToString(), TipoAuditoria.Eliminar);

                //return RedirectToAction("Index");
                var mensaje = "Ha sido eliminado el caso: " + cADCONDUCTA.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu });
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