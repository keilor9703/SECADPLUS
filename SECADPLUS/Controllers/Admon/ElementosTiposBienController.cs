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
    public class ElementosTiposBienController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: ElementosTiposBien/Create
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosTiposBien/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION")] CADTIPOS_BIEN cADTIPOS_BIEN)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    short CODIGO = db.CADTIPOS_BIEN.Max(c => c.CODIGO);
                    CODIGO += 1;
                    cADTIPOS_BIEN.CODIGO = CODIGO;

                    db.CADTIPOS_BIEN.Add(cADTIPOS_BIEN);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADTIPOS_BIEN>(cADTIPOS_BIEN, "CADSITIOS_GRABACION", cADTIPOS_BIEN.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADTIPOS_BIEN.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras");
                }

                return View(cADTIPOS_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ElementosTiposBien/Edit/5
        public ActionResult Edit(byte? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOS_BIEN cADTIPOS_BIEN = db.CADTIPOS_BIEN.Find(id);
                if (cADTIPOS_BIEN == null)
                {
                    return HttpNotFound();
                }
                return View(cADTIPOS_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosTiposBien/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,DESCRIPCION")] CADTIPOS_BIEN cADTIPOS_BIEN)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADTIPOS_BIEN).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADTIPOS_BIEN>(cADTIPOS_BIEN, "CADTIPOS_BIEN", cADTIPOS_BIEN.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro: " + cADTIPOS_BIEN.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras");
                }
                return View(cADTIPOS_BIEN);
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

        public ActionResult Eliminar(byte? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADTIPOS_BIEN cADTIPOS_BIEN = db.CADTIPOS_BIEN.Find(id);
                if (cADTIPOS_BIEN == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADTIPOS_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosTiposBien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            try
            {
                CADTIPOS_BIEN cADTIPOS_BIEN = db.CADTIPOS_BIEN.Find(id);
                db.CADTIPOS_BIEN.Remove(cADTIPOS_BIEN);
                db.SaveChanges();



                CADTIPOS_BIEN cADTIPOS_BIEN_copy = new CADTIPOS_BIEN();
                cADTIPOS_BIEN_copy = (CADTIPOS_BIEN)auditoria.GenerarCopiaAuditoria(cADTIPOS_BIEN);

                auditoria.RegistrarAuditoria<CADTIPOS_BIEN>(cADTIPOS_BIEN_copy, "CADTIPOS_BIEN", cADTIPOS_BIEN.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el tipo de bien: " + cADTIPOS_BIEN.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", "ElementosMaestras");
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