using SECADEntities.SecadLugaresGeograficos;
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
    public class BarriosController : Controller
    {
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        /// <summary>
        /// Carga el listado de barrios
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string message = null)
        {
            try
            {
                var cADBARRIOS = db.CADBARRIOS.Include(c => c.CADCOMUNA);
                return View(await cADBARRIOS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga el detalle de un barrio
        /// </summary>
        /// <param name="id">número que identifica al barrio</param>
        /// <returns></returns>
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADBARRIO cADBARRIO = await db.CADBARRIOS.FindAsync(id);
            if (cADBARRIO == null)
            {
                return HttpNotFound();
            }
            return View(cADBARRIO);
        }

        /// <summary>
        /// Carga página para crear un barrio
        /// </summary>
        /// <param name="idFather">Lugar geográfico del barrio</param>
        /// <param name="idComuna">Comuna del barrio</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create(int idFather, int idComuna)
        {
            try
            {
                ViewBag.COZO_CODIGO = new SelectList(db.CADCOMUNAS.Where(t => t.CODIGO == idComuna && t.LUGE_CODIGO == idFather), "CODIGO", "DESCRIPCION");
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");
                ViewBag.COZO_LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Guarda en base de datos el nuevo barrio
        /// </summary>
        /// <param name="idFather">Lugar geográfico del barrio</param>
        /// <param name="idComuna">id de la comuna del barrio</param>
        /// <param name="cADBARRIO">Objeto con datos del barrio</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int idFather, int idComuna, [Bind(Include = "CODIGO,COZO_CODIGO,DESCRIPCION,LUGE_CODIGO,COZO_LUGE_CODIGO,CODIGO_MPIO,TIPO_REGISTRO,AREAM2")] CADBARRIO cADBARRIO)
        {
            try
            {
                ViewBag.COZO_CODIGO = new SelectList(db.CADCOMUNAS.Where(t => t.CODIGO == idComuna && t.LUGE_CODIGO == idFather), "CODIGO", "DESCRIPCION");
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");
                ViewBag.COZO_LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");

                if (ModelState.IsValid)
                {
                    if (db.CADBARRIOS.Find(cADBARRIO.CODIGO, cADBARRIO.LUGE_CODIGO) == null)
                    {
                        db.CADBARRIOS.Add(cADBARRIO);
                        await db.SaveChangesAsync();
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                        return RedirectToAction("Create", "Barrios", new { idFather = cADBARRIO.LUGE_CODIGO, idComuna = cADBARRIO.COZO_CODIGO });
                    }
                    else
                    {
                        ViewBag.MENSAJE_EXITOSO = "El código de barrio para el lugar geográfico ya existe";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                }
                return View(cADBARRIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página para editar un barrio
        /// </summary>
        /// <param name="id">identificador del barrio</param>
        /// <param name="idFather">lugar geográfico del barrio</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public ActionResult Edit(int? id, int idFather)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBARRIO cADBARRIO = db.CADBARRIOS.Where(t => t.CODIGO == id && t.LUGE_CODIGO == idFather).FirstOrDefault();

                if (cADBARRIO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.COZO_CODIGO = new SelectList(db.CADCOMUNAS.Where(t => t.LUGE_CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.COZO_CODIGO);
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.LUGE_CODIGO);
                ViewBag.COZO_LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.COZO_LUGE_CODIGO);

                return View(cADBARRIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Guarda datos modificados del barrio
        /// </summary>
        /// <param name="id">identificador del barrio</param>
        /// <param name="idFather">lugar geográfico del barrio</param>
        /// <param name="cADBARRIO">objeto con datos del barrio</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public async Task<ActionResult> Edit(int id, int idFather, [Bind(Include = "CODIGO,COZO_CODIGO,DESCRIPCION,LUGE_CODIGO,COZO_LUGE_CODIGO,CODIGO_MPIO,TIPO_REGISTRO,AREAM2")] CADBARRIO cADBARRIO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADBARRIO).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADBARRIO>(cADBARRIO, "CADBARRIO", "CODIGO=" + cADBARRIO.CODIGO.ToString() + ",LUGE_CODIGO=" + cADBARRIO.LUGE_CODIGO, TipoAuditoria.Modificar);


                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                ViewBag.COZO_CODIGO = new SelectList(db.CADCOMUNAS.Where(t => t.LUGE_CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.COZO_CODIGO);
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.LUGE_CODIGO);
                ViewBag.COZO_LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION", cADBARRIO.COZO_LUGE_CODIGO);
                return View(cADBARRIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página para borrar un barrio
        /// </summary>
        /// <param name="id">identificador del barrio</param>
        /// <param name="idFather">lugar geográfico del barrio</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/LugaresGeograficos/Delete")]
        public async Task<ActionResult> Delete(int? id, int? idFather)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBARRIO cADBARRIO = await db.CADBARRIOS.FindAsync(id, idFather);
                if (cADBARRIO == null)
                {
                    return HttpNotFound();
                }
                return View(cADBARRIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Confirma la eliminación del barrio
        /// </summary>
        /// <param name="id">identificador del barrio</param>
        /// <param name="idFather">lugar geográfico del barrio</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult DeleteConfirmed(int id, int idFather)
        {
            try
            {
                CADBARRIO cADBARRIO = db.CADBARRIOS.Find(id, idFather);
                db.CADBARRIOS.Remove(cADBARRIO);
                db.SaveChanges();


                CADBARRIO cADBARRIO_copy = new CADBARRIO();
                cADBARRIO_copy = (CADBARRIO)auditoria.GenerarCopiaAuditoria(cADBARRIO);

                auditoria.RegistrarAuditoria<CADBARRIO>(cADBARRIO_copy, "CADBARRIO", "CODIGO=" + cADBARRIO.CODIGO.ToString() + ",LUGE_CODIGO=" + cADBARRIO.LUGE_CODIGO, TipoAuditoria.Eliminar);



                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DBConcurrencyException ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = "Error de base de datos" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Libera la clase de memoria
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