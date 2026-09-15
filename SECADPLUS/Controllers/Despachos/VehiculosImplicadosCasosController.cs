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
    public class VehiculosImplicadosCasosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        // GET: VehiculosImplicadosCasos
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            {
                var cADBIENES_IMPLICADOS = db.CADBIENES_IMPLICADOS.Include(c => c.CADBIENE).Include(c => c.CADLUGARES_GEOGRAFICOS).Include(c => c.CADCASO).Include(c => c.CADBARRIO);
                return View(await cADBIENES_IMPLICADOS.ToListAsync());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult GetBarriosByMunicipio(int? id)
        {
            try
            {
                List<CADBARRIO> lugares = new List<CADBARRIO>();

                if (id != null)
                {
                    lugares = db.CADBARRIOS.Where(m => m.LUGE_CODIGO == id).ToList();
                }
                else
                {
                    lugares.Insert(0, new CADBARRIO { CODIGO = 0, DESCRIPCION = " " });
                }

                var barrios = (from r in lugares
                               select new
                               {
                                   CODIGO = r.CODIGO,
                                   DESCRIPCION = r.DESCRIPCION
                               }).ToList();

                return Json(barrios);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: VehiculosImplicadosCasos/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS = await db.CADBIENES_IMPLICADOS.FindAsync(id);
                if (cADBIENES_IMPLICADOS == null)
                {
                    return HttpNotFound();
                }
                return View(cADBIENES_IMPLICADOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: VehiculosImplicadosCasos/Create
        [Authorize]
        public ActionResult Create(int idSitio, long idBien)
        {
            try
            {
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                //traer usuario autenticado
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                var maquina = db.CADCONEXIONs.Where(c => c.CADUSUA_USUARIO == usr.USUARIO && c.VIGENTE == "S").OrderByDescending(c => c.SESION_ID).Select(c => c.MAQUINA).First();

                if (string.IsNullOrWhiteSpace(maquina))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.CADBIEN_SITIOGRABA = idSitio;
                ViewBag.CADBIEN_CONSECUTIVO = idBien;
                // El consecutivo se genera en la fase de guardar
                ViewBag.CONSECUTIVO = 0;
                //
                ViewBag.CASO_CODIGO = new SelectList(db.CADCASOS.Where(c => c.CODIGO.StartsWith("9") && c.CODIGO != "900").Select(c => new { c.CODIGO, DESCRIPCION = c.CODIGO + " - " + c.DESCRIPCION }), "CODIGO", "DESCRIPCION");

                //ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION");

                GestorAdministrador gestor = new GestorAdministrador();
                var lugares = gestor.ConsultarMunicipios(strConexion);

                var municipios = (from DataRow dr in lugares.Rows
                                  select new
                                  {
                                      CODIGO = dr["CODIGO"],
                                      DESCRIPCION = dr["DESCRIPCION"],
                                  }).ToList();

                ViewBag.LUGE_CODIGO = new SelectList(municipios, "CODIGO", "DESCRIPCION");

                List<CADBARRIO> listaBarrios = new List<CADBARRIO>();
                listaBarrios.Insert(0, new CADBARRIO { CODIGO = 0, DESCRIPCION = " " });
                ViewBag.LUGE_BARRIO = new SelectList(listaBarrios, "CODIGO", "DESCRIPCION");

                // El usuario por medio de autenticación //  User.Identity.GetUserName()
                //ViewBag.USUA_TRANSACCION = User.Identity.GetUserName();
                //ViewBag.USUA_TRANSACCION = "USR_CAD";

                ViewBag.USUA_TRANSACCION = usr.USUARIO;

                //IPHostEntry host = new IPHostEntry();
                //host = Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]);
                //ViewBag.MAQUINA_TRANSACCION = host.HostName;
                ViewBag.MAQUINA_TRANSACCION = maquina;
                //ViewBag.CADBIEN_SITIOGRABA = new SelectList(db.CADBIENES, "SITIO_GRABA", "PLACA");
                //ViewBag.CODI_BARRIO = new SelectList(db.CADBARRIOS, "CODIGO", "DESCRIPCION");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosCasos/Create To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CADBIEN_SITIOGRABA,CADBIEN_CONSECUTIVO,CONSECUTIVO,CASO_CODIGO,FECHA_NOVEDAD,DIRECCION,LUGE_CODIGO,LUGE_BARRIO,CODI_BARRIO,ESTADO_NOVEDAD,FECHA_TERMINO,OBSERVACIONES,PATRULLA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS)
        {
            try
            {
                long CONSECUTIVO = db.CADBIENES_IMPLICADOS.Where(m => m.CADBIEN_SITIOGRABA == cADBIENES_IMPLICADOS.CADBIEN_SITIOGRABA).Max(m => m.CONSECUTIVO);
                CONSECUTIVO += 1;
                cADBIENES_IMPLICADOS.CONSECUTIVO = CONSECUTIVO;

                cADBIENES_IMPLICADOS.CODI_BARRIO = cADBIENES_IMPLICADOS.LUGE_BARRIO;
                cADBIENES_IMPLICADOS.LUGE_BARRIO = cADBIENES_IMPLICADOS.LUGE_CODIGO;
                cADBIENES_IMPLICADOS.FECHA_TRANSACCION = DateTime.Now;

                if (cADBIENES_IMPLICADOS.ESTADO_NOVEDAD == "true")
                    cADBIENES_IMPLICADOS.ESTADO_NOVEDAD = "S";
                else
                    cADBIENES_IMPLICADOS.ESTADO_NOVEDAD = "N";

                if (ModelState.IsValid)
                {
                    db.CADBIENES_IMPLICADOS.Add(cADBIENES_IMPLICADOS);
                    await db.SaveChangesAsync();

                    //auditoria.RegistrarAuditoria<CADBIENES_IMPLICADOS>(cADBIENES_IMPLICADOS, "CADBIENES_IMPLICADOS", cADBIENES_IMPLICADOS.CONSECUTIVO.ToString(), TipoAuditoria.Crear);

                    var mensaje = "Ha sido creado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "VehiculosImplicados", new { idCons = cADBIENES_IMPLICADOS.CADBIEN_CONSECUTIVO, strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index", "VehiculosImplicados");
                }
                //traer usuario autenticado
                //CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                //ViewBag.USUA_TRANSACCION = usr.USUARIO;

                ViewBag.CADBIEN_SITIOGRABA = new SelectList(db.CADBIENES, "SITIO_GRABA", "PLACA", cADBIENES_IMPLICADOS.CADBIEN_SITIOGRABA);
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.LUGE_CODIGO);
                ViewBag.LUGE_BARRIO = new SelectList(db.CADBARRIOS.Where(m => m.LUGE_CODIGO == cADBIENES_IMPLICADOS.LUGE_CODIGO), "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.LUGE_BARRIO);
                ViewBag.CASO_CODIGO = new SelectList(db.CADCASOS.Where(c => c.CODIGO.StartsWith("9") && c.CODIGO != "900").Select(c => new { c.CODIGO, DESCRIPCION = c.CODIGO + " - " + c.DESCRIPCION }), "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CASO_CODIGO);
                ViewBag.CODI_BARRIO = new SelectList(db.CADBARRIOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CODI_BARRIO);
                ViewBag.ESTADO_NOVEDAD = cADBIENES_IMPLICADOS.ESTADO_NOVEDAD;

                return View(cADBIENES_IMPLICADOS);
            }
            catch (DbUpdateException ex)
            {
                var mensaje = "No se puede crear, restricción de integridad." + ex;
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: VehiculosImplicadosCasos/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS = await db.CADBIENES_IMPLICADOS.FindAsync(id);
                if (cADBIENES_IMPLICADOS == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CADBIEN_SITIOGRABA = new SelectList(db.CADBIENES, "SITIO_GRABA", "PLACA", cADBIENES_IMPLICADOS.CADBIEN_SITIOGRABA);
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.LUGE_CODIGO);
                ViewBag.CASO_CODIGO = new SelectList(db.CADCASOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CASO_CODIGO);
                ViewBag.CODI_BARRIO = new SelectList(db.CADBARRIOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CODI_BARRIO);
                return View(cADBIENES_IMPLICADOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosCasos/Edit/5 To protect from overposting attacks, please enable
        // the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CADBIEN_SITIOGRABA,CADBIEN_CONSECUTIVO,CONSECUTIVO,CASO_CODIGO,FECHA_NOVEDAD,DIRECCION,LUGE_CODIGO,LUGE_BARRIO,CODI_BARRIO,ESTADO_NOVEDAD,FECHA_TERMINO,OBSERVACIONES,PATRULLA,USUA_TRANSACCION,FECHA_TRANSACCION,MAQUINA_TRANSACCION")] CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADBIENES_IMPLICADOS).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADBIENES_IMPLICADOS>(cADBIENES_IMPLICADOS, "CADBIENES_IMPLICADOS", cADBIENES_IMPLICADOS.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = "Ha sido editado el registro";
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", "ElementosMaestras", new { strMensaje = mensaje, strTipo = tipo });

                    //return RedirectToAction("Index");
                }
                ViewBag.CADBIEN_SITIOGRABA = new SelectList(db.CADBIENES, "SITIO_GRABA", "PLACA", cADBIENES_IMPLICADOS.CADBIEN_SITIOGRABA);
                ViewBag.LUGE_CODIGO = new SelectList(db.CADLUGARES_GEOGRAFICOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.LUGE_CODIGO);
                ViewBag.CASO_CODIGO = new SelectList(db.CADCASOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CASO_CODIGO);
                ViewBag.CODI_BARRIO = new SelectList(db.CADBARRIOS, "CODIGO", "DESCRIPCION", cADBIENES_IMPLICADOS.CODI_BARRIO);
                return View(cADBIENES_IMPLICADOS);
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

        // GET: VehiculosImplicadosCasos/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS = await db.CADBIENES_IMPLICADOS.FindAsync(id);
                if (cADBIENES_IMPLICADOS == null)
                {
                    return HttpNotFound();
                }
                return View(cADBIENES_IMPLICADOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: VehiculosImplicadosCasos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS = await db.CADBIENES_IMPLICADOS.FindAsync(id);
                db.CADBIENES_IMPLICADOS.Remove(cADBIENES_IMPLICADOS);
                await db.SaveChangesAsync();



                CADBIENES_IMPLICADOS cADBIENES_IMPLICADOS_copy = new CADBIENES_IMPLICADOS();
                cADBIENES_IMPLICADOS_copy = (CADBIENES_IMPLICADOS)auditoria.GenerarCopiaAuditoria(cADBIENES_IMPLICADOS);


                auditoria.RegistrarAuditoria<CADBIENES_IMPLICADOS>(cADBIENES_IMPLICADOS_copy, "CADBIENES_IMPLICADOS", cADBIENES_IMPLICADOS.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = "Ha sido eliminado el registro";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", "VehiculosImplicados", new { strMensaje = mensaje, strTipo = tipo });

                //return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, restricción de integridad ya que se encuentran registros asociados.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", "VehiculosImplicados", new { strMensaje = mensaje, strTipo = tipo });
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