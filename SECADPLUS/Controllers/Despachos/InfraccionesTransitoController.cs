using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class InfraccionesTransitoController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: InfraccionesTransito
        [SecadPlusAutorizador("/InfraccionesTransito/Index")]
        public ActionResult Index(string id, string nom, string strMensaje, string strTipo)
        {
            try
            {
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

                if (string.IsNullOrEmpty(id))
                {
                    id = db.CADINFRACCIONES_TRANSITO.Where(c => c.CADINFRACC_CODIGO == null).OrderBy(c => c.DESCRIPCION).Select(c => c.CODIGO).First().ToString();
                    nom = db.CADINFRACCIONES_TRANSITO.Where(c => c.CADINFRACC_CODIGO == null).OrderBy(c => c.DESCRIPCION).Select(c => c.DESCRIPCION).First().ToString();
                }

                var cADINFRACCIONES_TRANSITO_C = db.CADINFRACCIONES_TRANSITO.Include(c => c.CADINFRACCIONES_TRANSITO2).Where(c => c.CADINFRACC_CODIGO == null).OrderBy(c => c.DESCRIPCION);
                var cADINFRACCIONES_TRANSITO_T = db.CADINFRACCIONES_TRANSITO.Include(c => c.CADINFRACCIONES_TRANSITO2).Where(c => c.CADINFRACC_CODIGO == id).OrderBy(c => c.DESCRIPCION);

                var variosModelos = new Tuple<List<CADINFRACCIONES_TRANSITO>, List<CADINFRACCIONES_TRANSITO>>
                    (cADINFRACCIONES_TRANSITO_C.ToList(), cADINFRACCIONES_TRANSITO_T.ToList())
                { };

                ViewBag.COD_CAPITULO = id;
                ViewBag.NOM_CAPITULO = nom;


                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////



                return View(variosModelos);

                //return View(cADINFRACCIONES_TRANSITO.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetTitulos(string id)
        {
            try
            {
                var cADINFRACCIONES_TRANSITO_T = db.CADINFRACCIONES_TRANSITO.Include(c => c.CADINFRACCIONES_TRANSITO2).Where(c => c.CADINFRACC_CODIGO == id).OrderBy(c => c.DESCRIPCION);
                return PartialView("_titulos", cADINFRACCIONES_TRANSITO_T.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult EditarVigente(string id, bool checkVigente)
        {
            try
            {
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);

                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return Json(false);
                }

                if (checkVigente)
                    cADINFRACCIONES_TRANSITO.VIGENTE = "S";
                else
                    cADINFRACCIONES_TRANSITO.VIGENTE = "N";

                db.Entry(cADINFRACCIONES_TRANSITO).State = EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        [Authorize]
        public JsonResult EditaInmovilacion(string id, bool checkVigente)
        {
            try
            {
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);

                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return Json(false);
                }

                if (checkVigente)
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "S";
                else
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "N";

                db.Entry(cADINFRACCIONES_TRANSITO).State = EntityState.Modified;
                db.SaveChanges();

                return Json(true);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: InfraccionesTransito/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,DESCRIPCION,FECHA_DESDE,FECHA_HASTA,VIGENTE,CADINFRACC_CODIGO,INMOVILIZACION,NRO_SALARIOS")] CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO)
        {
            try
            {
                var existe = db.CADINFRACCIONES_TRANSITO.Find(cADINFRACCIONES_TRANSITO.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                if (cADINFRACCIONES_TRANSITO.NRO_SALARIOS == null)
                {
                    ModelState.AddModelError("NRO_SALARIOS", "El campo Nro. Salarios es obligatorio.");
                }

                if (ModelState.IsValid)
                {
                    if (cADINFRACCIONES_TRANSITO.VIGENTE == "true")
                        cADINFRACCIONES_TRANSITO.VIGENTE = "S";
                    else
                        cADINFRACCIONES_TRANSITO.VIGENTE = "N";

                    db.CADINFRACCIONES_TRANSITO.Add(cADINFRACCIONES_TRANSITO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE;
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION", cADINFRACCIONES_TRANSITO.CADINFRACC_CODIGO);
                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: InfraccionesTransito/CreateT
        [Authorize]
        public ActionResult CreateT(string id, string nom)
        {
            try
            {
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.CADINFRACC_CODIGO = id;
                ViewBag.CAPITULO = nom;
                //ViewBag.NRO_SALARIOS = 0;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateT(string id, string nom, [Bind(Include = "CODIGO,DESCRIPCION,FECHA_DESDE,FECHA_HASTA,VIGENTE,CADINFRACC_CODIGO,INMOVILIZACION,NRO_SALARIOS")] CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO)
        {
            try
            {
                var existe = db.CADINFRACCIONES_TRANSITO.Find(cADINFRACCIONES_TRANSITO.CODIGO);

                if (existe != null)
                    ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");

                if (cADINFRACCIONES_TRANSITO.VIGENTE == "true")
                    cADINFRACCIONES_TRANSITO.VIGENTE = "S";
                else
                    cADINFRACCIONES_TRANSITO.VIGENTE = "N";

                if (cADINFRACCIONES_TRANSITO.INMOVILIZACION == "true")
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "S";
                else
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "N";

                //cADINFRACCIONES_TRANSITO.NRO_SALARIOS = null;

                if (ModelState.IsValid)
                {
                    db.CADINFRACCIONES_TRANSITO.Add(cADINFRACCIONES_TRANSITO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION");
                ViewBag.id = id;
                ViewBag.nom = nom;
                ViewBag.CADINFRACC_CODIGO = id;
                ViewBag.CAPITULO = nom;
                //ViewBag.NRO_SALARIOS = 0;
                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE;
                ViewBag.INMOVILIZACION = cADINFRACCIONES_TRANSITO.INMOVILIZACION;

                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: InfraccionesTransito/Edit/5
        [Authorize]
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.antiguoCodigo = cADINFRACCIONES_TRANSITO.CODIGO;
                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE == "S" ? true : false;
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION", cADINFRACCIONES_TRANSITO.CADINFRACC_CODIGO);
                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, string nom, [Bind(Include = "CODIGO,DESCRIPCION,FECHA_DESDE,FECHA_HASTA,VIGENTE,CADINFRACC_CODIGO,INMOVILIZACION,NRO_SALARIOS")] CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO)
        {
            try
            {
                var antiguoCodigo = Request["antiguoCodigo"];

                if (antiguoCodigo != cADINFRACCIONES_TRANSITO.CODIGO)
                {
                    var existe = db.CADINFRACCIONES_TRANSITO.Find(cADINFRACCIONES_TRANSITO.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");
                }

                if (cADINFRACCIONES_TRANSITO.VIGENTE == "true")
                    cADINFRACCIONES_TRANSITO.VIGENTE = "S";
                else
                    cADINFRACCIONES_TRANSITO.VIGENTE = "N";

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADINFRACCIONES_TRANSITO.CODIGO)
                    {
                        db.Entry(cADINFRACCIONES_TRANSITO).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO1 = db.CADINFRACCIONES_TRANSITO.Find(antiguoCodigo);
                        db.CADINFRACCIONES_TRANSITO.Remove(cADINFRACCIONES_TRANSITO1);
                        db.SaveChanges();

                        //cADINFRACCIONES_TRANSITO.CODIGO = nuevoCodigo;

                        db.CADINFRACCIONES_TRANSITO.Add(cADINFRACCIONES_TRANSITO);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = id, nom = nom, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }
                ViewBag.antiguoCodigo = antiguoCodigo;
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION", cADINFRACCIONES_TRANSITO.CADINFRACC_CODIGO);
                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE;
                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: InfraccionesTransito/EditT/5
        [Authorize]
        public ActionResult EditT(string id, string idCap, string nomCap)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.antiguoCodigo = cADINFRACCIONES_TRANSITO.CODIGO;
                ViewBag.idCap = idCap;
                ViewBag.nomCap = nomCap;
                ViewBag.CADINFRACC_CODIGO = idCap;
                ViewBag.CAPITULO = nomCap;
                ViewBag.NRO_SALARIOS = 0;
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION", cADINFRACCIONES_TRANSITO.CADINFRACC_CODIGO);
                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE == "S" ? true : false;
                ViewBag.INMOVILIZACION = cADINFRACCIONES_TRANSITO.INMOVILIZACION == "S" ? true : false;

                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/EditT/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditT(string id, string idCap, string nomCap, [Bind(Include = "CODIGO,DESCRIPCION,FECHA_DESDE,FECHA_HASTA,VIGENTE,CADINFRACC_CODIGO,INMOVILIZACION,NRO_SALARIOS")] CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO)
        {
            try
            {
                var antiguoCodigo = Request["antiguoCodigo"];

                if (antiguoCodigo != cADINFRACCIONES_TRANSITO.CODIGO)
                {
                    var existe = db.CADINFRACCIONES_TRANSITO.Find(cADINFRACCIONES_TRANSITO.CODIGO);

                    if (existe != null)
                        ModelState.AddModelError("CODIGO", "Este Código esta siendo utilizado en otro registro.");
                }

                if (cADINFRACCIONES_TRANSITO.VIGENTE == "true")
                    cADINFRACCIONES_TRANSITO.VIGENTE = "S";
                else
                    cADINFRACCIONES_TRANSITO.VIGENTE = "N";

                if (cADINFRACCIONES_TRANSITO.INMOVILIZACION == "true")
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "S";
                else
                    cADINFRACCIONES_TRANSITO.INMOVILIZACION = "N";

                if (ModelState.IsValid)
                {
                    if (antiguoCodigo == cADINFRACCIONES_TRANSITO.CODIGO)
                    {
                        db.Entry(cADINFRACCIONES_TRANSITO).State = EntityState.Modified;
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }
                    else
                    {
                        CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO1 = db.CADINFRACCIONES_TRANSITO.Find(antiguoCodigo);
                        db.CADINFRACCIONES_TRANSITO.Remove(cADINFRACCIONES_TRANSITO1);
                        db.SaveChanges();

                        //cADINFRACCIONES_TRANSITO.CODIGO = nuevoCodigo;

                        db.CADINFRACCIONES_TRANSITO.Add(cADINFRACCIONES_TRANSITO);
                        db.SaveChanges();

                        auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Modificar);
                    }

                    var mensaje = "Ha sido editado el registro: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { id = idCap, nom = nomCap, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }

                ViewBag.antiguoCodigo = antiguoCodigo;
                ViewBag.idCap = idCap;
                ViewBag.nomCap = nomCap;
                ViewBag.CADINFRACC_CODIGO = idCap;
                ViewBag.CAPITULO = nomCap;
                ViewBag.NRO_SALARIOS = 0;
                //ViewBag.CADINFRACC_CODIGO = new SelectList(db.CADINFRACCIONES_TRANSITO, "CODIGO", "DESCRIPCION", cADINFRACCIONES_TRANSITO.CADINFRACC_CODIGO);
                ViewBag.VIGENTE = cADINFRACCIONES_TRANSITO.VIGENTE;
                ViewBag.INMOVILIZACION = cADINFRACCIONES_TRANSITO.INMOVILIZACION;

                return View(cADINFRACCIONES_TRANSITO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Eliminar(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                db.CADINFRACCIONES_TRANSITO.Remove(cADINFRACCIONES_TRANSITO);
                db.SaveChanges();

                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO_copy = new CADINFRACCIONES_TRANSITO();
                cADINFRACCIONES_TRANSITO_copy = (CADINFRACCIONES_TRANSITO)auditoria.GenerarCopiaAuditoria(cADINFRACCIONES_TRANSITO);

                auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO_copy, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el caso: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
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
        public ActionResult EliminarT(string id, string idCap, string nomCap)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                if (cADINFRACCIONES_TRANSITO == null)
                {
                    return HttpNotFound();
                }
                ViewBag.idCap = idCap;
                ViewBag.nomCap = nomCap;
                ViewBag.CADINFRACC_CODIGO = idCap;
                ViewBag.CAPITULO = nomCap;
                return View("DeleteT", null, cADINFRACCIONES_TRANSITO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: InfraccionesTransito/Delete/5
        [HttpPost, ActionName("DeleteT")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedT(string id, string idCap, string nomCap)
        {
            try
            {
                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO = db.CADINFRACCIONES_TRANSITO.Find(id);
                db.CADINFRACCIONES_TRANSITO.Remove(cADINFRACCIONES_TRANSITO);
                db.SaveChanges();

                CADINFRACCIONES_TRANSITO cADINFRACCIONES_TRANSITO_copy = new CADINFRACCIONES_TRANSITO();
                cADINFRACCIONES_TRANSITO_copy = (CADINFRACCIONES_TRANSITO)auditoria.GenerarCopiaAuditoria(cADINFRACCIONES_TRANSITO);

                auditoria.RegistrarAuditoria<CADINFRACCIONES_TRANSITO>(cADINFRACCIONES_TRANSITO_copy, "CADINFRACCIONES_TRANSITO", cADINFRACCIONES_TRANSITO.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el caso: " + cADINFRACCIONES_TRANSITO.DESCRIPCION.ToString();
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
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