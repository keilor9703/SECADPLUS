using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    public class EnvioMensajesController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: EnvioMensajes
        [SecadPlusAutorizador("/EnvioMensajes/Index")]
        public ActionResult Index(int? idSitio, long? idCons, long? idText, string strMensaje, string strTipo)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                var mensaje = "";
                var tipo = "";

                if (string.IsNullOrEmpty(strMensaje))
                    mensaje = string.Empty;
                else
                    mensaje = strMensaje;

                if (string.IsNullOrEmpty(strMensaje))
                    tipo = string.Empty;
                else
                    tipo = strTipo;

                ViewBag.MENSAJE_EXITOSO = strMensaje;
                ViewBag.TIPO_MENSAJE = strTipo;

                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;

                if (idSitio == null)
                {
                    //idSitio = 0;
                    idSitio = usr.SITIO_GRABACION;
                    // El sitio de grabación por medio de autenticación  //Validar que el usuario tenga un sitio de grabación para realizar todo.
                    //idSitio = 3947;
                }
                else
                {
                    ViewBag.CADMENS_SITIOGRABA = idSitio;
                }

                if (idCons == null)
                    idCons = 0;
                else
                    ViewBag.CADMENS_CONSECUTIVO = idCons;

                if (idText == null)
                    idText = 0;
                else
                    ViewBag.CONSECUTIVO = idText;

                //var cADMENSAJES = db.CADMENSAJES.Include(c => c.CADSITIOS_GRABACION).Where(c => c.SITIO_GRABA == idSitio).OrderByDescending(c => c.CONSECUTIVO);
                var cADMENSAJES_TEXTO = db.CADMENSAJES_TEXTO.Include(c => c.CADMENSAJE).Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == idCons);
                //var cADMENSAJES_TEXTO_USUARIOS = db.CADMENSAJES_TEXTO_USUARIOS.Include(c => c.CADMENSAJES_TEXTO);

                GestorAdministrador gestor = new GestorAdministrador();
                var mensajesTextoUsuarios = gestor.ConsultarMensajeTextoUsuarios(idSitio, idCons, idText, strConexion);

                List<ExtMensajesTextoUsuarios> cADMENSAJES_TEXTO_USUARIOS = new List<ExtMensajesTextoUsuarios>();

                foreach (DataRow fila in mensajesTextoUsuarios.Rows)
                {
                    cADMENSAJES_TEXTO_USUARIOS.Add(new ExtMensajesTextoUsuarios(fila));
                }

                var variosModelos = new Tuple<List<CADMENSAJE>, List<CADMENSAJES_TEXTO>, List<ExtMensajesTextoUsuarios>>
                                      (null, cADMENSAJES_TEXTO.ToList(), cADMENSAJES_TEXTO_USUARIOS)
                { };

                return View(variosModelos);
                //return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

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

        public ActionResult GetTextoMensajes(int idSitio, long idCons)
        {
            try
            {
                var cADMENSAJES_TEXTO = db.CADMENSAJES_TEXTO.Include(c => c.CADMENSAJE).Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == idCons);
                return PartialView("_textoMensajes", cADMENSAJES_TEXTO.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult GetEnviarMensajesA(int idSitio, long idCons, long idText)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var mensajesTextoUsuarios = gestor.ConsultarMensajeTextoUsuarios(idSitio, idCons, idText, strConexion);

                List<ExtMensajesTextoUsuarios> cADMENSAJES_TEXTO_USUARIOS = new List<ExtMensajesTextoUsuarios>();

                foreach (DataRow fila in mensajesTextoUsuarios.Rows)
                {
                    cADMENSAJES_TEXTO_USUARIOS.Add(new ExtMensajesTextoUsuarios(fila));
                }

                return PartialView("_EnviarMensajeA", cADMENSAJES_TEXTO_USUARIOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult EnviarMensaje(int idSitio, long idCons)
        {
            try
            {
                GestorAdministrador gestor = new GestorAdministrador();
                var valido = gestor.EjecutarEnviarMensaje(idSitio, idCons, strConexion);

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", idCons.ToString()) + " y enviado." });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede actualizar, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public ActionResult Create()
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                //Región Centro de Mensajes//
                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION.Where(c => c.CONSECUTIVO == idSitio), "CONSECUTIVO", "DESCRIPCION");

                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;
                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.CADUSUA_INICIA = User.Identity.GetUserName();

                //traer usuario autenticado
                ViewBag.CADUSUA_INICIA = usuario;

                //ViewBag.CADUSUA_INICIA = "USR_CAD";

                ViewBag.FECHA = DateTime.Now;
                ViewBag.ESTADO1 = "Sin Enviar";

                //Región Centro de Mensajes//
                //---------------------------------------------------------------------------------------------------------//

                //Region Mensaje Texto//
                //---------------------------------------------------------------------------------------------------------//
                ViewBag.CADUSUA_GRABA = usuario;
                //Region Mensaje Texto//
                //---------------------------------------------------------------------------------------------------------//

                //Region Enviar Mensaje A//
                //---------------------------------------------------------------------------------------------------------//
                List<SelectListItem> entidad = new SelectList(db.CADFUERZAS, "ID", "DESCRIPCION").ToList();
                entidad.Insert(0, (new SelectListItem { Text = "TODAS LAS ENTIDADES", Value = "0" }));
                ViewBag.CADFUER_DESTINO = entidad.OrderBy(c => c.Text);

                List<CADCANALE> listaCanales = new List<CADCANALE>();
                listaCanales.Insert(0, new CADCANALE { CODIGO = 0, DESCRIPCION = "TODOS LOS CANALES" });
                ViewBag.CADCANA_DESTINO = new SelectList(listaCanales, "CODIGO", "DESCRIPCION");

                GestorAdministrador gestor1 = new GestorAdministrador();
                var usuariosMensajeA = gestor1.ConsultarUsuariosMensajeA(0, 0, strConexion);

                var usuarios = (from DataRow dr in usuariosMensajeA.Rows
                                select new
                                {
                                    CODIGO = dr["usuario"],
                                    DESCRIPCION = dr["usuario"] + " - " + dr["NOMBRE"] + " - " + dr["DESC_FUERZA"] + " - " + dr["DESC_SITIO"],
                                }).ToList();

                ViewBag.CADUSUA_DESTINO = new SelectList(usuarios, "CODIGO", "DESCRIPCION");

                List<SelectListItem> listaEstados = new List<SelectListItem>();
                listaEstados.Insert(0, (new SelectListItem { Text = "SIN NOTIFICAR", Value = "S", Selected = true }));
                listaEstados.Insert(1, (new SelectListItem { Text = "NOTIFICADO", Value = "N" }));
                ViewBag.ESTADO = listaEstados;

                //trigger db insert
                ViewBag.FECHA_GRABACION = DateTime.Now;
                //Region Enviar Mensaje A//
                //---------------------------------------------------------------------------------------------------------//

                CADMENSAJE cADMENSAJES = new CADMENSAJE();
                CADMENSAJES_TEXTO cADMENSAJES_TEXTO = new CADMENSAJES_TEXTO();
                CADMENSAJES_TEXTO_USUARIOS cADMENSAJES_TEXTO_USUARIOS = new CADMENSAJES_TEXTO_USUARIOS();

                //var cADMENSAJES_TEXTO = db.CADMENSAJES_TEXTO.Include(c => c.CADMENSAJE).Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == idCons);
                var cADMENSAJES_TEXTO_LISTA = db.CADMENSAJES_TEXTO.Include(c => c.CADMENSAJE).Where(c => c.CADMENS_SITIOGRABA == idSitio && c.CADMENS_CONSECUTIVO == 245607);

                //GestorAdministrador gestor2 = new GestorAdministrador();
                //var mensajesTextoUsuarios = gestor2.ConsultarMensajeTextoUsuarios(idSitio, 0, 0, strConexion);

                List<ExtMensajesTextoUsuarios> cADMENSAJES_TEXTO_USUARIOS_LISTA = new List<ExtMensajesTextoUsuarios>();

                //foreach (DataRow fila in mensajesTextoUsuarios.Rows)
                //{
                //    cADMENSAJES_TEXTO_USUARIOS_LISTA.Add(new ExtMensajesTextoUsuarios(fila));
                //}

                var variosModelos = new Tuple<CADMENSAJE, CADMENSAJES_TEXTO, CADMENSAJES_TEXTO_USUARIOS, List<ExtMensajesTextoUsuarios>>
                      (cADMENSAJES, cADMENSAJES_TEXTO, cADMENSAJES_TEXTO_USUARIOS, cADMENSAJES_TEXTO_USUARIOS_LISTA)
                { };

                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

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

                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION.Where(c => c.CONSECUTIVO == cADMENSAJE.SITIO_GRABA), "CONSECUTIVO", "DESCRIPCION", cADMENSAJE.SITIO_GRABA);
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

                return RedirectToAction("Index", "EnvioMensajes");
                //return View(cADMENSAJE);
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

        [Authorize]
        public JsonResult CreacionMensajesCentro([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADUSUA_INICIA,FECHA,TIPO_ASOCIADO,NUME_ASOCIADO,ESTADO")] CADMENSAJE cADMENSAJE)
        {
            try
            {
                cADMENSAJE.ESTADO = "S";

                GestorAdministrador gestor = new GestorAdministrador();
                var CONSECUTIVO = gestor.ConsultarConsecutivoMensaje(strConexion);
                cADMENSAJE.CONSECUTIVO = CONSECUTIVO;

                cADMENSAJE.FECHA = DateTime.Now;
                cADMENSAJE.TIPO_ASOCIADO = "00";

                if (ModelState.IsValid)
                {
                    db.CADMENSAJES.Add(cADMENSAJE);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMENSAJE>(cADMENSAJE, "CADMENSAJES", cADMENSAJE.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    //var mensaje = "Ha sido creado el registro: " + cADMENSAJE.CONSECUTIVO.ToString();
                    //var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return Json(new { result = true, datos = new { SITIO = cADMENSAJE.SITIO_GRABA, CONSECUTIVO = cADMENSAJE.CONSECUTIVO, MENSAJE = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADMENSAJE.CONSECUTIVO.ToString()) } });

                    //return RedirectToAction("Index");
                }

                return Json(new { result = false, datos = "NO FUNCIONO" });

                //return View(cADMENSAJE);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult CreacionMensajesTexto([Bind(Include = "CADMENS_SITIOGRABA,CADMENS_CONSECUTIVO,CONSECUTIVO,CADUSUA_GRABA,TEXTO")] CADMENSAJES_TEXTO cADMENSAJES_TEXTO)
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

                    //var mensaje = "Ha sido creado el registro: " + cADMENSAJES_TEXTO.CONSECUTIVO.ToString();
                    //var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return Json(new { result = true, datos = new { SITIO = cADMENSAJES_TEXTO.CADMENS_SITIOGRABA, CONSECUTIVO = cADMENSAJES_TEXTO.CADMENS_CONSECUTIVO, MENCONSECUTIVO = cADMENSAJES_TEXTO.CONSECUTIVO, MENSAJE = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADMENSAJES_TEXTO.CADMENS_CONSECUTIVO.ToString()) } });

                    //return RedirectToAction("Index");
                }
                return Json(new { result = false, datos = "NO FUNCIONO" });

                //return View(cADMENSAJES_TEXTO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult CreacionMensajesTextoUsuario(CADMENSAJES_TEXTO_USUARIOS[] usuariosTabla)
        {
            try
            {
                foreach (var item in usuariosTabla)
                {
                    item.FECHA_GRABACION = DateTime.Now;
                    db.CADMENSAJES_TEXTO_USUARIOS.Add(item);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADMENSAJES_TEXTO_USUARIOS>(item, "CADMENSAJES_TEXTO_USUARIOS", item.CADUSUA_DESTINO.ToString(), TipoAuditoria.Crear);
                }
                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", usuariosTabla[0].CADMENTEX_CONSECUTIVO.ToString()) });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public class MensajeTextoUsuarios
        {
            public int CADMENTEX_MENS_SITIOGRABA { get; set; }
            public long CADMENTEX_MENS_CONSECUTIVO { get; set; }
            public long CADMENTEX_CONSECUTIVO { get; set; }
            public string CADUSUA_DESTINO { get; set; }
            public string ESTADO { get; set; }
            public DateTime? FECHA_NOTIFICACION { get; set; }
            public DateTime FECHA_GRABACION { get; set; }
            public int? CADFUER_DESTINO { get; set; }
            public int? CADCANA_DESTINO { get; set; }
        }

        public JsonResult MensajesTexto(MensajeTextoUsuarios[] usuariosTabla)
        {
            try
            {
                //foreach (var item in usuariosTabla)
                //{
                //    var i = item;
                //}

                return Json(new { result = true, datos = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "1") });

                //return Json(new { result = true, datos = "Mensaje Enviado" });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede actualizar, restricción de integridad.";
                return Json(new { result = false, datos = mensaje });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }


        /// <summary>
        /// Obtiene los mensajes enviados a un sitio de grabacion
        /// </summary>
        /// <param name="strSitioGrabaEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObteneEnvioMensajes()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPerRelacionada;
                List<Ext_EnvioMensajes> listaEnvioMensajes = new List<Ext_EnvioMensajes>();
                GestorGeneral objGestorGral = new GestorGeneral();
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                string strBotonDestino = "";
                string strBoton = "<ul class='icons-list'> " +
                            "<li class='dropdown'> " +
                                "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                    "<i class='icon-menu9'></i> " +
                                "</button> " +
                                "<ul class='dropdown-menu dropdown-menu-left'> " +
                                    "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                "title='' data-placement='bottom' data-original-title='Clic para ver texto mensajes' " +
                                                "onclick=\"VerMensajes(SITIO_GRABA,CONSECUTIVO)\"> " +
                                            "<i class='icon-trash'></i> Ver texto mensajes" +
                                        "</button>" +
                                    "</li>" +
                                     "<li> " +
                                        "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                "title='' data-placement='bottom' data-original-title='Clic para enviar' " +
                                                "onclick=\"Enviar(SITIO_GRABA,CONSECUTIVO)\"> " +
                                            "<i class='icon-trash'></i> Enviar" +
                                        "</button>" +
                                    "</li>" +

                                "</ul>" +
                            "</li>" +
                        "</ul>";

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (DESP_SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(FECHA, '" + parametros.ConsultaValor + "', 'i') " +
                                        "OR REGEXP_LIKE(CADUSUA_INICIA, '" + parametros.ConsultaValor + "', 'i') ";

                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CONSECUTIVO DESC";
                        break;

                    case 2:
                        strOrderna += " ORDER BY DESP_SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY CADUSUA_INICIA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY ESTADO " + parametros.TipoOrdenamiento + " ";
                        break;

                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPerRelacionada = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaEnvioMensajes", strConexion, "Nonquery");

                foreach (DataRow fila in dttPerRelacionada.Rows)
                {
                    strBotonDestino = strBoton.Replace("SITIO_GRABA", fila["SITIO_GRABA"].ToString()).Replace("CONSECUTIVO", fila["CONSECUTIVO"].ToString());

                    listaEnvioMensajes.Add(new Ext_EnvioMensajes(fila, strBotonDestino));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEnvioMensajes });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
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