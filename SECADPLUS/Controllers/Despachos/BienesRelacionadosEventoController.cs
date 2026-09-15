using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class BienesRelacionadosEventoController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorGeneral objGestorGral;
        private GestorAuditoria auditoria = new GestorAuditoria();

        // GET: CADBIENES
        [SecadPlusAutorizador("/BienesRelacionadosEvento/Index")]
        public ActionResult Index(string strMensaje = null, int? intNumeroLLamada = null, int? intSitioGraba = null)
        {
            try
            {
                // intNumeroLLamada = 6847915; intSitioGraba = 3947;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                if (Session["NUMERO_SITIO_GRABA"] == null && Session["NUMERO_LLAMADA"] == null)
                {
                    if (intSitioGraba != null && intNumeroLLamada != null)
                    {
                        Session["NUMERO_SITIO_GRABA"] = intSitioGraba;
                        Session["NUMERO_LLAMADA"] = intNumeroLLamada;
                    }
                    else
                    {
                        GestorBase.MSN_ALERTA_COMUN = "No se encontro un evento.";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: CADBIENES/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADBIENE cADBIENES = await db.CADBIENES.FindAsync(id);
            if (cADBIENES == null)
            {
                return HttpNotFound();
            }
            return View(cADBIENES);
        }

        // GET: CADBIENES/Create
        [Authorize]
        public ActionResult Create(string strMensaje = null, int? intNumeroLLamada = null, int? intSitioGraba = null, CADBIENE cadBien = null, CADBIENES_NOVEDADES cADBIENES_NOVEDADES = null)
        {
            try
            {
                if (Session["NUMERO_SITIO_GRABA"] == null && Session["NUMERO_LLAMADA"] == null)
                {
                    if (intSitioGraba != null && intNumeroLLamada != null)
                    {
                        Session["NUMERO_SITIO_GRABA"] = intSitioGraba;
                        Session["NUMERO_LLAMADA"] = intNumeroLLamada;
                    }
                    else
                    {
                        GestorBase.MSN_ALERTA_COMUN = "No se encontro un evento.";
                        return RedirectToAction("Index", "Home");
                    }
                }


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = "Vehiculos Involucrados";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////


                if (strMensaje != null)
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                }

                List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                List<CADTIPOSNOVEDADE> listaNovedades = new List<CADTIPOSNOVEDADE>();

                listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });

                ViewBag.CONSECUTIVO = 0;

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.LINEA_CODIGO = new SelectList(listaLineasMarcas, "CODIGO", "DESCRIPCION");
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.TIPO_VEHICULO = new SelectList(db.CADTIPOS_BIEN.Where(m => m.CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CLIN_CODIGO = new SelectList(listaNovedades, "CODIGO", "DESCRIPCION");
                ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA");

                ViewBag.USUA_TRANSACCION = usuario;
                ViewBag.SITIO_GRABA = idSitio;

                if (cadBien.SITIO_GRABA != 0 && cadBien.CONSECUTIVO != 0)
                {
                    ViewBag.MENSAJE_EXITOSO = "Se guardaron los cambios con éxito.";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                if (cadBien.SITIO_GRABA != 0 && cadBien.CONSECUTIVO != 0)
                {
                    CADBIENE cADBIENES = db.CADBIENES.Find(cadBien.SITIO_GRABA, cadBien.CONSECUTIVO);
                    ViewBag.CONSECUTIVO = cADBIENES.CONSECUTIVO;

                    ViewBag.SITIO_GRABA = cADBIENES.SITIO_GRABA;
                    ViewBag.CADCLBI_TIBI_CODIGO = 4;
                    ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.CADCLBI_CODIGO);
                    ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.MARCA_CODIGO);

                    ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENES.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.LINEA_CODIGO);

                    ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.COLO_CODIGO);

                    ViewBag.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;
                    return View(cADBIENES);
                }
                else if (cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA != 0 && cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO != 0)
                {
                    CADBIENE cADBIENES = db.CADBIENES.Find(cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA, cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO);
                    ViewBag.CONSECUTIVO = cADBIENES.CONSECUTIVO;

                    ViewBag.SITIO_GRABA = cADBIENES.SITIO_GRABA;
                    ViewBag.CADCLBI_TIBI_CODIGO = 4;
                    ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.CADCLBI_CODIGO);
                    ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.MARCA_CODIGO);

                    ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENES.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.LINEA_CODIGO);

                    ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.COLO_CODIGO);

                    ViewBag.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;

                    cADBIENES.CADBIENES_NOVEDADES.Add(cADBIENES_NOVEDADES);
                    return View(cADBIENES);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADBIENES/Create Para protegerse de ataques de publicación excesiva, habilite las
        // propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,OBSERVACIONES,USUA_TRANSACCION,FECHA_TRANSACCION")] CADBIENE cADBIENES)
        {
            try
            {
                if (cADBIENES.CONSECUTIVO == 0)
                {
                    CADBIENE existe;
                    existe = db.CADBIENES.SingleOrDefault(c => c.PLACA == cADBIENES.PLACA);

                    if (cADBIENES.LINEA_CODIGO == 0 && cADBIENES.MARCA_CODIGO == 0)
                    {
                        ModelState.AddModelError("LINEA_CODIGO", "El campo Línea es obligatorio.");
                    }

                    if (existe != null)
                        ModelState.AddModelError("PLACA", "Esta placa esta siendo utilizado en otro registro.");

                    existe = db.CADBIENES.SingleOrDefault(c => c.CHASIS == cADBIENES.CHASIS);

                    //if (existe != null)
                    //    ModelState.AddModelError("CHASIS", "Este chasis esta siendo utilizado en otro registro.");

                    //existe = db.CADBIENES.SingleOrDefault(c => c.MOTOR == cADBIENES.MOTOR);

                    //if (existe != null)
                    //    ModelState.AddModelError("MOTOR", "Este motor esta siendo utilizado en otro registro.");

                    ModelState.Remove("CHASIS");
                    ModelState.Remove("MOTOR");
                }

                if (ModelState.IsValid)
                {
                    CADBIENE cADBIENESMod = db.CADBIENES.Find(cADBIENES.SITIO_GRABA, cADBIENES.CONSECUTIVO);

                    if (cADBIENESMod != null)
                    {
                        cADBIENESMod.CADCLASES_BIEN = cADBIENES.CADCLASES_BIEN;
                        cADBIENESMod.CADCLBI_CODIGO = cADBIENES.CADCLBI_CODIGO;
                        cADBIENESMod.CADCLBI_TIBI_CODIGO = cADBIENES.CADCLBI_TIBI_CODIGO;
                        cADBIENESMod.CADCOLORE = cADBIENES.CADCOLORE;
                        cADBIENESMod.CADLINEAS_MARCA = cADBIENES.CADLINEAS_MARCA;
                        cADBIENESMod.CHASIS = cADBIENES.CHASIS;
                        cADBIENESMod.COLO_CODIGO = cADBIENES.COLO_CODIGO;
                        cADBIENESMod.FECHA_TRANSACCION = cADBIENES.FECHA_TRANSACCION;
                        cADBIENESMod.LINEA_CODIGO = cADBIENES.LINEA_CODIGO;
                        cADBIENESMod.MARCA_CODIGO = cADBIENES.MARCA_CODIGO;
                        cADBIENESMod.MOTOR = cADBIENES.MOTOR;
                        cADBIENESMod.OBSERVACIONES = cADBIENES.OBSERVACIONES;
                        cADBIENESMod.PLACA = cADBIENES.PLACA;
                        cADBIENESMod.SERIE = cADBIENES.SERIE;
                        cADBIENESMod.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;

                        db.Entry(cADBIENESMod).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Create", "BienesRelacionadosEvento", cADBIENESMod);
                    }
                    else
                    {
                        GestorAdministrador gestor = new GestorAdministrador();
                        var CONSECUTIVO = gestor.ConsultarConsecutivoBienes(strConexion);
                        cADBIENES.CONSECUTIVO = CONSECUTIVO;
                        cADBIENES.MOTOR = cADBIENES.MOTOR == null ? "." : cADBIENES.MOTOR;
                        cADBIENES.CHASIS = cADBIENES.CHASIS == null ? "." : cADBIENES.CHASIS;

                        cADBIENES.FECHA_TRANSACCION = DateTime.Now;

                        db.CADBIENES.Add(cADBIENES);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Create", "BienesRelacionadosEvento", cADBIENES); //new { strMensaje = mensaje, cadBien = cADBIENES });
                    }
                }
                List<CADTIPOSNOVEDADE> listaNovedades = new List<CADTIPOSNOVEDADE>();
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                ViewBag.CLIN_CODIGO = new SelectList(listaNovedades, "CODIGO", "DESCRIPCION");
                ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA");

                ViewBag.TIPO_VEHICULO = new SelectList(db.CADTIPOS_BIEN.Where(m => m.CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                ViewBag.SITIO_GRABA = cADBIENES.SITIO_GRABA;
                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CONSECUTIVO = cADBIENES.CONSECUTIVO;

                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.CADCLBI_CODIGO);
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.MARCA_CODIGO);

                if (cADBIENES.LINEA_CODIGO == 0)
                {
                    List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                    listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });
                    ViewBag.LINEA_CODIGO = new SelectList(listaLineasMarcas, "CODIGO", "DESCRIPCION");
                }
                else
                {
                    ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENES.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.LINEA_CODIGO);
                }

                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.COLO_CODIGO);
                // El usuario por medio de autenticación
                ViewBag.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADBIENES);
        }

        // GET: CADBIENES/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(string strMensaje = null, int? intSitioGraba = null, Int64? intConsecutivo = null, CADBIENES_NOVEDADES cADBIENES_NOVEDADES = null)
        {
            try
            {
                if (strMensaje != null)
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }

                if (cADBIENES_NOVEDADES != null && cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO != 0)
                {
                    intSitioGraba = cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA;
                    intConsecutivo = cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO;
                }
                if (intSitioGraba == null && intConsecutivo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENE cADBIENES = await db.CADBIENES.FindAsync(intSitioGraba, intConsecutivo);
                if (cADBIENES == null)
                {
                    return HttpNotFound();
                }

                List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                List<CADTIPOSNOVEDADE> listaNovedades = new List<CADTIPOSNOVEDADE>();

                listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });

                ViewBag.CONSECUTIVO = 0;

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                ViewBag.CONSECUTIVO = cADBIENES.CONSECUTIVO;
                ViewBag.SITIO_GRABA = cADBIENES.SITIO_GRABA;
                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.CADCLBI_CODIGO);
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.MARCA_CODIGO);
                ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENES.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.LINEA_CODIGO);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.COLO_CODIGO);
                ViewBag.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;
                ViewBag.TIPO_VEHICULO = new SelectList(db.CADTIPOS_BIEN.Where(m => m.CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                if (cADBIENES_NOVEDADES != null && cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO != 0)
                {
                    ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES_NOVEDADES.TIIN_CODIGO);
                    ViewBag.CLIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.CADNOVBIN == cADBIENES_NOVEDADES.TIIN_CODIGO), "CODIGO", "DESCRIPCION", cADBIENES_NOVEDADES.CLIN_CODIGO);
                    ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES_NOVEDADES.MODA_CODIGO);
                    ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA", cADBIENES_NOVEDADES.VEHI_SIGLA);
                    ViewBag.ESTADO_NOVEDAD = cADBIENES_NOVEDADES.ESTADO_NOVEDAD;
                    ViewBag.DESCR_LUGE = (from t in db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == cADBIENES_NOVEDADES.LUGE_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                    ViewBag.DESCR_BARRIO = (from t in db.CADBARRIOS.Where(t => t.CODIGO == cADBIENES_NOVEDADES.CODI_BARRIO && t.LUGE_CODIGO == cADBIENES_NOVEDADES.LUGE_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                    ViewBag.INFR_DESCRIPCION = (from t in db.CADINFRACCIONES_TRANSITO.Where(t => t.CODIGO == cADBIENES_NOVEDADES.INFR_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                    ViewBag.DESCR_PARQUEADERO = (from t in db.CADPARQUEADEROS.Where(t => t.CODIGO == cADBIENES_NOVEDADES.PARQ_CODIGO && t.SITIO_GRABA == cADBIENES_NOVEDADES.PARQ_SITIOGRABA) select t.DESCRIPCION).FirstOrDefault();

                    if (cADBIENES_NOVEDADES.MEDISP_CONSECUTIVO != null)
                    {
                        ViewBag.DESCR_UNIDAD = ObtenerDescUnidad(cADBIENES_NOVEDADES.MEDISP_CONSECUTIVO, cADBIENES_NOVEDADES.MEDISP_SITIOGRABA);
                        ViewBag.DESCR_PATRULLA = ObtenerDescPatrulla(cADBIENES_NOVEDADES.MEDISP_CONSECUTIVO);
                        ViewBag.CADUNITUR_UNDEFUERZA = (from t in contexto.CADMEDIOSDISPONIBLES.Where(t => t.CONSECUTIVO == cADBIENES_NOVEDADES.MEDISP_CONSECUTIVO && t.SITIO_GRABA == cADBIENES_NOVEDADES.MEDISP_SITIOGRABA) select t.CADUNITUR_UNDEFUERZA).FirstOrDefault();
                        ViewBag.CADUNITUR_UNDECODIGO = (from t in contexto.CADMEDIOSDISPONIBLES.Where(t => t.CONSECUTIVO == cADBIENES_NOVEDADES.MEDISP_CONSECUTIVO && t.SITIO_GRABA == cADBIENES_NOVEDADES.MEDISP_SITIOGRABA) select t.CADUNITUR_UNDECODIGO).FirstOrDefault();
                    }
                    cADBIENES.CADBIENES_NOVEDADES = null;
                    cADBIENES.CADBIENES_NOVEDADES.Add(cADBIENES_NOVEDADES);

                    return View(cADBIENES);
                }

                if (cADBIENES.CADBIENES_NOVEDADES.Count > 0)
                {
                    foreach (var novedad in cADBIENES.CADBIENES_NOVEDADES)
                    {
                        ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", novedad.TIIN_CODIGO);
                        ViewBag.CLIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.CADNOVBIN == novedad.TIIN_CODIGO), "CODIGO", "DESCRIPCION", novedad.CLIN_CODIGO);
                        ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", novedad.MODA_CODIGO);
                        ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA", novedad.VEHI_SIGLA);
                        ViewBag.ESTADO_NOVEDAD = novedad.ESTADO_NOVEDAD;
                        ViewBag.DESCR_LUGE = (from t in db.CADLUGARES_GEOGRAFICOS.Where(t => t.CODIGO == novedad.LUGE_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                        ViewBag.DESCR_BARRIO = (from t in db.CADBARRIOS.Where(t => t.CODIGO == novedad.CODI_BARRIO && t.LUGE_CODIGO == novedad.LUGE_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                        ViewBag.INFR_DESCRIPCION = (from t in db.CADINFRACCIONES_TRANSITO.Where(t => t.CODIGO == novedad.INFR_CODIGO) select t.DESCRIPCION).FirstOrDefault();
                        ViewBag.DESCR_PARQUEADERO = (from t in db.CADPARQUEADEROS.Where(t => t.CODIGO == novedad.PARQ_CODIGO && t.SITIO_GRABA == novedad.PARQ_SITIOGRABA) select t.DESCRIPCION).FirstOrDefault();

                        if (novedad.MEDISP_CONSECUTIVO != null)
                        {
                            ViewBag.DESCR_UNIDAD = ObtenerDescUnidad(novedad.MEDISP_CONSECUTIVO, novedad.MEDISP_SITIOGRABA);
                            ViewBag.DESCR_PATRULLA = ObtenerDescPatrulla(novedad.MEDISP_CONSECUTIVO);
                            ViewBag.CADUNITUR_UNDEFUERZA = (from t in contexto.CADMEDIOSDISPONIBLES.Where(t => t.CONSECUTIVO == novedad.MEDISP_CONSECUTIVO && t.SITIO_GRABA == novedad.MEDISP_SITIOGRABA) select t.CADUNITUR_UNDEFUERZA).FirstOrDefault();
                            ViewBag.CADUNITUR_UNDECODIGO = (from t in contexto.CADMEDIOSDISPONIBLES.Where(t => t.CONSECUTIVO == novedad.MEDISP_CONSECUTIVO && t.SITIO_GRABA == novedad.MEDISP_SITIOGRABA) select t.CADUNITUR_UNDECODIGO).FirstOrDefault();
                        }

                        break;
                    }
                }
                else
                {
                    ViewBag.CLIN_CODIGO = new SelectList(listaNovedades, "CODIGO", "DESCRIPCION");
                    ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                    ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                    ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA");
                }

                return View(cADBIENES);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADBIENES/Edit/5 Para protegerse de ataques de publicación excesiva, habilite las
        // propiedades específicas a las que desea enlazarse. Para obtener más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,OBSERVACIONES,USUA_TRANSACCION,FECHA_TRANSACCION")] CADBIENE cADBIENES)
        {
            try
            {
                if (cADBIENES.LINEA_CODIGO == 0 && cADBIENES.MARCA_CODIGO == 0)
                {
                    ModelState.AddModelError("LINEA_CODIGO", "El campo Línea es obligatorio.");
                }

                if (ModelState.IsValid)
                {
                    db.Entry(cADBIENES).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    auditoria.RegistrarAuditoria<CADBIENE>(cADBIENES, "CADBIENES", "SITIO_GRABA=" + cADBIENES.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADBIENES.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Se guardaron los cambios con éxito.";
                    return RedirectToAction("Edit", "BienesRelacionadosEvento", new { strMensaje = mensaje, intSitioGraba = cADBIENES.SITIO_GRABA, intConsecutivo = cADBIENES.CONSECUTIVO });
                }

                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                List<CADTIPOSNOVEDADE> listaNovedades = new List<CADTIPOSNOVEDADE>();

                ViewBag.CLIN_CODIGO = new SelectList(listaNovedades, "CODIGO", "DESCRIPCION");
                ViewBag.TIIN_CODIGO = new SelectList(db.CADTIPOSNOVEDADES.Where(m => m.NIVEL == 1).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MODA_CODIGO = new SelectList(contexto.CADMODALIDADES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                ViewBag.TIPO_VEHICULO = new SelectList(db.CADTIPOS_BIEN.Where(m => m.CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                ViewBag.SITIO_GRABA = cADBIENES.SITIO_GRABA;
                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CONSECUTIVO = cADBIENES.CONSECUTIVO;

                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.CADCLBI_CODIGO);
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.MARCA_CODIGO);
                ViewBag.VEHI_SIGLA = new SelectList(contexto.CADVEHIDISPONIBLES.Where(m => m.CADMEDISP_SITIOGRABA == usr.SITIO_GRABACION).OrderBy(m => m.CADVEHI_NUMESIGLA), "CADVEHI_NUMESIGLA", "CADVEHI_NUMESIGLA");

                if (cADBIENES.LINEA_CODIGO == 0)
                {
                    List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                    listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });
                    ViewBag.LINEA_CODIGO = new SelectList(listaLineasMarcas, "CODIGO", "DESCRIPCION");
                }
                else
                {
                    ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENES.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.LINEA_CODIGO);
                }

                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENES.COLO_CODIGO);
                // El usuario por medio de autenticación
                ViewBag.USUA_TRANSACCION = cADBIENES.USUA_TRANSACCION;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADBIENES);
        }

        // GET: CADBIENES/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADBIENE cADBIENES = await db.CADBIENES.FindAsync(id);
            if (cADBIENES == null)
            {
                return HttpNotFound();
            }
            return View(cADBIENES);
        }

        // POST: CADBIENES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CADBIENE cADBIENES = await db.CADBIENES.FindAsync(id);
                db.CADBIENES.Remove(cADBIENES);
                await db.SaveChangesAsync();


                CADBIENE cADBIENES_copy = new CADBIENE();
                cADBIENES_copy = (CADBIENE)auditoria.GenerarCopiaAuditoria(cADBIENES);

                auditoria.RegistrarAuditoria<CADBIENE>(cADBIENES_copy, "CADBIENES", "SITIO_GRABA=" + cADBIENES.SITIO_GRABA.ToString() + ",CONSECUTIVO=" + cADBIENES.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);


                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

        }

        [Authorize]
        public JsonResult ObtenerBienesRelacionados()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttBienesRelacionados;

                string strBoton = "<ul class='icons-list'> " +
                                        "<li class='dropdown'> " +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                "<i class='icon-menu9'></i> " +
                                            "</button> " +
                                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para editar' " +
                                                              // "onclick=\"window.location.href =
                                                              // '/BienesRelacionadosEvento/Edit?intSitioGraba=SITIO_GRABA&intConsecutivo=_CONSECUTIVO'\">
                                                              // " +
                                                              "onclick =\"location.href='_EDITAR'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                                                    "</button> " +
                                                "</li> " +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";

                List<_BienesRelacionadosModel> listaBienesRelacionados = new List<_BienesRelacionadosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (USUA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (FECHA_TRANSACCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CHASIS, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (MOTOR, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (SERIE, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (OBSERVACIONES, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (PLACA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY USUA_TRANSACCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 4:
                        strOrderna += " ORDER BY FECHA_TRANSACCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY PLACA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY CHASIS " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY MOTOR " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY SERIE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 14:
                        strOrderna += " ORDER BY OBSERVACIONES " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttBienesRelacionados = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBienesRelacionadosEvent", strConexion, "Nonquery");

                foreach (DataRow fila in dttBienesRelacionados.Rows)
                {
                    // strBotonDestino = strBoton.Replace("SITIO_GRABA",
                    // fila["SITIO_GRABA"].ToString() ).Replace("_CONSECUTIVO",
                    // fila["CONSECUTIVO"].ToString() );
                    strBotonDestino = strBoton.Replace("_EDITAR", (Url.Action("Edit", new { intSitioGraba = fila["SITIO_GRABA"].ToString(), intConsecutivo = fila["CONSECUTIVO"].ToString() })).ToString());

                    listaBienesRelacionados.Add(new _BienesRelacionadosModel("", strBotonDestino, fila["CONSECUTIVO"].ToString(), fila["USUA_TRANSACCION"].ToString(), fila["FECHA_TRANSACCION"].ToString(),
                        fila["PLACA"].ToString(), fila["CHASIS"].ToString(), fila["MOTOR"].ToString(), fila["SERIE"].ToString(), fila["TIPO_VEHICULO"].ToString(), fila["CLASE_BIEN"].ToString(), fila["COLOR"].ToString(),
                        fila["MARCA"].ToString(), fila["LINEA"].ToString(), fila["OBSERVACIONES"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaBienesRelacionados });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult GetLineasByMarca(int? id)
        {
            try
            {
                List<CADLINEAS_MARCA> ListalineasMarcas = new List<CADLINEAS_MARCA>();

                if (id != null)
                {
                    GestorAdministrador gestor = new GestorAdministrador();
                    var lineasMarcas = gestor.ConsultarLineasMarca(id);

                    var lineas = (from DataRow dr in lineasMarcas.Rows
                                  select new
                                  {
                                      CODIGO = dr["CODIGO"],
                                      DESCRIPCION = dr["DESCRIPCION"],
                                  }).ToList();

                    return Json(lineas);
                }
                else
                {
                    ListalineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });

                    return Json(ListalineasMarcas);
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
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

        #region Novedades

        public ActionResult _Novedades()
        {
            try
            {
                return PartialView("_Novedades");
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADBIENES_NOVEDADES/Create Para protegerse de ataques de publicación excesiva,
        // habilite las propiedades específicas a las que desea enlazarse. Para obtener más
        // información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateN([Bind(Include = "CADBIEN_SITIOGRABA,CADBIEN_CONSECUTIVO,CONSECUTIVO,TIIN_CODIGO,CLIN_CODIGO,MODA_CODIGO,FECHA_NOVEDAD,INFR_CODIGO,PARQ_SITIOGRABA,PARQ_CODIGO,MEDISP_SITIOGRABA,MEDISP_CONSECUTIVO,VEHI_SIGLA,ESTADO_NOVEDAD,FECHA_TERMINO,OBSERVACIONES,PEDI_SITIOGRABA,PEDI_NUMELLAMADA,USUA_TRANSACCION,FECHA_TRANSACCION,DIRECCION,LUGE_CODIGO,LUGE_BARRIO,CODI_BARRIO")] CADBIENES_NOVEDADES cADBIENES_NOVEDADES)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var numerollamada = Session["NUMERO_LLAMADA"];
                    var sitioGraba = Session["NUMERO_SITIO_GRABA"];

                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                    if (numerollamada == null || usr == null || sitioGraba == null)
                    {
                        return RedirectToAction("Create", "BienesRelacionadosEvento", "Sección inactiva.");
                    }

                    var consecutivo = (from t in db.CADBIENES_NOVEDADES where t.CADBIEN_SITIOGRABA == cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA && t.CADBIEN_CONSECUTIVO == cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();

                    int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                    cADBIENES_NOVEDADES.PEDI_NUMELLAMADA = Convert.ToInt64(numerollamada);
                    cADBIENES_NOVEDADES.FECHA_TRANSACCION = DateTime.Now;
                    cADBIENES_NOVEDADES.USUA_TRANSACCION = usr.USUARIO;
                    cADBIENES_NOVEDADES.PEDI_SITIOGRABA = Convert.ToInt32(sitioGraba);
                    cADBIENES_NOVEDADES.CONSECUTIVO = intConsecutivo;

                    db.CADBIENES_NOVEDADES.Add(cADBIENES_NOVEDADES);
                    await db.SaveChangesAsync();
                    var mensaje = "Se guardaron los cambios con éxito.";

                    return RedirectToAction("Index", "BienesRelacionadosEvento", new { strMensaje = mensaje });
                }
                CADBIENE cADBIENES = db.CADBIENES.Find(cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA, cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO);
                cADBIENES.CADBIENES_NOVEDADES.Add(cADBIENES_NOVEDADES);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Create", "BienesRelacionadosEvento", cADBIENES_NOVEDADES);

            // return View(cADBIENES_NOVEDADES);
        }

        // GET: CADBIENES_NOVEDADES/Edit/5
        public async Task<ActionResult> EditN(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADBIENES_NOVEDADES cADBIENES_NOVEDADES = await db.CADBIENES_NOVEDADES.FindAsync(id);
            if (cADBIENES_NOVEDADES == null)
            {
                return HttpNotFound();
            }
            ViewBag.CADBIEN_SITIOGRABA = new SelectList(db.CADBIENES, "SITIO_GRABA", "PLACA", cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA);
            return View(cADBIENES_NOVEDADES);
        }

        // POST: CADBIENES_NOVEDADES/Edit/5 Para protegerse de ataques de publicación excesiva,
        // habilite las propiedades específicas a las que desea enlazarse. Para obtener más
        // información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditN([Bind(Include = "CADBIEN_SITIOGRABA,CADBIEN_CONSECUTIVO,CONSECUTIVO,TIIN_CODIGO,CLIN_CODIGO,MODA_CODIGO,FECHA_NOVEDAD,INFR_CODIGO,PARQ_SITIOGRABA,PARQ_CODIGO,MEDISP_SITIOGRABA,MEDISP_CONSECUTIVO,VEHI_SIGLA,ESTADO_NOVEDAD,FECHA_TERMINO,OBSERVACIONES,PEDI_SITIOGRABA,PEDI_NUMELLAMADA,USUA_TRANSACCION,FECHA_TRANSACCION,DIRECCION,LUGE_CODIGO,LUGE_BARRIO,CODI_BARRIO")] CADBIENES_NOVEDADES cADBIENES_NOVEDADES)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var consecutivo = (from t in db.CADBIENES_NOVEDADES where t.CADBIEN_SITIOGRABA == cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA && t.CADBIEN_CONSECUTIVO == cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();

                    int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                    if (intConsecutivo == 1)
                    {
                        cADBIENES_NOVEDADES.CONSECUTIVO = intConsecutivo;
                        db.CADBIENES_NOVEDADES.Add(cADBIENES_NOVEDADES);
                        await db.SaveChangesAsync();
                        var mensaje = "Se guardaron los cambios con éxito.";
                        return RedirectToAction("Index", "BienesRelacionadosEvento", new { strMensaje = mensaje });
                    }
                    else
                    {
                        cADBIENES_NOVEDADES.CONSECUTIVO = 1;
                        db.Entry(cADBIENES_NOVEDADES).State = EntityState.Modified;
                        auditoria.RegistrarAuditoria<CADBIENES_NOVEDADES>(cADBIENES_NOVEDADES, "CADBIENES_NOVEDADES", "CADBIEN_SITIOGRABA=" + cADBIENES_NOVEDADES.CADBIEN_SITIOGRABA.ToString() + ",CADBIEN_CONSECUTIVO=" + cADBIENES_NOVEDADES.CADBIEN_CONSECUTIVO.ToString() + ",CONSECUTIVO=" + cADBIENES_NOVEDADES.CONSECUTIVO, TipoAuditoria.Modificar);

                        await db.SaveChangesAsync();
                        var mensaje = "Se guardaron los cambios con éxito.";
                        return RedirectToAction("Index", "BienesRelacionadosEvento", new { strMensaje = mensaje });
                    }
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Edit", "BienesRelacionadosEvento", cADBIENES_NOVEDADES);
        }

        // GET: CADBIENES_NOVEDADES/Delete/5
        public async Task<ActionResult> DeleteN(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CADBIENES_NOVEDADES cADBIENES_NOVEDADES = await db.CADBIENES_NOVEDADES.FindAsync(id);
            if (cADBIENES_NOVEDADES == null)
            {
                return HttpNotFound();
            }
            return View(cADBIENES_NOVEDADES);
        }

        // POST: CADBIENES_NOVEDADES/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedN(int id)
        {
            CADBIENES_NOVEDADES cADBIENES_NOVEDADES = await db.CADBIENES_NOVEDADES.FindAsync(id);
            db.CADBIENES_NOVEDADES.Remove(cADBIENES_NOVEDADES);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult ConsultarBien(string strPlaca)
        {
            try
            {
                long consecutivo = 0;
                int sitioGraba = 0;
                if (strPlaca != "")
                {
                    var datos = db.CADBIENES.Where(t => t.PLACA == strPlaca).Select(t => t).FirstOrDefault();

                    if (datos != null)
                    {
                        sitioGraba = datos.SITIO_GRABA;
                        consecutivo = datos.CONSECUTIVO;
                    }
                }

                if (consecutivo != 0)
                {
                    //return RedirectToAction("Edit", new { intConsecutivo = consecutivo });
                    return RedirectToAction("Edit", "BienesRelacionadosEvento", new { strMensaje = "", intSitioGraba = sitioGraba, intConsecutivo = consecutivo });
                }
                else
                {
                    return RedirectToAction("Index", new { strMensaje = "No se encontraron resultados; realice una búsqueda en la grilla o cree el nuevo registro" });
                }
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                throw;
            }
        }

        [Authorize]
        public JsonResult GetClaseNovedad(int? id)
        {
            try
            {
                List<CADTIPOSNOVEDADE> ListaClaseNovedad = new List<CADTIPOSNOVEDADE>();

                if (id != null)
                {
                    GestorAdministrador gestor = new GestorAdministrador();
                    var claseNovedad = gestor.ConsultarClaseNovedad(id);

                    var clasesNovedad = (from DataRow dr in claseNovedad.Rows
                                         select new
                                         {
                                             CODIGO = dr["CODIGO"],
                                             DESCRIPCION = dr["DESCRIPCION"],
                                         }).ToList();

                    return Json(clasesNovedad);
                }
                else
                {
                    ListaClaseNovedad.Insert(0, new CADTIPOSNOVEDADE { CODIGO = "", DESCRIPCION = " " });

                    return Json(ListaClaseNovedad);
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        [Authorize]
        public JsonResult CargaCiudades()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";

                strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarCiudad(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca Ciudades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                int? intLugeCodigo = usr.LUGE_CODIGO_DPTO;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_Ciudades> listaCiudades = new List<Ext_Ciudades>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery = " WHERE (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";
                    int codigoBarr;
                    bool result = Int32.TryParse(parametros.ConsultaValor, out codigoBarr);
                    if (result)
                    {
                        strQuery += " OR CODIGO LIKE ('%" + codigoBarr + "%') ";
                    }
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                // strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                if (strQuery == string.Empty)
                    strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                else
                    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", intLugeCodigo.ToString(), typeof(Int32)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaCiudadesAsignarBarrios", strConexion, "Nonquery");

                foreach (DataRow fila in dttCiudades.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaCiudades.Add(new Ext_Ciudades(fila, strBotonSelect));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCiudades });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public JsonResult CargarTodosLosBarrios(string lugeCodigo = null)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                 "onclick = \"CargarBarrio(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                 " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttBarrios;
                List<_BarriosModel> listaBarrios = new List<_BarriosModel>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                /// strQuery = " WHERE (LUGE_CODIGO = " + lugeCodigo + ") ";

                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    strQuery += " WHERE (upper(CODIGO) LIKE upper('%" + parametros.ConsultaValor + "%') ) " +
                                " OR (upper(DESCRIPCION) LIKE upper('%" + parametros.ConsultaValor + "%') ) ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESCRIPCION ASC ";

                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_lugar", lugeCodigo, typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttBarrios = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaBarriosLugarBienes", strConexion, "Nonquery");

                foreach (DataRow fila in dttBarrios.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", fila["CODIGO"].ToString()).Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaBarrios.Add(new _BarriosModel("", strBotonSelect, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaBarrios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult CargaInfracciones()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strBoton = "";

                strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarInfraccion(CODIGO,DESCRIPCION)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca Infracciones sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttCiudades;
                List<Ext_Infracciones> listaInfracciones = new List<Ext_Infracciones>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (INMOVILIZACION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (NRO_SALARIOS, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (VIGENTE, '" + parametros.ConsultaValor + "', 'i') ";
                }

                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY INMOVILIZACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY NRO_SALARIOS " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY VIGENTE " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                // strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                //if (strQuery == string.Empty)
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                //else
                //    strLimites = " AND DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttCiudades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaInfracciones", strConexion, "Nonquery");

                foreach (DataRow fila in dttCiudades.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", "'" + fila["CODIGO"].ToString() + "'").Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'");
                    listaInfracciones.Add(new Ext_Infracciones(fila, strBotonSelect));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaInfracciones });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Muestra los parqueaderos para un sitio de grabación
        /// </summary>
        /// <param name="idConsecutivo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult VerParqueaderos()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPersonas;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                if (usr == null)
                {
                    Exception ex = new Exception("Busca parqueaderos sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarParqueadero(CODIGO,DESCRIPCION,PARQUEADERO_SITIOGRABA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";
                List<Ext_V_Parqueaderos> listaParqueaderos = new List<Ext_V_Parqueaderos>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (CODIGO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (SITIO_GRABA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (NOMBRECIUDAD, '" + parametros.ConsultaValor + "', 'i') ";

                    strQuery += " AND SITIO_GRABA ='" + usr.SITIO_GRABACION + "'";
                }
                else
                {
                    strQuery += " WHERE SITIO_GRABA ='" + usr.SITIO_GRABACION + "'";
                }

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY SITIO_GRABA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY NOMBRECIUDAD " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttPersonas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaParqueaderos", strConexion, "Nonquery");

                foreach (DataRow fila in dttPersonas.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", "'" + fila["CODIGO"].ToString() + "'").Replace("DESCRIPCION", "'" + fila["DESCRIPCION"].ToString() + "'").Replace("PARQUEADERO_SITIOGRABA", "'" + fila["SITIO_GRABA"].ToString() + "'");
                    listaParqueaderos.Add(new Ext_V_Parqueaderos(fila, strBotonSelect));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaParqueaderos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Muestra las unidades para ese turno
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerUnidades()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPersonas;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var sitioGraba = Session["NUMERO_SITIO_GRABA"];
                var numerollamada = Session["NUMERO_LLAMADA"];
                if (usr == null || sitioGraba == null || numerollamada == null)
                {
                    Exception ex = new Exception("Busca unidades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarUnidad(CODIGO,DESCRIPCION,DESCR_UNIDAD)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";

                List<Ext_CadUnidadEvento> listaUnidades = new List<Ext_CadUnidadEvento>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                Int32 intNumeroLLamada = Convert.ToInt32(numerollamada);
                Int32 intSitioGraba = Convert.ToInt32(sitioGraba);

                var turnos = (from t in contexto.CADEVENTOS where t.CADPEDCA_NUMELLAMADA == intNumeroLLamada && t.SITIO_GRABA == intSitioGraba select t).FirstOrDefault();    // ObtenerTurnoAccesorio();
                                                                                                                                                                              // decimal
                                                                                                                                                                              // turnoN
                                                                                                                                                                              // = ObtenerTurnoNormal();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (COD_UNIDAD, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESC_UNIDAD, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (FUERZA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //  else
                //{
                //    strQuery += " WHERE SITIO_GRABA ='" + usr.SITIO_GRABACION + "'";

                //}

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESC_UNIDAD DESC";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY COD_UNIDAD " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY DESC_UNIDAD " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", usr.CADCANA_FUERZA_ID.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", usr.CADCANA_CODIGO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoA", turnos.CADTURN_TURNO_ACCESORIO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoN", turnos.CADTURN_TURNO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPersonas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaUnidadEvento", strConexion, "Nonquery");

                foreach (DataRow fila in dttPersonas.Rows)
                {
                    string strBotonSelect = strBoton.Replace("CODIGO", "'" + fila["FUERZA"].ToString() + "'").Replace("DESCRIPCION", "'" + fila["COD_UNIDAD"].ToString() + "'").Replace("DESCR_UNIDAD", "'" + fila["DESC_UNIDAD"].ToString() + "'");
                    listaUnidades.Add(new Ext_CadUnidadEvento(fila, strBotonSelect));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaUnidades });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Muestra las medios para ese turno
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerMedios(decimal dcmFuerzaUndeUnidad, decimal dcmCodigoUndeUnidad)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimites = "", strOrdenamiento = string.Empty;
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttPersonas;
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var sitioGraba = Session["NUMERO_SITIO_GRABA"];
                var numerollamada = Session["NUMERO_LLAMADA"];
                if (usr == null || sitioGraba == null || numerollamada == null)
                {
                    Exception ex = new Exception("Busca unidades sin sesion activa");
                    GestorBase.GestionarExcepcion(ex, GetType().Name);
                    return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
                }

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                       "onclick = \"CargarMedio(SITIO_GRABA,CONSECUTIVO,DESC_PATRULLA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                       " Seleccionar </button>";

                List<Ext_MediosVehiculosEvento> listaMedios = new List<Ext_MediosVehiculosEvento>();
                objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                Int32 intNumeroLLamada = Convert.ToInt32(numerollamada);
                Int32 intSitioGraba = Convert.ToInt32(sitioGraba);

                var turnos = (from t in contexto.CADEVENTOS where t.CADPEDCA_NUMELLAMADA == intNumeroLLamada && t.SITIO_GRABA == intSitioGraba select t).FirstOrDefault();    // ObtenerTurnoAccesorio();
                                                                                                                                                                              // decimal
                                                                                                                                                                              // turnoN
                                                                                                                                                                              // = ObtenerTurnoNormal();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery = " WHERE REGEXP_LIKE (ESTADO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (TIPO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ESTADO_PATRULLA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CUENTA, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (personal, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DESC_PATRULLA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //  else
                //{
                //    strQuery += " WHERE SITIO_GRABA ='" + usr.SITIO_GRABACION + "'";

                //}

                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrdenamiento += " ORDER BY DESC_PATRULLA DESC";
                        break;

                    case 2:
                        strOrdenamiento += " ORDER BY ESTADO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY DESC_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY ESTADO_PATRULLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY CUENTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY personal " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio.ToString() + " AND " + parametros.Final.ToString();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerzaUndeUnidad", dcmFuerzaUndeUnidad.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigoUndeUnidad", dcmCodigoUndeUnidad.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoA", turnos.CADTURN_TURNO_ACCESORIO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_turnoN", turnos.CADTURN_TURNO.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", usr.SITIO_GRABACION.ToString(), typeof(decimal)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttPersonas = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaMediosVehiculos", strConexion, "Nonquery");

                foreach (DataRow fila in dttPersonas.Rows)
                {
                    string strBotonSelect = strBoton.Replace("SITIO_GRABA", "'" + fila["SITIO_GRABA"].ToString() + "'").Replace("CONSECUTIVO", "'" + fila["CONSECUTIVO"].ToString() + "'").Replace("DESC_PATRULLA", "'" + fila["DESC_PATRULLA"].ToString() + "'");
                    listaMedios.Add(new Ext_MediosVehiculosEvento(fila, strBotonSelect));

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaMedios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Metodo que consulta la descripcion de una patrulla
        /// </summary>
        /// <param name="intcadMedisConsecutivo"></param>
        /// <returns></returns>
        public string ObtenerDescPatrulla(Int64? intcadMedisConsecutivo)
        {
            string patrulla = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cadMedisConsecutivo", intcadMedisConsecutivo.ToString(), typeof(decimal)));

                patrulla = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_descPatrullaMedios", strConexion);
                return patrulla;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Metodo que consulta la descripcion de una unidad
        /// </summary>
        /// <param name="intcadMedisConsecutivo"></param>
        /// <param name="intSitioGraba"></param>
        /// <returns></returns>
        public string ObtenerDescUnidad(Int64? intcadMedisConsecutivo, int? intSitioGraba)
        {
            string strUnidad = "";

            try
            {
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cadMedisConsecutivo", intcadMedisConsecutivo.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_sitioGraba", intSitioGraba.ToString(), typeof(decimal)));

                strUnidad = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_descUnidadMedios", strConexion);
                return strUnidad;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        #endregion Novedades
    }
}