using Oracle.ManagedDataAccess.Client;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADEntities.SecadLugaresGeograficos;
using SECADPLUS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Owin.Logging;
using SecadDatos.Context;
using System.Configuration;

namespace SECADPLUS.Controllers

{
   
    public class HomeController : Controller
    {



        /// <summary>
        /// /////////////////////////
        /// </summary>
       
        private readonly ILogger _logger;
        private string strConexion;
        private Validador validar = new Validador();
        private EntitiesComunasBarrios db = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));

       

        //-----------------------------------------------------------------------------------------------------------
        public Task<DtoResultado<List<DtoSlider>>> F_GetSilerSuperior()
        {
            DataTable resultado = new DataTable();
            List<DtoSlider> retorno = new List<DtoSlider>();
            DtoResultado<List<DtoSlider>> resp = new DtoResultado<List<DtoSlider>>();

            OracleConnection Conexion = null;
            OracleCommand objCommand = null;

            try
            {
                
                string connectionString = ConfigurationManager.ConnectionStrings["strConexionTelepol"].ConnectionString;

                Conexion = new OracleConnection(connectionString);
                objCommand = new OracleCommand
                {
                    Connection = Conexion,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PK_CTR_ADMINISTRACION.F_GetSlider",
                    BindByName = true
                };

                Conexion.Open();

                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                if (Conexion.State == ConnectionState.Open)
                {
                    
                    using (var reader = objCommand.ExecuteReader())
                    {
                        resultado.Load(reader);
                    }

                    // Convertir DataTable a lista de DTOs
                    retorno = UtilidadesDeMapeo.ConvertirDataTableAListaDto<DtoSlider>(resultado);

                    if (retorno.Count > 0)
                    {
                        resp.IdRespuesta = 1;
                        resp.Mensaje = "Consulta Exitosa";
                        resp.Operacion = "F_GetMenu";
                        resp.Data = retorno;
                    }
                    else
                    {
                        resp.IdRespuesta = 0;
                        resp.Mensaje = "No se encontraron datos";
                        resp.Operacion = "0";
                    }
                }
                else
                {
                    resp.IdRespuesta = 0;
                    resp.Mensaje = "Error conexión base de datos";
                    resp.Operacion = "0";
                }
            }
            catch (Exception e)
            {
                // Manejo de excepciones y liberación de recursos
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }

               
                System.Diagnostics.Trace.WriteLine("Error Ejecutando PK_CTR_ADMINISTRACION.F_GetSlider " + e);

                resp.IdRespuesta = 0;
                resp.Mensaje = $"{e.Message} - {e.InnerException}";
                resp.Operacion = "0";
            }
            finally
            {
               
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }
                if (resultado != null)
                {
                    resultado.Dispose();
                }
            }

            // Retornar la tarea
            return Task.FromResult(resp);
        }


        //-----------------------------------------------------------------------------------------------------------

        public DataTable F_GetImagenes(int Consecutivo)
        {
            DataTable objRetorno = new DataTable();

            OracleConnection Conexion = null;
            OracleCommand objCommand = null;
            OracleDataAdapter adaptador = null;

            try
            {
            
                string connectionString = ConfigurationManager.ConnectionStrings["strConexionTelepol"].ConnectionString;

                // Inicialización de objetos
                Conexion = new OracleConnection(connectionString);
                objCommand = new OracleCommand();
                adaptador = new OracleDataAdapter();

                // Configuración del comando
                objCommand.Connection = Conexion;
                objCommand.CommandType = CommandType.Text;
                objCommand.CommandText = "SELECT CONSECUTIVO, CONTENT_TYPE, FILENAME, FOTO FROM IMAGENES WHERE CONSECUTIVO = :consecutivo";

                // Abrir la conexión
                Conexion.Open();

                // Configuración de los parámetros
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("consecutivo", OracleDbType.Int32)).Value = Consecutivo;

                // Llenar el DataTable con los resultados
                if (Conexion.State == ConnectionState.Open)
                {
                    adaptador.SelectCommand = objCommand;
                    adaptador.Fill(objRetorno);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
              
                System.Diagnostics.Trace.WriteLine("Error Ejecutando la consulta de imagenes(): " + ex);

               
            }
            finally
            {
            
                if (Conexion != null)
                {
                    if (Conexion.State == ConnectionState.Open)
                    {
                        Conexion.Close();
                    }
                    Conexion.Dispose();
                }
                if (objCommand != null)
                {
                    objCommand.Dispose();
                }
                if (adaptador != null)
                {
                    adaptador.Dispose();
                }
            }

            return objRetorno;
        }


        //-----------------------------------------------------------------------------------------------------------

        public async Task<ActionResult> Slider()
        {
            try
            {
                // Llamada a método creado en esta misma sección
                var ImagenesSlider = F_GetSilerSuperior().Result;
                var SlidersView = new List<DtoSlider>();

                 foreach (var item in ImagenesSlider.Data)
                {
                    string ruta = ConsultarRuta(Convert.ToInt32(item.IMAGENES_CONSECUTIVO));
                    string ruta1 = "";

                    if (string.IsNullOrEmpty(ruta))
                    {
                        ruta1 = F_GetImagen(Convert.ToInt32(item.IMAGENES_CONSECUTIVO));
                    }
                    else
                    {
                        ruta1 = ruta;
                    }

                    var SliderView = new DtoSlider
                    {
                        CONSECUTIVO = item.CONSECUTIVO,
                        IMAGENES_CONSECUTIVO = item.IMAGENES_CONSECUTIVO,
                        URL = item.URL,
                        FILENAME = item.FILENAME,
                        ORDEN = item.ORDEN,
                        RUTA = ruta1
                    };
                    SlidersView.Add(SliderView);
                }
                ViewBag.SlidersView = SlidersView;
                return View();
            }
            catch (Exception e)
            {
                var SlidersView = new List<DtoSlider>();
                var SliderView = new DtoSlider
                {
                    IMAGENES_CONSECUTIVO = "19957",
                    FILENAME = "ARTE4_polired.jpg",
                    RUTA = "~/Images/Carrusel/19957.jpg"
                };
                ViewBag.SlidersView = SlidersView;
                return View();
            }
        }

   
        //-----------------------------------------------------------------------------------------------------------

        public string ConsultarRuta(int Consecutivo)
        {
            // Verificar si la imagen existe en una carpeta
            string[] formatos = new[] { ".tiff", ".ief", ".gif", ".jpg", ".png" };
            string ruta = "";
            string extensionArchivo = "";
            bool existe = false;
            string Resultado = "";

            // Obtener el directorio raíz del proyecto
  
            string directorioRaiz = AppDomain.CurrentDomain.BaseDirectory;

            foreach (string formato in formatos)
            {
                // Construir la ruta completa al archivo
                ruta = Path.Combine(directorioRaiz, "Images", "Carrusel", Consecutivo.ToString() + formato);

                if (System.IO.File.Exists(ruta))
                {
                    existe = true;
                    extensionArchivo = formato;
                    break;
                }
            }

            // Si existe, devolver la ruta relativa
            if (existe)
            {
                // La ruta debe ser relativa a la raíz del sitio web
                return "~/Images/Carrusel/" + Consecutivo.ToString() + extensionArchivo;
            }
            else
            {
                return Resultado;
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        public string F_GetImagen(int Consecutivo)
        {
            bool existe = false;
            string ruta = "";
            string extensionArchivo = "";
            string Resultado = "";

            // Obtener la imagen desde la base de datos
            DataTable dsImagen = F_GetImagenes(Consecutivo);

            if (dsImagen != null && dsImagen.Rows.Count == 1)
            {
                DataRow fila = dsImagen.Rows[0];
                string ContentType = fila[1].ToString(); 
                string FileName = fila[2].ToString();
                byte[] Foto = (byte[])fila[3]; 

                // Obtener la extensión del archivo según el ContentType
                switch (ContentType)
                {
                    case "image/tiff":
                        extensionArchivo = ".tiff";
                        break;
                    case "image/ief":
                        extensionArchivo = ".ief";
                        break;
                    case "image/gif":
                        extensionArchivo = ".gif";
                        break;
                    case "image/jpg":
                    case "image/jpeg":
                        extensionArchivo = ".jpg";
                        break;
                    case "image/png":
                        extensionArchivo = ".png";
                        break;
                    default:
                        extensionArchivo = "";
                        break;
                }

                if (!string.IsNullOrEmpty(extensionArchivo))
                {
                    // Construir la ruta para guardar la imagen
                    string directorioRaiz = AppDomain.CurrentDomain.BaseDirectory;
                    ruta = Path.Combine(directorioRaiz, "Images", "Carrusel", Consecutivo.ToString() + extensionArchivo);

                    // Guardar la imagen en la carpeta
                    System.IO.File.WriteAllBytes(ruta, Foto);
                    existe = true;
                }
            }

            // Si existe, devolver la ruta relativa
            if (existe)
            {
                return "~/Images/Carrusel/" + Consecutivo.ToString() + extensionArchivo;
            }
            else
            {
                return Resultado;
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        [Authorize]
        public async Task<ActionResult> Index()
        {
          
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            string UsaICP = System.Configuration.ConfigurationManager.AppSettings["UsaICP"].ToString();
            string UsaIVS = System.Configuration.ConfigurationManager.AppSettings["UsaIVS"].ToString();
            string UsaeLTE = System.Configuration.ConfigurationManager.AppSettings["UsaeLTE"].ToString();

            ViewBag.UsaICP = UsaICP;
            ViewBag.UsaIVS = UsaIVS;
            ViewBag.UsaeLTE = UsaeLTE;

            try
            {
                SecadPlusAutorizador autoriza;
                CADUSUARIO usr = null;
                List<CADBANNERIMAGENE> datos;
                Validador validar = new Validador();
                bool blValidaMaquina = false;
                InicioModel modelo = new InicioModel();
                byte[] btImagen = null;
                autoriza = new SecadPlusAutorizador("SeleccionarMaquina");
                modelo.ListaACD = null;
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
                {
                    datos = new List<CADBANNERIMAGENE>();
                    modelo.Imagenes = datos;
                    ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                    GestorBase.EX_CODIGO = "";
                    GestorBase.EX_CODIGO_INTERNO = "";
                    return View(modelo);
                }
                //Si el usuario de secad no se encuentra creado en la sesión intenta crearlo
                if (Session["USUARIO_SECAD"] == null)
                {
                    Session["MENUUSUARIO"] = null;
                    var valor = await GestorBase.ConsultarUsuarioSecad();
                    Session["MAQUINA"] = "";
                    usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    //Intenta identificar la máquina
                    blValidaMaquina = validar.IdentificarCliente(Request, Session);
                    validar.VerificarEquipoRegistrado(Session["MAQUINA"].ToString(), usr.SITIO_GRABACION);
                    //Verifica que la maquina se encuentre registrada
                    //if (validar.VerificarEquipoRegistrado(Session["MAQUINA"].ToString(), usr.SITIO_GRABACION))
                    //{
                    //    //Intenta realizar el registro de la conexion
                    //    GestorAuditoria.RegistrarConexion(true, Session["MAQUINA"].ToString(), usr);
                    //}
                }
                else
                {
                    //Obliga a que se actualice el menu si el usuario existe
                    Session["MENUUSUARIO"] = null;
                    //Actualiza la información de usuario de secad Autenticado
                    var valor = await GestorBase.ConsultarUsuarioSecad();
                    usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    Session["MAQUINA"] = "";
                    blValidaMaquina = validar.IdentificarCliente(Request, Session);
                    //Intenta identificar la máquina
                    //Verifica si el usuario ha seleccionado un nombre de máquina
                    if (Request.Form.Count > 0)
                    {
                        Session["MAQUINA"] = Request.Form["MaquinaSeleccionada"].ToString();
                        //crea cookies
                        HttpCookie cookie = new HttpCookie("NOMBRE_MAQUINA");
                        cookie.Value = Request.Form["MaquinaSeleccionada"].ToString();
                        cookie.Expires = Convert.ToDateTime("12/12/2900");
                        Response.Cookies.Add(cookie);
                        blValidaMaquina = true;
                    }
                    else
                    {
                        blValidaMaquina = validar.IdentificarCliente(Request, Session);
                    }
                    validar.VerificarEquipoRegistrado(Session["MAQUINA"].ToString(), usr.SITIO_GRABACION);
                    ////Verifica que la maquina se encuentre registrada
                    //if (validar.VerificarEquipoRegistrado(Session["MAQUINA"].ToString(), usr.SITIO_GRABACION))
                    //{
                    //    //Intenta realizar el registro de la conexion
                    //    GestorAuditoria.RegistrarConexion(true, Session["MAQUINA"].ToString(), usr);
                    //}
                }
                ViewBag.USUARIO = Session["MENUUSUARIO"];
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.USER = Session["USUARIO_SECAD"];
                ViewBag.Entidad = "";
                ViewBag.Canal = "";
                ViewBag.Sitio = "";
                ViewBag.ACD = "";
                ViewBag.Maquinas = "";
                ViewBag.FechaHora = ((DateTime)validar.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                //Por defecto intenta asignar una imagen de perfil temporal
                //Consulta la imagen asociada al policia autenticado.
                //btImagen = (from t in db.CADPOLICIAS_FOTOS where t.CEDU_EMPLEADO == usr.IDENTIFICACION select t.FOTO).FirstOrDefault();
                //if (btImagen != null)
                //   Session["UrlFotoUsuario"] = Url.Content("~" + Bytes_A_Imagen(btImagen, usr.IDENTIFICACION));

                string docId = @ClsEncriptar.Encriptar(Convert.ToString(usr.IDENTIFICACION)); // Asume que user.Id es el DocID
                string token = "Mxl7995Julabdfjughyts1*_58$$"; // Asegúrate de manejar el token de manera segura
                string urlFotoUsuario = $"https://sinac.policia.gov.co:8443/SinacPicture/picture.aspx?DocID={docId}&Token={token}";
                Session["UrlFotoUsuario"] = urlFotoUsuario;


                if (Session["MAQUINA"] != null && !string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    ViewBag.ACD = validar.ObtenerAcd(usr.SITIO_GRABACION, Session["MAQUINA"].ToString());
                }

                if (usr != null)
                {
                    var policia = (from registro in db.CADPOLICIAS
                                   where registro.CEDU_EMPLEADO == usr.IDENTIFICACION
                                   select new { registro.NOMB_EMPLEADO, registro.GRAD_ALFABETICO }).FirstOrDefault();
                    if (policia != null)
                    {
                        Session["NOMBRE_USUARIO"] = policia.GRAD_ALFABETICO + ". " + policia.NOMB_EMPLEADO;

                    }
                    using (SecadEntities contextoSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                    {

                        if (usr.CADCANA_FUERZA_ID > 0)
                            ViewBag.Entidad = contextoSecad.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                        if (usr.CADCANA_CODIGO > 0)
                        {
                            ViewBag.Canal = (from registro in contextoSecad.CADCANALES
                                             where registro.CODIGO == usr.CADCANA_CODIGO
                                             && registro.CADFUERZ_ID == usr.CADCANA_FUERZA_ID
                                             select registro).FirstOrDefault().DESCRIPCION;
                        }
                        if (usr.SITIO_GRABACION > 0)
                        {
                            ViewBag.Sitio = contextoSecad.CADSITIOS_GRABACION.Find(usr.SITIO_GRABACION).DESCRIPCION;
                        }
                        //Verifica si el sitio es BUCARAMANGA
                        //if (usr.SITIO_GRABACION == 7243)
                        //{

                        //    var acd = contextoSecad.CADREFERENCIAS.SqlQuery("SELECT * FROM USR_CAD.CADREFERENCIAS " +
                        //                                 "WHERE SITIO_GRABA = " + usr.SITIO_GRABACION + " " +

                        //                                 "AND NOMBRE = 'ARCHIVO_ACD' " +
                        //                                 "AND CODIGO NOT IN (SELECT NVL(ACD,'-1') FROM USR_CAD.CADEQUIPOS " +
                        //                                 "WHERE SITIO_GRABA = " + usr.SITIO_GRABACION + ") ORDER BY DESCRIPCION").Select(c => new { CODIGO = c.CODIGO, DESCRIPCION = c.DESCRIPCION });
                        //    //ViewBag.ListaACD = new SelectList(acd, "CODIGO", "DESCRIPCION");

                        //    modelo.ListaACD = new List<SelectListItem>();
                        //    foreach (var item in acd)
                        //    {
                        //        modelo.ListaACD.Add(new SelectListItem { Text = item.DESCRIPCION, Value = item.CODIGO });
                        //    }
                        //}
                        //verifica si tiene que presentar las máquinas
                        if (!blValidaMaquina)
                        {
                            GestorBase.MSN_ALERTA_COMUN = "";
                            if (autoriza.VerificarPermiso())
                            {
                                GestorBase.MSN_ALERTA_COMUN = "";
                                ViewBag.Maquinas = "PRESENTAR";
                                modelo.ListaMaquinas = new List<SelectListItem>();
                                var maquinas = contextoSecad.CADEQUIPOS.Where(c => c.SITIO_GRABA == usr.SITIO_GRABACION).Select(c => new { MAQUINA = c.IDENTIFICACION, ID = c.IDENTIFICACION });
                                foreach (var maquina in maquinas)
                                {
                                    modelo.ListaMaquinas.Add(new SelectListItem { Text = maquina.MAQUINA, Value = maquina.ID });
                                }
                            }
                            else
                            {
                                ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO;
                                ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                                GestorBase.MSN_ALERTA_COMUN = "";
                            }
                        }
                    }
                }


                modelo.Imagenes = db.CADBANNERIMAGENES.Where(t => t.ACTIVO == true).OrderBy(t => t.ORDEN).ToList();
                await Slider();
                return View(modelo);

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }


        //FUNCION PARA CONVERTIR DE BYTES A IMAGEN
        public string Bytes_A_Imagen(byte[] ImgBytes, string cedula)
        {
            Bitmap imagen = null;
            try
            {
                Byte[] bytes = (Byte[])(ImgBytes);
                MemoryStream ms = new MemoryStream(bytes);
                imagen = new Bitmap(ms);
                string rutaFinal = Redimensionar(imagen, cedula);
                imagen.Dispose();
                return rutaFinal;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string Redimensionar(Image Imagen_Original, string nombre)
        {
            try
            {

                String DirTemp = "~/images/FotosUsuarios/" + nombre + ".jpg";
                Bitmap imagen = new Bitmap(Imagen_Original);
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
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
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
        /// <summary>
        /// Permite presentar la ventana de inicio con una vista base personalizada
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ActionResult> InicioEmbebido()
        {
            try
            {
                string strNombre = "";
                CADUSUARIO usr = null;
                List<CADBANNERIMAGENE> datos;
                Validador validar = new Validador();
                IPHostEntry host = new IPHostEntry();
                InicioModel modelo = new InicioModel();
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
                {
                    datos = new List<CADBANNERIMAGENE>();
                    modelo.Imagenes = datos;
                    ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                    GestorBase.EX_CODIGO = "";
                    GestorBase.EX_CODIGO_INTERNO = "";
                    return View("Index", "MapaBase", modelo);
                }
                //Si el usuario de secad no se encuentra creado en la sesión intenta crearlo
                if (Session["USUARIO_SECAD"] == null)
                {
                    var valor = await GestorBase.ConsultarUsuarioSecad();
                    usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    //Intenta identificar la máquina
                    try
                    {

                        Session["MAQUINA"] = "";
                        validar.IdentificarCliente(Request, Session);
                        //Verifica que la maquina se encuentre registrada
                        if (validar.VerificarEquipoRegistrado(strNombre, usr.SITIO_GRABACION))
                        {
                            //Intenta realizar el registro de la conexion
                            GestorAuditoria.RegistrarConexion(true, strNombre, usr);
                        }
                    }
                    catch (Exception exControlada)
                    {
                        GestorBase.GestionarExcepcion(exControlada, GetType().Name);
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + strNombre + ")";
                    }
                }
                else
                {
                    //Obliga a que se actualice el menu si el usuario existe
                    Session["MENUUSUARIO"] = null;
                    //Actualiza la información de usuario de secad Autenticado
                    var valor = await GestorBase.ConsultarUsuarioSecad();
                    usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                }
                //Session["MAQUINA"] = "SRVSECADTEST";
                ViewBag.USUARIO = Session["MENUUSUARIO"];
                ViewBag.Maquina = Session["MAQUINA"];
                ViewBag.USER = Session["USUARIO_SECAD"];
                ViewBag.Entidad = "";
                ViewBag.Canal = "";
                ViewBag.Sitio = "";
                ViewBag.ACD = "";
                ViewBag.FechaHora = ((DateTime)validar.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                if (!string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    ViewBag.ACD = validar.ObtenerAcd(usr.SITIO_GRABACION, Session["MAQUINA"].ToString());
                }
                if (usr != null)
                {
                    using (SecadEntities contextoSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                    {
                        if (usr.CADCANA_FUERZA_ID > 0)
                            ViewBag.Entidad = contextoSecad.CADFUERZAS.Find(usr.CADCANA_FUERZA_ID).DESCRIPCION;
                        if (usr.CADCANA_CODIGO > 0)
                        {
                            ViewBag.Canal = (from registro in contextoSecad.CADCANALES
                                             where registro.CODIGO == usr.CADCANA_CODIGO
                                             && registro.CADFUERZ_ID == usr.CADCANA_FUERZA_ID
                                             select registro).FirstOrDefault().DESCRIPCION;
                        }
                        if (usr.SITIO_GRABACION > 0)
                        {
                            ViewBag.Sitio = contextoSecad.CADSITIOS_GRABACION.Find(usr.SITIO_GRABACION).DESCRIPCION;
                        }
                    }
                }
                //Consulta las imagenes configuradas para el banner                
                modelo.Imagenes = db.CADBANNERIMAGENES.Where(t => t.ACTIVO == true).OrderBy(t => t.ORDEN).ToList();
                return View("Index", "MapaBase", modelo);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("InicioEmbebido");
            }
        }

        [Authorize]
        public ActionResult Menu()
        {
            GestorSeguridad seguridad;
            PrincipalViewModel menuVista = new PrincipalViewModel();
            seguridad = new GestorSeguridad();
            //Carga el mennu asociado al usuario
            if (Session["MENUUSUARIO"] != null)
            {
                menuVista.MenuPrincipal = ((List<Menu>)Session["MENUUSUARIO"]);
            }
            else
            {
                menuVista.MenuPrincipal = seguridad.ObtenerMenu();
                Session["MENUUSUARIO"] = menuVista.MenuPrincipal;
            }
            return View("_MenuPartial", null, menuVista);
        }

        [Authorize]
        public JsonResult ObtenerMenu()
        {
            try
            {
                string strMenu = "";
                GestorSeguridad seguridad;
                PrincipalViewModel menuVista = new PrincipalViewModel();
                seguridad = new GestorSeguridad();
                //Carga el mennu asociado al usuario
                menuVista.MenuPrincipal = seguridad.ObtenerMenu();
                //recorre el menú consultado
                foreach (Menu seleccion in menuVista.MenuPrincipal)
                {
                    if (seleccion.Nivel == 1) { continue; }
                    //Identifica padres
                    if (seleccion.Tipo == "MODULO" && seleccion.Nivel == 2)
                    {
                        strMenu += "<li>";
                        strMenu += "<a href='#'><i class='icon" + seleccion.Icono + "'></i> <span>" + seleccion.Nombre + "</span></a>";
                        strMenu += "<ul>";
                        //Recorre valores hijos
                        foreach (Menu componente in menuVista.MenuPrincipal)
                        {
                            //Identifica menús
                            if (componente.Tipo == "MENU" && componente.Padre_Id == seleccion.Id)
                            {
                                strMenu += "<li>";
                                strMenu += "<a href='#'><i class='icon" + componente.Icono + "'></i> <span>" + componente.Nombre + "</span></a>";
                                strMenu += "<ul>";
                                foreach (Menu subComponente in menuVista.MenuPrincipal)
                                {
                                    //Identifica opcioens de menú
                                    if (subComponente.Tipo == "OPCION" && subComponente.Padre_Id == componente.Id)
                                    {
                                        strMenu += "<li>";
                                        strMenu += "<a href='" + Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme).ToString().Replace("/Home/Index", "") + subComponente.Ruta + "'><i class='icon" + subComponente.Icono + "'></i> <span>" + subComponente.Nombre + "</span></a>";
                                        strMenu += "</li>";
                                    }
                                }
                                strMenu += "</ul>";
                                strMenu += "</li>";
                            }
                            //Identifica hijos del tipo opcion
                            if (componente.Tipo == "OPCION" && componente.Padre_Id == seleccion.Id)
                            {
                                strMenu += "<li>";
                                strMenu += "<a href='" + Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme).ToString().Replace("/Home/Index", "") + componente.Ruta + "'><i class='icon" + componente.Icono + "'></i> <span>" + componente.Nombre + "</span></a>";
                                strMenu += "</li>";
                            }
                        }
                        strMenu += "</ul>";
                        strMenu += "</li>";
                    }
                    //identifica padres tipo opcion
                    if (seleccion.Tipo == "OPCION" && seleccion.Nivel == 2)
                    {
                        strMenu += "<li>";
                        strMenu += "<a href='" + Url.Action("Index", "Home", routeValues: null, protocol: Request.Url.Scheme).ToString().Replace("/Home/Index", "") + seleccion.Ruta + "'><i class='icon" + seleccion.Icono + "'></i> <span>" + seleccion.Nombre + "</span></a>";
                        strMenu += "</li>";
                    }
                }
                var resultados = Json(new { result = true, datos = strMenu });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Login.";
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public JsonResult ObtenerDatosNotificacion()
        {
            try
            {
                string strDatos = "", strEmergente = "False", strGlobo = "False";
                strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

                if (Session["USUARIO_SECAD"] == null)
                    return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USUARIO_NO_ENCONTRADO });
                else
                {
                    CADUSUARIO usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                    strDatos += usr.USUARIO + ",";
                    strDatos += usr.SITIO_GRABACION + ",";
                    strDatos += usr.CADCANA_CODIGO + ",";
                    strDatos += usr.CADCANA_FUERZA_ID;

                    GestorAdministrador gestor;
                    DataTable dttNotificaciones;

                    GestorGeneral objGestorGral = new GestorGeneral();
                    gestor = new GestorAdministrador();
                    objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                    dttNotificaciones = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_estadoNotificaciones", strConexion, "Nonquery");

                    foreach (DataRow fila in dttNotificaciones.Rows)
                    {
                        strDatos += "," + fila["NOMBRE"].ToString() + "=" + fila["PARAMETRO"].ToString();
                        if (fila["NOMBRE"].ToString().Equals("EMERGENTE"))
                        {
                            strEmergente = fila["PARAMETRO"].ToString();
                        }
                        else
                        {
                            strGlobo = fila["PARAMETRO"].ToString();
                        }
                    }
                }

                var resultados = Json(new { result = true, datos = strDatos, parametros = new { emergente = strEmergente, globo = strGlobo } });
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        public JsonResult ActualizarNotificaciones(string strCodigosNotificacion)
        {
            try
            {
                decimal strValor = 0;
                strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

                GestorGeneral objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cadenaCodigos", strCodigosNotificacion, typeof(string)));
                strValor = objGestorGral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_actualizarNotificaciones", strConexion);
                var resultado = Json(new { result = true, datos = strValor });
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult CambiarAcd(int intSitio, string strAcd)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                CADEQUIPOS equipo;
                string strMaquina = "";
                //Verifica si la máquina ha sido identificada.
                if (string.IsNullOrEmpty(Session["MAQUINA"].ToString()))
                {
                    return resultado = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NOMBRE_EQUIPO_FALLIDO + " (Nombre identificado:" + Session["MAQUINA"].ToString() + ")" });
                }
                strMaquina = Session["MAQUINA"].ToString();
                using (SecadEntities contextoSecad = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD")))
                {
                    equipo = (from registro in contextoSecad.CADEQUIPOS
                              where registro.SITIO_GRABA == intSitio
                              && registro.IDENTIFICACION == strMaquina
                              select registro).FirstOrDefault();
                    if (equipo != null)
                    {
                        equipo.ACD = strAcd;
                        contextoSecad.SaveChanges();
                        resultado = Json(new { result = true, datos = "ACD Actualizado." });
                    }
                    else
                    {
                        resultado = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EQUIPO_NO_EXISTE });
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite obtener la fecha y hora en formato militar del servidor de base de datos
        /// </summary>
        /// <param name="strSitio"></param>
        /// <param name="strTurno"></param>
        /// <param name="strClase"></param>
        /// <param name="strFechaInicia"></param>
        /// <param name="strFechaFin"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerHoraServidorBD()
        {
            try
            {
                JsonResult resultado = new JsonResult();
                GestorGeneral gestorGeneral = new GestorGeneral();
                string strDato = "";
                strDato = ((DateTime)validar.ObtenerTiempoServidor).ToString("dd/MM/yyyy HH:mm:ss");
                resultado = Json(new { result = true, datos = strDato });
                resultado.MaxJsonLength = int.MaxValue;
                return resultado;
            }
            catch (DbUpdateException dbEx)
            {
                var mensaje = dbEx.Message;
                return Json(new { result = false, datos = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", "Unidad de Turno Seleccionado") });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ComprobarVideo(string strUrl)
        {
            try
            {
                JsonResult resultado = new JsonResult();
                bool blExiste = false;
                // string ulr = Directory.GetCurrentDirectory() + @strUrl.ToString();
                string ulr = Server.MapPath("~/Videos/" + strUrl);

                if (System.IO.File.Exists(ulr))
                {
                    blExiste = true;
                }


                resultado = Json(new { result = blExiste, urlVirtual = ulr });
                resultado.MaxJsonLength = int.MaxValue;
                return resultado;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }
    }
}