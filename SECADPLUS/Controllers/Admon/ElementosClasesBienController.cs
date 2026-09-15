using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ElementosClasesBienController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: ElementosClasesBien/Create
        [SecadPlusAutorizador("/ElementosClasesBien/Create")]
        public ActionResult Create(int tibiCodigo, string nomTipoBien)
        {
            try
            {
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION");
                ViewBag.id = tibiCodigo;
                ViewBag.nom = nomTipoBien;
                ViewBag.TIBI_CODIGO = tibiCodigo;
                ViewBag.TIPO_BIEN = nomTipoBien;
                ViewBag.UNME_CODIGO = new SelectList(db.CADUNIDADES_MEDIDA.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosClasesBien/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TIBI_CODIGO,CODIGO,DESCRIPCION,UNME_CODIGO,TIBI_CODIGO_FISCALIA,CODIGO_FISCALIA")] CADCLASES_BIEN cADCLASES_BIEN)
        {
            try
            {
                var id = int.Parse(Request["id"]);
                var nom = Request["nom"];

                if (ModelState.IsValid)
                {
                    Int16 CODIGO;
                    var existe = db.CADCLASES_BIEN.FirstOrDefault(c => c.TIBI_CODIGO == id);

                    if (existe == null)
                        CODIGO = 1;
                    else
                    {
                        CODIGO = db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == id).Max(c => c.CODIGO);
                        CODIGO += 1;
                    }

                    cADCLASES_BIEN.CODIGO = CODIGO;

                    db.CADCLASES_BIEN.Add(cADCLASES_BIEN);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCLASES_BIEN>(cADCLASES_BIEN, "CADCLASES_BIEN", "LLAVE COMPUESTA: TIBI_CODIGO:" + cADCLASES_BIEN.TIBI_CODIGO + " | CODIGO:" + cADCLASES_BIEN.CODIGO, TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADCLASES_BIEN.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras", new { id = id, nom = nom });
                }

                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.TIBI_CODIGO = id;
                ViewBag.TIPO_BIEN = nom;
                ViewBag.UNME_CODIGO = new SelectList(db.CADUNIDADES_MEDIDA.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCLASES_BIEN.UNME_CODIGO);
                return View(cADCLASES_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ElementosClasesBien/Edit/5
        [SecadPlusAutorizador("/ElementosClasesBien/Edit")]
        public ActionResult Edit(int idTibiCodigo, byte? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCLASES_BIEN cADCLASES_BIEN = db.CADCLASES_BIEN.Find(idTibiCodigo, id);
                if (cADCLASES_BIEN == null)
                {
                    return HttpNotFound();
                }
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION", cADCLASES_BIEN.TIBI_CODIGO);
                ViewBag.nom = cADCLASES_BIEN.CADTIPOS_BIEN.DESCRIPCION;
                ViewBag.TIPO_BIEN = cADCLASES_BIEN.CADTIPOS_BIEN.DESCRIPCION;
                ViewBag.UNME_CODIGO = new SelectList(db.CADUNIDADES_MEDIDA.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCLASES_BIEN.UNME_CODIGO);
                return View(cADCLASES_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosClasesBien/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TIBI_CODIGO,CODIGO,DESCRIPCION,UNME_CODIGO,TIBI_CODIGO_FISCALIA,CODIGO_FISCALIA")] CADCLASES_BIEN cADCLASES_BIEN)
        {
            try
            {
                var TIPO_BIEN = Request["TIPO_BIEN"].ToString();

                if (ModelState.IsValid)
                {
                    db.Entry(cADCLASES_BIEN).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCLASES_BIEN>(cADCLASES_BIEN, "CADCLASES_BIEN", "LLAVE COMPUESTA: TIBI_CODIGO:" + cADCLASES_BIEN.TIBI_CODIGO + " | CODIGO:" + cADCLASES_BIEN.CODIGO, TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editada la clase de bien: " + cADCLASES_BIEN.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { id = cADCLASES_BIEN.TIBI_CODIGO, nom = TIPO_BIEN, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "ElementosMaestras", new { id = cADCLASES_BIEN.TIBI_CODIGO, nom = TIPO_BIEN });
                }
                //ViewBag.TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN, "CODIGO", "DESCRIPCION", cADCLASES_BIEN.TIBI_CODIGO);
                ViewBag.nom = TIPO_BIEN;
                ViewBag.TIPO_BIEN = TIPO_BIEN;
                ViewBag.UNME_CODIGO = new SelectList(db.CADUNIDADES_MEDIDA.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADCLASES_BIEN.UNME_CODIGO);
                return View(cADCLASES_BIEN);
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

        [SecadPlusAutorizador("/ElementosClasesBien/Eliminar")]
        public ActionResult Eliminar(int idTibiCodigo, byte? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCLASES_BIEN cADCLASES_BIEN = db.CADCLASES_BIEN.Find(idTibiCodigo, id);
                if (cADCLASES_BIEN == null)
                {
                    return HttpNotFound();
                }
                ViewBag.nom = cADCLASES_BIEN.CADTIPOS_BIEN.DESCRIPCION;
                return View("Delete", null, cADCLASES_BIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: ElementosClasesBien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int idTibiCodigo, string nom, byte? id)
        {
            try
            {
                CADCLASES_BIEN cADCLASES_BIEN = db.CADCLASES_BIEN.Find(idTibiCodigo, id);
                db.CADCLASES_BIEN.Remove(cADCLASES_BIEN);
                db.SaveChanges();



                CADCLASES_BIEN cADCLASES_BIEN_copy = new CADCLASES_BIEN();
                cADCLASES_BIEN_copy = (CADCLASES_BIEN)auditoria.GenerarCopiaAuditoria(cADCLASES_BIEN);

                auditoria.RegistrarAuditoria<CADCLASES_BIEN>(cADCLASES_BIEN_copy, "CADCLASES_BIEN", "LLAVE COMPUESTA: TIBI_CODIGO:" + cADCLASES_BIEN.TIBI_CODIGO + " | CODIGO:" + cADCLASES_BIEN.CODIGO, TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminada la clase de bien: " + cADCLASES_BIEN.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { id = cADCLASES_BIEN.TIBI_CODIGO, nom = nom, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", "ElementosMaestras", new { id = cADCLASES_BIEN.TIBI_CODIGO, nom = nom });
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