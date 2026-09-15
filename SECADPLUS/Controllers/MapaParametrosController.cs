using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class MapaParametrosController : Controller
    {
        private GestorAuditoria auditoria = new GestorAuditoria();
        private DesRepMinEntities dbDes = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));

        // GET: MapaParametros
        [Authorize]
        public ActionResult Index(string strMensaje, string strTipo)
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
                //---------------------------------------------------------------------------------------------------------//
                //traer usuario autenticado - almacenar datos de usuario en variable globales.
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                var idSitio = usr.SITIO_GRABACION;
                var Usuario = usr.USUARIO;
                var Condicion = usr.CONDICION;

                ViewBag.USUARIO = Usuario;
                ViewBag.CONDICION = Condicion;

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

                List<CADVISORPARAMETRO> cADVISORPARAMETRO = new List<CADVISORPARAMETRO>();

                if (Condicion == "A" || Condicion == "J" || Condicion == "P")
                    cADVISORPARAMETRO = dbDes.CADVISORPARAMETROS.OrderBy(c => c.CODIGO).ToList();
                else
                    cADVISORPARAMETRO = dbDes.CADVISORPARAMETROS.Where(c => c.USUARIO == Usuario).OrderBy(c => c.CODIGO).ToList();

                return View(cADVISORPARAMETRO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MapaParametros/Create
        [Authorize]
        public ActionResult Create(string strUsuario, string strCondicion)
        {
            try
            {
                ViewBag.CODIGO = 0;
                ViewBag.strUsuario = strUsuario;
                ViewBag.strCondicion = strCondicion;

                if (strCondicion == "A" || strCondicion == "J" || strCondicion == "P")
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario },
                                                             new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                                 }, "value", "text");
                }
                else
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario }
                                                                 }, "value", "text");
                }

                ViewBag.TIPO = new SelectList(new[] { new { value = "CAPA" , text = "CAPA" },
                                                      new { value = "TIEMPO_RECARGA" , text = "TIEMPO_RECARGA" },
                                                      new { value = "TIEMPO_DB_CONSULTA" , text = "TIEMPO_DB_CONSULTA" }
                                                    }, "value", "text");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MapaParametros/Create To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CODIGO,USUARIO,TIPO,PARAMETRO,NOMBRE")] CADVISORPARAMETRO cADVISORPARAMETRO)
        {
            try
            {
                string strUsuario = Request["strUsuario"];
                string strCondicion = Request["strCondicion"];

                if (cADVISORPARAMETRO.TIPO == "TIEMPO_RECARGA" || cADVISORPARAMETRO.TIPO == "TIEMPO_DB_CONSULTA")
                {
                    var existe = dbDes.CADVISORPARAMETROS.FirstOrDefault(c => c.USUARIO == cADVISORPARAMETRO.USUARIO && c.TIPO == cADVISORPARAMETRO.TIPO);

                    if (existe != null)
                    {
                        ModelState.AddModelError("USUARIO", "No se puede crear, registro ya existe.");
                    }
                }

                if (cADVISORPARAMETRO.TIPO == "CAPA" && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.NOMBRE) && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.PARAMETRO))
                {
                    Uri urlResultado;
                    Uri.TryCreate(cADVISORPARAMETRO.PARAMETRO, UriKind.RelativeOrAbsolute, out urlResultado);

                    if (!urlResultado.IsAbsoluteUri)
                        ModelState.AddModelError("PARAMETRO", "Url no es valida.");
                }

                if ((cADVISORPARAMETRO.TIPO == "TIEMPO_RECARGA" || cADVISORPARAMETRO.TIPO == "TIEMPO_DB_CONSULTA") && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.PARAMETRO))
                {
                    int numero;
                    bool esNumero = int.TryParse(cADVISORPARAMETRO.PARAMETRO, out numero);

                    if (!esNumero)
                        ModelState.AddModelError("PARAMETRO", "El valor no es numerico.");
                }

                if (ModelState.IsValid)
                {
                    Int16 CODIGO;
                    var existeC = dbDes.CADVISORPARAMETROS.FirstOrDefault();

                    if (existeC == null)
                        CODIGO = 1;
                    else
                    {
                        CODIGO = dbDes.CADVISORPARAMETROS.Max(c => c.CODIGO);
                        CODIGO += 1;
                    }

                    cADVISORPARAMETRO.CODIGO = CODIGO;

                    dbDes.CADVISORPARAMETROS.Add(cADVISORPARAMETRO);
                    dbDes.SaveChanges();

                    //auditoria.RegistrarAuditoria<CADVISORPARAMETRO>(cADVISORPARAMETRO, "CADVISORPARAMETROS", cADVISORPARAMETRO.CODIGO.ToString(), TipoAuditoria.Crear);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADVISORPARAMETRO.CODIGO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }
                ViewBag.strUsuario = strUsuario;
                ViewBag.strCondicion = strCondicion;

                if (strCondicion == "A" || strCondicion == "J" || strCondicion == "P")
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario },
                                                             new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }
                else
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }

                ViewBag.TIPO = new SelectList(new[] { new { value = "CAPA" , text = "CAPA" },
                                                      new { value = "TIEMPO_RECARGA" , text = "TIEMPO_RECARGA" },
                                                      new { value = "TIEMPO_DB_CONSULTA" , text = "TIEMPO_DB_CONSULTA" }
                                                    }, "value", "text", cADVISORPARAMETRO.TIPO);

                //var existe = dbDes.CADVISORPARAMETROS.FirstOrDefault(c => c.USUARIO == cADVISORPARAMETRO.USUARIO && c.TIPO == "TIEMPO");

                //if (existe == null)
                //    ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" },
                //                                          new { value = "TIEMPO" , text = "TIEMPO" }
                //                                        }, "value", "text", cADVISORPARAMETRO.TIPO);
                //else
                //    ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" }
                //                                              }, "value", "text", cADVISORPARAMETRO.TIPO);

                return View(cADVISORPARAMETRO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede crear, url ya existe.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MapaParametros/Edit/5
        [Authorize]
        public ActionResult Edit(short? id, string strUsuario, string strCondicion)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADVISORPARAMETRO cADVISORPARAMETRO = dbDes.CADVISORPARAMETROS.Find(id);
                if (cADVISORPARAMETRO == null)
                {
                    return HttpNotFound();
                }

                ViewBag.strUsuario = strUsuario;
                ViewBag.strCondicion = strCondicion;

                if (strCondicion == "A" || strCondicion == "J" || strCondicion == "P")
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario },
                                                             new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }
                else
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }

                ViewBag.TIPO = new SelectList(new[] { new { value = "CAPA" , text = "CAPA" },
                                                      new { value = "TIEMPO_RECARGA" , text = "TIEMPO_RECARGA" },
                                                      new { value = "TIEMPO_DB_CONSULTA" , text = "TIEMPO_DB_CONSULTA" }
                                                    }, "value", "text", cADVISORPARAMETRO.TIPO);

                //if (cADVISORPARAMETRO.TIPO == "TIEMPO")
                //{
                //    ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" },
                //                                          new { value = "TIEMPO" , text = "TIEMPO" }
                //                                        }, "value", "text", cADVISORPARAMETRO.TIPO);
                //}
                //else
                //{
                //    var existe = dbDes.CADVISORPARAMETROS.FirstOrDefault(c => c.USUARIO == cADVISORPARAMETRO.USUARIO && c.TIPO == "TIEMPO");

                //    if (existe == null)
                //        ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" },
                //                                          new { value = "TIEMPO" , text = "TIEMPO" }
                //                                        }, "value", "text", cADVISORPARAMETRO.TIPO);
                //    else
                //        ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" }
                //                                              }, "value", "text", cADVISORPARAMETRO.TIPO);
                //}

                return View(cADVISORPARAMETRO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MapaParametros/Edit/5 To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CODIGO,USUARIO,TIPO,PARAMETRO,NOMBRE")] CADVISORPARAMETRO cADVISORPARAMETRO)
        {
            try
            {
                string strUsuario = Request["strUsuario"];
                string strCondicion = Request["strCondicion"];

                if (cADVISORPARAMETRO.TIPO == "CAPA" && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.NOMBRE) && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.PARAMETRO))
                {
                    Uri urlResultado;
                    Uri.TryCreate(cADVISORPARAMETRO.PARAMETRO, UriKind.RelativeOrAbsolute, out urlResultado);

                    if (!urlResultado.IsAbsoluteUri)
                        ModelState.AddModelError("PARAMETRO", "Url no es valida.");
                }

                if ((cADVISORPARAMETRO.TIPO == "TIEMPO_RECARGA" || cADVISORPARAMETRO.TIPO == "TIEMPO_DB_CONSULTA") && !String.IsNullOrWhiteSpace(cADVISORPARAMETRO.PARAMETRO))
                {
                    int numero;
                    bool esNumero = int.TryParse(cADVISORPARAMETRO.PARAMETRO, out numero);

                    if (!esNumero)
                        ModelState.AddModelError("PARAMETRO", "El valor no es numerico.");
                }

                if (ModelState.IsValid)
                {
                    dbDes.Entry(cADVISORPARAMETRO).State = EntityState.Modified;
                    dbDes.SaveChanges();

                    auditoria.RegistrarAuditoria<CADVISORPARAMETRO>(cADVISORPARAMETRO, "CADVISORPARAMETROS", cADVISORPARAMETRO.CODIGO.ToString(), TipoAuditoria.Modificar);

                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADVISORPARAMETRO.CODIGO.ToString());
                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                    return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
                }

                ViewBag.strUsuario = strUsuario;
                ViewBag.strCondicion = strCondicion;

                if (strCondicion == "A" || strCondicion == "J" || strCondicion == "P")
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario },
                                                             new { value = "TODOS LOS USUARIOS" , text = "TODOS LOS USUARIOS" }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }
                else
                {
                    ViewBag.USUARIO = new SelectList(new[] { new { value = strUsuario, text = strUsuario }
                                                                 }, "value", "text", cADVISORPARAMETRO.USUARIO);
                }

                ViewBag.TIPO = new SelectList(new[] { new { value = "CAPA" , text = "CAPA" },
                                                      new { value = "TIEMPO_RECARGA" , text = "TIEMPO_RECARGA" },
                                                      new { value = "TIEMPO_DB_CONSULTA" , text = "TIEMPO_DB_CONSULTA" }
                                                    }, "value", "text", cADVISORPARAMETRO.TIPO);

                //if (cADVISORPARAMETRO.TIPO == "TIEMPO")
                //{
                //    ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" },
                //                                          new { value = "TIEMPO" , text = "TIEMPO" }
                //                                        }, "value", "text", cADVISORPARAMETRO.TIPO);
                //}
                //else
                //{
                //    var existe = dbDes.CADVISORPARAMETROS.FirstOrDefault(c => c.USUARIO == cADVISORPARAMETRO.USUARIO && c.TIPO == "TIEMPO");

                //    if (existe == null)
                //        ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" },
                //                                          new { value = "TIEMPO" , text = "TIEMPO" }
                //                                        }, "value", "text", cADVISORPARAMETRO.TIPO);
                //    else
                //        ViewBag.TIPO = new SelectList(new[] { new { value = "MAPA" , text = "MAPA" }
                //                                              }, "value", "text", cADVISORPARAMETRO.TIPO);
                //}

                return View(cADVISORPARAMETRO);
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede editar, url ya existe.";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: MapaParametros/Delete/5
        [Authorize]
        public ActionResult Eliminar(short? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADVISORPARAMETRO cADVISORPARAMETRO = dbDes.CADVISORPARAMETROS.Find(id);
                if (cADVISORPARAMETRO == null)
                {
                    return HttpNotFound();
                }
                //return View(cADVISORPARAMETRO);
                return View("Delete", null, cADVISORPARAMETRO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: MapaParametros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            try
            {
                CADVISORPARAMETRO cADVISORPARAMETRO = dbDes.CADVISORPARAMETROS.Find(id);
                dbDes.CADVISORPARAMETROS.Remove(cADVISORPARAMETRO);
                dbDes.SaveChanges();


                CADVISORPARAMETRO cADVISORPARAMETRO_copy = new CADVISORPARAMETRO();
                cADVISORPARAMETRO_copy = (CADVISORPARAMETRO)auditoria.GenerarCopiaAuditoria(cADVISORPARAMETRO);

                auditoria.RegistrarAuditoria<CADVISORPARAMETRO>(cADVISORPARAMETRO_copy, "CADVISORPARAMETROS", cADVISORPARAMETRO.CODIGO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO.Replace("NUMERO", cADVISORPARAMETRO.CODIGO.ToString());
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                return RedirectToAction("Index", new { strMensaje = mensaje, strTipo = tipo });
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados.";
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
                dbDes.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}