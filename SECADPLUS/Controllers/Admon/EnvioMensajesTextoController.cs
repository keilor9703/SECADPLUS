using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class EnvioMensajesTextoController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: EnvioMensajesTexto/Create
        public ActionResult Create(int idSitio, long idCons)
        {
            //ViewBag.CADMENS_SITIOGRABA = new SelectList(db.CADMENSAJES, "SITIO_GRABA", "CADUSUA_INICIA");
            ViewBag.idSitio = idSitio;
            ViewBag.idCons = idCons;

            ViewBag.CADMENS_SITIOGRABA = idSitio;
            ViewBag.CADMENS_CONSECUTIVO = idCons;
            // El consecutivo se genera en la fase de guardar
            ViewBag.CONSECUTIVO = 0;
            // El usuario por medio de autenticación //  User.Identity.GetUserName()
            //ViewBag.CADUSUA_GRABA = User.Identity.GetUserName();
            //ViewBag.CADUSUA_GRABA = "USR_CAD";

            //traer usuario autenticado
            CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
            ViewBag.CADUSUA_GRABA = usr.USUARIO;

            return View();
        }

        // POST: EnvioMensajesTexto/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CADMENS_SITIOGRABA,CADMENS_CONSECUTIVO,CONSECUTIVO,CADUSUA_GRABA,TEXTO")] CADMENSAJES_TEXTO cADMENSAJES_TEXTO)
        {
            try
            {
                var idSitio = int.Parse(Request["idSitio"]);
                var idCons = long.Parse(Request["idCons"]);

                if (string.IsNullOrWhiteSpace(Request["CONSECUTIVO1"]))
                {
                    long CODIGO;
                    var existe = db.CADMENSAJES_TEXTO.Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == idCons).FirstOrDefault();

                    if (existe == null)
                        CODIGO = 1;
                    else
                    {
                        CODIGO = db.CADMENSAJES_TEXTO.Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == idCons).Select(c => c.CONSECUTIVO).ToList().Max(c => c);
                        CODIGO += 1;
                    }

                    cADMENSAJES_TEXTO.CONSECUTIVO = CODIGO;
                }
                else
                {
                    cADMENSAJES_TEXTO.CONSECUTIVO = long.Parse(Request["CONSECUTIVO1"]);
                }

                if (ModelState.IsValid)
                {
                    db.CADMENSAJES_TEXTO.Add(cADMENSAJES_TEXTO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMENSAJES_TEXTO>(cADMENSAJES_TEXTO, "CADMENSAJES_TEXTO", cADMENSAJES_TEXTO.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADMENSAJES_TEXTO.CONSECUTIVO.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "EnvioMensajes", new { idSitio = idSitio, idCons = idCons, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                //ViewBag.CADMENS_SITIOGRABA = new SelectList(db.CADMENSAJES, "SITIO_GRABA", "CADUSUA_INICIA", cADMENSAJES_TEXTO.CADMENS_SITIOGRABA);
                ViewBag.idSitio = idSitio;
                ViewBag.idCons = idCons;

                ViewBag.CADMENS_SITIOGRABA = idSitio;
                ViewBag.CADMENS_CONSECUTIVO = idCons;
                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;
                ViewBag.CONSECUTIVO1 = Request["CONSECUTIVO1"];
                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.CADUSUA_GRABA = User.Identity.GetUserName();
                //ViewBag.CADUSUA_GRABA = "USR_CAD";

                //traer usuario autenticado
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                ViewBag.CADUSUA_GRABA = cADMENSAJES_TEXTO.CADUSUA_GRABA;

                return View(cADMENSAJES_TEXTO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", "EnvioMensajes", new { strMensaje = mensaje, strTipo = tipo });
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