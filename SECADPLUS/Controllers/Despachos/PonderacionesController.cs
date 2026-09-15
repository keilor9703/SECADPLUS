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
    public class PonderacionesController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));

        // GET: Ponderaciones
        [SecadPlusAutorizador("/Ponderaciones/Index")]
        public ActionResult Index(string idTipo, string nomTipo, int? idTitu, string nomTitu, int? idCond, string nomCond, string idPond, string strMensaje, string strTipo)
        {
            try //Arreglar los hidden y Carga Total este y anterior
            {
                var mensaje = "";
                var tipo = "";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
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

                if (string.IsNullOrEmpty(idTipo))
                {
                    idTipo = "SP";
                    nomTipo = "SERVICIOS DE POLICIA";

                    //idTipo = "DE";
                    //nomTipo = "DELITOS";
                }

                if (idTitu == null)
                {
                    //idTitu = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(m => m.CODIGO).Select(m => m.CODIGO).FirstOrDefault().ToString();
                    //nomTitu = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(m => m.CODIGO).Select(m => m.DESCRIPCION).FirstOrDefault().ToString();

                    idTitu = 0;
                    nomTitu = "";
                }

                if (idCond == null)
                {
                    //idCond = int.Parse(db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 2).OrderBy(m => m.CODIGO).Select(m => m.CODIGO).FirstOrDefault().ToString());
                    //nomCond = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 2).OrderBy(m => m.CODIGO).Select(m => m.DESCRIPCION).FirstOrDefault().ToString();

                    idCond = 0;
                    nomCond = "";
                }

                if (string.IsNullOrEmpty(idPond))
                {
                    //idPond = db.CADCONDCLASESBIENs.Where(c => c.COND_CODIGO == idCond).OrderBy(m => m.CONSECUTIVO).Select(m => m.CONSECUTIVO).FirstOrDefault().ToString();
                    idPond = "";
                }

                //var idTitulo = int.Parse(idTitu);

                ViewBag.TIPO = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "TIPO_CONDUCTA").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", idTipo);
                var cADCONDUCTA_T = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(c => c.CODIGO);
                var cADCONDUCTA_C = db.CADCONDUCTAS.Where(c => c.COND_CODIGO == idTitu && c.NIVEL == 2).OrderBy(c => c.CODIGO);
                var cADCONDCLASESBIEN = db.CADCONDCLASESBIENs.Where(c => c.COND_CODIGO == idCond).OrderBy(c => c.COND_CODIGO);
                //var cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.Where(c => c.CADCONDCLBICONS == idPond).OrderBy(c => c.CONSECUTIVO);

                var cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.SqlQuery("Select CADCONDCLBICONS, CONSECUTIVO, " +
                                                              "decode(CLASE_PONDERADO, 'CA', 'CASO', 'EL', 'ELEMENTO', 'PE','PERSONA') as CLASE_PONDERADO, " +
                                                              "VLR_MINIMO, VLR_MAXIMO, VLR_UNIDAD, VLR_PONDERADO, " +
                                                              "PK_CAD.f_referenciassecad('MODO_CONDUCTA', MODOCONDUCTA) As MODOCONDUCTA " +
                                                              "From CADCONDCLBIPONDERADO " +
                                                              "Where CADCONDCLBICONS = '" + idPond + "'" +
                                                              "Order by CONSECUTIVO");

                var variosModelos = new Tuple<List<CADCONDUCTA>, List<CADCONDUCTA>, List<CADCONDCLASESBIEN>, List<CADCONDCLBIPONDERADO>>
                    (cADCONDUCTA_T.ToList(), cADCONDUCTA_C.ToList(), cADCONDCLASESBIEN.ToList(), cADCONDCLBIPONDERADO.ToList())
                { };

                ViewBag.NOM_TITULO = nomTitu;

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.NOM_CONDUCTA = nomCond;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.NOM_PONDERAR = idPond;
                ViewBag.idPond = idPond;

                //return View(cADCONDUCTA.ToList());
                return View(variosModelos);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetClasesBien(int? id)
        {
            try
            {
                List<CADCLASES_BIEN> clasesBien = new List<CADCLASES_BIEN>();

                if (id != null)
                {
                    clasesBien = db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == id).OrderBy(m => m.DESCRIPCION).ToList();
                }
                else
                {
                    clasesBien.Insert(0, new CADCLASES_BIEN { CODIGO = 0, DESCRIPCION = " " });
                }

                var clase = (from r in clasesBien
                             select new
                             {
                                 CODIGO = r.CODIGO,
                                 DESCRIPCION = r.DESCRIPCION
                             }).ToList();

                return Json(clase);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        [Authorize]
        public ActionResult GetTitulos(string idTipo, string nomTipo)
        {
            try
            {
                ViewBag.nomTipo = nomTipo;
                var cADCONDUCTA = db.CADCONDUCTAS.Where(c => c.TIPO == idTipo && c.NIVEL == 1).OrderBy(c => c.CODIGO);
                return PartialView("_titulos", cADCONDUCTA.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetConductas(int idTitu, string nomTitu)
        {
            try
            {
                ViewBag.nomTitu = nomTitu;
                var cADCONDUCTA = db.CADCONDUCTAS.Where(c => c.COND_CODIGO == idTitu && c.NIVEL == 2).OrderBy(c => c.CODIGO);
                return PartialView("_conductas", cADCONDUCTA.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetPonderar(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond)
        {
            try
            {
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                var cADCONDCLASESBIEN = db.CADCONDCLASESBIENs.Where(c => c.COND_CODIGO == idCond).OrderBy(c => c.COND_CODIGO);

                return PartialView("_ponderar", cADCONDCLASESBIEN.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult GetDetallePonderar(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond)
        {
            try
            {
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;

                //var cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.Where(c => c.CADCONDCLBICONS == idPond);
                var cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.SqlQuery("Select CADCONDCLBICONS, CONSECUTIVO, " +
                                                                              "decode(CLASE_PONDERADO, 'CA', 'CASO', 'EL', 'ELEMENTO', 'PE','PERSONA') as CLASE_PONDERADO, " +
                                                                              "VLR_MINIMO, VLR_MAXIMO, VLR_UNIDAD, VLR_PONDERADO, " +
                                                                              "PK_CAD.f_referenciassecad('MODO_CONDUCTA', MODOCONDUCTA) As MODOCONDUCTA " +
                                                                              "From CADCONDCLBIPONDERADO " +
                                                                              "Where CADCONDCLBICONS = '" + idPond + "'" +
                                                                              "Order by CONSECUTIVO");

                return PartialView("_detallePonderar", cADCONDCLBIPONDERADO.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ponderaciones/CreateP
        [Authorize]
        public ActionResult CreateP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond)
        {
            try
            {
                ViewBag.CONSECUTIVO = "0";

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CONDUCTA = idTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;
                ViewBag.COND_CODIGO = idCond;
                ViewBag.NOM_CONDUCTA = nomCond;

                var idTibi = db.CADTIPOS_BIEN.OrderBy(m => m.DESCRIPCION).Select(m => m.CODIGO).FirstOrDefault();
                ViewBag.CLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == idTibi).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/CreateP To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, [Bind(Include = "CONSECUTIVO,TIPO_CONDUCTA,COND_CODIGO,CLBI_TIBI_CODIGO,CLBI_CODIGO")] CADCONDCLASESBIEN cADCONDCLASESBIEN)
        {
            try
            {
                var a = cADCONDCLASESBIEN.COND_CODIGO.ToString().PadLeft(6, '0');
                var b = cADCONDCLASESBIEN.CLBI_TIBI_CODIGO.ToString().PadLeft(4, '0');
                var c = cADCONDCLASESBIEN.CLBI_CODIGO.ToString().PadLeft(4, '0');

                cADCONDCLASESBIEN.CONSECUTIVO = a + b + c;

                if (ModelState.IsValid)
                {
                    db.CADCONDCLASESBIENs.Add(cADCONDCLASESBIEN);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCONDCLASESBIEN>(cADCONDCLASESBIEN, "CADCONDCLASESBIEN", cADCONDCLASESBIEN.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: ";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond });
                }

                ViewBag.CONSECUTIVO = "0";

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CONDUCTA = idTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;
                ViewBag.COND_CODIGO = idCond;
                ViewBag.NOM_CONDUCTA = nomCond;

                var idTibi = db.CADTIPOS_BIEN.OrderBy(m => m.DESCRIPCION).Select(m => m.CODIGO).FirstOrDefault();
                ViewBag.CLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(m => new { CODIGO = m.CODIGO, DESCRIPCION = m.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.CLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == idTibi).Select(m => new { CODIGO = m.CODIGO, DESCRIPCION = m.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                return View(cADCONDCLASESBIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ponderaciones/EditP/5
        [Authorize]
        public ActionResult EditP(string id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDCLASESBIEN cADCONDCLASESBIEN = db.CADCONDCLASESBIENs.Find(id);
                if (cADCONDCLASESBIEN == null)
                {
                    return HttpNotFound();
                }
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CONDUCTA = idTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;
                ViewBag.COND_CODIGO = idCond;
                ViewBag.NOM_CONDUCTA = nomCond;

                //var idTibi = db.CADTIPOS_BIEN.OrderBy(m => m.DESCRIPCION).Select(m => m.CODIGO).FirstOrDefault();
                ViewBag.CLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLASESBIEN.CLBI_TIBI_CODIGO);
                ViewBag.CLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == cADCONDCLASESBIEN.CLBI_TIBI_CODIGO).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLASESBIEN.CLBI_CODIGO);

                return View(cADCONDCLASESBIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, [Bind(Include = "CONSECUTIVO,TIPO_CONDUCTA,COND_CODIGO,CLBI_TIBI_CODIGO,CLBI_CODIGO")] CADCONDCLASESBIEN cADCONDCLASESBIEN)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADCONDCLASESBIEN).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDCLASESBIEN>(cADCONDCLASESBIEN, "CADCONDCLASESBIEN", cADCONDCLASESBIEN.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro: ";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond });
                }
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CONDUCTA = idTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;
                ViewBag.COND_CODIGO = idCond;
                ViewBag.NOM_CONDUCTA = nomCond;

                //var idTibi = db.CADTIPOS_BIEN.OrderBy(m => m.DESCRIPCION).Select(m => m.CODIGO).FirstOrDefault();
                ViewBag.CLBI_TIBI_CODIGO = new SelectList(db.CADTIPOS_BIEN.Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLASESBIEN.CLBI_TIBI_CODIGO);
                ViewBag.CLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == cADCONDCLASESBIEN.CLBI_TIBI_CODIGO).Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLASESBIEN.CLBI_CODIGO);

                return View(cADCONDCLASESBIEN);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult EliminarP(string id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDCLASESBIEN cADCONDCLASESBIEN = db.CADCONDCLASESBIENs.Find(id);
                if (cADCONDCLASESBIEN == null)
                {
                    return HttpNotFound();
                }

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;
                ViewBag.TIPO_CONDUCTA = idTipo;
                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;
                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;
                ViewBag.COND_CODIGO = idCond;
                ViewBag.NOM_CONDUCTA = nomCond;

                //ViewBag.CLBI_TIBI_CODIGO = db.CADTIPOS_BIEN.Where(m => m.CODIGO == cADCONDCLASESBIEN.CLBI_TIBI_CODIGO).Select(m => m.DESCRIPCION);
                //ViewBag.CLBI_CODIGO = db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == cADCONDCLASESBIEN.CLBI_TIBI_CODIGO && m.CODIGO == cADCONDCLASESBIEN.CLBI_CODIGO).Select(m => m.DESCRIPCION);

                return View("DeleteP", null, cADCONDCLASESBIEN);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/DeleteP/5
        [HttpPost, ActionName("DeleteP")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedP(string id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond)
        {
            try
            {
                CADCONDCLASESBIEN cADCONDCLASESBIEN = db.CADCONDCLASESBIENs.Find(id);
                db.CADCONDCLASESBIENs.Remove(cADCONDCLASESBIEN);
                db.SaveChanges();

                CADCONDCLASESBIEN cADCONDCLASESBIEN_copy = new CADCONDCLASESBIEN();
                cADCONDCLASESBIEN_copy = (CADCONDCLASESBIEN)auditoria.GenerarCopiaAuditoria(cADCONDCLASESBIEN);

                auditoria.RegistrarAuditoria<CADCONDCLASESBIEN>(cADCONDCLASESBIEN_copy, "CADCONDCLASESBIEN", cADCONDCLASESBIEN.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro: ";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ponderaciones/CreateDP
        [SecadPlusAutorizador("/Ponderaciones/Crear")]
        public ActionResult CreateDP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond)
        {
            try
            {
                //
                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;
                ViewBag.CADCONDCLBICONS = idPond;

                ViewBag.CLASE_PONDERADO = new SelectList(new[] { new { value = "CA" , text = "CASO" },
                                                             new { value = "EL" , text = "ELEMENTO" },
                                                             new { value = "PE" , text = "PERSONA"} }, "value", "text");

                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                // return RedirectToAction("Index", "Home");
                //ViewBag.CLASE_PONDERADO = new SelectList(listaClasePonderado, "CODIGO", "DESCRIPCION");
                //ViewBag.CADCONDCLBICONS = new SelectList(db.CADCONDCLASESBIENs, "CONSECUTIVO", "TIPO_CONDUCTA");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/CreateDP To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateDP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond, [Bind(Include = "CADCONDCLBICONS,CONSECUTIVO,CLASE_PONDERADO,VLR_MINIMO,VLR_MAXIMO,VLR_UNIDAD,VLR_PONDERADO,MODOCONDUCTA")] CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO)
        {
            try
            {
                Int16 CODIGO;
                var existe = db.CADCONDCLBIPONDERADOes.FirstOrDefault(c => c.CADCONDCLBICONS == cADCONDCLBIPONDERADO.CADCONDCLBICONS);

                if (existe == null)
                    CODIGO = 1;
                else
                {
                    CODIGO = db.CADCONDCLBIPONDERADOes.Where(c => c.CADCONDCLBICONS == cADCONDCLBIPONDERADO.CADCONDCLBICONS).Max(c => c.CONSECUTIVO);
                    CODIGO += 1;
                }

                cADCONDCLBIPONDERADO.CONSECUTIVO = CODIGO;

                if (ModelState.IsValid)
                {
                    db.CADCONDCLBIPONDERADOes.Add(cADCONDCLBIPONDERADO);
                    db.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADCONDCLBIPONDERADO>(cADCONDCLBIPONDERADO, "CADCONDCLBIPONDERADO", cADCONDCLBIPONDERADO.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro: ";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond });
                }

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;
                ViewBag.CADCONDCLBICONS = idPond;

                ViewBag.CLASE_PONDERADO = new SelectList(new[] { new { value = "CA" , text = "CASO" },
                                                             new { value = "EL" , text = "ELEMENTO" },
                                                             new { value = "PE" , text = "PERSONA"} }, "value", "text", cADCONDCLBIPONDERADO.CLASE_PONDERADO);

                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLBIPONDERADO.MODOCONDUCTA);

                //ViewBag.CADCONDCLBICONS = new SelectList(db.CADCONDCLASESBIENs, "CONSECUTIVO", "TIPO_CONDUCTA", cADCONDCLBIPONDERADO.CADCONDCLBICONS);
                return View(cADCONDCLBIPONDERADO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Ponderaciones/EditDP/5
        [Authorize]
        public ActionResult EditDP(int? id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.Find(idPond, id);
                if (cADCONDCLBIPONDERADO == null)
                {
                    return HttpNotFound();
                }

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;
                ViewBag.CADCONDCLBICONS = idPond;

                ViewBag.CLASE_PONDERADO = new SelectList(new[] { new { value = "CA" , text = "CASO" },
                                                             new { value = "EL" , text = "ELEMENTO" },
                                                             new { value = "PE" , text = "PERSONA"} }, "value", "text", cADCONDCLBIPONDERADO.CLASE_PONDERADO);

                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADCONDCLBIPONDERADO.MODOCONDUCTA);

                //ViewBag.CADCONDCLBICONS = new SelectList(db.CADCONDCLASESBIENs, "CONSECUTIVO", "TIPO_CONDUCTA", cADCONDCLBIPONDERADO.CADCONDCLBICONS);
                return View(cADCONDCLBIPONDERADO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/EditDP/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDP(string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond, [Bind(Include = "CADCONDCLBICONS,CONSECUTIVO,CLASE_PONDERADO,VLR_MINIMO,VLR_MAXIMO,VLR_UNIDAD,VLR_PONDERADO,MODOCONDUCTA")] CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADCONDCLBIPONDERADO).State = EntityState.Modified;
                    db.SaveChanges();

                    auditoria.RegistrarAuditoria<CADCONDCLBIPONDERADO>(cADCONDCLBIPONDERADO, "CADCONDCLBIPONDERADO", cADCONDCLBIPONDERADO.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro: ";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond, strMensaje = mensaje, strTipo = tipo });
                    //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond });
                }

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;
                ViewBag.CADCONDCLBICONS = idPond;

                ViewBag.CLASE_PONDERADO = new SelectList(new[] { new { value = "CA" , text = "CASO" },
                                                             new { value = "EL" , text = "ELEMENTO" },
                                                             new { value = "PE" , text = "PERSONA"} }, "value", "text", cADCONDCLBIPONDERADO.CLASE_PONDERADO);

                ViewBag.MODOCONDUCTA = new SelectList(db.CADREFERENCIASSECADs.Where(m => m.NOMBRE == "MODO_CONDUCTA").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION }).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");

                //ViewBag.CADCONDCLBICONS = new SelectList(db.CADCONDCLASESBIENs, "CONSECUTIVO", "TIPO_CONDUCTA", cADCONDCLBIPONDERADO.CADCONDCLBICONS);
                return View(cADCONDCLBIPONDERADO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult EliminarDP(int? id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.Find(idPond, id);
                if (cADCONDCLBIPONDERADO == null)
                {
                    return HttpNotFound();
                }

                ViewBag.idTipo = idTipo;
                ViewBag.nomTipo = nomTipo;

                ViewBag.idTitu = idTitu;
                ViewBag.nomTitu = nomTitu;

                ViewBag.idCond = idCond;
                ViewBag.nomCond = nomCond;

                ViewBag.idPond = idPond;

                return View("DeleteDP", null, cADCONDCLBIPONDERADO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Ponderaciones/DeleteDP/5
        [HttpPost, ActionName("DeleteDP")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedDP(int? id, string idTipo, string nomTipo, int idTitu, string nomTitu, int idCond, string nomCond, string idPond)
        {
            try
            {
                CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO = db.CADCONDCLBIPONDERADOes.Find(idPond, id);
                db.CADCONDCLBIPONDERADOes.Remove(cADCONDCLBIPONDERADO);
                db.SaveChanges();


                CADCONDCLBIPONDERADO cADCONDCLBIPONDERADO_copy = new CADCONDCLBIPONDERADO();
                cADCONDCLBIPONDERADO_copy = (CADCONDCLBIPONDERADO)auditoria.GenerarCopiaAuditoria(cADCONDCLBIPONDERADO);


                auditoria.RegistrarAuditoria<CADCONDCLBIPONDERADO>(cADCONDCLBIPONDERADO_copy, "CADCONDCLBIPONDERADO", cADCONDCLBIPONDERADO.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro ";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond, strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, idPond = idPond });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { idTipo = idTipo, nomTipo = nomTipo, idTitu = idTitu, nomTitu = nomTitu, idCond = idCond, nomCond = nomCond, strMensaje = mensaje, strTipo = tipo });
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