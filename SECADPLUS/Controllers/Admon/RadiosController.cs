using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class RadiosController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        [SecadPlusAutorizador("/Radios/Index")]
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
                var cADRADIOS = db.CADRADIOS.Where(c => c.CADUNID_CODIGO == intUnidadCodigo && c.CADUNID_FUERZA == intUnidadFuerza).Include(c => c.CADCLASES_BIEN).Include(c => c.CADMARCA).Include(c => c.CADUNIDADE);
                ViewBag.SIGLA = unidad.SIGLA;
                ViewBag.CODPADRE = unidad.CODIGOPADRE;
                ViewBag.DESCRIPCION = unidad.DESCRIPCION;
                ViewBag.ESTADOS = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                ViewBag.Padres = GestorGeneral.ConsultarPadresUnidad(intUnidadFuerza, intUnidadCodigo);
                return View(await cADRADIOS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Radios/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADRADIO cADRADIO = await db.CADRADIOS.FindAsync(id);
            if (cADRADIO == null)
            {
                return HttpNotFound();
            }
            return View(cADRADIO);
        }

        [SecadPlusAutorizador("/Radios/Crear")]
        public ActionResult Crear(int intUnidadFuerza, int intUnidadCodigo)
        {
            try
            {
                ViewBag.UNIDAD_FUERZA = intUnidadFuerza;
                ViewBag.UNIDAD_CODIGO = intUnidadCodigo;
                ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION");
                return View("Create");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Radios/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "NUME_SERIE,DESC_DESCRIPTIVO,CADPOLI_CEDU_EMPLEADO,CADUNID_FUERZA,CADUNID_CODIGO,CADESTAD_CODIGO,CADMARC_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,CADMODELO_RADIO")] CADRADIO cADRADIO)
        {
            try
            {
                CADRADIO radio = db.CADRADIOS.Find(cADRADIO.NUME_SERIE);
                if (radio != null)
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.CADRADIOS.Add(cADRADIO);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { intUnidadFuerza = cADRADIO.CADUNID_FUERZA, intUnidadCodigo = cADRADIO.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADRADIO.NUME_SERIE) });
                    }
                }
                GenerarElementosEdicion(cADRADIO);
                return View(cADRADIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        private void GenerarElementosEdicion(CADRADIO cADRADIO)
        {
            ViewBag.UNIDAD_FUERZA = cADRADIO.CADUNID_FUERZA;
            ViewBag.UNIDAD_CODIGO = cADRADIO.CADUNID_CODIGO;
            ViewBag.CLASES = new SelectList(db.CADCLASES_BIEN.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.TIBI_CODIGO + "-" + c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADRADIO.CADCLBI_TIBI_CODIGO + "-" + cADRADIO.CADCLBI_CODIGO);
            ViewBag.CADMARC_CODIGO = new SelectList(db.CADMARCAS.Where(c => c.TIBI_CODIGO == 12).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(c => c.DESCRIPCION), "CODIGO", "DESCRIPCION", cADRADIO.CADMARC_CODIGO);
            ViewBag.CADESTAD_CODIGO = new SelectList(db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "ESTADO").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADRADIO.CADESTAD_CODIGO);
        }

        // GET: Radios/Edit/5
        [SecadPlusAutorizador("/Radios/Editar")]
        public async Task<ActionResult> Editar(string strSerie)
        {
            try
            {
                if (string.IsNullOrEmpty(strSerie))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADRADIO cADRADIO = await db.CADRADIOS.FindAsync(strSerie);
                if (cADRADIO == null)
                {
                    return HttpNotFound();
                }
                GenerarElementosEdicion(cADRADIO);
                return View("Edit", cADRADIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Radios/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Edit([Bind(Include = "NUME_SERIE,DESC_DESCRIPTIVO,CADPOLI_CEDU_EMPLEADO,CADUNID_FUERZA,CADUNID_CODIGO,CADESTAD_CODIGO,CADMARC_CODIGO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,CADMODELO_RADIO")] CADRADIO cADRADIO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADRADIO).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADRADIO>(cADRADIO, "CADRADIO", "NUME_SERIE=" + cADRADIO.NUME_SERIE.ToString(), TipoAuditoria.Modificar);


                    return RedirectToAction("Index", new { intUnidadFuerza = cADRADIO.CADUNID_FUERZA, intUnidadCodigo = cADRADIO.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADRADIO.NUME_SERIE) });
                }
                GenerarElementosEdicion(cADRADIO);
                return View(cADRADIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Radios/Eliminar")]
        public async Task<ActionResult> Eliminar(string strSerie)
        {
            try
            {
                if (string.IsNullOrEmpty(strSerie.Trim()))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADRADIO cADRADIO = await db.CADRADIOS.FindAsync(strSerie.Trim());
                if (cADRADIO == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", cADRADIO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Radios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int intUnidadFuerza, int intUnidadCodigo, string strSerie)
        {
            try
            {
                CADRADIO cADRADIO = await db.CADRADIOS.FindAsync(strSerie);
                db.CADRADIOS.Remove(cADRADIO);
                await db.SaveChangesAsync();



                CADRADIO cADRADIO_copy = new CADRADIO();
                cADRADIO_copy = (CADRADIO)auditoria.GenerarCopiaAuditoria(cADRADIO);

                auditoria.RegistrarAuditoria<CADRADIO>(cADRADIO_copy, "CADRADIO", "NUME_SERIE=" + cADRADIO.NUME_SERIE.ToString(), TipoAuditoria.Eliminar);



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

        /// <summary>
        /// Permite consultar los datos de un empleado asociado a una cédula ingresada por el usuario
        /// </summary>
        /// <param name="strCedula">Número de cédula ingresado por el usuario</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ConsultarDatoPersona(string strCedula)
        {
            string strMensaje = "";
            try
            {
                GestorGeneral gestor = new GestorGeneral();
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cedula", strCedula, typeof(string)));
                string strValor = gestor.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultar_dato_persona", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    strMensaje = GestorBase.MSN_ALERTA_COMUN;
                    GestorBase.MSN_ALERTA_COMUN = "";
                    return Json(new { result = false, datos = strMensaje });
                }
                else
                {
                    return Json(new { result = true, datos = strValor });
                }
            }
            catch (OracleException oex)
            {
                if (oex.Number == 1403)
                {
                    strMensaje = "No se encontró un empleado asociado a la cédula ingresada.";
                    GestorBase.MSN_ALERTA_COMUN = "";
                    return Json(new { result = false, datos = strMensaje });
                }
                strMensaje = oex.Message;
                return Json(new { result = false, datos = strMensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
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