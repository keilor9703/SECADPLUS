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
    public class VehiculosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: Vehiculos
        [SecadPlusAutorizador("/Vehiculos/Index")]
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
                ViewBag.SIGLA = unidad.SIGLA;
                ViewBag.CODPADRE = unidad.CODIGOPADRE;
                ViewBag.DESCRIPCION = unidad.DESCRIPCION;
                ViewBag.Padres = GestorGeneral.ConsultarPadresUnidad(intUnidadFuerza, intUnidadCodigo);
                var cADVEHICULOS = db.CADVEHICULOS.Where(c => c.CADUNID_CODIGO == intUnidadCodigo && c.CADUNID_FUERZA == intUnidadFuerza).Include(c => c.CADCLASES_BIEN).Include(c => c.CADMARCA).Include(c => c.CADUNIDADE);
                return View(await cADVEHICULOS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Vehiculos/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADVEHICULO cADVEHICULO = await db.CADVEHICULOS.FindAsync(id);
            if (cADVEHICULO == null)
            {
                return HttpNotFound();
            }
            return View(cADVEHICULO);
        }

        // GET: Armas/Create
        [SecadPlusAutorizador("/Vehiculos/Crear")]
        public ActionResult Crear(int intUnidadFuerza, int intUnidadCodigo)
        {
            try
            {
                ViewBag.UNIDAD_FUERZA = intUnidadFuerza;
                ViewBag.UNIDAD_CODIGO = intUnidadCodigo;
                ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 4).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 4).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                return View("Create");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Vehiculos/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "NUME_SIGLA,NUME_PLACA,CADUNID_FUERZA,CADUNID_CODIGO,CADMARC_CODIGO,CADESTAD_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO")] CADVEHICULO cADVEHICULO)
        {
            try
            {
                CADVEHICULO vehiculo = db.CADVEHICULOS.Find(cADVEHICULO.NUME_SIGLA);
                if (vehiculo != null)
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.CADVEHICULOS.Add(cADVEHICULO);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { intUnidadFuerza = cADVEHICULO.CADUNID_FUERZA, intUnidadCodigo = cADVEHICULO.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADVEHICULO.NUME_SIGLA) });
                    }
                }
                GenerarElementosEdicion(cADVEHICULO);
                return View(cADVEHICULO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite generar elemenos comunes para las diferentes funcionalides delcontrolador
        /// </summary>
        /// <param name="cADVEHICULO"></param>
        private void GenerarElementosEdicion(CADVEHICULO cADVEHICULO)
        {
            ViewBag.UNIDAD_FUERZA = cADVEHICULO.CADUNID_FUERZA;
            ViewBag.UNIDAD_CODIGO = cADVEHICULO.CADUNID_CODIGO;
            ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 4).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADVEHICULO.CADCLBI_TIBI_CODIGO + "-" + cADVEHICULO.CADCLBI_CODIGO);
            ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 4).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADVEHICULO.CADMARC_CODIGO);
            ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADVEHICULO.CADESTAD_CODIGO);
        }

        // GET: Vehiculos/Edit/5
        [SecadPlusAutorizador("/Vehiculos/Editar")]
        public async Task<ActionResult> Editar(string strSigla)
        {
            try
            {
                if (strSigla == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADVEHICULO cADVEHICULO = await db.CADVEHICULOS.FindAsync(strSigla);
                if (cADVEHICULO == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = "No se ha encontrado el registro seleccionado";
                    return RedirectToAction("Index", "Home");
                }
                GenerarElementosEdicion(cADVEHICULO);
                return View("Edit", cADVEHICULO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Vehiculos/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "NUME_SIGLA,NUME_PLACA,CADUNID_FUERZA,CADUNID_CODIGO,CADMARC_CODIGO,CADESTAD_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO")] CADVEHICULO cADVEHICULO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADVEHICULO).State = EntityState.Modified;
                    await db.SaveChangesAsync();


                    auditoria.RegistrarAuditoria<CADVEHICULO>(cADVEHICULO, "CADVEHICULO", "NUME_SIGLA=" + cADVEHICULO.NUME_SIGLA.ToString() + ",NUME_PLACA=" + cADVEHICULO.NUME_PLACA
                      , TipoAuditoria.Modificar);

                    return RedirectToAction("Index", new { intUnidadFuerza = cADVEHICULO.CADUNID_FUERZA, intUnidadCodigo = cADVEHICULO.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADVEHICULO.NUME_SIGLA) });
                }
                GenerarElementosEdicion(cADVEHICULO);
                return View(cADVEHICULO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Vehiculos/Eliminar")]
        public async Task<ActionResult> Eliminar(string strSigla)
        {
            try
            {
                CADVEHICULO cADVEHICULO = await db.CADVEHICULOS.FindAsync(strSigla);
                if (cADVEHICULO == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADVEHICULO);
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
        public async Task<ActionResult> DeleteConfirmed(int intUnidadFuerza, int intUnidadCodigo, string strSigla)
        {
            try
            {
                CADVEHICULO cADVEHICULO = await db.CADVEHICULOS.FindAsync(strSigla);
                db.CADVEHICULOS.Remove(cADVEHICULO);
                await db.SaveChangesAsync();


                CADVEHICULO cADVEHICULO_copy = new CADVEHICULO();
                cADVEHICULO_copy = (CADVEHICULO)auditoria.GenerarCopiaAuditoria(cADVEHICULO);

                auditoria.RegistrarAuditoria<CADVEHICULO>(cADVEHICULO_copy, "CADVEHICULO", "NUME_SIGLA=" + cADVEHICULO.NUME_SIGLA.ToString() + ",NUME_PLACA=" + cADVEHICULO.NUME_PLACA
                  , TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { intUnidadFuerza = intUnidadFuerza, intUnidadCodigo = intUnidadCodigo, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                GestorBase.GestionarExcepcion(dbEx, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strSigla);
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