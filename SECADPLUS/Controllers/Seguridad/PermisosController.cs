using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class PermisosController : Controller
    {
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();


        // GET: Permisos
        public async Task<ActionResult> Index()
        {
            return View(await db.CADNETPERMISOes.ToListAsync());
        }

        // GET: Permisos/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
            
            CADNETPERMISO cADNETPERMISO = await db.CADNETPERMISOes.FindAsync(id);
            if (cADNETPERMISO == null)
            {
                return HttpNotFound();
            }
            return View(cADNETPERMISO);
        }

        // GET: Permisos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permisos/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PERMISO_ID,PERMISO_NOMBRE,PERMISO_DESCRIPCION,PERMISO_ESTADO,PERMISO_FECHA_CREACION")] CADNETPERMISO cADNETPERMISO)
        {
            if (ModelState.IsValid)
            {
                db.CADNETPERMISOes.Add(cADNETPERMISO);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(cADNETPERMISO);
        }

        // GET: Permisos/Edit/5
        public async Task<ActionResult> Edit(decimal id)
        {
            
            CADNETPERMISO cADNETPERMISO = await db.CADNETPERMISOes.FindAsync(id);
            if (cADNETPERMISO == null)
            {
                return HttpNotFound();
            }
            return View(cADNETPERMISO);
        }

        // POST: Permisos/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PERMISO_ID,PERMISO_NOMBRE,PERMISO_DESCRIPCION,PERMISO_ESTADO,PERMISO_FECHA_CREACION")] CADNETPERMISO cADNETPERMISO)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cADNETPERMISO).State = EntityState.Modified;
                await db.SaveChangesAsync();

                auditoria.RegistrarAuditoria<CADNETPERMISO>(cADNETPERMISO, "CADNETPERMISO", "PERMISO_ID=" + cADNETPERMISO.PERMISO_ID.ToString(), TipoAuditoria.Modificar);

                return RedirectToAction("Index");
            }
            return View(cADNETPERMISO);
        }

        // GET: Permisos/Delete/5
        public async Task<ActionResult> Delete(decimal id)
        {
            
            CADNETPERMISO cADNETPERMISO = await db.CADNETPERMISOes.FindAsync(id);
            if (cADNETPERMISO == null)
            {
                return HttpNotFound();
            }
            return View(cADNETPERMISO);
        }

        // POST: Permisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id)
        {
            CADNETPERMISO cADNETPERMISO = await db.CADNETPERMISOes.FindAsync(id);
            db.CADNETPERMISOes.Remove(cADNETPERMISO);
            await db.SaveChangesAsync();

            CADNETPERMISO cADNETPERMISO_copy = new CADNETPERMISO();
            cADNETPERMISO_copy = (CADNETPERMISO)auditoria.GenerarCopiaAuditoria(cADNETPERMISO);

            auditoria.RegistrarAuditoria<CADNETPERMISO>(cADNETPERMISO_copy, "CADNETPERMISO", "PERMISO_ID=" + cADNETPERMISO.PERMISO_ID.ToString(), TipoAuditoria.Eliminar);

            return RedirectToAction("Index");
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