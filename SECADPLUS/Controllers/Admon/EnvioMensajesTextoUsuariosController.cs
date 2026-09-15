using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class EnvioMensajesTextoUsuariosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        [Authorize]
        public JsonResult GetCanalesByEntidad(int? id)
        {
            try
            {
                List<CADCANALE> listaCanales = new List<CADCANALE>();

                if (id != null)
                    listaCanales = db.CADCANALES.Where(m => m.CADFUERZ_ID == id && m.CADFUERZ_ID != 0).ToList();

                listaCanales.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS CANALES" });

                var canales = (from r in listaCanales
                               select new
                               {
                                   CODIGO = r.CODIGO,
                                   DESCRIPCION = r.DESCRIPCION
                               }).ToList();

                return Json(canales.OrderBy(c => c.DESCRIPCION));
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        [Authorize]
        public JsonResult GetUsuariosByEntidadCanal(long? idFuerza, int? idCanal)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var usuariosMensajeA = gestor.ConsultarUsuariosMensajeA(idFuerza, idCanal, strConexion);

                var usuarios = (from DataRow dr in usuariosMensajeA.Rows
                                select new
                                {
                                    CODIGO = dr["usuario"],
                                    DESCRIPCION = dr["usuario"] + " - " + dr["NOMBRE"] + " - " + dr["DESC_FUERZA"] + " - " + dr["DESC_SITIO"],
                                    //DESCRIPCION = dr["usuario"] + " - " + dr["NOMBRE"],
                                }).ToList();

                return Json(usuarios);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: EnvioMensajesTextoUsuarios/Create
        public ActionResult Create(int idSitio, long idCons, long idText)
        {
            try
            {
                ViewBag.idSitio = idSitio;
                ViewBag.idCons = idCons;
                ViewBag.idText = idText;

                ViewBag.CADMENTEX_MENS_SITIOGRABA = idSitio;
                ViewBag.CADMENTEX_MENS_CONSECUTIVO = idCons;
                ViewBag.CADMENTEX_CONSECUTIVO = idText;

                List<SelectListItem> entidad = new SelectList(db.CADFUERZAS, "ID", "DESCRIPCION").ToList();
                entidad.Insert(0, (new SelectListItem { Text = "TODAS LAS ENTIDADES", Value = "0" }));
                ViewBag.CADFUER_DESTINO = entidad.OrderBy(c => c.Text);

                List<CADCANALE> listaCanales = new List<CADCANALE>();
                listaCanales.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS CANALES" });
                ViewBag.CADCANA_DESTINO = new SelectList(listaCanales, "CODIGO", "DESCRIPCION");

                ////Revisar esto si es necesario cambiarlo de modo USR_CAD
                //List<SelectListItem> listaUsuarios = new List<SelectListItem>();
                //listaUsuarios.Insert(0, (new SelectListItem { Text = "TODOS LOS USUARIOS", Value = "0" }));
                //listaUsuarios.Insert(1, (new SelectListItem { Text = "USR_CAD", Value = "USR_CAD" }));
                //ViewBag.CADUSUA_DESTINO = listaUsuarios.OrderBy(c => c.Text);

                ////traer usuario autenticado
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                //ViewBag.CADUSUA_DESTINO = usr.USUARIO;

                //Revisar esto si es necesario cambiarlo de modo USR_CAD
                //List<CADCANALE> listaUsuarios = new List<CADCANALE>();
                //listaUsuarios.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS USUARIOS" });
                //listaUsuarios.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS USUARIOS" });
                //ViewBag.CADUSUA_DESTINO = new SelectList(listaUsuarios, "CODIGO", "DESCRIPCION");

                GestorAdministrador gestor = new GestorAdministrador();
                var usuariosMensajeA = gestor.ConsultarUsuariosMensajeA(0, 0, strConexion);

                var usuarios = (from DataRow dr in usuariosMensajeA.Rows
                                select new
                                {
                                    CODIGO = dr["usuario"],
                                    DESCRIPCION = dr["usuario"] + " - " + dr["NOMBRE"] + " - " + dr["DESC_FUERZA"] + " - " + dr["DESC_SITIO"],
                                }).ToList();

                ViewBag.CADUSUA_DESTINO = new SelectList(usuarios, "CODIGO", "DESCRIPCION");

                List<SelectListItem> listaEstados = new List<SelectListItem>();
                listaEstados.Insert(0, (new SelectListItem { Text = "SIN NOTIFICAR", Value = "S" }));
                listaEstados.Insert(1, (new SelectListItem { Text = "NOTIFICADO", Value = "N" }));
                ViewBag.ESTADO = listaEstados;

                //trigger db insert
                ViewBag.FECHA_GRABACION = DateTime.Now;

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: EnvioMensajesTextoUsuarios/Create To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CADMENTEX_MENS_SITIOGRABA,CADMENTEX_MENS_CONSECUTIVO,CADMENTEX_CONSECUTIVO,CADUSUA_DESTINO,ESTADO,FECHA_NOTIFICACION,FECHA_GRABACION,CADFUER_DESTINO,CADCANA_DESTINO")] CADMENSAJES_TEXTO_USUARIOS cADMENSAJES_TEXTO_USUARIOS)
        {
            try
            {
                var idSitio = int.Parse(Request["idSitio"]);
                var idCons = long.Parse(Request["idCons"]);
                var idText = long.Parse(Request["idText"]);

                cADMENSAJES_TEXTO_USUARIOS.FECHA_GRABACION = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.CADMENSAJES_TEXTO_USUARIOS.Add(cADMENSAJES_TEXTO_USUARIOS);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMENSAJES_TEXTO_USUARIOS>(cADMENSAJES_TEXTO_USUARIOS, "CADMENSAJES_TEXTO_USUARIOS", cADMENSAJES_TEXTO_USUARIOS.CADUSUA_DESTINO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "EnvioMensajes", new { idSitio = idSitio, idCons = idCons, idText = idText, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.idSitio = idSitio;
                ViewBag.idCons = idCons;
                ViewBag.idText = idText;

                List<SelectListItem> entidad = new SelectList(db.CADFUERZAS, "ID", "DESCRIPCION").ToList();
                entidad.Insert(0, (new SelectListItem { Text = "TODAS LAS ENTIDADES", Value = "0" }));
                ViewBag.CADFUER_DESTINO = entidad.OrderBy(c => c.Text);

                List<CADCANALE> listaCanales = new List<CADCANALE>();
                listaCanales = db.CADCANALES.Where(m => m.CADFUERZ_ID == cADMENSAJES_TEXTO_USUARIOS.CADFUER_DESTINO && m.CADFUERZ_ID != 0).ToList();
                listaCanales.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS CANALES" });
                ViewBag.CADCANA_DESTINO = new SelectList(listaCanales, "CODIGO", "DESCRIPCION");

                GestorAdministrador gestor = new GestorAdministrador();
                var usuariosMensajeA = gestor.ConsultarUsuariosMensajeA(cADMENSAJES_TEXTO_USUARIOS.CADFUER_DESTINO, cADMENSAJES_TEXTO_USUARIOS.CADCANA_DESTINO, strConexion);

                var usuarios = (from DataRow dr in usuariosMensajeA.Rows
                                select new
                                {
                                    CODIGO = dr["usuario"],
                                    DESCRIPCION = dr["usuario"] + " - " + dr["NOMBRE"] + " - " + dr["DESC_FUERZA"] + " - " + dr["DESC_SITIO"],
                                }).ToList();

                ViewBag.CADUSUA_DESTINO = new SelectList(usuarios, "CODIGO", "DESCRIPCION");

                List<SelectListItem> listaEstados = new List<SelectListItem>();
                listaEstados.Insert(0, (new SelectListItem { Text = "SIN NOTIFICAR", Value = "S" }));
                listaEstados.Insert(1, (new SelectListItem { Text = "NOTIFICADO", Value = "N" }));
                ViewBag.ESTADO = listaEstados;

                ViewBag.FECHA_GRABACION = DateTime.Now;

                return View(cADMENSAJES_TEXTO_USUARIOS);
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