using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class PerfilController : Controller
    {
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));

        [SecadPlusAutorizador("/Perfil/Index")]
        public async Task<ActionResult> Index(string strMensaje)
        {
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            string strAccion = ControllerContext.RouteData.Values["action"].ToString();
            string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
            ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
            ///////////////////////////Variables BreadCrumb////////////////////////////////////////
            if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
            {
                ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                GestorBase.MSN_ALERTA_COMUN = "";
            }
            if (!string.IsNullOrEmpty(strMensaje))
            {
                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
            }

            return View(await db.CADNETPERFILs.ToListAsync());
        }
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: Perfil/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Perfil/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PERFIL_ID,PERFIL_NOMBRE,PERFIL_ESTADO")] CADNETPERFIL cADNETPERFIL)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cADNETPERFIL.PERFIL_NOMBRE = cADNETPERFIL.PERFIL_NOMBRE.ToUpper();
                    db.CADNETPERFILs.Add(cADNETPERFIL);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADNETPERFIL.PERFIL_NOMBRE) });
                }

                return View(cADNETPERFIL);
            }
            catch (Exception ex) {
                if (ex.HResult == -2146233087)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                }else
                {
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                }
                return RedirectToAction("Index", "Perfil");
            }
            
        }

        // GET: Perfil/Edit/5
        public async Task<ActionResult> Edit(decimal id)
        {
            
            CADNETPERFIL cADNETPERFIL = await db.CADNETPERFILs.FindAsync(id);
            if (cADNETPERFIL == null)
            {
                return HttpNotFound();
            }
            return View(cADNETPERFIL);
        }

        // POST: Perfil/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PERFIL_ID,PERFIL_NOMBRE,PERFIL_ESTADO, PERFIL_FECHA_CREACION")] CADNETPERFIL cADNETPERFIL)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cADNETPERFIL.PERFIL_NOMBRE = cADNETPERFIL.PERFIL_NOMBRE.ToUpper();
                    db.Entry(cADNETPERFIL).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADNETPERFIL>(cADNETPERFIL, "CADNETPERFIL", "PERFIL_ID=" + cADNETPERFIL.PERFIL_ID.ToString(), TipoAuditoria.Modificar);

                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADNETPERFIL.PERFIL_NOMBRE) });
                }
                return View(cADNETPERFIL);
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146233087)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                }
                else
                {
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                }
                return RedirectToAction("Index", "Perfil");
            }            
        }

        [Authorize]
        public ActionResult Eliminar(decimal id)
        {
            CADNETPERFIL cADNETPERFIL = db.CADNETPERFILs.Find(id);
            if (cADNETPERFIL == null)
            {
                return HttpNotFound();
            }
            return View("Delete", null, cADNETPERFIL);
        }

        // POST: Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(decimal id, string strNombre)
        {
            
            try
            {
                CADNETPERFIL cADNETPERFIL = await db.CADNETPERFILs.FindAsync(id);
                db.CADNETPERFILs.Remove(cADNETPERFIL);
                await db.SaveChangesAsync();

                CADNETPERFIL cADNETPERFIL_copy = new CADNETPERFIL();
                cADNETPERFIL_copy = (CADNETPERFIL)auditoria.GenerarCopiaAuditoria(cADNETPERFIL);

                auditoria.RegistrarAuditoria<CADNETPERFIL>(cADNETPERFIL_copy, "CADNETPERFIL", "PERFIL_ID=" + cADNETPERFIL.PERFIL_ID.ToString(), TipoAuditoria.Eliminar);


                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strNombre);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ex.GetType();
                return RedirectToAction("Index");
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