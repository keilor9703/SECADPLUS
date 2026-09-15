using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Controllers
{
    public class GestionarLlamadasPEntidadController : Controller
    {
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private DesRepMinEntities contexto = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities"));
        private SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private GestorAuditoria auditoria = new GestorAuditoria();
        private GestorGeneral objGestorGral;

        // GET: AdministrarNotificacion
        [SecadPlusAutorizador("/GestionarLlamadasPEntidad/Index")]
        public ActionResult Index(string strMensaje = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                }
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (Session["USUARIO_SECAD"] == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO;
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Metodo que obtiene Entidades
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerEntidades()
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;

                string strbotonImagen = " <a href='#' class='secadImg'><img src='URL_IMAGEN' alt ='Foto miniatura' WIDTH='44' HEIGHT='27' />" +
                                     "<span>" +
                                         "<img src='URL_IMAGEN' alt = 'Foto ampliada' />" +
                                     "</span>" +
                                     "</a> ";

                string strDestinoImagen = "";

                List<Ext_CadEntidades> listaEntidades = new List<Ext_CadEntidades>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 4)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (ABREVIATURA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY CADUNDE_CODIGO_PPAL ASC ";
                        break;

                    case 1:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 2:
                        strOrderna += " ORDER BY ABREVIATURA " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listFuerzasLlamadas", strConexion, "Nonquery");

                int intId = 0;
                byte[] data = null;
                foreach (DataRow fila in dttEntidades.Rows)
                {
                    try
                    {
                        intId = Convert.ToInt32(fila["ID"].ToString());
                        if (fila["ESCUDO"].ToString() != "")
                        {
                            data = (byte[])fila["ESCUDO"];
                            if (data != null && data.Length > 0)
                            {
                                strDestinoImagen = strbotonImagen.Replace("URL_IMAGEN", Url.Content("~" + Bytes_A_Imagen(data, intId.ToString())));
                            }
                            else
                                strDestinoImagen = "Sin Imagen";
                        }
                        else
                            strDestinoImagen = "Sin Imagen";
                    }
                    catch (Exception ex)
                    {
                        strDestinoImagen = ex.Message;
                    }

                    listaEntidades.Add(new Ext_CadEntidades(fila, "", strDestinoImagen));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaEntidades });
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
        /// Metodo que obtiene canales
        /// </summary>
        /// <param name="strFuerza"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerCanales(string strFuerza)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;

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
                                                           // '/GestionarLlamadasPEntidad/EditC?fuerzaId=FUERZA&codigo=_CODIGO'\">
                                                           // " +
                                                           "onclick =\"location.href='_EDITAR'\"> " +
                                                        "<i class='glyphicon glyphicon-edit'></i> Editar " +
                                                    "</button> " +
                                                "</li> " +
                                            "</ul>" +
                                        "</li>" +
                                    "</ul>";

                string strBotonDestino = "";

                List<Ext_CadCanales> listaCanales = new List<Ext_CadCanales>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') " +
                               " OR REGEXP_LIKE (DISPONIBLE, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY DESCRIPCION ASC ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";

                        break;

                    case 3:
                        strOrderna += " ORDER BY DISPONIBLE " + parametros.TipoOrdenamiento + " ";

                        break;
                }

                strLimite = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", strFuerza, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listCanalesXFuerzaLlamadas", strConexion, "Nonquery");

                foreach (DataRow fila in dttEntidades.Rows)
                {
                    // strBotonDestino = strBoton.Replace("FUERZA",
                    // fila["CADFUERZ_ID"].ToString()).Replace("_CODIGO", fila["CODIGO"].ToString());
                    // "onclick=\"window.location.href =
                    // '/GestionarLlamadasPEntidad/EditC?fuerzaId=FUERZA&codigo=_CODIGO'\"> " +

                    strBotonDestino = strBoton.Replace("_EDITAR", (Url.Action("EditC", new { fuerzaId = fila["CADFUERZ_ID"].ToString(), codigo = fila["CODIGO"].ToString() })).ToString());

                    listaCanales.Add(new Ext_CadCanales(fila, strBotonDestino, true));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaCanales });
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
        /// Metodo que obtiene pedidos
        /// </summary>
        /// <param name="strCanalFuerza"></param>
        /// <param name="strCanalCodigo"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult ObtenerPedidosEfectivos(string strCanalFuerza, string strCanalCodigo)
        {
            try
            {
                DatatableForm parametros = new DatatableForm(Request.Form);
                string strQuery = "", strLimite = "", strOrderna = "";
                bool blFiltro = false;
                GestorAdministrador gestor;
                DataTable dttEntidades;
                List<Ext_PedidosEfectivos> listaPedidos = new List<Ext_PedidosEfectivos>();
                GestorGeneral objGestorGral = new GestorGeneral();
                gestor = new GestorAdministrador();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;

                    strQuery += " WHERE REGEXP_LIKE (NUME_LLAMADA, '" + parametros.ConsultaValor + "', 'i') " +

                        " OR REGEXP_LIKE (NUME_TELEFONO, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (ESTADO_LLAMADA, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (MPIO, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (DIRE_CASO, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (BARRIO, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (CODIGO1, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (PRIORIDAD, '" + parametros.ConsultaValor + "', 'i') " +
                        " OR REGEXP_LIKE (HORA, '" + parametros.ConsultaValor + "', 'i') ";
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    default:
                        strOrderna += " ORDER BY HORA desc ";
                        break;

                    case 1:
                        strOrderna += " ORDER BY NUME_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 2:
                        strOrderna += " ORDER BY HORA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrderna += " ORDER BY NUME_TELEFONO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrderna += " ORDER BY ESTADO_LLAMADA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrderna += " ORDER BY MPIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrderna += " ORDER BY DIRE_CASO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrderna += " ORDER BY BARRIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrderna += " ORDER BY CODIGO1 " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrderna += " ORDER BY PRIORIDAD " + parametros.TipoOrdenamiento + " ";
                        break;
                }

                strLimite = "  OFFSET " + parametros.Inicio + " ROWS FETCH NEXT " + parametros.CantidadRegistros + " ROWS ONLY ";

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrderna, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal_fuerza", strCanalFuerza, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal_codigo", strCanalCodigo, typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dttEntidades = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listPedidosEfectivosLlamadas", strConexion, "Nonquery");

                foreach (DataRow fila in dttEntidades.Rows)
                {
                    listaPedidos.Add(new Ext_PedidosEfectivos(fila, ""));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }

                var resultados = Json(new { draw = Request.Form["Draw"], recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = listaPedidos });
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
        /// Metodo calcula diferencias fechas pedidos
        /// </summary>
        /// <param name="strFechaPedido"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult CalcularMinutosPedido(string strFechaPedido)
        {
            try
            {
                CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                objGestorGral = new GestorGeneral();

                decimal minutosSer = 0;
                DateTime? fecha = new DateTime();
                fecha = objGestorGral.ObtenerTiempoServidor;

                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_tot_min", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha1", strFechaPedido, typeof(DateTime)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha2", fecha.ToString(), typeof(DateTime)));

                minutosSer = (decimal)objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_resta_horas", strConexion);

                return Json(new { result = true, minutos = minutosSer });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = "Se presento un problema." });
            }
        }

        #region Canales

        /// <summary>
        /// Cargar Editar
        /// </summary>
        /// <param name="fuerzaId"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public async Task<ActionResult> EditC(int fuerzaId, int? codigo)
        {
            try
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                CADCANALE cADCANALE = await db.CADCANALES.FindAsync(fuerzaId, codigo);
                if (cADCANALE == null)
                {
                    return HttpNotFound();
                }

                ViewBag.DISPONIBLE = cADCANALE.DISPONIBLE == "S" ? true : false;
                return View(cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Canales/Edit/5 To protect from overposting attacks, please enable the specific
        // properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditC([Bind(Include = "CADFUERZ_ID,CODIGO,DESCRIPCION,ABREVIATURA,VIGENTE,DESDE,HASTA,OBSERVACION,DISPONIBLE,TIPO_CANAL,CLASE_TURNO,FECH_TRANSACCION,TIPO_ATENCION,AUTORIZACION_MEDIOS,AUTORIZACION_ANOTACIONES")] CADCANALE cADCANALE)
        {
            try
            {
                cADCANALE.DISPONIBLE = cADCANALE.DISPONIBLE == "true" ? "S" : "N";
                ModelState.Clear();
                TryValidateModel(cADCANALE);
                if (ModelState.IsValid)
                {
                    db.Entry(cADCANALE).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    auditoria.RegistrarAuditoria<CADCANALE>(cADCANALE, "CADCANALES", "CADFUERZ_ID =" + cADCANALE.CADFUERZ_ID.ToString() + ",CODIGO=" + cADCANALE.CODIGO, TipoAuditoria.Modificar);
                    return RedirectToAction("Index", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADCANALE.DESCRIPCION.ToString()) });
                }

                return View(cADCANALE);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion Canales

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
                String DirTemp = "~/Images/Fuerzas/" + nombre + ".bmp";
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
    }
}