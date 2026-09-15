using SecadDatos.Areas.MinutaSivicc;
using SecadDatos.Areas.Patrullas;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SecadModelos.Areas.Patrullas;
using SECADPLUS.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class PatrullasController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();



        [SecadPlusAutorizador("/Patrullas/Index")]
        public ActionResult Index(int intUnidadFuerza, int intUnidadCodigo, string strMensaje = null)
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
                DbPatrulla objDbPatrulla = new DbPatrulla();
                var cADPATRULLAS = objDbPatrulla.F_GetConsCuadrante(intUnidadFuerza, intUnidadCodigo);
                //var cADPATRULLAS = db.CADPATRULLAS.Include(c => c.CADUNIDADE).Where(c => c.CADUNID_CODIGO == intUnidadCodigo && c.CADUNID_FUERZA == intUnidadFuerza);
                var claseTurno = new SelectList((db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CLASE_TURNO")), "CODIGO", "DESCRIPCION");
                var unidad = db.CADUNIDADES.Where(c => c.CODIGO == intUnidadCodigo && c.FUERZA == intUnidadFuerza).Select(c => new { SIGLA = c.SIGLA, CODIGOPADRE = c.CADUNDE_CODIGO, DESCRIPCION = c.DESCRIPCION }).FirstOrDefault();
                ViewBag.UnidadFuerza = intUnidadFuerza;
                ViewBag.UnidadCodigo = intUnidadCodigo;
                ViewBag.LISTA_TURNO = claseTurno;
                ViewBag.SIGLA = unidad.SIGLA;
                ViewBag.CODPADRE = unidad.CODIGOPADRE;
                ViewBag.DESCRIPCION = unidad.DESCRIPCION;
                ViewBag.Padres = GestorGeneral.ConsultarPadresUnidad(intUnidadFuerza, intUnidadCodigo);
                //return View(await cADPATRULLAS.ToListAsync());
                return View(cADPATRULLAS.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Patrullas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADPATRULLA cADPATRULLA = await db.CADPATRULLAS.FindAsync(id);
            if (cADPATRULLA == null)
            {
                return HttpNotFound();
            }
            return View(cADPATRULLA);
        }

        [SecadPlusAutorizador("/Patrullas/Crear")]
        public ActionResult Crear(int intUnidadFuerza, int intUnidadCodigo)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.UNIDAD_FUERZA = intUnidadFuerza;
                ViewBag.UNIDAD_CODIGO = intUnidadCodigo;
                ViewBag.USUARIO_NOMBRE = usuarioActual.USUARIO;
                ViewBag.USUARIO_SITIO = usuarioActual.SITIO_GRABACION;
                ViewBag.CLASE_TURNO = new SelectList((db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CLASE_TURNO")), "CODIGO", "DESCRIPCION");
                return View("Create");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Patrullas/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CADUNID_FUERZA,CADUNID_CODIGO,DESC_PATRULLA,CLASE_TURNO,VIGENTE,FECHA_DESDE,FECHA_HASTA,OBSERVACION,USUA_TRANSACCION,SITIO_GRABA,ID_RADIO")] CADPATRULLA cADPATRULLA)
        {
            try
            {
                int intCantidad = 0;
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                intCantidad = (from registro in db.CADPATRULLAS
                               where registro.DESC_PATRULLA == cADPATRULLA.DESC_PATRULLA.ToUpper().TrimStart().Trim().TrimEnd()
                               && registro.CADUNID_CODIGO == cADPATRULLA.CADUNID_CODIGO
                               && registro.CADUNID_FUERZA == cADPATRULLA.CADUNID_FUERZA
                               select registro).Count();

                if (intCantidad > 0)
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_REGISTRO_DUPLICADO;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        cADPATRULLA.DESC_PATRULLA = cADPATRULLA.DESC_PATRULLA.ToUpper().TrimStart().Trim().TrimEnd();
                        db.CADPATRULLAS.Add(cADPATRULLA);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { intUnidadFuerza = cADPATRULLA.CADUNID_FUERZA, intUnidadCodigo = cADPATRULLA.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADPATRULLA.DESC_PATRULLA) });
                    }
                }

                GenerarDatosComunes(cADPATRULLA);
                return View(cADPATRULLA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        private void GenerarDatosComunes(CADPATRULLA cADPATRULLA)
        {
            try
            {
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.UNIDAD_FUERZA = cADPATRULLA.CADUNID_FUERZA;
                ViewBag.UNIDAD_CODIGO = cADPATRULLA.CADUNID_CODIGO;
                ViewBag.USUARIO_NOMBRE = usuarioActual.USUARIO;
                ViewBag.USUARIO_SITIO = usuarioActual.SITIO_GRABACION;
                ViewBag.CLASE_TURNO = new SelectList((db.CADREFERENCIASSECADs.Where(c => c.NOMBRE == "CLASE_TURNO")), "CODIGO", "DESCRIPCION");
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        [SecadPlusAutorizador("/Patrullas/Editar")]
        public ActionResult Editar(int intUnidadFuerza, int intUnidadCodigo, int intCodigo)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.UNIDAD_FUERZA = intUnidadFuerza;
                ViewBag.UNIDAD_CODIGO = intUnidadCodigo;
                ViewBag.USUARIO_NOMBRE = usuarioActual.USUARIO;
                ViewBag.USUARIO_SITIO = usuarioActual.SITIO_GRABACION;


                DbPatrulla objDbPatrulla = new DbPatrulla();
                var cADPATRULLA = objDbPatrulla.F_GetCuadranteId(intUnidadFuerza, intUnidadCodigo, intCodigo);

                if (cADPATRULLA == null)
                {
                    throw new HttpException();
                }

                ViewBag.IdClaseTurno = new SelectList(new DbMinuta().F_GetClaseTurno().OrderBy(t => t.Descripcion).ToList(), "ReferenciaId", "Descripcion", cADPATRULLA.IdClaseTurno);
                ViewBag.CuadranteId = new SelectList(new DbMinuta().F_GetCuadrantes().OrderBy(c => c.CodigoCuadrante).ToList(), "CuadranteId", "CodigoCuadrante", cADPATRULLA.CuadranteId);

                //GenerarDatosComunes(cADPATRULLA);
                return View(cADPATRULLA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Patrullas/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        public ActionResult Editar(MdCadPatrulla mdCadPatrulla)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.UNIDAD_FUERZA = mdCadPatrulla.CadUniFuerza;
                ViewBag.UNIDAD_CODIGO = mdCadPatrulla.CadUniCodigo;
                ViewBag.Codigo = mdCadPatrulla.Codigo;
                ViewBag.DESCPATRULLA = mdCadPatrulla.DescPatrulla;
                ViewBag.ClaseTurno = mdCadPatrulla.ClaseTurno;
                ViewBag.USUARIO_SITIO = usuarioActual.SITIO_GRABACION;

                DbPatrulla objDbPatrulla = new DbPatrulla();
                //var cADPATRULLA = objDbPatrulla.UpdateCuadrante(mdCadPatrulla);

                Udp_Patrullas(mdCadPatrulla);


                //    auditoria.RegistrarAuditoria<CADPATRULLA>(cADPATRULLA, "CADPATRULLA", "CADUNID_FUERZA=" + mdCadPatrulla.CadUniFuerza.ToString() + ",CADUNID_CODIGO=" + mdCadPatrulla.CadUniCodigo
                //+ ",CODIGO=" + mdCadPatrulla.Codigo, TipoAuditoria.Modificar);
                return RedirectToAction("Index", new { intUnidadFuerza = mdCadPatrulla.CadUniFuerza, intUnidadCodigo = mdCadPatrulla.CadUniCodigo, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", mdCadPatrulla.DescPatrulla) });
                //return Json(new { success = true, mensaje="El registro se ha modificado coreectamente" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Patrullas/Eliminar")]
        public async Task<ActionResult> Eliminar(int intUnidadFuerza, int intUnidadCodigo, int intCodigo)
        {
            try
            {
                CADPATRULLA cADPATRULLA = await db.CADPATRULLAS.FindAsync(intUnidadFuerza, intUnidadCodigo, intCodigo);
                if (cADPATRULLA == null)
                {
                    ViewBag.MENSAJE_EXITOSO = "El registro consultado fue encontrado.";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    return View("Delete", null, null);
                }
                return View("Delete", null, cADPATRULLA);
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

        // POST: Patrullas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DeleteConfirmed(int intUnidadFuerza, int intUnidadCodigo, int intCodigo)
        {
            try
            {
                CADPATRULLA cADPATRULLA = await db.CADPATRULLAS.FindAsync(intUnidadFuerza, intUnidadCodigo, intCodigo);
                db.CADPATRULLAS.Remove(cADPATRULLA);
                await db.SaveChangesAsync();



                CADPATRULLA cADPATRULLA_copy = new CADPATRULLA();
                cADPATRULLA_copy = (CADPATRULLA)auditoria.GenerarCopiaAuditoria(cADPATRULLA);


                auditoria.RegistrarAuditoria<CADPATRULLA>(cADPATRULLA_copy, "CADPATRULLA", "CADUNID_FUERZA=" + cADPATRULLA.CADUNID_FUERZA.ToString() + ",CADUNID_CODIGO=" + cADPATRULLA.CADUNID_CODIGO
                + ",CODIGO=" + cADPATRULLA.CODIGO, TipoAuditoria.Modificar);




                return RedirectToAction("Index", new { intUnidadFuerza = cADPATRULLA.CADUNID_FUERZA, intUnidadCodigo = cADPATRULLA.CADUNID_CODIGO, strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (DbUpdateException dbEx)
            {
                GestorBase.GestionarExcepcion(dbEx, GetType().Name);
                GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Registro seleccionado");
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

        public JsonResult Udp_Patrullas(MdCadPatrulla obj)
        {

            //Validación
            if (obj.Codigo == 0)
            {
                return Json(new { success = false, message = "Debe seleccionar código" }, JsonRequestBehavior.AllowGet);
            }

            if (obj.ClaseTurno == null)
            {
                return Json(new { success = false, message = "Debe seleccionar un turno" }, JsonRequestBehavior.AllowGet);
            }

            var Result = new DbPatrulla().UpdateCuadrante(obj);

            if (Result > 0)
            {
                return Json(new { success = true, message = "El registro se grabó exitosamente" }, JsonRequestBehavior.AllowGet);
            }
            else if (Result == -1)
            {
                return Json(new { success = true, message = "El registro se actualizó exitosamente" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No es posible grabar, revise ......." }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult PruebaGraba(string vriable)
        {

            //Validación



            var Result = 1;

            if (Result > 0)
            {
                return Json(new { success = true, message = "El registro se grabó exitosamente vvv" }, JsonRequestBehavior.AllowGet);
            }
            else if (Result == -1)
            {
                return Json(new { success = true, message = "El registro se actualizó exitosamente" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No es posible grabar, revise ......." }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}