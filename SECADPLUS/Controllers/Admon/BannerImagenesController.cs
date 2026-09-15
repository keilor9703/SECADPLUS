using SECADEntities.SecadLugaresGeograficos;
using SECADPLUS.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class BannerImagenesController : Controller
    {
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
        private GestorAuditoria auditoria = new GestorAuditoria();




        /// <summary>
        /// Carga la página con el listado de los banner disponibles
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [SecadPlusAutorizador("/BannerImagenes/Index")]
        public ActionResult Index(string message = null)
        {
            try
            {
                if (message != null)
                {
                    ViewBag.MENSAJE_EXITOSO = message;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                var cADBANNERIMAGENES = (from t in db.CADBANNERIMAGENES select t).ToList();
                return View(cADBANNERIMAGENES);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página con el detalle de cada banner
        /// </summary>
        /// <param name="id">número que identifica al banner</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/BannerImagenes/Index")]
        public async Task<ActionResult> Details(decimal id)
        {
            try
            {
                CADBANNERIMAGENE cADBANNERIMAGENE = await db.CADBANNERIMAGENES.FindAsync(id);
                if (cADBANNERIMAGENE == null)
                {
                    return HttpNotFound();
                }
                return View(cADBANNERIMAGENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Carga la página para crear un nuevo banner
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/BannerImagenes/Create")]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Guarda en base de datos el nuevo banner
        /// </summary>
        /// <param name="cADBANNERIMAGENE"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/BannerImagenes/Create")]
        public async Task<ActionResult> Create([Bind(Include = "NOMBREARCHIVO,ACTIVO,ORDEN")] CADBANNERIMAGENE cADBANNERIMAGENE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0)
                    {
                        foreach (string upload in Request.Files)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Request.Files[upload].InputStream.CopyTo(ms);
                                if (ms.Length > 0)
                                {
                                    db.CADBANNERIMAGENES.Add(cADBANNERIMAGENE);
                                    await db.SaveChangesAsync();
                                    Bitmap imagen = new Bitmap(ms);
                                    GuardaImagenRedimensionada(imagen, cADBANNERIMAGENE.IDBANNER.ToString());
                                }
                                else
                                {
                                    ViewBag.MENSAJE_EXITOSO = "Debe seleccionar una imagen";
                                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                                    return View(cADBANNERIMAGENE);
                                }
                            }
                        }
                    }
                    else
                    {
                        return View(cADBANNERIMAGENE);
                    }

                    return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADBANNERIMAGENE.IDBANNER.ToString()) });
                }

                return View(cADBANNERIMAGENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// carga página que permite editar un banner
        /// </summary>
        /// <param name="id">número identiicador del banner</param>
        /// <returns></returns>
        [SecadPlusAutorizador("/BannerImagenes/Edit")]
        public async Task<ActionResult> Edit(decimal id)
        {
            try
            {
                CADBANNERIMAGENE cADBANNERIMAGENE = await db.CADBANNERIMAGENES.FindAsync(id);
                ViewBag.banner = Request.Url.AbsoluteUri.Replace("/BannerImagenes", string.Empty).Replace("/Edit?id=" + cADBANNERIMAGENE.IDBANNER, string.Empty) + Resources.RecursoGeneral.RutaBannerImagenes + cADBANNERIMAGENE.IDBANNER.ToString() + ".jpg";

                if (cADBANNERIMAGENE == null)
                {
                    return HttpNotFound();
                }
                return View(cADBANNERIMAGENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Guarda los datos del banner que fue editado
        /// </summary>
        /// <param name="cADBANNERIMAGENE"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/BannerImagenes/Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "IDBANNER,NOMBREARCHIVO,ACTIVO,ORDEN,FECHACREACION")] CADBANNERIMAGENE cADBANNERIMAGENE)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cADBANNERIMAGENE).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    auditoria.RegistrarAuditoria<CADBANNERIMAGENE>(cADBANNERIMAGENE, "CADBANNERIMAGENE", "IDBANNER=" + cADBANNERIMAGENE.IDBANNER.ToString(), TipoAuditoria.Modificar);


                    if (Request.Files.Count > 0)
                    {
                        foreach (string upload in Request.Files)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                Request.Files[upload].InputStream.CopyTo(ms);
                                if (ms.Length > 0)
                                {
                                    Bitmap imagen = new Bitmap(ms);
                                    GuardaImagenRedimensionada(imagen, cADBANNERIMAGENE.IDBANNER.ToString());
                                }
                            }
                        }
                    }
                    return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADBANNERIMAGENE.IDBANNER.ToString()) });
                }

                ViewBag.banner = Request.Url.AbsoluteUri.Replace("/BannerImagenes", string.Empty).Replace("/Edit", string.Empty) + Resources.RecursoGeneral.RutaBannerImagenes + cADBANNERIMAGENE.IDBANNER.ToString() + ".jpg";

                return View(cADBANNERIMAGENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Elimina el banner indicado
        /// </summary>
        /// <param name="id">número que identifica al banner</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Eliminar(decimal id)
        {
            try
            {

                CADBANNERIMAGENE cADBANNERIMAGENE = db.CADBANNERIMAGENES.Find(id);
                ViewBag.banner = Request.Url.AbsoluteUri.Replace("/BannerImagenes", string.Empty).Replace("/Delete?id=" + cADBANNERIMAGENE.IDBANNER, string.Empty) + Resources.RecursoGeneral.RutaBannerImagenes + cADBANNERIMAGENE.NOMBREARCHIVO + ".jpg";
                if (cADBANNERIMAGENE == null)
                {
                    return HttpNotFound();
                }
                return View("Delete", null, cADBANNERIMAGENE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Confirma la eliminación del banner
        /// </summary>
        /// <param name="id">número que identifica al banner</param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [SecadPlusAutorizador("/BannerImagenes/Delete")]
        public async Task<ActionResult> DeleteConfirmed(decimal id)
        {
            try
            {
                CADBANNERIMAGENE cADBANNERIMAGENE = await db.CADBANNERIMAGENES.FindAsync(id);
                string ruta = Resources.RecursoGeneral.RutaBannerImagenes + cADBANNERIMAGENE.NOMBREARCHIVO + ".jpg";
                System.IO.File.Delete(Server.MapPath("~" + ruta));
                db.CADBANNERIMAGENES.Remove(cADBANNERIMAGENE);
                await db.SaveChangesAsync();


                CADBANNERIMAGENE cADBANNERIMAGENE_copy = new CADBANNERIMAGENE();
                cADBANNERIMAGENE_copy = (CADBANNERIMAGENE)auditoria.GenerarCopiaAuditoria(cADBANNERIMAGENE);


                auditoria.RegistrarAuditoria<CADBANNERIMAGENE>(cADBANNERIMAGENE_copy, "CADBANNERIMAGENE", "IDBANNER=" + cADBANNERIMAGENE.IDBANNER.ToString(), TipoAuditoria.Eliminar);





                return RedirectToAction("Index", new { message = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite limpiar de memoria la clase
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Guarda la Imagen redimensionada
        /// </summary>
        /// <param name="Imagen_Original">Imagen original</param>
        /// <param name="nombre">nuevo nombre de la imagen</param>
        /// <returns></returns>
        public string GuardaImagenRedimensionada(Image Imagen_Original, string nombre)
        {
            //RUTA DEL DIRECTORIO TEMPORAL
            //String DirTemp = Path.GetTempPath() + @"~\Images\" + nombre + ".jpg";
            String DirTemp = "~/Images/Banner/" + nombre + ".jpg";
            //IMAGEN ORIGINAL A REDIMENSIONAR
            Bitmap imagen = new Bitmap(Imagen_Original);
            //CREAMOS UN MAPA DE BIT CON LAS DIMENSIONES QUE QUEREMOS PARA LA NUEVA IMAGEN
            Bitmap nuevaImagen = new Bitmap(821, 250);
            //CREAMOS UN NUEVO GRAFICO
            Graphics gr = Graphics.FromImage(nuevaImagen);
            //DIBUJAMOS LA NUEVA IMAGEN
            gr.DrawImage(imagen, 0, 0, 821, 250);
            //LIBERAMOS RECURSOS
            gr.Dispose();
            //GUARDAMOS LA NUEVA IMAGEN ESPECIFICAMOS LA RUTA Y EL FORMATO
            nuevaImagen.Save(Server.MapPath(DirTemp), System.Drawing.Imaging.ImageFormat.Jpeg);
            //LIBERAMOS RECURSOS
            nuevaImagen.Dispose();
            imagen.Dispose();
            return DirTemp.Replace("~", "");
        }
    }
}