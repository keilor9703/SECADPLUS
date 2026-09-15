using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class ArmasController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        [SecadPlusAutorizador("/Armas/Index")]
        public async Task<ActionResult> Index(int intUnidadFuerza, int intUnidadCodigo, string strMensaje = null)
        {
            try
            {
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
                ViewBag.UnidadFuerza = intUnidadFuerza;
                ViewBag.UnidadCodigo = intUnidadCodigo;
                var unidad = db.CADUNIDADES.Where(c => c.CODIGO == intUnidadCodigo && c.FUERZA == intUnidadFuerza).Select(c => new { SIGLA = c.SIGLA, CODIGOPADRE = c.CADUNDE_CODIGO, DESCRIPCION = c.DESCRIPCION }).FirstOrDefault();
                ViewBag.ESTADOS = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.CALIBRES = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION");
                ViewBag.SIGLA = unidad.SIGLA;
                ViewBag.CODPADRE = unidad.CODIGOPADRE;
                ViewBag.DESCRIPCION = unidad.DESCRIPCION;
                ViewBag.Padres = GestorGeneral.ConsultarPadresUnidad(intUnidadFuerza, intUnidadCodigo);
                var cADARMAS = db.CADARMAS.Where(c => c.CADUNID_CODIGO == intUnidadCodigo && c.CADUNID_FUERZA == intUnidadFuerza);
                return View(await cADARMAS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Armas/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADARMA cADARMA = await db.CADARMAS.FindAsync(id);
            if (cADARMA == null)
            {
                return HttpNotFound();
            }
            return View(cADARMA);
        }

        [SecadPlusAutorizador("/Armas/Crear")]
        public ActionResult Crear(int intUnidadFuerza, int intUnidadCodigo)
        {
            try
            {
                ViewBag.UNIDAD_FUERZA = intUnidadFuerza;
                ViewBag.UNIDAD_CODIGO = intUnidadCodigo;
                ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO"), "CODIGO", "DESCRIPCION");
                ViewBag.CALIBRE = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION");
                return View("Create");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Armas/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "NUME_SERIE,CALIBRE,CADUNID_FUERZA,CADUNID_CODIGO,CADMARC_CODIGO,CADESTAD_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO")] CADARMA cADARMA)
        {
            try
            {
                CADARMA arma = db.CADARMAS.Find(cADARMA.NUME_SERIE);
                if (arma != null)
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.CADARMAS.Add(cADARMA);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { intUnidadFuerza = cADARMA.CADUNID_FUERZA, intUnidadCodigo = cADARMA.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADARMA.NUME_SERIE) });
                    }
                }
                ViewBag.UNIDAD_FUERZA = cADARMA.CADUNID_FUERZA;
                ViewBag.UNIDAD_CODIGO = cADARMA.CADUNID_CODIGO;
                ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADCLBI_TIBI_CODIGO + "-" + cADARMA.CADCLBI_CODIGO);
                ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADMARC_CODIGO);
                ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO"), "CODIGO", "DESCRIPCION", cADARMA.CADESTAD_CODIGO);
                ViewBag.CALIBRE = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION", cADARMA.CALIBRE);
                return View(cADARMA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Armas/Editar")]
        public async Task<ActionResult> Editar(string strSerie)
        {
            try
            {
                if (strSerie == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADARMA cADARMA = await db.CADARMAS.FindAsync(strSerie);
                if (cADARMA == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = "No se ha encontrado el registro seleccionado";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.UNIDAD_FUERZA = cADARMA.CADUNID_FUERZA;
                ViewBag.UNIDAD_CODIGO = cADARMA.CADUNID_CODIGO;
                ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADCLBI_TIBI_CODIGO + "-" + cADARMA.CADCLBI_CODIGO);
                ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADMARC_CODIGO);
                ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO"), "CODIGO", "DESCRIPCION", cADARMA.CADESTAD_CODIGO);
                ViewBag.CALIBRE = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION", cADARMA.CALIBRE);
                return View("Edit", cADARMA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        private void GenerarDatosComunes(CADARMA cADARMA)
        {
            ViewBag.UNIDAD_FUERZA = cADARMA.CADUNID_FUERZA;
            ViewBag.UNIDAD_CODIGO = cADARMA.CADUNID_CODIGO;
            ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
            ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
            ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
            ViewBag.CALIBRE = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION");
        }

        // POST: Armas/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "NUME_SERIE,CALIBRE,CADUNID_FUERZA,CADUNID_CODIGO,CADMARC_CODIGO,CADESTAD_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO")] CADARMA cADARMA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cADARMA).State = EntityState.Modified;
                await db.SaveChangesAsync();

                auditoria.RegistrarAuditoria<CADARMA>(cADARMA, "CADPERSATEND_REMEVENMUJ", "NUME_SERIE=" + cADARMA.NUME_SERIE.ToString(), TipoAuditoria.Modificar);



                return RedirectToAction("Index", new { intUnidadFuerza = cADARMA.CADUNID_FUERZA, intUnidadCodigo = cADARMA.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADARMA.NUME_SERIE) });
            }
            ViewBag.UNIDAD_FUERZA = cADARMA.CADUNID_FUERZA;
            ViewBag.UNIDAD_CODIGO = cADARMA.CADUNID_CODIGO;
            ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 3).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADCLBI_TIBI_CODIGO + "-" + cADARMA.CADCLBI_CODIGO);
            ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADARMA.CADMARC_CODIGO);
            ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADARMA.CADESTAD_CODIGO);
            ViewBag.CALIBRE = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CALIBRE"), "CODIGO", "DESCRIPCION", cADARMA.CALIBRE);
            return View(cADARMA);
        }

        // GET: Patrullas/Delete/5
        [SecadPlusAutorizador("/Armas/Eliminar")]
        public async Task<ActionResult> Eliminar(string strSerie)
        {
            try
            {
                CADARMA cADARMA = await db.CADARMAS.FindAsync(strSerie);
                if (cADARMA == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADARMA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                GestorBase.EX_CODIGO = "";
                GestorBase.EX_CODIGO_INTERNO = "";
                return View("Delete", null, null);
            }
        }

        // POST: Armas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int intUnidadFuerza, int intUnidadCodigo, string strSerie)
        {
            try
            {
                CADARMA cADARMA = await db.CADARMAS.FindAsync(strSerie);
                db.CADARMAS.Remove(cADARMA);
                await db.SaveChangesAsync();

                CADARMA cADARMA_copy = new CADARMA();
                cADARMA_copy = (CADARMA)auditoria.GenerarCopiaAuditoria(cADARMA_copy);

                auditoria.RegistrarAuditoria<CADARMA>(cADARMA_copy, "CADPERSATEND_REMEVENMUJ", "NUME_SERIE=" + cADARMA.NUME_SERIE.ToString(), TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { intUnidadFuerza = intUnidadFuerza, intUnidadCodigo = intUnidadCodigo, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                GestorBase.GestionarExcepcion(dbEx, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strSerie);
                return RedirectToAction("Index", new { intUnidadFuerza = intUnidadFuerza, intUnidadCodigo = intUnidadCodigo });
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