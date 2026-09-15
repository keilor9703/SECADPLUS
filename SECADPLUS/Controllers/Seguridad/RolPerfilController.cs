using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class RolPerfilController : Controller
    {
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();


        // GET: RolPerfil
        public async Task<ActionResult> Index(string strMensaje)
        {
            try
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

                var cADNETROLPERFILs = db.CADNETROLPERFILs.Include(c => c.CADNETPERFIL).Include(c => c.CADNETROLE);
                return View(await cADNETROLPERFILs.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: RolPerfil/Details/5
        public async Task<ActionResult> Details(decimal id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADNETROLPERFIL cADNETROLPERFIL = await db.CADNETROLPERFILs.FindAsync(id);
            if (cADNETROLPERFIL == null)
            {
                return HttpNotFound();
            }
            return View(cADNETROLPERFIL);
        }

        // GET: RolPerfil/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.PERFIL_ID = new SelectList(db.CADNETPERFILs, "PERFIL_ID", "PERFIL_NOMBRE");
                ViewBag.ROL_ID = new SelectList(db.CADNETROLES, "Id", "Name");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: RolPerfil/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PERFIL_ID,ROL_ID,ESTADO")] CADNETROLPERFIL cADNETROLPERFIL, String btnGuardar)
        {
            try
            {
                if (btnGuardar == null)
                {
                    var listaPerfiles = (from perfiles in db.CADNETPERFILs
                                         where !(from registro in db.CADNETROLPERFILs
                                                 where registro.ROL_ID == cADNETROLPERFIL.ROL_ID
                                                 select registro.PERFIL_ID).Contains(perfiles.PERFIL_ID)
                                         select perfiles);
                    ViewBag.PERFIL_ID = new SelectList(listaPerfiles, "PERFIL_ID", "PERFIL_NOMBRE", cADNETROLPERFIL.PERFIL_ID);
                    ViewBag.ROL_ID = new SelectList(db.CADNETROLES, "Id", "Name", cADNETROLPERFIL.ROL_ID);
                }
                else
                {
                    ViewBag.PERFIL_ID = new SelectList(db.CADNETPERFILs, "PERFIL_ID", "PERFIL_NOMBRE", cADNETROLPERFIL.PERFIL_ID);
                    ViewBag.ROL_ID = new SelectList(db.CADNETROLES, "Id", "Name", cADNETROLPERFIL.ROL_ID);
                    if (ModelState.IsValid)
                    {
                        db.CADNETROLPERFILs.Add(cADNETROLPERFIL);
                        //CADNETROLPERFIL registro = db.CADNETROLPERFILs.Find(cADNETROLPERFIL.PERFIL_ID, cADNETROLPERFIL.ROL_ID);
                        var dato = (from registro in db.CADNETROLPERFILs
                                    where registro.PERFIL_ID == cADNETROLPERFIL.PERFIL_ID && registro.ROL_ID == cADNETROLPERFIL.ROL_ID
                                    select new { registro.PERFIL_ID }).FirstOrDefault();
                        if (dato != null)
                        {
                            ViewBag.MENSAJE_EXITOSO = "Ya existe un registro con la información ingresada.";
                            ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                            return View(cADNETROLPERFIL);
                        }
                        else
                        {
                            await db.SaveChangesAsync();
                        }
                        return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "Nuevo") });
                    }
                }
                return View(cADNETROLPERFIL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: RolPerfil/Edit/5

        public ActionResult Edit(string rol_id, decimal perfil_id)
        {
            try
            {
                CADNETROLPERFIL cADNETROLPERFIL = (from registro in db.CADNETROLPERFILs
                                                   where registro.PERFIL_ID == perfil_id && registro.ROL_ID == rol_id
                                                   select registro).FirstOrDefault();
                if (cADNETROLPERFIL == null)
                {
                    return HttpNotFound();
                }
                ViewBag.PERFIL_ID = new SelectList(db.CADNETPERFILs, "PERFIL_ID", "PERFIL_NOMBRE", cADNETROLPERFIL.PERFIL_ID);
                ViewBag.ROL_ID = new SelectList(db.CADNETROLES, "Id", "Name", cADNETROLPERFIL.ROL_ID);
                return View(cADNETROLPERFIL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: RolPerfil/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PERFIL_ID,ROL_ID,ESTADO,FECHA_CREACION")] CADNETROLPERFIL cADNETROLPERFIL)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADNETROLPERFIL).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADNETROLPERFIL>(cADNETROLPERFIL, "CADNETROLPERFIL", "PERFIL_ID=" + cADNETROLPERFIL.PERFIL_ID.ToString() + ",ROL_ID=" + cADNETROLPERFIL.ROL_ID
                       , TipoAuditoria.Modificar);



                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "Seleccionado") });
                }
                ViewBag.PERFIL_ID = new SelectList(db.CADNETPERFILs, "PERFIL_ID", "PERFIL_NOMBRE", cADNETROLPERFIL.PERFIL_ID);
                ViewBag.ROL_ID = new SelectList(db.CADNETROLES, "Id", "Name", cADNETROLPERFIL.ROL_ID);
                return View(cADNETROLPERFIL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Eliminar(string rol_id, decimal perfil_id)
        {
            try
            {
                CADNETROLPERFIL cADNETROLPERFIL = (from registro in db.CADNETROLPERFILs
                                                   where registro.PERFIL_ID == perfil_id && registro.ROL_ID == rol_id
                                                   select registro).FirstOrDefault();
                if (cADNETROLPERFIL == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADNETROLPERFIL);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: RolPerfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string rol_id, decimal perfil_id)
        {
            
            try
            {
                CADNETROLPERFIL cADNETROLPERFIL = (from registro in db.CADNETROLPERFILs
                                                   where registro.PERFIL_ID == perfil_id && registro.ROL_ID == rol_id
                                                   select registro).FirstOrDefault();
                db.CADNETROLPERFILs.Remove(cADNETROLPERFIL);
                await db.SaveChangesAsync();


                CADNETROLPERFIL cADNETROLPERFIL_copy = new CADNETROLPERFIL();
                cADNETROLPERFIL_copy = (CADNETROLPERFIL)auditoria.GenerarCopiaAuditoria(cADNETROLPERFIL);

                auditoria.RegistrarAuditoria<CADNETROLPERFIL>(cADNETROLPERFIL_copy, "CADNETROLPERFIL", "PERFIL_ID=" + cADNETROLPERFIL.PERFIL_ID.ToString() + ",ROL_ID=" + cADNETROLPERFIL.ROL_ID
                   , TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Seleccionado");
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