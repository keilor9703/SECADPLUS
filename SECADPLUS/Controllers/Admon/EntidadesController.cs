using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class EntidadesController : Controller
    {
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private GestorAuditoria auditoria = new GestorAuditoria();



        // GET: Entidades
        [SecadPlusAutorizador("/Entidades/Index")]
        public async Task<ActionResult> Index(int idSitioGrabacion, string nombre = null, string message = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (message != null)
                {
                    ViewBag.MENSAJE_EXITOSO = message;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                var vFuerzas = await (from t in db.V_FUERZAS where t.SITIO_GRABA.Equals(idSitioGrabacion) select t).ToListAsync();
                return View(vFuerzas);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Entidades/SitiosGrabacion
        [SecadPlusAutorizador("/Entidades/SitiosGrabacion")]
        public ActionResult SitiosGrabacion()
        {
            try
            {
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                return View(db.V_FORM_SITIOS_GRABACION.ToList());
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Entidades/Create
        [SecadPlusAutorizador("/Entidades/Create")]
        public ActionResult Create(int idSitioGrabacion, string nombre = null)
        {
            try
            {
                CADFUERZA fuerza = new CADFUERZA();
                ViewBag.SITIO_GRABA = idSitioGrabacion.ToString();
                ViewBag.SITIO_GRABA_nombre = nombre;
                ViewBag.CADUNDE_FUERZA_PPAL = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion");
                ViewBag.CADUNDE_FUERZA = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion");
                ViewBag.CADUNDE_CODIGO_PPAL = new SelectList(new List<SelectListItem>(), "CODIGO", "DESCRIPCION");
                ViewBag.CADUNDE_CODIGO = string.Empty;
                ViewBag.foto = Url.Content("~/images/Avatar.png");

                return View(fuerza);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Entidades/Create To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/Entidades/Create")]
        public async Task<ActionResult> Create(string SITIO_GRABA_nombre, [Bind(Include = "DESCRIPCION,ABREVIATURA,CADUNDE_FUERZA,CADUNDE_CODIGO,VIGENTE,DESDE,HASTA,OBSERVACION,HORAS_TURNO,CADUNDE_FUERZA_PPAL,CADUNDE_CODIGO_PPAL,SITIO_GRABA,ESCUDO,FECH_TRANSACCION")] CADFUERZA cADFUERZA)
        {
            try
            {
                string mensaje = string.Empty;
                cADFUERZA.VIGENTE = cADFUERZA.VIGENTE == "true" ? "S" : "N";
                cADFUERZA.ID = db.CADFUERZAS.Max(t => t.ID) + 1;
                cADFUERZA.CADUNDE_FUERZA_PPAL = (from t in db.CADUNIDADES where t.CADUNDE_FUERZA == cADFUERZA.CADUNDE_FUERZA select t.FUERZA).FirstOrDefault();
                if (cADFUERZA.CADUNDE_FUERZA_PPAL == 0) cADFUERZA.CADUNDE_FUERZA_PPAL = cADFUERZA.CADUNDE_FUERZA;
                ModelState.Clear();
                TryValidateModel(cADFUERZA);
                if ((from t in db.CADFUERZAS where t.ID == cADFUERZA.ID select t).Count() > 0)
                {
                    ViewBag.MENSAJE_EXITOSO = "El registro ya existe para este código";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                else
                {
                    byte[] data = cADFUERZA.ESCUDO;
                    if (data != null)
                    {
                        ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, cADFUERZA.ID.ToString()));
                    }
                    else
                    {
                        ViewBag.foto = Url.Content("~/images/Avatar.png");
                    }
                    if (ModelState.IsValid)
                    {
                        foreach (string upload in Request.Files)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Request.Files[upload].InputStream.CopyTo(ms);
                                if (ms.Length > 0)
                                {
                                    Bitmap mifoto = new Bitmap(ms);
                                    string rutaArchivo = Redimensionar(mifoto, "tmp");

                                    using (MemoryStream ms2 = new MemoryStream())
                                    {
                                        data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                        if (data.Length > 0 && data.Length < 3500)
                                        {
                                            cADFUERZA.ESCUDO = data;
                                            mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADFUERZA.ID.ToString());
                                        }
                                        else
                                        {
                                            mensaje = "Registro fue guardado, el tamaño de la imagen NO permite ser guardada";
                                        }
                                    }
                                }
                            }
                        }

                        db.CADFUERZAS.Add(cADFUERZA);
                        await db.SaveChangesAsync();
                        mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADFUERZA.ID.ToString());
                        return RedirectToAction("Index", new { idSitioGrabacion = cADFUERZA.SITIO_GRABA, nombre = SITIO_GRABA_nombre, message = mensaje });
                    }
                }
                ViewBag.SITIO_GRABA = cADFUERZA.SITIO_GRABA.ToString();

                ViewBag.CADUNDE_FUERZA_PPAL = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion", cADFUERZA.CADUNDE_FUERZA_PPAL);
                ViewBag.CADUNDE_FUERZA = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion", cADFUERZA.CADUNDE_FUERZA);
                ViewBag.CADUNDE_CODIGO_PPAL = new SelectList(db.CADUNIDADES.Where(t => t.FUERZA == cADFUERZA.CADUNDE_FUERZA && t.NIVEL == 1 && t.CADUNDE_CODIGO == null), "CODIGO", "DESCRIPCION", cADFUERZA.CADUNDE_CODIGO_PPAL);
                ViewBag.CADUNDE_CODIGO = cADFUERZA.CADUNDE_CODIGO;
                if (cADFUERZA.CADUNDE_CODIGO != 0)
                    ViewBag._UNIDAD = db.CADUNIDADES.Where(t => t.CODIGO == cADFUERZA.CADUNDE_CODIGO).FirstOrDefault().DESCRIPCION;
                else ViewBag._UNIDAD = string.Empty;
                ViewBag.VIGENTE = cADFUERZA.VIGENTE == "S" ? true : false;
                return View(cADFUERZA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Entidades/Edit/5
        [SecadPlusAutorizador("/Entidades/Edit")]
        public async Task<ActionResult> Edit(int? id, int idSitioGrabacion, string nombre)
        {
            try
            {
                Session["nombreSitio"] = nombre;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADFUERZA cADFUERZA = await db.CADFUERZAS.FindAsync(id);
                if (cADFUERZA == null)
                {
                    return HttpNotFound();
                }
                ViewBag.VIGENTE = cADFUERZA.VIGENTE == "S" ? true : false;
                ViewBag.SITIO_GRABA = idSitioGrabacion.ToString();
                ViewBag.SITIO_GRABA_nombre = nombre;
                ViewBag.CADUNDE_FUERZA = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion", cADFUERZA.CADUNDE_FUERZA);
                ViewBag.CADUNDE_CODIGO_PPAL = new SelectList(db.CADUNIDADES.Where(t => t.FUERZA == cADFUERZA.CADUNDE_FUERZA && t.NIVEL == 1 && t.CADUNDE_CODIGO == null), "CODIGO", "DESCRIPCION", cADFUERZA.CADUNDE_CODIGO_PPAL);
                ViewBag.CADUNDE_CODIGO = cADFUERZA.CADUNDE_CODIGO;
                ViewBag._UNIDAD = (from t in db.CADUNIDADES where t.CODIGO == cADFUERZA.CADUNDE_CODIGO select t.DESCRIPCION).FirstOrDefault();
                byte[] data = cADFUERZA.ESCUDO;
                if (data != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, cADFUERZA.ID.ToString()));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }
                return View(cADFUERZA);
            }
            catch (Exception ex)
            {
                ex.GetType();
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Entidades/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/Entidades/Edit")]
        public async Task<ActionResult> Edit(string nombre, [Bind(Include = "ID,DESCRIPCION,ABREVIATURA,CADUNDE_FUERZA,CADUNDE_CODIGO,VIGENTE,DESDE,HASTA,OBSERVACION,HORAS_TURNO,CADUNDE_FUERZA_PPAL,CADUNDE_CODIGO_PPAL,SITIO_GRABA,ESCUDO,FECH_TRANSACCION")] CADFUERZA cADFUERZA)
        {
            try
            {
                string mensaje = string.Empty;
                byte[] data;
                cADFUERZA.VIGENTE = cADFUERZA.VIGENTE == "true" ? "S" : "N";
                cADFUERZA.CADUNDE_FUERZA_PPAL = (from t in db.CADUNIDADES where t.CADUNDE_FUERZA == cADFUERZA.CADUNDE_FUERZA select t.FUERZA).FirstOrDefault();
                cADFUERZA.FECH_TRANSACCION = DateTime.Now;

                data = cADFUERZA.ESCUDO;
                if (data != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, cADFUERZA.ID.ToString()));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }
                ModelState.Clear();
                TryValidateModel(cADFUERZA);
                if (ModelState.IsValid)
                {
                    db.Entry(cADFUERZA).State = EntityState.Modified;
                    foreach (string upload in Request.Files)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Request.Files[upload].InputStream.CopyTo(ms);
                            if (ms.Length > 0)
                            {
                                Bitmap mifoto = new Bitmap(ms);
                                string rutaArchivo = Redimensionar(mifoto, "tmp");

                                using (MemoryStream ms2 = new MemoryStream())
                                {
                                    data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                    if (data.Length > 0 && data.Length < 3500)
                                    {
                                        cADFUERZA.ESCUDO = data;
                                        db.Entry(cADFUERZA).State = EntityState.Modified;
                                        mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADFUERZA.ID.ToString());
                                    }
                                    else
                                    {
                                        mensaje = "Registro fue guardado, el tamaño de la imagen NO permite ser guardada";
                                    }
                                }
                            }
                            else
                            {
                                cADFUERZA.ESCUDO = db.CADFUERZAS.Where(t => t.ID == cADFUERZA.ID).FirstOrDefault().ESCUDO;
                            }
                        }
                    }
                    db.Entry(cADFUERZA).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADFUERZA>(cADFUERZA, "CADFUERZA", "ID=" + cADFUERZA.ID.ToString(), TipoAuditoria.Modificar);



                    mensaje = mensaje.Contains("la imagen NO permite ser guardada") == true ? mensaje : Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADFUERZA.ID.ToString());
                    return RedirectToAction("Index", new { idSitioGrabacion = cADFUERZA.SITIO_GRABA, nombre = nombre, message = mensaje });
                }
                else
                {
                    ViewBag.MENSAJE_EXITOSO = "Error guardando el registro, por favor verifique la información";
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
                ViewBag.VIGENTE = cADFUERZA.VIGENTE == "S" ? true : false;
                ViewBag.SITIO_GRABA = cADFUERZA.SITIO_GRABA.ToString();
                ViewBag.SITIO_GRABA_nombre = nombre;
                ViewBag.CADUNDE_FUERZA = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion", cADFUERZA.CADUNDE_FUERZA);
                ViewBag.CADUNDE_CODIGO_PPAL = new SelectList(db.CADUNIDADES.Where(t => t.FUERZA == cADFUERZA.CADUNDE_FUERZA && t.NIVEL == 1 && t.CADUNDE_CODIGO == null), "CODIGO", "DESCRIPCION", cADFUERZA.CADUNDE_CODIGO_PPAL);
                ViewBag.CADUNDE_CODIGO = cADFUERZA.CADUNDE_CODIGO;
                if (cADFUERZA.CADUNDE_CODIGO == 0)
                    ViewBag._UNIDAD = String.Empty;
                else
                    ViewBag._UNIDAD = db.CADUNIDADES.Where(t => t.CODIGO == cADFUERZA.CADUNDE_CODIGO).FirstOrDefault().DESCRIPCION;
                return View(cADFUERZA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Entidades/Delete/5
        [SecadPlusAutorizador("/Entidades/Delete")]
        public async Task<ActionResult> Delete(int? id, int idSitioGrabacion, string nombre)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADFUERZA cADFUERZA = await db.CADFUERZAS.FindAsync(id);
                if (cADFUERZA == null)
                {
                    return HttpNotFound();
                }
                return View(cADFUERZA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public ActionResult Eliminar(int? id, int idSitioGrabacion, string nombre)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADFUERZA cADFUERZA = db.CADFUERZAS.Find(id);
                if (cADFUERZA == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADFUERZA);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Entidades/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id, int idSitioGrabacion, string nombre = null)
        {
            try
            {
                CADFUERZA cADFUERZA = await db.CADFUERZAS.FindAsync(id);

                if ((from t in db.CADCANALES where t.CADFUERZ_ID == id select t).Count() > 0)
                    return RedirectToAction("Index", new { idSitioGrabacion = cADFUERZA.SITIO_GRABA, message = "Registro no puede ser borrado mientras tenga canales asociados" });
                if ((from t in db.CADCLASIFICACION_CIERRE where t.ID_FUERZA == id select t).Count() > 0)
                    return RedirectToAction("Index", new { idSitioGrabacion = cADFUERZA.SITIO_GRABA, message = "Registro no puede ser borrado mientras tenga clasificaciones de cierre asociadas" });

                db.CADFUERZAS.Remove(cADFUERZA);
                await db.SaveChangesAsync();


                CADFUERZA cADFUERZA_copy = new CADFUERZA();
                cADFUERZA_copy = (CADFUERZA)auditoria.GenerarCopiaAuditoria(cADFUERZA);

                auditoria.RegistrarAuditoria<CADFUERZA>(cADFUERZA_copy, "CADFUERZA", "ID=" + cADFUERZA.ID.ToString(), TipoAuditoria.Eliminar);



                return RedirectToAction("Index", new { idSitioGrabacion = cADFUERZA.SITIO_GRABA, message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public JsonResult BuscaUnidadesPorFuerza(int idFuerza)
        {
            return Json(from t in db.CADUNIDADES
                        where t.FUERZA == idFuerza && t.NIVEL == 1 && t.CADUNDE_CODIGO == null
                        select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION });
        }

        [Authorize]
        public JsonResult BuscaUnidadesPorUnidadPrincipal(int idFuerza)
        {
            return Json(from t in db.CADUNIDADES
                        where t.FUERZA == idFuerza //&& t.VIGENTE == "S" && t.OPERATIVA == "S"
                        select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION });
        }

        //FUNCION PARA CONVERTIR DE BYTES A IMAGEN
        public string Bytes_A_Imagen(byte[] ImgBytes, string cedula)
        {
            try
            {
                Bitmap imagen = null;
                Byte[] bytes = (Byte[])(ImgBytes);
                MemoryStream ms = new MemoryStream(bytes);
                imagen = new Bitmap(ms);
                string rutaFinal = Redimensionar(imagen, cedula);
                imagen.Dispose();
                return rutaFinal;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return string.Empty;
            }
        }

        private string Redimensionar(Image Imagen_Original, string nombre)
        {
            try
            {
                //RUTA DEL DIRECTORIO TEMPORAL
                //String DirTemp = Path.GetTempPath() + @"~\Images\" + nombre + ".jpg";
                String DirTemp = "~/Images/Entidades/" + nombre + ".jpg";
                //IMAGEN ORIGINAL A REDIMENSIONAR
                Bitmap imagen = new Bitmap(Imagen_Original);
                //CREAMOS UN MAPA DE BIT CON LAS DIMENSIONES QUE QUEREMOS PARA LA NUEVA IMAGEN
                Bitmap nuevaImagen = new Bitmap(150, 200);
                //CREAMOS UN NUEVO GRAFICO
                Graphics gr = Graphics.FromImage(nuevaImagen);
                //DIBUJAMOS LA NUEVA IMAGEN
                gr.DrawImage(imagen, 0, 0, 150, 200);
                //LIBERAMOS RECURSOS
                gr.Dispose();

                //GUARDAMOS LA NUEVA IMAGEN ESPECIFICAMOS LA RUTA Y EL FORMATO
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter;
                System.Drawing.Imaging.Encoder myEncoder;
                ImageCodecInfo myImageCodecInfo;
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameter = new EncoderParameter(myEncoder, 25L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                nuevaImagen.Save(Server.MapPath(DirTemp), myImageCodecInfo, myEncoderParameters);
                //LIBERAMOS RECURSOS
                nuevaImagen.Dispose();
                imagen.Dispose();
                return DirTemp.Replace("~", "");
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            try
            {
                int j;
                ImageCodecInfo[] encoders;
                encoders = ImageCodecInfo.GetImageEncoders();
                for (j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType == mimeType)
                        return encoders[j];
                }
                return null;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        [SecadPlusAutorizador("/Entidades/Edit")]
        public async Task<ActionResult> ActualizaPersonalPonal(int? id, int idSitioGrabacion, string nombre)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADFUERZA cADFUERZA = await db.CADFUERZAS.FindAsync(id);
                if (cADFUERZA == null)
                {
                    return HttpNotFound();
                }
                GestorGeneral gestor = new GestorGeneral();
                DataTable resultado = gestor.ActualizaEmpleados(cADFUERZA.ID);
                ViewBag.resultadosActualizacion = resultado.Rows[0]["Errores"];
                ViewBag.MENSAJE_EXITOSO = resultado.Rows[0]["Resultado"]; ;
                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                Session["nombreSitio"] = nombre;

                ViewBag.VIGENTE = cADFUERZA.VIGENTE == "S" ? true : false;
                ViewBag.SITIO_GRABA = idSitioGrabacion.ToString();
                ViewBag.SITIO_GRABA_nombre = nombre;
                ViewBag.CADUNDE_FUERZA = new SelectList(from t in db.CADREFERENCIASSECADs where t.NOMBRE == "TIPO_FUERZA" orderby t.DESCRIPCION select new { codigo = t.CODIGO, descripcion = t.DESCRIPCION }, "codigo", "descripcion", cADFUERZA.CADUNDE_FUERZA);
                ViewBag.CADUNDE_CODIGO_PPAL = new SelectList(db.CADUNIDADES.Where(t => t.FUERZA == cADFUERZA.CADUNDE_FUERZA && t.VIGENTE == "S" && t.OPERATIVA == "S" && t.NIVEL == 1), "CODIGO", "DESCRIPCION", cADFUERZA.CADUNDE_CODIGO_PPAL);
                ViewBag.CADUNDE_CODIGO = new SelectList(db.CADUNIDADES.Where(t => t.CADUNDE_CODIGO == cADFUERZA.CADUNDE_CODIGO_PPAL && t.VIGENTE == "S" && t.OPERATIVA == "S"), "CODIGO", "DESCRIPCION", cADFUERZA.CADUNDE_CODIGO);
                ViewBag._UNIDAD = (from t in db.CADUNIDADES where t.CODIGO == cADFUERZA.CADUNDE_CODIGO select t.DESCRIPCION).FirstOrDefault();
                byte[] data = cADFUERZA.ESCUDO;
                if (data != null)
                {
                    ViewBag.foto = Bytes_A_Imagen(data, cADFUERZA.ID.ToString());
                }
                else
                {
                    ViewBag.foto = "/images/Avatar.png";
                }
                return View("Edit", cADFUERZA);
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        [Authorize]
        public JsonResult GetUnidades(int id, int intIdUnidadPrincipal)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);

                string strQuery = "", strLimite = "", strOrderna = "", strFiltro = " WHERE FUERZA = " + id;
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;
                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                  "onclick = \"cargarValorUnidad(CODIGO,'DESCRIPCION')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                                  " Seleccionar </button>";

                string strBotonDestino = "";
                List<_UnidadesModel> listaUnidades = new List<_UnidadesModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = "WHERE  (INTERNO2.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = "WHERE  REGEXP_LIKE(INTERNO2.DESCRIPCION , '" + parametros.ConsultaValor + "','i') ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrderna += " ORDER BY INTERNO2.CODIGO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY INTERNO2.DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;
                }
                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                //objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "LUGARES", typeof(string)));

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_filtro", strFiltro, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", intIdUnidadPrincipal.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_unidades_prior", strConexion, "Nonquery");

                foreach (DataRow fila in dttLugar.Rows)
                {
                    strBotonDestino = strBoton.Replace("CODIGO", fila["CODIGO"].ToString());
                    strBotonDestino = strBotonDestino.Replace("DESCRIPCION", fila["DESCRIPCION"].ToString());

                    listaUnidades.Add(new _UnidadesModel("", strBotonDestino, fila["CODIGO"].ToString(), fila["DESCRIPCION"].ToString()));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaUnidades });
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
        public JsonResult GetUnidadesByFuerza(int? id)
        {
            try
            {
                StringBuilder opciones = new StringBuilder();

                var unidades = db.CADUNIDADES.Where(m => m.FUERZA == id).Select(m => new { CODIGO = m.CODIGO, DESCRIPCION = m.DESCRIPCION }).OrderBy(m => m.DESCRIPCION).ToList();

                if (unidades.Count == 0)
                {
                    unidades.Insert(0, new { CODIGO = 0, DESCRIPCION = " " });
                }

                for (int i = 0; i < unidades.Count(); i++)
                {
                    opciones.AppendFormat("<option value = '{0}'  > {1} </option>", unidades[i].CODIGO, unidades[i].DESCRIPCION);
                }

                return Json(opciones.ToString());
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