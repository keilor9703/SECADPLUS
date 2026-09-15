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
    public class ComunasController : Controller
    {
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        public int codFather { get; private set; }
        public int codLevel { get; private set; }

        // GET: CADCOMUNAs
        public async Task<ActionResult> Index()
        {
            try
            {
                return View(await db.CADCOMUNAS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADCOMUNAs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADCOMUNA cADCOMUNA = await db.CADCOMUNAS.FindAsync(id);
            if (cADCOMUNA == null)
            {
                return HttpNotFound();
            }
            return View(cADCOMUNA);
        }

        // GET: CADLUGARES_GEOGRAFICOS/Create
        public ActionResult Create(int idFather, int? level)
        {
            try
            {
                codLevel = 6;
                //Lleno el drop downlist del luge_codigo
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == idFather), "CODIGO", "DESCRIPCION");
                ViewBag.NivelHijo = codLevel;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADLUGARES_GEOGRAFICOS/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/Create")]
        public async Task<ActionResult> Create([Bind(Include = "LUGE_CODIGO,CODIGO,DESCRIPCION,VIGENTE,ORDEN,IDENTIFICACION,AREAM2")] CADCOMUNA cADCOMUNA)
        {
            try
            {
                cADCOMUNA.VIGENTE = cADCOMUNA.VIGENTE == "true" ? "S" : "N";
                ModelState.Clear();
                TryValidateModel(cADCOMUNA);
                if (ModelState.IsValid)
                {
                    if (db.CADCOMUNAS.Find(cADCOMUNA.LUGE_CODIGO, cADCOMUNA.CODIGO) == null)
                    {
                        db.CADCOMUNAS.Add(cADCOMUNA);
                        await db.SaveChangesAsync();
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                        ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADCOMUNA.LUGE_CODIGO), "CODIGO", "DESCRIPCION");
                        return View();
                    }
                    else
                    {
                        ViewBag.MENSAJE_EXITOSO = "Ya existe una comuna con el código indicado";
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                }

                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADCOMUNA.LUGE_CODIGO), "CODIGO", "DESCRIPCION");
                ViewBag.NivelHijo = codLevel;
                ViewBag.VIGENTE = cADCOMUNA.VIGENTE == "S" ? true : false;
                return View(cADCOMUNA);
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

        // GET: CADCOMUNAs/Edit/5
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public async Task<ActionResult> Edit(int id, int idPadre)
        {
            try
            {
                CADCOMUNA cADCOMUNA = db.CADCOMUNAS.Where(t => t.CODIGO == id && t.LUGE_CODIGO == idPadre).FirstOrDefault();
                if (cADCOMUNA == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("Index", "Home");
                }
                CADLUGARES_GEOGRAFICOS cADLUGARES_GEOGRAFICOS = await db.CADLUGARES_GEOGRAFICOS.FindAsync(idPadre);
                if (cADCOMUNA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADLUGARES_GEOGRAFICOS.CODIGO),
                    "CODIGO", "DESCRIPCION", cADLUGARES_GEOGRAFICOS.CODIGO);
                ViewBag.VIGENTE = cADCOMUNA.VIGENTE == "S" ? true : false;

                return View(cADCOMUNA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADCOMUNAs/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/LugaresGeograficos/Edit")]
        public async Task<ActionResult> Edit(int idPadre, [Bind(Include = "LUGE_CODIGO,CODIGO,DESCRIPCION,VIGENTE,ORDEN,IDENTIFICACION,AREAM2")] CADCOMUNA cADCOMUNA)
        {
            try
            {
                ViewBag.VIGENTE = cADCOMUNA.VIGENTE == "S" ? true : false;
                cADCOMUNA.VIGENTE = cADCOMUNA.VIGENTE == "true" ? "S" : "N";
                ModelState.Clear();
                TryValidateModel(cADCOMUNA);
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(cADCOMUNA).State = EntityState.Modified;
                        await db.SaveChangesAsync();


                        auditoria.RegistrarAuditoria<CADCOMUNA>(cADCOMUNA, "CADCOMUNA", "LUGE_CODIGO=" + cADCOMUNA.LUGE_CODIGO.ToString() + ",CODIGO=" + cADCOMUNA.CODIGO
                       , TipoAuditoria.Modificar);



                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.Guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                        ViewBag.VIGENTE = cADCOMUNA.VIGENTE == "S" ? true : false;
                    }
                    catch (DBConcurrencyException ex)
                    {
                        var mensaje = ex.Message;
                        ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.No_guardo;
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                    }
                }

                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION", idPadre);
                return View(cADCOMUNA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADCOMUNAs/Delete/5
        [SecadPlusAutorizador("/LugaresGeograficos/Delete")]
        public async Task<ActionResult> Delete(int? id, int? idPadre)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCOMUNA cADCOMUNA = await db.CADCOMUNAS.FindAsync(idPadre, id);
                if (cADCOMUNA == null)
                {
                    return HttpNotFound();
                }
                return View(cADCOMUNA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADCOMUNAs/Delete/5
        [Authorize]
        public JsonResult DeleteConfirmed(int id, int? idPadre)
        {
            try
            {
                CADCOMUNA cADCOMUNA = db.CADCOMUNAS.Find(idPadre, id);
                var barrios = db.CADBARRIOS.Where(t => t.COZO_CODIGO == id);
                if (barrios.Count() > 0) return Json(new { result = false, datos = "No puede borrar la comuna mientras tenga barrios relacionados" });
                db.CADCOMUNAS.Remove(cADCOMUNA);
                db.SaveChanges();



                CADCOMUNA cADCOMUNA_copy = new CADCOMUNA();
                cADCOMUNA_copy = (CADCOMUNA)auditoria.GenerarCopiaAuditoria(cADCOMUNA);

                auditoria.RegistrarAuditoria<CADCOMUNA>(cADCOMUNA_copy, "CADCOMUNA", "LUGE_CODIGO=" + cADCOMUNA.LUGE_CODIGO.ToString() + ",CODIGO=" + cADCOMUNA.CODIGO
                , TipoAuditoria.Eliminar);




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