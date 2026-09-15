using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ClasificacionCierreController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        /// <summary>
        /// Carga lista de clasificaciones de cierre
        /// </summary>
        /// <param name="idSitio">id del sitio de grabación</param>
        /// <param name="idEntidad">id de la endidad</param>
        /// <param name="nombreEntidad">Nombre de la entidad</param>
        /// <param name="message">Mensaje de alerta</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/ClasificacionCierre/Index")]
        public async Task<ActionResult> Index(int idSitio, int idEntidad, string nombreEntidad, string message = null)
        {
            try
            {
                if (message != null)
                {
                    ViewBag.MENSAJE_EXITOSO = message;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                ViewBag.idSitio = idSitio;
                ViewBag.idEntidad = idEntidad;
                ViewBag.nombreEntidad = nombreEntidad;
                var cADCLASIFICACION_CIERRE = db.CADCLASIFICACION_CIERRE.Where(t => t.CADFUERZA.SITIO_GRABA == idSitio && t.ID_FUERZA == idEntidad).Include(c => c.CADFUERZA);
                return View(await cADCLASIFICACION_CIERRE.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Muestra en detalle una clasificación de cierre
        /// </summary>
        /// <param name="id">id de la clasificación</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE = await db.CADCLASIFICACION_CIERRE.FindAsync(id);
            if (cADCLASIFICACION_CIERRE == null)
            {
                return HttpNotFound();
            }
            return View(cADCLASIFICACION_CIERRE);
        }

        /// <summary>
        /// Crea una nueva clasificación de cierre
        /// </summary>
        /// <param name="idSitio">id del sitio de grabación</param>
        /// <param name="idEntidad">id de la entidad</param>
        /// <param name="nombreEntidad">nombre de la entidad</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/ClasificacionCierre/Create")]
        public ActionResult Create(int idSitio, int idEntidad, string nombreEntidad)
        {
            try
            {
                ViewBag.idSitio = idSitio;
                ViewBag.idEntidad = idEntidad;
                ViewBag.nombreEntidad = nombreEntidad;
                ViewBag.ID_FUERZA = new SelectList(db.CADFUERZAS.Where(t => t.SITIO_GRABA == idSitio && t.ID == idEntidad), "ID", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga los datos de una nueva clasificación de cierre a la base de datos
        /// </summary>
        /// <param name="idSitio">id del sitio</param>
        /// <param name="nombreEntidad">nombre entidad</param>
        /// <param name="idEntidad">id entidad</param>
        /// <param name="cADCLASIFICACION_CIERRE">datos</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(int idSitio, string nombreEntidad, int idEntidad, [Bind(Include = "ID_FUERZA,CODIGO,DESCRIPCION")] CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.CADCLASIFICACION_CIERRE.Find(cADCLASIFICACION_CIERRE.ID_FUERZA, cADCLASIFICACION_CIERRE.CODIGO) == null)
                    {
                        db.CADCLASIFICACION_CIERRE.Add(cADCLASIFICACION_CIERRE);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { idSitio = idSitio, idEntidad = cADCLASIFICACION_CIERRE.ID_FUERZA, nombreEntidad = nombreEntidad, message = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADCLASIFICACION_CIERRE.CODIGO.ToString()) });
                    }
                    else
                    {
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", cADCLASIFICACION_CIERRE.CODIGO);
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                }
                ViewBag.idSitio = idSitio;
                ViewBag.idEntidad = cADCLASIFICACION_CIERRE.ID_FUERZA;
                ViewBag.nombreEntidad = nombreEntidad;
                ViewBag.ID_FUERZA = new SelectList(db.CADFUERZAS.Where(t => t.SITIO_GRABA == idSitio && t.ID == cADCLASIFICACION_CIERRE.ID_FUERZA), "ID", "DESCRIPCION", cADCLASIFICACION_CIERRE.ID_FUERZA);
                return View(cADCLASIFICACION_CIERRE);
            }
            catch (DBConcurrencyException ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página para editar una clasificación de cierre
        /// </summary>
        /// <param name="id">id clasificación</param>
        /// <param name="idSitio">id del sitio</param>
        /// <param name="idEntidad">id de la entidad</param>
        /// <param name="nombreEntidad">nombre de la entidad</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/ClasificacionCierre/Edit")]
        public ActionResult Edit(string id, int idSitio, int idEntidad, string nombreEntidad)
        {
            try
            {
                ViewBag.idSitio = idSitio;
                ViewBag.idEntidad = idEntidad;
                ViewBag.nombreEntidad = nombreEntidad;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE = db.CADCLASIFICACION_CIERRE.Where(t => t.ID_FUERZA == idEntidad && t.CODIGO == id).FirstOrDefault();
                if (cADCLASIFICACION_CIERRE == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ID_FUERZA = new SelectList(db.CADFUERZAS.Where(t => t.SITIO_GRABA == idSitio && t.ID == cADCLASIFICACION_CIERRE.ID_FUERZA), "ID", "DESCRIPCION", cADCLASIFICACION_CIERRE.ID_FUERZA);
                return View(cADCLASIFICACION_CIERRE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Guarda los datos de modificación de clasificación de cierre
        /// </summary>
        /// <param name="idSitio">id del sitio de grabación</param>
        /// <param name="nombreEntidad">nombre de la entidad</param>
        /// <param name="idEntidad">id de la entidad</param>
        /// <param name="cADCLASIFICACION_CIERRE">datos</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/ClasificacionCierre/Edit")]
        public async Task<ActionResult> Edit(int idSitio, string nombreEntidad, int idEntidad, [Bind(Include = "ID_FUERZA,CODIGO,DESCRIPCION")] CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(cADCLASIFICACION_CIERRE).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        auditoria.RegistrarAuditoria<CADCLASIFICACION_CIERRE>(cADCLASIFICACION_CIERRE, "CADCLASIFICACION_CIERRE", "ID_FUERZA=" + cADCLASIFICACION_CIERRE.ID_FUERZA.ToString() + ",CODIGO=" + cADCLASIFICACION_CIERRE.CODIGO
                       , TipoAuditoria.Modificar);

                        return RedirectToAction("Index", new { idSitio = idSitio, idEntidad = cADCLASIFICACION_CIERRE.ID_FUERZA, nombreEntidad = nombreEntidad, message = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCLASIFICACION_CIERRE.CODIGO.ToString()) });
                    }
                    catch (DBConcurrencyException ex)
                    {
                        var mensaje = ex.Message;
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                }
                ViewBag.idSitio = idSitio;
                ViewBag.idEntidad = cADCLASIFICACION_CIERRE.ID_FUERZA;
                ViewBag.nombreEntidad = nombreEntidad;
                ViewBag.ID_FUERZA = new SelectList(db.CADFUERZAS.Where(t => t.SITIO_GRABA == idSitio && t.ID == cADCLASIFICACION_CIERRE.ID_FUERZA), "ID", "DESCRIPCION", cADCLASIFICACION_CIERRE.ID_FUERZA);
                return View(cADCLASIFICACION_CIERRE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga página de verificación de eliminación
        /// </summary>
        /// <param name="id">id de la clasificación</param>
        /// <param name="idSitio">id sitio de grabación</param>
        /// <param name="idEntidad">id entidad</param>
        /// <param name="nombreEntidad">nombre entidad</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Eliminar(string id, int idSitio, int idEntidad, string nombreEntidad)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE = db.CADCLASIFICACION_CIERRE.Where(t => t.ID_FUERZA == idEntidad && t.CODIGO == id).FirstOrDefault();
                if (cADCLASIFICACION_CIERRE == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADCLASIFICACION_CIERRE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Confirma eliminación de la base de datos de una clasificación de cierre
        /// </summary>
        /// <param name="id">id clasificación</param>
        /// <param name="idSitio">id sitio de grabación</param>
        /// <param name="idEntidad">id entidad</param>
        /// <param name="nombreEntidad">nombre entidad</param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, int idSitio, int idEntidad, string nombreEntidad)
        {
            try
            {
                CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE = db.CADCLASIFICACION_CIERRE.Where(t => t.ID_FUERZA == idEntidad && t.CODIGO == id).FirstOrDefault();
                db.CADCLASIFICACION_CIERRE.Remove(cADCLASIFICACION_CIERRE);
                await db.SaveChangesAsync();



                CADCLASIFICACION_CIERRE cADCLASIFICACION_CIERRE_copy = new CADCLASIFICACION_CIERRE();
                cADCLASIFICACION_CIERRE_copy = (CADCLASIFICACION_CIERRE)auditoria.GenerarCopiaAuditoria(cADCLASIFICACION_CIERRE);

                auditoria.RegistrarAuditoria<CADCLASIFICACION_CIERRE>(cADCLASIFICACION_CIERRE_copy, "CADCLASIFICACION_CIERRE", "ID_FUERZA=" + cADCLASIFICACION_CIERRE.ID_FUERZA.ToString() + ",CODIGO=" + cADCLASIFICACION_CIERRE.CODIGO
              , TipoAuditoria.Eliminar);




                return RedirectToAction("Index", new { idSitio = idSitio, idEntidad = cADCLASIFICACION_CIERRE.ID_FUERZA, nombreEntidad = nombreEntidad, message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Libera la clase de la memoria
        /// </summary>
        /// <param name="disposing"></param>
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