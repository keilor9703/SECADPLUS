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
    public class ElementosCaracteristicasController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: ElementosCaracteristicas/Create
        [SecadPlusAutorizador("/ElementosCaracteristicas/Create")]
        public ActionResult Create(int tibiCodigo, string nomTipoBien)
        {
            try
            {
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

        // POST: ElementosCaracteristicas/Create To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, string nom, [Bind(Include = "CODIGO,DESCRIPCION,TIBI_CODIGO,TIPO,TAMANO")] CADCARACTERISTICA cADCARACTERISTICA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CODIGO = db.CADCARACTERISTICAS.Max(c => c.CODIGO);
                    CODIGO += 1;
                    cADCARACTERISTICA.CODIGO = CODIGO;

                    db.CADCARACTERISTICAS.Add(cADCARACTERISTICA);
                    db.SaveChanges();
                    auditoria.RegistrarAuditoria<CADCARACTERISTICA>(cADCARACTERISTICA, "CADCARACTERISTICAS", cADCARACTERISTICA.CODIGO.ToString(), TipoAuditoria.Crear);
                    var mensaje = "Ha sido creado el registro: " + cADCARACTERISTICA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    return RedirectToAction("Index", "ElementosMaestras", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });
                }
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.TIBI_CODIGO = id;
                ViewBag.TIPO_BIEN = nom;
                return View(cADCARACTERISTICA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ElementosCaracteristicas/Edit/5
        [SecadPlusAutorizador("/ElementosCaracteristicas/Edit")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCARACTERISTICA cADCARACTERISTICA = db.CADCARACTERISTICAS.Find(id);
                if (cADCARACTERISTICA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.nom = cADCARACTERISTICA.CADTIPOS_BIEN.DESCRIPCION;
                ViewBag.TIPO_BIEN = cADCARACTERISTICA.CADTIPOS_BIEN.DESCRIPCION;
                return View(cADCARACTERISTICA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosCaracteristicas/Edit/5 To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string nom, [Bind(Include = "CODIGO,DESCRIPCION,TIBI_CODIGO,TIPO,TAMANO")] CADCARACTERISTICA cADCARACTERISTICA)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADCARACTERISTICA).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCARACTERISTICA>(cADCARACTERISTICA, "CADCARACTERISTICAS", cADCARACTERISTICA.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editada la caracteristica: " + cADCARACTERISTICA.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                    return RedirectToAction("Index", "ElementosMaestras", new { id = cADCARACTERISTICA.TIBI_CODIGO, nom = nom, strMensaje = mensaje, strTipo = tipo });
                }
                ViewBag.nom = nom;
                ViewBag.TIPO_BIEN = nom;
                return View(cADCARACTERISTICA);
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

        [SecadPlusAutorizador("/ElementosCaracteristicas/Eliminar")]
        public ActionResult Eliminar(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCARACTERISTICA cADCARACTERISTICA = db.CADCARACTERISTICAS.Find(id);
                if (cADCARACTERISTICA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.nom = cADCARACTERISTICA.CADTIPOS_BIEN.DESCRIPCION;
                return View("Delete", null, cADCARACTERISTICA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosCaracteristicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string nom)
        {
            try
            {
                CADCARACTERISTICA cADCARACTERISTICA = db.CADCARACTERISTICAS.Find(id);
                db.CADCARACTERISTICAS.Remove(cADCARACTERISTICA);
                db.SaveChanges();



                CADCARACTERISTICA cADCARACTERISTICA_copy = new CADCARACTERISTICA();
                cADCARACTERISTICA_copy = (CADCARACTERISTICA)auditoria.GenerarCopiaAuditoria(cADCARACTERISTICA);
                auditoria.RegistrarAuditoria<CADCARACTERISTICA>(cADCARACTERISTICA_copy, "CADCARACTERISTICAS", cADCARACTERISTICA.CODIGO.ToString(), TipoAuditoria.Eliminar);
                var mensaje = "Ha sido eliminada la caracteristica: " + cADCARACTERISTICA.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                return RedirectToAction("Index", "ElementosMaestras", new { id = cADCARACTERISTICA.TIBI_CODIGO, nom = nom, strMensaje = mensaje, strTipo = tipo });
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