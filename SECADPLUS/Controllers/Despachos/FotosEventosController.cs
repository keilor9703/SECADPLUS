using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class FotosEventosController : Controller
    {
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private GestorAuditoria auditoria = new GestorAuditoria();

        // GET: AdministrarNotificacion
        [SecadPlusAutorizador("/FotosEventos/FotosEventos")]
        public ActionResult FotosEventos(string strMensaje = null, string strTipo = null, string strSitioGraba = null, string strNumeroEvento = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    if (strTipo == null)
                        ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                    else
                        ViewBag.TIPO_MENSAJE = strTipo;
                }

                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }
                if (strSitioGraba != null || strNumeroEvento != null)
                {
                    Session["SitioGrabaFotos"] = strSitioGraba;
                    Session["NumeroEventoFotos"] = strNumeroEvento;
                }

                if (strSitioGraba == null || strNumeroEvento == null)
                {
                    if (Session["SitioGrabaFotos"] == null && Session["NumeroEventoFotos"] == null)
                    {
                        GestorBase.MSN_ALERTA_COMUN = "No se encuentra un evento para fotos evento.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        strSitioGraba = Session["SitioGrabaFotos"].ToString();
                        strNumeroEvento = Session["NumeroEventoFotos"].ToString();
                    }
                }
                //strNumeroEvento = "216861";
                //strSitioGraba = "3947";

                ViewBag.SitioGraba = strSitioGraba;
                ViewBag.NumeroEvento = strNumeroEvento;
                ViewBag.CADEVEN_SITIOGRABA = strSitioGraba;

                CADEVENTOS_FOTOS cADEVENTOS_FOTO = new CADEVENTOS_FOTOS();

                Int64 intNumeEvento = Convert.ToInt64(strNumeroEvento);
                cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO = intNumeEvento;
                int intSitioGrabaEventos = Convert.ToInt32(strSitioGraba);

                var consecutivo = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == intNumeEvento && t.CADEVEN_SITIOGRABA == intSitioGrabaEventos orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();
                int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                cADEVENTOS_FOTO.CONSECUTIVO = Convert.ToByte(intConsecutivo);

                // return View(cADEVENTOS_FOTO);
                return View("FotosEventos", "MapaBase", cADEVENTOS_FOTO);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que lista todas fotos de los eventos
        /// </summary>
        /// <param name="numeroEvento"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerEventoFotos(string numeroEvento, string strSitioGraba)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";

                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttLugar;

                string strBoton = "<ul class='icons-list'> " +
                                        "<li class='dropdown'> " +
                                            "<button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown' aria-expanded='true'> " +
                                                "<i class='icon-menu9'></i> " +
                                            "</button> " +
                                            "<ul class='dropdown-menu dropdown-menu-left'> " +
                                                "<li> " +

                                                   "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                                                    "onclick = \"EditaFotosEvento(SITIO_GRABA,NUMERO_EVENTO,_CONSECUTIVO)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para Editar' >" +
                                                    "<i class='glyphicon glyphicon-edit'></i> Editar </button>" +

                                                "</li> " +

                                                "<li> " +
                                                    "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip' " +
                                                            "title='' data-placement='bottom' data-original-title='Clic para eliminar' " +
                                                           "onclick=\"EliminarFotoEvento(_ELIMINAR)\"> " +
                                                        "<i class='icon-trash'></i> Eliminar" +
                                                    "</button>" +
                                               "</li>" +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strbotonImagen = " <a href='#' class='secadImg'><img src='URL_IMAGEN' alt ='Foto miniatura' WIDTH='44' HEIGHT='27' />" +
                                     "<span>" +
                                         "<img src='URL_IMAGEN' alt = 'Foto ampliada' />" +
                                     "</span>" +
                                     "</a> ";

                string strBotonDestino = "";
                string strDestinoImagen = "";

                List<_FotosEventosModel> listaEventoFotos = new List<_FotosEventosModel>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                strQuery = "WHERE CADEVEN_NUMEEVENTO = " + numeroEvento + " AND CADEVEN_SITIOGRABA = " + strSitioGraba;

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " AND REGEXP_LIKE (CADEVEN_NUMEEVENTO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (CONSECUTIVO, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (OBSERVACION, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 1:
                        strOrderna += " ORDER BY CADEVEN_NUMEEVENTO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY CONSECUTIVO " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY OBSERVACION " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordena", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listaEventosFotos", strConexion, "Nonquery");

                int? intSitioGraba = 0;
                Int64 intNumeEvento = 0;
                int intConsecutivo = 0;

                foreach (DataRow fila in dttLugar.Rows)
                {
                    try
                    {
                        intSitioGraba = Convert.ToInt32(fila["CADEVEN_SITIOGRABA"].ToString());
                        intNumeEvento = Convert.ToInt64(fila["CADEVEN_NUMEEVENTO"].ToString());
                        intConsecutivo = Convert.ToInt32(fila["CONSECUTIVO"].ToString());

                        strBotonDestino = strBoton.Replace("SITIO_GRABA", intSitioGraba.ToString()).Replace("NUMERO_EVENTO", intNumeEvento.ToString()).Replace("_CONSECUTIVO", intConsecutivo.ToString());//strBoton.Replace("_EDITAR", (Url.Action("Edit", new { sitioGraba = intSitioGraba, numeroEvento = intNumeEvento, consecutivo = intConsecutivo })).ToString());
                        strBotonDestino = strBotonDestino.Replace("_ELIMINAR", intSitioGraba + "," + intNumeEvento + "," + intConsecutivo);

                        byte[] data = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == intNumeEvento && t.CONSECUTIVO == intConsecutivo select t.FOTO).FirstOrDefault();

                        if (data != null)
                        {
                            strDestinoImagen = strbotonImagen.Replace("URL_IMAGEN", Url.Content("~" + Bytes_A_Imagen(data, intConsecutivo.ToString())));
                        }
                        else
                            strDestinoImagen = "Sin Imagen";
                    }
                    catch (Exception ex)
                    {
                        strDestinoImagen = ex.Message;
                    }

                    listaEventoFotos.Add(new _FotosEventosModel("", strBotonDestino, fila["CONSECUTIVO"].ToString(), fila["CADEVEN_NUMEEVENTO"].ToString(), fila["OBSERVACION"].ToString(), strDestinoImagen));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEventoFotos });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        // GET: FotosEventos/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(string strNumeroEvento)
        {
            CADEVENTOS_FOTOS cADEVENTOS_FOTO = new CADEVENTOS_FOTOS();
            try
            {
                Int64 intNumeEvento = Convert.ToInt64(strNumeroEvento);
                cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO = intNumeEvento;

                var consecutivo = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == intNumeEvento orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();
                int intConsecutivo = consecutivo == 0 ? 1 : Convert.ToInt32(consecutivo) + 1;

                cADEVENTOS_FOTO.CONSECUTIVO = Convert.ToByte(intConsecutivo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }

            return View(cADEVENTOS_FOTO);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear([Bind(Include = "CADEVEN_NUMEEVENTO,CONSECUTIVO,OBSERVACION")] CADEVENTOS_FOTOS cADEVENTOS_FOTO)//(string OBSERVACION, int txtNumeroEvento, byte txtConsecutivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (string upload in Request.Files)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Request.Files[upload].InputStream.CopyTo(ms);

                            byte[] data = null;

                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                if (ms.Length > 0)
                                {
                                    Bitmap mifoto = new Bitmap(ms);
                                    string rutaArchivo = Redimensionar(mifoto, "tmp");

                                    data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                    System.IO.File.Delete(Server.MapPath("~/" + rutaArchivo));
                                }
                                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];

                                if (data != null)
                                {
                                    cADEVENTOS_FOTO.FOTO = data;
                                }

                                cADEVENTOS_FOTO.USUA_GRABA = usr.USUARIO;
                                cADEVENTOS_FOTO.CADEVEN_SITIOGRABA = usr.SITIO_GRABACION;
                                cADEVENTOS_FOTO.FECHA_GRABACION = DateTime.Now;

                                CADEVENTOS_FOTOS existe = contexto.CADEVENTOS_FOTOS.Where(t => t.CADEVEN_SITIOGRABA == cADEVENTOS_FOTO.CADEVEN_SITIOGRABA && t.CADEVEN_NUMEEVENTO == cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO && t.CONSECUTIVO == cADEVENTOS_FOTO.CONSECUTIVO).FirstOrDefault();

                                if (existe != null)
                                {
                                    if (data != null)
                                    {
                                        existe.FOTO = data;
                                    }
                                    existe.OBSERVACION = cADEVENTOS_FOTO.OBSERVACION.TrimEnd();
                                    contexto.Entry(existe).State = EntityState.Modified;
                                    await contexto.SaveChangesAsync();
                                    auditoria.RegistrarAuditoria<CADEVENTOS_FOTOS>(cADEVENTOS_FOTO, "CADEVENTOS_FOTOS", "CADEVEN_SITIOGRABA=" + cADEVENTOS_FOTO.CADEVEN_SITIOGRABA.ToString() + ",CADEVEN_NUMEEVENTO=" + cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADEVENTOS_FOTO.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADEVENTOS_FOTO.CONSECUTIVO.ToString());
                                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                    return RedirectToAction("FotosEventos", new { strMensaje = mensaje, strTipo = tipo, strSitioGraba = cADEVENTOS_FOTO.CADEVEN_SITIOGRABA, strNumeroEvento = cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO });
                                }
                                else
                                {
                                    contexto.CADEVENTOS_FOTOS.Add(cADEVENTOS_FOTO);
                                    await contexto.SaveChangesAsync();

                                    var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", "con el consecutivo : " + cADEVENTOS_FOTO.CONSECUTIVO.ToString());
                                    var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                                    return RedirectToAction("FotosEventos", new { strMensaje = mensaje, strTipo = tipo, strSitioGraba = cADEVENTOS_FOTO.CADEVEN_SITIOGRABA, strNumeroEvento = cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO });
                                }
                            }
                        }
                    }
                }
                Int32 strSitioGraba = Convert.ToInt32(Session["SitioGrabaFotos"].ToString());
                cADEVENTOS_FOTO.CADEVEN_SITIOGRABA = strSitioGraba;
                var mensajeValida = "Ingrese los campos requeridos.";
                var tipoValida = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return RedirectToAction("FotosEventos", new { strMensaje = mensajeValida, strTipo = tipoValida, strSitioGraba = cADEVENTOS_FOTO.CADEVEN_SITIOGRABA, strNumeroEvento = cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO });
                // return View("FotosEventos", "MapaBase", cADEVENTOS_FOTO); //new {strSitioGraba =
                // strSitioGraba, strNumeroEvento = cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: CADANOTAGRALs/Delete/5
        [Authorize]
        public async Task<ActionResult> Eliminar(int? sitioGraba, int? numeroEvento, int? consecutivo)
        {
            try
            {
                if (sitioGraba == null && consecutivo == null && numeroEvento == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CADEVENTOS_FOTOS cADEVENTOS_FOTOS = await contexto.CADEVENTOS_FOTOS.FindAsync(sitioGraba, numeroEvento, consecutivo);
                if (cADEVENTOS_FOTOS == null)
                {
                    return HttpNotFound();
                }

                return View("Delete", null, cADEVENTOS_FOTOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: CADEVENTOS_FOTOS/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? sitioGraba, int? numeroEvento, int? consecutivo)
        {
            try
            {
                CADEVENTOS_FOTOS cADEVENTOS_FOTOS = await contexto.CADEVENTOS_FOTOS.FindAsync(sitioGraba, numeroEvento, consecutivo);
                contexto.CADEVENTOS_FOTOS.Remove(cADEVENTOS_FOTOS);
                await contexto.SaveChangesAsync();



                CADEVENTOS_FOTOS cADEVENTOS_FOTOS_copy = new CADEVENTOS_FOTOS();
                cADEVENTOS_FOTOS_copy = (CADEVENTOS_FOTOS)auditoria.GenerarCopiaAuditoria(cADEVENTOS_FOTOS);

                auditoria.RegistrarAuditoria<CADEVENTOS_FOTOS>(cADEVENTOS_FOTOS_copy, "CADEVENTOS_FOTOS", "CADEVEN_SITIOGRABA=" + cADEVENTOS_FOTOS.CADEVEN_SITIOGRABA.ToString() + ",CADEVEN_NUMEEVENTO=" + cADEVENTOS_FOTOS.CADEVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADEVENTOS_FOTOS.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();
                return RedirectToAction("FotosEventos", new { strMensaje = mensaje, strTipo = tipo, strSitioGraba = cADEVENTOS_FOTOS.CADEVEN_SITIOGRABA, strNumeroEvento = cADEVENTOS_FOTOS.CADEVEN_NUMEEVENTO });
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

        // GET: CADEVENTOS_FOTOS/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? sitioGraba, int? numeroEvento, int? consecutivo)
        {
            try
            {
                CADEVENTOS_FOTOS cADEVENTOS_FOTOS = await contexto.CADEVENTOS_FOTOS.FindAsync(sitioGraba, numeroEvento, consecutivo);
                if (cADEVENTOS_FOTOS == null)
                {
                    return HttpNotFound();
                }

                byte[] data = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == numeroEvento && t.CONSECUTIVO == consecutivo && t.CADEVEN_SITIOGRABA == sitioGraba select t.FOTO).FirstOrDefault();

                if (data != null)
                {
                    ViewBag.foto = Url.Content("~" + Bytes_A_Imagen(data, consecutivo.ToString()));
                }
                else
                {
                    ViewBag.foto = Url.Content("~/images/Avatar.png");
                }

                return View(cADEVENTOS_FOTOS);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que actualiza un evento foto por id
        /// </summary>
        /// <param name="txtObservaciones"></param>
        /// <param name="txtNumeroEvento"></param>
        /// <param name="txtConsecutivo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CADEVEN_SITIOGRABA,FECHA_GRABACION,USUA_GRABA,CADEVEN_NUMEEVENTO,CONSECUTIVO,OBSERVACION,FOTO")] CADEVENTOS_FOTOS cADEVENTOS_FOTO)
        {
            try
            {
                //string txtObservaciones = ""; int intNumeroEvento = 0; int intConsecutivo = 0;
                if (ModelState.IsValid)
                {
                    foreach (string upload in Request.Files)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Request.Files[upload].InputStream.CopyTo(ms);

                            byte[] data = null;

                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                if (ms.Length > 0)
                                {
                                    Bitmap mifoto = new Bitmap(ms);
                                    string rutaArchivo = Redimensionar(mifoto, "tmp");

                                    data = System.IO.File.ReadAllBytes(Server.MapPath("~/" + rutaArchivo));
                                    System.IO.File.Delete(Server.MapPath("~/" + rutaArchivo));
                                }
                                cADEVENTOS_FOTO.OBSERVACION = cADEVENTOS_FOTO.OBSERVACION.TrimEnd();

                                if (data != null)
                                {
                                    cADEVENTOS_FOTO.FOTO = data;
                                }

                                contexto.Entry(cADEVENTOS_FOTO).State = EntityState.Modified;
                                await contexto.SaveChangesAsync();
                                auditoria.RegistrarAuditoria<CADEVENTOS_FOTOS>(cADEVENTOS_FOTO, "CADEVENTOS_FOTOS", "CADEVEN_SITIOGRABA=" + cADEVENTOS_FOTO.CADEVEN_SITIOGRABA.ToString() + ",CADEVEN_NUMEEVENTO=" + cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADEVENTOS_FOTO.CONSECUTIVO.ToString(), TipoAuditoria.Modificar);

                                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", "con el consecutivo : " + cADEVENTOS_FOTO.CONSECUTIVO.ToString());
                                var tipo = Resources.RecursoGeneral.MSN_TIPO_SUCCES.ToString();

                                return RedirectToAction("FotosEventos", new { strMensaje = mensaje, strTipo = tipo, strSitioGraba = cADEVENTOS_FOTO.CADEVEN_SITIOGRABA, strNumeroEvento = cADEVENTOS_FOTO.CADEVEN_NUMEEVENTO });
                            }
                        }
                    }
                }
                return View(cADEVENTOS_FOTO);
            }
            catch (DbEntityValidationException exE)
            {
                foreach (var eve in exE.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        //FUNCIONES PARA CONVERTIR DE BYTES A IMAGEN
        public string Bytes_A_Imagen(byte[] ImgBytes, string strConsecutivo)
        {
            Bitmap imagen = null;
            try
            {
                Byte[] bytes = (Byte[])(ImgBytes);
                MemoryStream ms = new MemoryStream(bytes);
                imagen = new Bitmap(ms);
                string rutaFinal = Redimensionar(imagen, strConsecutivo);
                imagen.Dispose();
                return rutaFinal;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        public string Redimensionar(Image Imagen_Original, string nombre)
        {
            try
            {
                //RUTA DEL DIRECTORIO TEMPORAL
                //String DirTemp = Path.GetTempPath() + @"~\Images\" + nombre + ".jpg";
                String DirTemp = "~/Images/Eventos/" + nombre + ".jpg";
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

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarFotoEditar(int? sitioGraba, int? numeroEvento, int? consecutivo)
        {
            try
            {
                JsonResult resultados = new JsonResult();
                CADEVENTOS_FOTOS cADEVENTOS_FOTOS = contexto.CADEVENTOS_FOTOS.Find(sitioGraba, numeroEvento, consecutivo);

                byte[] data = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == numeroEvento && t.CONSECUTIVO == consecutivo && t.CADEVEN_SITIOGRABA == sitioGraba select t.FOTO).FirstOrDefault();
                string strRuta = Url.Content("~/images/Avatar.png");

                if (data != null)
                {
                    strRuta = Url.Content("~" + Bytes_A_Imagen(data, consecutivo.ToString()));
                }

                resultados = Json(new
                {
                    result = true,
                    observaciones = cADEVENTOS_FOTOS.OBSERVACION,
                    sitioGraba = cADEVENTOS_FOTOS.CADEVEN_SITIOGRABA,
                    numeroEvento = cADEVENTOS_FOTOS.CADEVEN_NUMEEVENTO,
                    consecutivo = cADEVENTOS_FOTOS.CONSECUTIVO,
                    ruta = strRuta,
                    usuario = cADEVENTOS_FOTOS.USUA_GRABA
                });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult EliminarFotoEvento(int? sitioGraba, int? numeroEvento, int? consecutivo)
        {
            try
            {
                CADEVENTOS_FOTOS cADEVENTOS_FOTOS = contexto.CADEVENTOS_FOTOS.Find(sitioGraba, numeroEvento, consecutivo);


                contexto.CADEVENTOS_FOTOS.Remove(cADEVENTOS_FOTOS);
                contexto.SaveChangesAsync();

                CADEVENTOS_FOTOS cADEVENTOS_FOTOS_copy = new CADEVENTOS_FOTOS();
                cADEVENTOS_FOTOS_copy = (CADEVENTOS_FOTOS)auditoria.GenerarCopiaAuditoria(cADEVENTOS_FOTOS);

                auditoria.RegistrarAuditoria<CADEVENTOS_FOTOS>(cADEVENTOS_FOTOS_copy, "CADEVENTOS_FOTOS", "CADEVEN_SITIOGRABA=" + cADEVENTOS_FOTOS.CADEVEN_SITIOGRABA.ToString() + ",CADEVEN_NUMEEVENTO=" + cADEVENTOS_FOTOS.CADEVEN_NUMEEVENTO + ",CONSECUTIVO=" + cADEVENTOS_FOTOS.CONSECUTIVO.ToString(), TipoAuditoria.Eliminar);

                var ultimoConsecutivo = (from t in contexto.CADEVENTOS_FOTOS where t.CADEVEN_NUMEEVENTO == numeroEvento && t.CADEVEN_SITIOGRABA == sitioGraba orderby t.CONSECUTIVO descending select t.CONSECUTIVO).FirstOrDefault();
                int intConsecutivo = ultimoConsecutivo == 0 ? 1 : Convert.ToInt32(ultimoConsecutivo) + 1;

                var mensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO;
                var resultados = Json(new { result = true, datos = mensaje, consecutivo = intConsecutivo });

                return resultados;
            }
            catch (DbUpdateException)
            {
                var mensaje = "No se puede eliminar, restricción de integridad ya que se encuentran registros asociados";
                var tipo = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();

                return Json(new { result = false, strMensaje = mensaje, strTipo = tipo });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}