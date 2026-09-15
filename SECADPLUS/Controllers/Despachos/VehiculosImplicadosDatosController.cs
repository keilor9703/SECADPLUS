using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class VehiculosImplicadosDatosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

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

        // GET: VehiculosImplicadosDatos/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });

                //// El sitio de grabación por medio de autenticación  //Validar que el usuario tenga un sitio de grabación para realizar todo.
                //ViewBag.SITIO_GRABA = 3947;
                ////
                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;
                //

                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var usuario = usr.USUARIO;
                var idSitio = usr.SITIO_GRABACION;

                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                ViewBag.LINEA_CODIGO = new SelectList(listaLineasMarcas, "CODIGO", "DESCRIPCION");
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.USUA_TRANSACCION = User.Identity.GetUserName();
                ViewBag.USUA_TRANSACCION = usuario;
                ViewBag.SITIO_GRABA = idSitio;
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosDatos/Create To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,OBSERVACIONES,USUA_TRANSACCION,FECHA_TRANSACCION")] CADBIENE cADBIENE)
        {
            try
            {
                CADBIENE existe;
                existe = db.CADBIENES.SingleOrDefault(c => c.PLACA == cADBIENE.PLACA);

                if (cADBIENE.LINEA_CODIGO == 0)
                {
                    ModelState.AddModelError("LINEA_CODIGO", "El campo Línea es obligatorio.");
                }

                if (existe != null)
                    ModelState.AddModelError("PLACA", "Esta placa esta siendo utilizado en otro registro.");

                existe = db.CADBIENES.SingleOrDefault(c => c.CHASIS == cADBIENE.CHASIS);

                if (existe != null)
                    ModelState.AddModelError("CHASIS", "Este chasis esta siendo utilizado en otro registro.");

                existe = db.CADBIENES.SingleOrDefault(c => c.MOTOR == cADBIENE.MOTOR);

                if (existe != null)
                    ModelState.AddModelError("MOTOR", "Este motor esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    //long CONSECUTIVO = db.CADBIENES.Where(m => m.SITIO_GRABA == cADBIENE.SITIO_GRABA).Max(m => m.CONSECUTIVO);
                    //CONSECUTIVO += 1;
                    //cADBIENE.CONSECUTIVO = CONSECUTIVO;

                    GestorAdministrador gestor = new GestorAdministrador();
                    var CONSECUTIVO = gestor.ConsultarConsecutivoBienes(strConexion);
                    cADBIENE.CONSECUTIVO = CONSECUTIVO;

                    ////Eliminar cuando funcione autenticación
                    //cADBIENE.USUA_TRANSACCION = "USR_CAD";
                    ////
                    cADBIENE.FECHA_TRANSACCION = DateTime.Now;

                    db.CADBIENES.Add(cADBIENE);
                    await db.SaveChangesAsync();

                    //auditoria.RegistrarAuditoria<CADBIENE>(cADBIENE, "CADBIENES", cADBIENE.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "VehiculosImplicados", new { strMensaje = mensaje, strTipo = tipo });
                }

                //List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                //listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });

                // El sitio de grabación por medio de autenticación
                ViewBag.SITIO_GRABA = cADBIENE.SITIO_GRABA;
                ViewBag.CADCLBI_TIBI_CODIGO = 4;
                ViewBag.CADCLBI_CODIGO = new SelectList(db.CADCLASES_BIEN.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENE.CADCLBI_CODIGO);
                ViewBag.MARCA_CODIGO = new SelectList(db.CADMARCAS.Where(m => m.TIBI_CODIGO == 4).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENE.MARCA_CODIGO);

                if (cADBIENE.LINEA_CODIGO == 0)
                {
                    List<CADLINEAS_MARCA> listaLineasMarcas = new List<CADLINEAS_MARCA>();
                    listaLineasMarcas.Insert(0, new CADLINEAS_MARCA { CODIGO = 0, DESCRIPCION = " " });
                    ViewBag.LINEA_CODIGO = new SelectList(listaLineasMarcas, "CODIGO", "DESCRIPCION");
                }
                else
                {
                    ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA.Where(m => m.MARC_CODIGO == cADBIENE.MARCA_CODIGO).OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENE.LINEA_CODIGO);
                }

                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES.OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION", cADBIENE.COLO_CODIGO);
                // El usuario por medio de autenticación
                ViewBag.USUA_TRANSACCION = cADBIENE.USUA_TRANSACCION;
                return View(cADBIENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: VehiculosImplicadosDatos/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENE cADBIENE = await db.CADBIENES.FindAsync(id);
                if (cADBIENE == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADCLASES_BIEN, "TIBI_CODIGO", "DESCRIPCION", cADBIENE.CADCLBI_TIBI_CODIGO);
                ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA, "CODIGO", "DESCRIPCION", cADBIENE.LINEA_CODIGO);
                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", cADBIENE.SITIO_GRABA);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES, "CODIGO", "DESCRIPCION", cADBIENE.COLO_CODIGO);
                return View(cADBIENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosDatos/Edit/5 To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SITIO_GRABA,CONSECUTIVO,CADCLBI_TIBI_CODIGO,CADCLBI_CODIGO,MARCA_CODIGO,LINEA_CODIGO,COLO_CODIGO,PLACA,CHASIS,MOTOR,SERIE,OBSERVACIONES,USUA_TRANSACCION,FECHA_TRANSACCION")] CADBIENE cADBIENE)
        {
            try
            {
                CADBIENE existe;
                existe = db.CADBIENES.SingleOrDefault(c => c.PLACA == cADBIENE.PLACA);

                if (existe != null)
                    ModelState.AddModelError("PLACA", "Esta placa esta siendo utilizado en otro registro.");

                existe = db.CADBIENES.SingleOrDefault(c => c.CHASIS == cADBIENE.CHASIS);

                if (existe != null)
                    ModelState.AddModelError("CHASIS", "Este chasis esta siendo utilizado en otro registro.");

                existe = db.CADBIENES.SingleOrDefault(c => c.MOTOR == cADBIENE.MOTOR);

                if (existe != null)
                    ModelState.AddModelError("MOTOR", "Este motor esta siendo utilizado en otro registro.");

                if (ModelState.IsValid)
                {
                    db.Entry(cADBIENE).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADBIENE>(cADBIENE, "CADBIENES", cADBIENE.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }
                ViewBag.CADCLBI_TIBI_CODIGO = new SelectList(db.CADCLASES_BIEN, "TIBI_CODIGO", "DESCRIPCION", cADBIENE.CADCLBI_TIBI_CODIGO);
                ViewBag.LINEA_CODIGO = new SelectList(db.CADLINEAS_MARCA, "CODIGO", "DESCRIPCION", cADBIENE.LINEA_CODIGO);
                ViewBag.SITIO_GRABA = new SelectList(db.CADSITIOS_GRABACION, "CONSECUTIVO", "DESCRIPCION", cADBIENE.SITIO_GRABA);
                ViewBag.COLO_CODIGO = new SelectList(db.CADCOLORES, "CODIGO", "DESCRIPCION", cADBIENE.COLO_CODIGO);
                return View(cADBIENE);
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

        // GET: VehiculosImplicadosDatos/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENE cADBIENE = await db.CADBIENES.FindAsync(id);
                if (cADBIENE == null)
                {
                    return HttpNotFound();
                }
                return View(cADBIENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosDatos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CADBIENE cADBIENE = await db.CADBIENES.FindAsync(id);
                db.CADBIENES.Remove(cADBIENE);
                await db.SaveChangesAsync();

                CADBIENE cADBIENE_copy = new CADBIENE();
                cADBIENE_copy = (CADBIENE)auditoria.GenerarCopiaAuditoria(cADBIENE);

                auditoria.RegistrarAuditoria<CADBIENE>(cADBIENE_copy, "CADBIENES", cADBIENE.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

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