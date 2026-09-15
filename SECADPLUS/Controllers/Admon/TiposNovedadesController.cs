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
    public class TiposNovedadesController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: TiposNovedades
        [SecadPlusAutorizador("/TiposNovedades/Index")]
        public ActionResult Index(string id, string nom, string strMensaje, string strTipo)
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

                if (string.IsNullOrEmpty(id))
                {
                    id = db.CADTIPOSNOVEDADES.Where(c => c.NIVEL == 1).OrderBy(c => c.CODIGO).Select(c => c.CODIGO).First().ToString();
                    nom = db.CADTIPOSNOVEDADES.Where(c => c.NIVEL == 1).OrderBy(c => c.CODIGO).Select(c => c.DESCRIPCION).First().ToString();
                }

                //var cADINFRACCIONES_TRANSITO_C = db.CADINFRACCIONES_TRANSITO.Include(c => c.CADINFRACCIONES_TRANSITO2).Where(c => c.CADINFRACC_CODIGO == null).OrderBy(c => c.DESCRIPCION);
                //var cADINFRACCIONES_TRANSITO_T = db.CADINFRACCIONES_TRANSITO.Include(c => c.CADINFRACCIONES_TRANSITO2).Where(c => c.CADINFRACC_CODIGO == id).OrderBy(c => c.DESCRIPCION);

                var cADTIPOSNOVEDADES_T = db.CADTIPOSNOVEDADES.Include(c => c.CADTIPOSNOVEDADE1).Where(c => c.NIVEL == 1).OrderBy(c => c.CODIGO);
                var cADTIPOSNOVEDADES_C = db.CADTIPOSNOVEDADES.Include(c => c.CADTIPOSNOVEDADE1).Where(c => c.CADNOVBIN == id).OrderBy(c => c.CODIGO);

                var variosModelos = new Tuple<List<CADTIPOSNOVEDADE>, List<CADTIPOSNOVEDADE>>
                    (cADTIPOSNOVEDADES_T.ToList(), cADTIPOSNOVEDADES_C.ToList())
                { };

                ViewBag.COD_TIPO = id;
                ViewBag.NOM_TIPO = nom;


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetClases(string id)
        {
            try
            {
                var cADTIPOSNOVEDADES_C = db.CADTIPOSNOVEDADES.Include(c => c.CADTIPOSNOVEDADE1).Where(c => c.CADNOVBIN == id).OrderBy(c => c.CODIGO);
                return PartialView("_clases", cADTIPOSNOVEDADES_C.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: TiposNovedades/Create
        public ActionResult Create()
        {
            try
            {
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION");
                ViewBag.CODIGO = 0;
                ViewBag.NIVEL = 1;
                ViewBag.CADNOVBIN = null;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION,NIVEL,CADNOVBIN")] CADTIPOSNOVEDADE cADTIPOSNOVEDADE)
        {
            try
            {
                int CODIGO;
                var existe = db.CADTIPOSNOVEDADES.FirstOrDefault();

                if (existe == null)
                    CODIGO = 1;
                else
                {
                    CODIGO = db.CADTIPOSNOVEDADES.Select(c => c.CODIGO).ToList().Max(c => int.Parse(c));
                    CODIGO += 1;
                }

                cADTIPOSNOVEDADE.CODIGO = CODIGO.ToString();

                if (ModelState.IsValid)
                {
                    db.CADTIPOSNOVEDADES.Add(cADTIPOSNOVEDADE);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "TiposNovedades", new { strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index");
                }

                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                return View(cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: TiposNovedades/CreateC
        public ActionResult CreateC(string id, string nom)
        {
            try
            {
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.CODIGO = 0;
                ViewBag.NIVEL = 2;
                ViewBag.CADNOVBIN = id;
                ViewBag.TIPO = nom;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/CreateC To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateC(string id, string nom, [Bind(Include = "CODIGO,DESCRIPCION,NIVEL,CADNOVBIN")] CADTIPOSNOVEDADE cADTIPOSNOVEDADE)
        {
            try
            {
                int CODIGO;
                var existe = db.CADTIPOSNOVEDADES.FirstOrDefault();

                if (existe == null)
                    CODIGO = 1;
                else
                {
                    CODIGO = db.CADTIPOSNOVEDADES.Select(c => c.CODIGO).ToList().Max(c => int.Parse(c));
                    CODIGO += 1;
                }

                cADTIPOSNOVEDADE.CODIGO = CODIGO.ToString();

                if (ModelState.IsValid)
                {
                    db.CADTIPOSNOVEDADES.Add(cADTIPOSNOVEDADE);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "TiposNovedades", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { id = id, nom = nom });
                }

                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.CADNOVBIN = id;
                ViewBag.TIPO = nom;
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                return View(cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: TiposNovedades/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                if (cADTIPOSNOVEDADE == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                return View(cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,DESCRIPCION,NIVEL,CADNOVBIN")] CADTIPOSNOVEDADE cADTIPOSNOVEDADE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cADTIPOSNOVEDADE.NIVEL = 1;
                    db.Entry(cADTIPOSNOVEDADE).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index");
                }
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                return View(cADTIPOSNOVEDADE);
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

        // GET: TiposNovedades/Edit/5
        public ActionResult EditC(string id, string idTip, string nomTip)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                if (cADTIPOSNOVEDADE == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                ViewBag.idTip = idTip;
                ViewBag.nomTip = nomTip;
                ViewBag.CADNOVBIN = idTip;
                ViewBag.TIPO = nomTip;

                return View(cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditC(string idTip, string nomTip, [Bind(Include = "CODIGO,DESCRIPCION,NIVEL,CADNOVBIN")] CADTIPOSNOVEDADE cADTIPOSNOVEDADE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cADTIPOSNOVEDADE.NIVEL = 2;
                    db.Entry(cADTIPOSNOVEDADE).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = idTip, nom = nomTip, strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index", new { id = idTip, nom = nomTip });
                }
                //ViewBag.CADNOVBIN = new SelectList(db.CADTIPOSNOVEDADES, "CODIGO", "DESCRIPCION", cADTIPOSNOVEDADE.CADNOVBIN);
                ViewBag.idTip = idTip;
                ViewBag.nomTip = nomTip;
                ViewBag.CADNOVBIN = idTip;
                ViewBag.TIPO = nomTip;
                return View(cADTIPOSNOVEDADE);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Eliminar(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                if (cADTIPOSNOVEDADE == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                db.CADTIPOSNOVEDADES.Remove(cADTIPOSNOVEDADE);
                db.SaveChanges();



                CADTIPOSNOVEDADE cADTIPOSNOVEDADE_copy = new CADTIPOSNOVEDADE();
                cADTIPOSNOVEDADE_copy = (CADTIPOSNOVEDADE)auditoria.GenerarCopiaAuditoria(cADTIPOSNOVEDADE);

                auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE_copy, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
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

        public ActionResult EliminarC(string id, string idTip, string nomTip)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                if (cADTIPOSNOVEDADE == null)
                {
                    return HttpNotFound();
                }

                ViewBag.idTip = idTip;
                ViewBag.nomTip = nomTip;
                ViewBag.CADNOVBIN = idTip;
                ViewBag.TIPO = nomTip;

                return View("DeleteC", null, cADTIPOSNOVEDADE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: TiposNovedades/Delete/5
        [HttpPost, ActionName("DeleteC")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedC(string id, string idTip, string nomTip)
        {
            try
            {
                CADTIPOSNOVEDADE cADTIPOSNOVEDADE = db.CADTIPOSNOVEDADES.Find(id);
                db.CADTIPOSNOVEDADES.Remove(cADTIPOSNOVEDADE);
                db.SaveChanges();


                CADTIPOSNOVEDADE cADTIPOSNOVEDADE_copy = new CADTIPOSNOVEDADE();
                cADTIPOSNOVEDADE_copy = (CADTIPOSNOVEDADE)auditoria.GenerarCopiaAuditoria(cADTIPOSNOVEDADE);


                auditoria.RegistrarAuditoria<CADTIPOSNOVEDADE>(cADTIPOSNOVEDADE_copy, "CADTIPOSNOVEDADES", cADTIPOSNOVEDADE.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el caso: " + cADTIPOSNOVEDADE.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { id = idTip, nom = nomTip, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { id = idTip, nom = nomTip });
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