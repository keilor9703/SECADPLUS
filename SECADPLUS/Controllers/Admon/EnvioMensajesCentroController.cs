using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class EnvioMensajesCentroController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        [Authorize]
        public JsonResult EnviarMensaje(int idSitio, long idCons)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var valido = gestor.EjecutarEnviarMensaje(idSitio, idCons, strConexion);

                return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: EnvioMensajesCentro/Create
        public ActionResult Create()
        {
            try
            {
                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", idSitio);

                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;
                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.CADUSUA_INICIA = User.Identity.GetUserName();

                //traer usuario autenticado
                ViewBag.CADUSUA_INICIA = usuario;

                //ViewBag.CADUSUA_INICIA = "USR_CAD";

                ViewBag.FECHA = DateTime.Now;
                ViewBag.ESTADO = "Sin Enviar";

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EnvioMensajesCentro/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADUSUA_INICIA,FECHA,TIPO_ASOCIADO,NUME_ASOCIADO,ESTADO")] CADMENSAJE cADMENSAJE)
        {
            try
            {
                cADMENSAJE.ESTADO = "S";

                GestorAdministrador gestor = new GestorAdministrador();
                var CONSECUTIVO = gestor.ConsultarConsecutivoMensaje(strConexion);
                cADMENSAJE.CONSECUTIVO = CONSECUTIVO;

                cADMENSAJE.FECHA = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.CADMENSAJES.Add(cADMENSAJE);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMENSAJE>(cADMENSAJE, "CADMENSAJES", cADMENSAJE.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADMENSAJE.CONSECUTIVO.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "EnvioMensajes", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", cADMENSAJE.SITIO_GRABA);
                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = cADMENSAJE.SITIO_GRABA;
                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.CADUSUA_INICIA = User.Identity.GetUserName();
                //ViewBag.CADUSUA_INICIA = "USR_CAD";

                //traer usuario autenticado
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.CADUSUA_INICIA = cADMENSAJE.CADUSUA_INICIA;

                ViewBag.FECHA = DateTime.Now;
                ViewBag.ESTADO = "Sin Enviar";

                return View(cADMENSAJE);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
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