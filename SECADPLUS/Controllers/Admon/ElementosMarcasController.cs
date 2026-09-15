using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ElementosMarcasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: ElementosMarcas/Create
        public ActionResult Create(int tibiCodigo, string nomTipoBien)
        {
            try
            {
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION");
                ViewBag.id = tibiCodigo;
                ViewBag.nom = nomTipoBien;
                ViewBag.TIBI_CODIGO = tibiCodigo;
                ViewBag.TIPO_BIEN = nomTipoBien;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosMarcas/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION,TIBI_CODIGO")] CADMARCA cADMARCA)
        {
            try
            {
                var id = Request["id"];
                var nom = Request["nom"];

                if (ModelState.IsValid)
                {
                    Int16 CODIGO = db.CADMARCAS.Max(c => c.CODIGO);
                    CODIGO += 1;
                    cADMARCA.CODIGO = CODIGO;

                    db.CADMARCAS.Add(cADMARCA);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMARCA>(cADMARCA, "CADMARCAS", cADMARCA.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADMARCA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras", new { id = id, nom = nom });
                }

                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.TIBI_CODIGO = id;
                ViewBag.TIPO_BIEN = nom;

                return View(cADMARCA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ElementosMarcas/Edit/5
        public ActionResult Edit(short? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADMARCA cADMARCA = db.CADMARCAS.Find(id);
                if (cADMARCA == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION", cADMARCA.TIBI_CODIGO);
                ViewBag.TIPO_BIEN = cADMARCA.CADTIPOS_BIEN.DESCRIPCION;
                return View(cADMARCA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosMarcas/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,DESCRIPCION,TIBI_CODIGO")] CADMARCA cADMARCA)
        {
            try
            {
                var TIPO_BIEN = Request["TIPO_BIEN"].ToString();

                if (ModelState.IsValid)
                {
                    db.Entry(cADMARCA).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADMARCA>(cADMARCA, "CADMARCAS", cADMARCA.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editada la marca: " + cADMARCA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { id = cADMARCA.TIBI_CODIGO, nom = TIPO_BIEN, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras", new { id = cADMARCA.TIBI_CODIGO, nom = TIPO_BIEN });
                }
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION", cADMARCA.TIBI_CODIGO);
                ViewBag.TIPO_BIEN = TIPO_BIEN;
                return View(cADMARCA);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Eliminar(short? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADMARCA cADMARCA = db.CADMARCAS.Find(id);
                if (cADMARCA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.nom = cADMARCA.CADTIPOS_BIEN.DESCRIPCION;
                return View("Delete", null, cADMARCA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosMarcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id, string nom)
        {
            try
            {
                CADMARCA cADMARCA = db.CADMARCAS.Find(id);
                db.CADMARCAS.Remove(cADMARCA);
                db.SaveChanges();

                CADMARCA cADMARCA_copy = new CADMARCA();
                cADMARCA_copy = (CADMARCA)auditoria.GenerarCopiaAuditoria(cADMARCA);

                auditoria.RegistrarAuditoria<CADMARCA>(cADMARCA, "CADMARCAS", cADMARCA.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminada la marca: " + cADMARCA.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { id = cADMARCA.TIBI_CODIGO, nom = nom, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", "ElementosMaestras", new { id = cADMARCA.TIBI_CODIGO, nom = nom });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });
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