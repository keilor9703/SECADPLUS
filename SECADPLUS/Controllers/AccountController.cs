using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SecadDatos.Areas.Despacho;
using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using SECADEntities.SecadLugaresGeograficos;
using SECADPLUS.Controllers.Seguridad;
using SECADPLUS.Models;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SECADPLUS.Controllers
{
    // [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private Integrador integrador;
        private LoginViewModel loginModel;
        private ApplicationUser user;
        private GestorGeneral objGestorGral;
        private GestorSeguridad seguridad;
        private PasswordHasher passManager;
        private string strNewPassword;
        private string strOidId = "";
        private string strProblemaLogin = "";
        private bool autenticarOid = false;
        private string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
        private PermisosEntities db = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities"));
        private SecadEntities contexto = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
        private SignInStatus result = SignInStatus.Success;
        private Validador validador = new Validador();
        private JsonResult resultados;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Permite presentar el listado de todos los usuarios
        /// </summary>
        /// <returns></returns>
        [SecadPlusAutorizador("/Account/Usuarios")]
        public ActionResult Usuarios()
        {
            try
            {
                //Verifica que el usuario cuente con un empleado asociado
                GestorSeguridad gestorSeguridad = new GestorSeguridad();
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                if (!gestorSeguridad.VerificarUsuarioCambioCanal())
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_ALERTA_USUARIO_SIN_EMPLEADO.ToString();
                    return RedirectToAction("Index", "Home");
                }
                return View("Usuarios", null, null);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [SecadPlusAutorizador("/Account/AdministrarUsuarios")]
        public ActionResult AdministrarUsuarios(string strMensaje = null)
        {
            try
            {
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador);
                ViewBag.NombreUbicacion = GestorBase.BREADCRUMB;
                if (!string.IsNullOrEmpty(GestorBase.MSN_ALERTA_COMUN))
                {
                    ViewBag.MENSAJE_EXITOSO = GestorBase.MSN_ALERTA_COMUN;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING;
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!string.IsNullOrEmpty(strMensaje))
                {
                    ViewBag.MENSAJE_EXITOSO = strMensaje;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_SUCCES;
                    strMensaje = "";
                }
                //Verifica que el usuario cuente con un empleado asociado
                GestorSeguridad gestorSeguridad = new GestorSeguridad();
                if (!gestorSeguridad.VerificarUsuarioCambioCanal())
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_ALERTA_USUARIO_SIN_EMPLEADO.ToString();
                    return RedirectToAction("Index", "Home");
                }

                return View("AdministrarUsuarios", null, null);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarUsuarios(string strOpcion)
        {
            try
            {
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                string strBotonFinal = "";
                UsuarioModel modelo = new UsuarioModel();
                DatatableForm parametros = new DatatableForm(Request.Form);
                modelo.Usuarios = new List<ExtUsuarioSecad>();
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl = 0;
                bool blControl = false, blFiltro = false;
                string strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat'" +
                    "data-toggle='modal' data-target='#modal_form_horizontal' data-popup='tooltip' data-placement='bottom' data-original-title='Clic para cambiar el canal del usuario' "
                    + "onclick=\"AbrirCambioCanal('_USUARIO', _SITIO, _FUERZAID, _CANALID, '_CONDICION');\"> " +
                    "<i class='icon-pencil'></i> </button>  </li></ul>";
                if (strOpcion.Equals("ADMINISTRAR"))
                {
                    strBoton = "<ul class='icons-list decad_menu'> <li class='dropdown'> <button type='button' class='btn btn-tabla border-success text-success-600 btn-flat dropdown-toggle' data-toggle='dropdown'" +
                    " aria-expanded='true'> <i class='icon-menu9'></i> </button> <ul class='dropdown-menu secad dropdown-menu-left'> " +
                    "<li> <button class='btn btn-default' type='button' data-popup='tooltip' onclick=\"AbrirCambioRol('_USUARIO_ID')\" data-placement='bottom' data-original-title='Clic para asignar roles'> <i class='icon-list'>" +
                    "</i> Asignar Roles </button> </li>" +
                    "<li> <button class='btn btn-default ' type='button' " +
                    " data-popup='tooltip' onclick=\"location.href='_EDITAR'\" data-placement='bottom' data-original-title='Clic para editar'> <i class='icon-pencil'>" +
                    "</i> Editar </button> </li><li> <button class='btn btn-default ' type='button' data-popup='tooltip' onclick=\"EliminarRegistroGeneral('_ELIMINAR')\" data-placement='bottom' data-original-title='Clic para eliminar'> <i class='icon-trash'></i> Eliminar </button> </li></ul> </li></ul>";
                }
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (US.IDENTIFICACION = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE (US.USUARIO,'" + parametros.ConsultaValor + "','i') OR REGEXP_LIKE(OIDUSU.\"UserName\", '" + parametros.ConsultaValor + "','i')  "
                            + "OR CANAL.DESCRIPCION LIKE '%" + parametros.ConsultaValor + "%' OR REGEXP_LIKE(CIUDAD.DESCRIPCION,'" + parametros.ConsultaValor + "','i') "
                            + "OR REGEXP_LIKE(DPTO.DESCRIPCION,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(SITIO.DESCRIPCION,'" + parametros.ConsultaValor + "','i')  "
                        + "OR REGEXP_LIKE(COND.DESCRIPCION,'" + parametros.ConsultaValor + "', 'i') OR REGEXP_LIKE(FUERZA.DESCRIPCION,'" + parametros.ConsultaValor + "','i') ) ";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY US.USUARIO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY US.IDENTIFICACION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento += " ORDER BY PERSONA.NOMB_EMPLEADO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento += " ORDER BY PERSONA.GRAD_ALFABETICO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 6:
                        strOrdenamiento += " ORDER BY DPTO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 7:
                        strOrdenamiento += " ORDER BY CIUDAD.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 8:
                        strOrdenamiento += " ORDER BY US.FECHA_DESDE " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 9:
                        strOrdenamiento += " ORDER BY US.FECHA_HASTA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 10:
                        strOrdenamiento += " ORDER BY COND.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 11:
                        strOrdenamiento += " ORDER BY SITIO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 12:
                        strOrdenamiento += " ORDER BY FUERZA.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 13:
                        strOrdenamiento += " ORDER BY CANAL.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 14:
                        strOrdenamiento += " ORDER BY OIDUSU.\"UserName\" " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "USUARIOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    if (strOpcion.Equals("ADMINISTRAR"))
                    {
                        strBotonFinal = strBoton;
                        strBotonFinal = strBotonFinal.Replace("_EDITAR", Url.Action("EditarUsuario", "Account", new { strUsuario = fila["USUARIO"].ToString() })).Replace("_USUARIO_ID", fila["USUARIO_OID_ID"].ToString());
                        strBotonFinal = strBotonFinal.Replace("_ELIMINAR", Url.Action("Eliminar", "Account", new { strUsuario = fila["USUARIO"].ToString() }));
                        modelo.Usuarios.Add(new Models.Extendidas.ExtUsuarioSecad(fila, true, strBotonFinal));
                    }
                    else
                    {
                        modelo.Usuarios.Add(new Models.Extendidas.ExtUsuarioSecad(fila, strBoton));
                    }

                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                var resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = modelo.Usuarios });
                resultados.MaxJsonLength = int.MaxValue;
                return resultados;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = true, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }


        #region Metodos Huawei
        /// <summary>
        /// Creado por: Jhon Noel Salcedo.
        /// Fecha: 21/03/2019
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConfiguracionICP()
        {
            string NombrePC = Session["MAQUINA"].ToString();
            PONAL.SECAD.Huawei.Model.Context db = new PONAL.SECAD.Huawei.Model.Context();
            PONAL.SECAD.Huawei.Model.ConfiguracionICPModel Configuracion = (from c in db.CADCONEXION
                                                                            join e in db.CADEQUIPOS on c.MAQUINA equals e.IDENTIFICACION
                                                                            join h in db.CADHUAWEI_DCONEXION on e.ACD equals h.EXT_ACD
                                                                            where e.IDENTIFICACION == NombrePC && c.VIGENTE == "S" && h.TIPO == "CNXICP"
                                                                            select new PONAL.SECAD.Huawei.Model.ConfiguracionICPModel { SitioGraba = c.SITIO_GRABA, UsuarioSecad = c.CADUSUA_USUARIO, NombrePC = e.IDENTIFICACION, Extension = e.ACD, Usuario = h.USUARIO, Contrasena = h.PASSWORD_, Agente = h.AGENTE }).FirstOrDefault();

            var t = (from s in db.CADHUAWEI_DCONEXION where s.TIPO == "SRVICP" select new { URL = s.IPSERVER }).FirstOrDefault();

            Configuracion.URL = (t.URL != null ? t.URL : "");

            return Json(Configuracion);
        }
        #endregion

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConfiguracionLTE()
        {
            string NombrePC = Session["MAQUINA"].ToString();
            PONAL.SECAD.Huawei.Model.Context db = new PONAL.SECAD.Huawei.Model.Context();
            PONAL.SECAD.Huawei.Model.ConfiguracionICPModel Configuracion = (from c in db.CADCONEXION
                                                                            join e in db.CADEQUIPOS on c.MAQUINA equals e.IDENTIFICACION
                                                                            join h in db.CADHUAWEI_DCONEXION on e.ACD equals h.EXT_ACD
                                                                            where e.IDENTIFICACION == NombrePC && c.VIGENTE == "S" && h.TIPO == "CNXICP"
                                                                            select new PONAL.SECAD.Huawei.Model.ConfiguracionICPModel { SitioGraba = c.SITIO_GRABA, UsuarioSecad = c.CADUSUA_USUARIO, NombrePC = e.IDENTIFICACION, Extension = e.ACD, Usuario = h.USUARIO, Contrasena = h.PASSWORD_, Agente = h.AGENTE }).FirstOrDefault();

            var t = (from s in db.CADHUAWEI_DCONEXION where s.TIPO == "SRVLTE" select new { URL = s.IPSERVER }).FirstOrDefault();

            Configuracion.URL = (t.URL != null ? t.URL : "");

            return Json(Configuracion);
        }

        [Authorize]
        [HttpPost]
        public JsonResult ObtenerConfiguracionIVS()
        {
            string NombrePC = Session["MAQUINA"].ToString();
            PONAL.SECAD.Huawei.Model.Context db = new PONAL.SECAD.Huawei.Model.Context();
            PONAL.SECAD.Huawei.Model.ConfiguracionICPModel Configuracion = (from c in db.CADCONEXION
                                                                            join e in db.CADEQUIPOS on c.MAQUINA equals e.IDENTIFICACION
                                                                            join h in db.CADHUAWEI_DCONEXION on e.ACD equals h.EXT_ACD
                                                                            where e.IDENTIFICACION == NombrePC && c.VIGENTE == "S" && h.TIPO == "CNXICP"
                                                                            select new PONAL.SECAD.Huawei.Model.ConfiguracionICPModel { SitioGraba = c.SITIO_GRABA, UsuarioSecad = c.CADUSUA_USUARIO, NombrePC = e.IDENTIFICACION, Extension = e.ACD, Usuario = h.USUARIO, Contrasena = h.PASSWORD_, Agente = h.AGENTE }).FirstOrDefault();

            var t = (from s in db.CADHUAWEI_DCONEXION where s.TIPO == "SRVIVS" select new { URL = s.IPSERVER }).FirstOrDefault();

            Configuracion.URL = (t.URL != null ? t.URL : "");

            return Json(Configuracion);
        }

        internal class temporal
        {
            public temporal(string nombre, string id, string nom)
            {
                Nombre = nombre;
                Identificacion = id;
                NombreEmpleado = nom;
            }

            public string Nombre { get; set; }
            public string Identificacion { get; set; }
            public string NombreEmpleado { get; set; }
        }

        [Authorize]
        public JsonResult ActualizarUsuarios()
        {
            try
            {
                GestorGeneral gestor = new GestorGeneral();
                gestor.ActualizarUsuariosOfite();
                return Json(new { result = true, datos = "Finalizado" });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(new { result = false, datos = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO) });
            }
        }

        /// <summary>
        /// Permite presentar las listas para el cambio de canales
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CambioCanal()
        {
            try
            {
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion");
                ViewBag.SITIOS = new SelectList((from sitio in contexto.CADSITIOS_GRABACION select new { codigo = sitio.CONSECUTIVO, descripcion = sitio.DESCRIPCION }).OrderBy(s => s.descripcion), "codigo", "descripcion");
                //Genera los listados necesarios
                //ViewBag.Codicion
                return View("_CambioCanal", null, null);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Permite cambiar el canal de un usuario
        /// </summary>
        /// <param name="strUsuarioId"></param>
        /// <param name="intSitio"></param>
        /// <param name="intEntidad"></param>
        /// <param name="intCanal"></param>
        /// <param name="strCondicion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult CambiarCanal(string strUsuario, int intSitio, int intEntidad, int intCanal, string strCondicion)
        {
            try
            {
                using (PermisosEntities contexto = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities")))
                {
                    CADUSUARIO usuario = contexto.CADUSUARIOS.Find(strUsuario);
                    if (intSitio > 0)
                    {
                        usuario.SITIO_GRABACION = intSitio;
                    }

                    if (intEntidad > 0)
                    {
                        usuario.CADCANA_FUERZA_ID = intEntidad;
                    }
                    else
                    {
                        usuario.CADCANA_FUERZA_ID = null;
                    }

                    if (intCanal > 0)
                    {
                        usuario.CADCANA_CODIGO = intCanal;
                    }
                    else
                    {
                        usuario.CADCANA_CODIGO = null;
                    }

                    if (!string.IsNullOrEmpty(strCondicion))
                        usuario.CONDICION = strCondicion;
                    contexto.SaveChanges();
                    contexto.Dispose();
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        /// <summary>
        /// Permite obtener los canales asociados a una entidad
        /// </summary>
        /// <param name="intEntidad">Identificador de la entidad consultada</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult ObtenerCanales(int intEntidad)
        {
            try
            {
                var datos = (from canal in contexto.CADCANALES where canal.CADFUERZ_ID == intEntidad select new { codigo = canal.CODIGO, descripcion = canal.DESCRIPCION }).OrderBy(c => c.descripcion).ToArray();
                return Json(datos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        /// <summary>
        /// Permite consultar las entidades asociadas a un sitio de grabación
        /// </summary>
        /// <param name="intSitio"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public JsonResult ObtenerEntidades(int intSitio)
        {
            try
            {
                EntitiesComunasBarrios contexto = new EntitiesComunasBarrios(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "ComunasBarriosModel", "SecadLugaresGeograficos"));
                var datos = (from fuerza in contexto.CADFUERZAS where fuerza.SITIO_GRABA == intSitio select new { codigo = fuerza.ID, descripcion = fuerza.DESCRIPCION }).OrderBy(c => c.descripcion).ToList();
                return Json(datos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return Json(false);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl, string _mensaje)
        {
            loginModel = new LoginViewModel();
            try
            {

                //credenciales 
                // _mensaje = "NEtoTVFmL1hnc3IyUTVCQy91NVZYcjMvMEd1YzgwOFB6ZFNCUjVraHBjUy9ZSTNnbXFRT1BTZncvSDMydEo5WTNRUHplSzNZTmMrMGxaS3FuZzV2aUdMU1BqU0ZtdThYbzNlOHp5dkhVakk9";

                if (_mensaje != null)
                {
                    string mensajeRecibido = new SeguridadRepositorio().ConvertirBase64Bytes(_mensaje);
                    string mensajeDesencriptado = new SeguridadRepositorio().Decript(mensajeRecibido, WebConfigurationManager.AppSettings["Key"]);

                    LoginUsuarioDTO loginUsuarioDTO = JsonConvert.DeserializeObject<LoginUsuarioDTO>(mensajeDesencriptado);
                    string claveRecibida = new SeguridadRepositorio().ConvertirBase64Bytes(loginUsuarioDTO.Clave);
                    string claveDesencriptada = new SeguridadRepositorio().Decript(claveRecibida, WebConfigurationManager.AppSettings["Key"]);

                    LoginViewModel model = new LoginViewModel()
                    {
                        UserName = loginUsuarioDTO.Usuario,
                        Password = claveDesencriptada
                    };

                    //Carga la lista de conexiones
                    integrador = new Integrador();
                    integrador.ParamsSimples = new List<string>();
                    integrador.ParamsSimples.Add(Resources.RecursoGeneral.Integrador_Opcion_ListarConexiones);
                    integrador.Ejecutar();
                    if (integrador.ListaSimple.Count < 1)
                    {
                        model.ListaConexiones = new List<SelectListItem>();
                        return Redirect(await new DbDespacho().ConsultaRutasSistemas(WebConfigurationManager.AppSettings["AplicacionLocal"], "LOGINCORE", WebConfigurationManager.AppSettings["AplicacionPruebas"]));
                    }
                    else
                    {
                        model.ListaConexiones = integrador.ListaSimple;
                        //model.Conexion = ConfigurationManager.AppSettings["ConexionPorDefecto"];


                    }
                    if (!ModelState.IsValid)
                    {
                        return View("Login", "LoginBase", model);
                    }
                    //genera variable de sesión con la conexion seleccionada
                    System.Web.HttpContext.Current.Session["ConexionActiva"] = model.Conexion;
                    seguridad = new GestorSeguridad();
                    //UserManager.Dispose();

                    UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext(new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString)))));
                    SignInManager = new ApplicationSignInManager(UserManager, Request.GetOwinContext().Authentication);
                    //ControllerContext.HttpContext.User;
                    user = UserManager.FindByName(model.UserName);
                    if (user != null) strOidId = user.Id;
                    autenticarOid = seguridad.VerificarEsquemaOid(strOidId);
                   
                   
                    if (autenticarOid)
                    {
                        //Realiza la autenticación a través del OID
                        if (seguridad.AutenticarOID(model.UserName, model.Password))
                        {
                            //Se realiza la actualización de la contraseña el usuario autenticado
                            passManager = new PasswordHasher();
                            strNewPassword = passManager.HashPassword(model.Password);
                            //model.Password = strNewPassword;
                            user.PasswordHash = strNewPassword;
                            IdentityResult resultado = UserManager.Update(user);
                            //UpdateModel(model);
                        }
                        else
                        {
                            //Verifica la respuesta de OID
                            if (seguridad.OidRespuesta == SecadEnums.OIDRespuesta.DatosIncorrectos)
                            {
                                ModelState.AddModelError("", Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos);
                                strProblemaLogin = Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos;
                                result = SignInStatus.Failure;
                                var registroOOID = await GestorAuditoria.RegistrarIntentoAutenticacion(autenticarOid, model.Conexion, result, strOidId, strProblemaLogin);
                                return View("Login", "LoginBase", model);
                            }
                            //Si la conexion a OID se encuentra caída entonces obliga a la autenticación alterna
                            if (seguridad.OidRespuesta == SecadEnums.OIDRespuesta.SinConexion)
                            {
                                result = SignInStatus.Success;
                            }
                        }
                        //FIN AUTENTICACIÓN POR OID
                    }
                    //verificar estado
                    if (user != null && !seguridad.ConsultarEstadoEmpleado(user.Id))
                    {
                        result = SignInStatus.Failure;
                        ModelState.AddModelError("", "El estado del empleado asociado al usuario no es activo.");
                        strProblemaLogin = "El estado del empleado asociado al usuario no es activo";
                    }
                    //Verifica si require realizar la autenticación por identity
                    if (result == SignInStatus.Success)
                        result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                    var registroIngreso = await GestorAuditoria.RegistrarIntentoAutenticacion(autenticarOid, model.Conexion, result, strOidId, strProblemaLogin);
                    //evalua el resultado de la autenticación
                    switch (result)
                    {
                        case SignInStatus.Success:
                            //Inicializa variables de sesión
                            Session["EX_CODIGO"] = "";
                            Session["EX_CODIGO_INTERNO"] = "";
                            Session["USUARIO_SECAD"] = null;
                            Session["S_PRIEMERA_ENTRADA"] = true;
                            Session["S_BL_NOTIFICACION"] = true;
                            Session["S_CodigoNotificacion"] = "";
                            Session["MAQUINA"] = "";
                            Session["BREADCRUMB"] = "";
                            Session["UrlFotoUsuario"] = Url.Content("~/images/Avatar.png");
                            Session["NOMBRE_USUARIO"] = "";
                            //Realiza el registro de la conexion
                            RegistrarConexion(strOidId);
                            return RedirectToAction("Index", "Home");
                        //return RedirectToLocal(returnUrl);

                        case SignInStatus.LockedOut:
                            return View("Lockout");

                        case SignInStatus.RequiresVerification:
                            return Redirect(await new DbDespacho().ConsultaRutasSistemas(WebConfigurationManager.AppSettings["AplicacionLocal"], "LOGINCORE", WebConfigurationManager.AppSettings["AplicacionPruebas"]));

                        case SignInStatus.Failure:
                            ModelState.AddModelError("", Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos);
                            return Redirect(await new DbDespacho().ConsultaRutasSistemas(WebConfigurationManager.AppSettings["AplicacionLocal"], "LOGINCORE", WebConfigurationManager.AppSettings["AplicacionPruebas"]));

                        default:
                            ModelState.AddModelError("", "Intento de autenticación fallido.");
                            return Redirect(await new DbDespacho().ConsultaRutasSistemas(WebConfigurationManager.AppSettings["AplicacionLocal"], "LOGINCORE", WebConfigurationManager.AppSettings["AplicacionPruebas"]));
                    }
                }
                else
                {

                    integrador = new Integrador();
                    integrador.ParamsSimples = new List<string>();
                    integrador.ParamsSimples.Add(Resources.RecursoGeneral.Integrador_Opcion_ListarConexiones);
                    integrador.Ejecutar();

                    if (integrador.ListaSimple.Count < 1)
                    {
                        loginModel.ListaConexiones = new List<SelectListItem>();
                        ViewData.Add("MSG_LISTAVACIA", Resources.RecursoGeneral.MSG_LISTAVACIA);
                    }
                    else
                    {
                        loginModel.ListaConexiones = integrador.ListaSimple;

                    }

                    if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
                    {
                        ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                        GestorBase.EX_CODIGO = "";
                        GestorBase.EX_CODIGO_INTERNO = "";
                    }

                    ViewBag.ReturnUrl = returnUrl;

                    return View("Login", "LoginBase", loginModel);

                   


                   
                }

            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
                {
                    ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                    GestorBase.EX_CODIGO = "";
                    GestorBase.EX_CODIGO_INTERNO = "";
                }
                return Redirect(await new DbDespacho().ConsultaRutasSistemas(WebConfigurationManager.AppSettings["AplicacionLocal"], "LOGINCORE", WebConfigurationManager.AppSettings["AplicacionPruebas"]));
            }
            //catch (Exception ex)
            //{
            //    GestorBase.GestionarExcepcion(ex, GetType().Name);
            //    return View("Login", "LoginBase", loginModel);
            //}
        }


       

    ///
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                //Carga la lista de conexiones
                integrador = new Integrador();
                integrador.ParamsSimples = new List<string>();
                integrador.ParamsSimples.Add(Resources.RecursoGeneral.Integrador_Opcion_ListarConexiones);
                integrador.Ejecutar();

                if (integrador.ListaSimple.Count < 1)
                {
                    model.ListaConexiones = new List<SelectListItem>();
                    ViewData.Add("MSG_LISTAVACIA", Resources.RecursoGeneral.MSG_LISTAVACIA);
                }
                else
                {
                    model.ListaConexiones = integrador.ListaSimple;

                }
                if (!ModelState.IsValid)
                {
                    return View("Login", "LoginBase", model);
                }



                //genera variable de sesión con la conexion seleccionada
                System.Web.HttpContext.Current.Session["ConexionActiva"] = model.Conexion; //model.Conexion;
                seguridad = new GestorSeguridad();



                //UserManager.Dispose();

                UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext(new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString)))));
                SignInManager = new ApplicationSignInManager(UserManager, Request.GetOwinContext().Authentication);
                //ControllerContext.HttpContext.User;
                user = UserManager.FindByName(model.UserName);
                if (user != null) strOidId = user.Id;
                autenticarOid = seguridad.VerificarEsquemaOid(strOidId);
                if (autenticarOid)
                {
                    //Realiza la autenticación a través del OID
                    if (seguridad.AutenticarOID(model.UserName, model.Password))
                    {
                        //Se realiza la actualización de la contraseña el usuario autenticado
                        passManager = new PasswordHasher();
                        strNewPassword = passManager.HashPassword(model.Password);
                        //model.Password = strNewPassword;
                        user.PasswordHash = strNewPassword;
                        IdentityResult resultado = UserManager.Update(user);
                        //UpdateModel(model);
                    }
                    else
                    {
                        //Verifica la respuesta de OID
                        if (seguridad.OidRespuesta == SecadEnums.OIDRespuesta.DatosIncorrectos)
                        {
                            ModelState.AddModelError("", Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos);
                            strProblemaLogin = Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos;
                            result = SignInStatus.Failure;
                            var registroOOID = await GestorAuditoria.RegistrarIntentoAutenticacion(autenticarOid, model.Conexion, result, strOidId, strProblemaLogin);
                            return View("Login", "LoginBase", model);
                        }
                        //Si la conexion a OID se encuentra caída entonces obliga a la autenticación alterna
                        if (seguridad.OidRespuesta == SecadEnums.OIDRespuesta.SinConexion)
                        {
                            result = SignInStatus.Success;
                        }
                    }
                    //FIN AUTENTICACIÓN POR OID
                }
                //verificar estado
                if (user != null && !seguridad.ConsultarEstadoEmpleado(user.Id))
                {
                    result = SignInStatus.Failure;
                    ModelState.AddModelError("", "El estado del empleado asociado al usuario no es activo.");
                    strProblemaLogin = "El estado del empleado asociado al usuario no es activo";
                }
                //Verifica si require realizar la autenticación por identity
                if (result == SignInStatus.Success)
                    result = SignInManager.PasswordSignIn(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                var registroIngreso = await GestorAuditoria.RegistrarIntentoAutenticacion(autenticarOid, model.Conexion, result, strOidId, strProblemaLogin);
                //evalua el resultado de la autenticación

                

                switch (result) 
                {
                    case SignInStatus.Success:
                        //Inicializa variables de sesión
                        Session["EX_CODIGO"] = "";
                        Session["EX_CODIGO_INTERNO"] = "";
                        Session["USUARIO_SECAD"] = null;
                        Session["S_PRIEMERA_ENTRADA"] = true;
                        Session["S_BL_NOTIFICACION"] = true;
                        Session["S_CodigoNotificacion"] = "";
                        Session["MAQUINA"] = "";
                        Session["BREADCRUMB"] = "";
                        //Session["UrlFotoUsuario"] = Url.Content("~/images/Avatar.png");
                        Session["UrlFotoUsuario"] = Url.Content("~/images/Avatar.png");
                        Session["NOMBRE_USUARIO"] = "";
                        //Realiza el registro de la conexion
                        RegistrarConexion(strOidId);
                        return RedirectToLocal(returnUrl);

                    case SignInStatus.LockedOut:
                        return View("Lockout");

                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                    case SignInStatus.Failure:
                        ModelState.AddModelError("", Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos);
                        return View("Login", "LoginBase", model);

                    default:
                        ModelState.AddModelError("", "Intento de autenticación fallido.");
                        return View("Login", "LoginBase", model);
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                if (!string.IsNullOrEmpty(GestorBase.EX_CODIGO))
                {
                    ViewBag.ERROR_MENSAJE = Resources.RecursoGeneral.EX_MENSAJE.Replace("ERROR_CODE", GestorBase.EX_CODIGO);
                    GestorBase.EX_CODIGO = "";
                    GestorBase.EX_CODIGO_INTERNO = "";
                }
                return View("Login", "LoginBase", model);
            }
        }


        //////////////////////////////////////////TERMINA NUEVO
        private void RegistrarConexion(string strIdOid)
        {
            try
            {
                CADUSUARIO usr;
                GestorBase.ConsultarUsuarioSecad(strIdOid);
                Session["MAQUINA"] = "";
                usr = (CADUSUARIO)Session["USUARIO_SECAD"];
                //Intenta identificar la máquina
                validador.IdentificarCliente(Request, Session);
                //Verifica que la maquina se encuentre registrada
                if (validador.VerificarEquipoRegistrado(Session["MAQUINA"].ToString(), usr.SITIO_GRABACION))
                {
                    //Intenta realizar el registro de la conexion
                    GestorAuditoria.RegistrarConexion(true, Session["MAQUINA"].ToString(), usr);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        // GET: /Account/Register

        public ActionResult Register()
        {
            GestorGeneral gestor;
            try
            {
                RegisterViewModel registerModelo = new RegisterViewModel();
                SecadEntities db = new SecadEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD"));
                registerModelo.Departamentos = new SelectList(db.CADLUGARES_GEOGRAFICOS.Where(m => m.TIPO == "DE").OrderBy(m => m.DESCRIPCION), "CODIGO", "DESCRIPCION");
                gestor = new GestorGeneral();
                //registerModelo.Empleados = gestor.ObtenerEmpleados();
                return View(registerModelo);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                return View();
            }
        }

        public ApplicationUser CrearUsuarioOid(string strNombre, string strContrasena)
        {
            try
            {
                ApplicationUser usuario;
                UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext(new OracleConnection(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString)))));
                usuario = UserManager.FindByName(strNombre);
                if (usuario == null)
                {
                    if (string.IsNullOrEmpty(strContrasena))
                        strContrasena = Resources.RecursoGeneral.DEFAULT_PASSWORD;
                    ///Crea el usuario
                    usuario = new ApplicationUser();
                    usuario.UserName = strNombre;
                    usuario.Email = strNombre;
                    usuario.estado = true;
                    var result = UserManager.Create(usuario, strContrasena);
                }
                else if (!string.IsNullOrEmpty(strContrasena))
                {
                    //Se realiza la actualización de la contraseña el usuario autenticado
                    passManager = new PasswordHasher();
                    strNewPassword = passManager.HashPassword(strContrasena);
                    //model.Password = strNewPassword;
                    usuario.PasswordHash = strNewPassword;
                    //Actualiza el usuario
                    var result = UserManager.Update(usuario);
                }
                return usuario;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }



        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                string strNombreEquipo = "";
                if (Session["USUARIO_SECAD"] != null)
                {
                    CADUSUARIO usuarioActual = (CADUSUARIO)Session["USUARIO_SECAD"];
                    Session["MAQUINA"] = "";

                    //Intenta identificar la máquina
                    validador.IdentificarCliente(Request, Session);
                    strNombreEquipo = Session["MAQUINA"].ToString();
                    GestorAuditoria.RegistrarConexion(false, strNombreEquipo, usuarioActual);
                }
                //LIMPIA LAS VARIABLES DE SESION
                Session.Clear();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
            finally
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            //return RedirectToAction("Login", "Account");
            return Redirect("https://sisec.policia.gov.co/");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario([Bind(Include = "USUARIO,IDENTIFICACION,LUGE_CODIGO_DPTO,CADUNDE_FUERZA,CADUNDE_CODIGO,FECHA_DESDE,FECHA_HASTA,CONDICION,LUGE_CODIGO_CAD,CADCANA_FUERZA_ID,CADCANA_CODIGO,SITIO_GRABACION,USUARIO_OID_ID,VIGENTE,AUTENTICA_OID,USUARIO_OID")] CADUSUARIO cADUSUARIO)
        {
            try
            {
                CADUSUARIO usuarioExiste;
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion", cADUSUARIO.CONDICION);
                ViewBag.SITIO_GRABACION = new SelectList((from sitio in contexto.CADSITIOS_GRABACION select new { codigo = sitio.CONSECUTIVO, descripcion = sitio.DESCRIPCION }).OrderBy(s => s.descripcion), "codigo", "descripcion", cADUSUARIO.SITIO_GRABACION);
                ViewBag.Grado = Request.Form["txtGrado"];
                ViewBag.Nombre = Request.Form["txtNombre"];
                ViewBag.Dpto = Request.Form["txtDpto"];
                ViewBag.Ciudad = Request.Form["txtCiudad"];
                ViewBag.Unidad = Request.Form["txtUnidad"];
                ViewBag.UsuarioOid = Request.Form["usuarioOid"];
                ViewBag.Editar = false;
                //Verifica si ya existe un usuario
                usuarioExiste = (from registro in db.CADUSUARIOS
                                 where registro.USUARIO == cADUSUARIO.USUARIO
                                 select registro).FirstOrDefault();
                if (usuarioExiste != null)
                {
                    ViewBag.MENSAJE_EXITOSO = Resources.RecursoGeneral.MSN_USUARIO_EXISTENTE;
                    ViewBag.TIPO_MENSAJE = Resources.RecursoGeneral.MSN_TIPO_WARNING.ToString();
                }
                else if (ModelState.IsValid)
                {
                    db.CADUSUARIOS.Add(cADUSUARIO);
                    db.SaveChanges();
                    //Crea o actualiza elysyarui de OID
                    var usuario = CrearUsuarioOid(Request.Form["usuarioOid"], Request.Form["password"]);
                    cADUSUARIO.USUARIO_OID_ID = usuario.Id;
                    db.SaveChanges();
                    return RedirectToAction("AdministrarUsuarios", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_CREACION.Replace("NUMERO", cADUSUARIO.USUARIO) });
                }
                return View(cADUSUARIO);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarUsuario([Bind(Include = "USUARIO,IDENTIFICACION,LUGE_CODIGO_DPTO,CADUNDE_FUERZA,CADUNDE_CODIGO,FECHA_DESDE,FECHA_HASTA,CONDICION,LUGE_CODIGO_CAD,CADCANA_FUERZA_ID,CADCANA_CODIGO,SITIO_GRABACION,USUARIO_OID_ID,VIGENTE,AUTENTICA_OID,USUARIO_OID")] CADUSUARIO cADUSUARIO)
        {
            try
            {
                ViewBag.Grado = Request.Form["txtGrado"];
                ViewBag.Nombre = Request.Form["txtNombre"];
                ViewBag.Dpto = Request.Form["txtDpto"];
                ViewBag.Ciudad = Request.Form["txtCiudad"];
                ViewBag.Unidad = Request.Form["txtUnidad"];
                ViewBag.UsuarioOid = Request.Form["usuarioOid"];
                ViewBag.OPCION = Request.Form["hdOpcion"];
                ViewBag.Editar = true;
                if (ModelState.IsValid)
                {
                    db.Entry(cADUSUARIO).State = EntityState.Modified;
                    db.SaveChanges();
                    //Crea o actualiza elysyarui de OID
                    var usuario = CrearUsuarioOid(Request.Form["usuarioOid"], Request.Form["password"]);
                    cADUSUARIO.USUARIO_OID_ID = usuario.Id;
                    db.Entry(cADUSUARIO).State = EntityState.Modified;
                    db.SaveChanges();
                    if (!Request.Form["hdOpcion"].ToString().Equals(""))
                    {
                        GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADUSUARIO.USUARIO);
                        return RedirectToAction("Index", "Home");
                    }
                    return RedirectToAction("AdministrarUsuarios", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_MODIFICADO.Replace("NUMERO", cADUSUARIO.USUARIO) });
                }
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion", cADUSUARIO.CONDICION);
                ViewBag.SITIO_GRABACION = new SelectList((from sitio in contexto.CADSITIOS_GRABACION select new { codigo = sitio.CONSECUTIVO, descripcion = sitio.DESCRIPCION }).OrderBy(s => s.descripcion), "codigo", "descripcion", cADUSUARIO.SITIO_GRABACION);
                return View("CrearUsuario", null, cADUSUARIO); ;
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
                GestorBase.GestionarExcepcion(exE, GetType().Name);
                return RedirectToAction("Index", "Home"); ;
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Usuarios/Create

        public ActionResult CrearUsuario()
        {
            try
            {
                ViewBag.Grado = "";
                ViewBag.Nombre = "";
                ViewBag.Dpto = "";
                ViewBag.Ciudad = "";
                ViewBag.Unidad = "";
                ViewBag.UsuarioOid = "";
                ViewBag.Editar = false;
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion");
                ViewBag.SITIO_GRABACION = new SelectList((from sitio in contexto.CADSITIOS_GRABACION select new { codigo = sitio.CONSECUTIVO, descripcion = sitio.DESCRIPCION }).OrderBy(s => s.descripcion), "codigo", "descripcion");
                return View();
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }
        [SecadPlusAutorizador("/Account/EditarUsuario")]
        public ActionResult EditarUsuario(string strUsuario, string strOpcion = null)
        {
            try
            {
                CADUSUARIO usuario = db.CADUSUARIOS.Find(strUsuario);
                objGestorGral = new GestorGeneral();
                ViewBag.Grado = "";
                ViewBag.Nombre = "";
                ViewBag.Dpto = "";
                ViewBag.Ciudad = "";
                ViewBag.Unidad = "";
                ViewBag.UsuarioOid = "";
                ViewBag.Editar = true;
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                if (usuario == null)
                {
                    return RedirectToAction("AdministrarUsuarios", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE });
                }
                //Consulta la información asociada al usuario
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("opcion", "USUARIOS", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", " WHERE US.USUARIO='" + strUsuario + "' ", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarLista", strConexion, "Nonquery");
                foreach (DataRow fila in dttLugar.Rows)
                {
                    ViewBag.Grado = fila["GRAD_ALFABETICO"].ToString();
                    ViewBag.Nombre = fila["NOMB_EMPLEADO"].ToString();
                    ViewBag.Dpto = fila["DEPARTAMENTO"].ToString();
                    ViewBag.Ciudad = fila["CIUDAD"].ToString();
                    ViewBag.Unidad = fila["DESCRIPCION"].ToString();
                    ViewBag.UsuarioOid = fila["USUARIOID"].ToString();
                }
                ViewBag.CONDICION = new SelectList((from condicion in contexto.CADREFERENCIASSECADs where condicion.NOMBRE == "CONDICION_USUARIO" select new { codigo = condicion.CODIGO, descripcion = condicion.DESCRIPCION }), "codigo", "descripcion", usuario.CONDICION);
                ViewBag.SITIO_GRABACION = new SelectList((from sitio in contexto.CADSITIOS_GRABACION select new { codigo = sitio.CONSECUTIVO, descripcion = sitio.DESCRIPCION }).OrderBy(s => s.descripcion), "codigo", "descripcion", usuario.SITIO_GRABACION);
                ViewBag.OPCION = "";
                if (!string.IsNullOrEmpty(strOpcion))
                    ViewBag.OPCION = strOpcion;
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////
                string strAccion = ControllerContext.RouteData.Values["action"].ToString();
                string strControlador = ControllerContext.RouteData.Values["controller"].ToString();
                ViewBag.RutaCompGeneral = Url.Action(strAccion, strControlador, new { strUsuario = strUsuario });
                ViewBag.NombreUbicacion = "Editar Usuario";
                ///////////////////////////Variables BreadCrumb////////////////////////////////////////

                return View("CrearUsuario", null, usuario);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ConsultarInfoPolicia(string strCedula)
        {
            try
            {
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cedula", strCedula, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultardatospolicia", strConexion, "Nonquery");

                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    resultados = Json(new { result = true, datos = fila.ItemArray });
                }
                if (dttLugar.Rows.Count <= 0)
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_NO_EXISTE_POLICIA });
                }
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
        public JsonResult ConsultarUsuEmpresarial(string strCedula)
        {
            try
            {
                GestorGeneral objGestorGral;
                JsonResult resultados = new JsonResult();
                string strResultado = "";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_cedula", strCedula, typeof(string)));
                strResultado = objGestorGral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultarusuempresarial", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));

                //var dttFuerza = gestor.GetFuerza(strConexion);
                if (string.IsNullOrEmpty(strResultado))
                {
                    resultados = Json(new { result = false, datos = Resources.RecursoGeneral.MSN_USU_EMP_NO_ENCONTRADO });
                }
                else
                {
                    resultados = Json(new { result = true, datos = strResultado });
                }
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
        public JsonResult ConsultarDeptos()
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadLugares> elementos = new List<Ext_CadLugares>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarLugar('_CODIGO' ,'_NOMBRE', 'DPTO')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = "AND (CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = "AND (REGEXP_LIKE(DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento = " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento = " ORDER BY DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento = " ORDER BY TIPO " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento = " ORDER BY CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                //if (blFiltro) { strLimites += " AND "; }
                strLimites += "  DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultardeptos", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_CadLugares(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });

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
        public JsonResult ConsultarCiudades(decimal dcDptoId)
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadLugares> elementos = new List<Ext_CadLugares>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarLugar('_CODIGO' ,'_NOMBRE', 'CIUDAD')\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento += " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento += " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                strLimites = " WHERE DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_dpto", dcDptoId.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultarciudades", strConexion, "Nonquery");
                //var dttFuerza = gestor.GetFuerza(strConexion);
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_CadLugares(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });

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
        public JsonResult ConsultarUnidades()
        {
            try
            {
                //Consulta el turno seleccionado
                DatatableForm parametros = new DatatableForm(Request.Form);
                List<Ext_CadUnidad> elementos = new List<Ext_CadUnidad>();
                GestorGeneral objGestorGral;
                string strQuery = "", strLimites = "", strOrdenamiento = "";
                double dblControl;
                bool blFiltro = false, blControl = false;

                string strBoton = "<button class='btn btn-xs btn-default' type='button' data-popup='tooltip'" +
                "onclick = \"SeleccionarUnidad(_CODIGO ,'_NOMBRE', _FUERZA)\" title = '' data-placement = 'bottom' data-original-title = 'Clic para seleccionar' >" +
                " Seleccionar </button>";
                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica si se debe realizar búsqueda de registros
                if (!string.IsNullOrEmpty(parametros.ConsultaValor) && parametros.ConsultaValor.Length > 1)
                {
                    blFiltro = true;
                    //Verifica si se debe buscar por código o descripción
                    blControl = Double.TryParse(parametros.ConsultaValor, out dblControl);
                    if (blControl)
                    {
                        strQuery = " WHERE (INTERNO.CODIGO = " + parametros.ConsultaValor + ") ";
                    }
                    else
                    {
                        strQuery = " WHERE (REGEXP_LIKE(INTERNO.DESCRIPCION, '" + parametros.ConsultaValor + "', 'i') )";
                    }
                }
                //Verifica si se deben realizar ordenamientos
                switch (parametros.IndiceOrdenamiento)
                {
                    case 2:
                        strOrdenamiento = " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 3:
                        strOrdenamiento = " ORDER BY INTERNO.DESCRIPCION " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 4:
                        strOrdenamiento = " ORDER BY INTERNO.SIGLA " + parametros.TipoOrdenamiento + " ";
                        break;

                    case 5:
                        strOrdenamiento = " ORDER BY INTERNO.FUERZA " + parametros.TipoOrdenamiento + " ";
                        break;

                    default:
                        strOrdenamiento = " ORDER BY INTERNO.CODIGO " + parametros.TipoOrdenamiento + " ";
                        break;
                }
                //if (blFiltro) { strLimites += " AND "; }
                strLimites += " WHERE  DATOS.FILA BETWEEN " + parametros.Inicio + " AND " + parametros.Final;
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ordenamiento", strOrdenamiento, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimites, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultarunidXusu", strConexion, "Nonquery");
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new Ext_CadUnidad(fila, strBoton.Replace("_CODIGO", fila["CODIGO"].ToString()).Replace("_NOMBRE", fila["DESCRIPCION"].ToString()).Replace("_FUERZA", fila["FUERZA"].ToString())));
                    parametros.TotalRegistros = Convert.ToInt32(fila["TOTAL"].ToString());
                    parametros.TotalRegistroFiltrados = Convert.ToInt32(fila["TOTAL_FILTRADO"].ToString());
                }
                //Verifica si se ha aplicado algun filtro a  la consulta
                if (!blFiltro)
                {
                    parametros.TotalRegistroFiltrados = parametros.TotalRegistros;
                }
                resultados = Json(new { draw = parametros.Draw, recordsTotal = parametros.TotalRegistros, recordsFiltered = parametros.TotalRegistroFiltrados, data = elementos });

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
        [HttpPost]
        public JsonResult ConsultarRoles(string strUsuarioEmpresarial)
        {
            try
            {
                //Consulta el turno seleccionado
                List<ExtCadNetRol> elementos = new List<ExtCadNetRol>();
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario_empresarial", strUsuarioEmpresarial, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                DataTable dttLugar = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_rolesXusuario", strConexion, "Nonquery");
                foreach (DataRow fila in dttLugar.Rows)
                {
                    elementos.Add(new ExtCadNetRol(fila));
                }
                resultados = Json(new { result = true, datos = elementos });
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
        [HttpPost]
        public JsonResult AsociarRol(string struUsuarioEmpresarial, string strRolId, int asociado)
        {
            try
            {
                CADNETOID usuarioOid = db.CADNETOIDs.Find(struUsuarioEmpresarial);
                CADNETOIDROLE relacion;
                string strMensaje = "";
                if (asociado == 1)
                {
                    relacion = db.CADNETOIDROLES.Find(struUsuarioEmpresarial, strRolId);
                    if (relacion != null)
                        usuarioOid.CADNETOIDROLES.Remove(relacion);
                    strMensaje = Resources.RecursoGeneral.MSN_ROL_DESASOCIADO;
                }
                else
                {
                    relacion = new CADNETOIDROLE();
                    relacion.RoleId = strRolId;
                    relacion.UserId = struUsuarioEmpresarial;
                    usuarioOid.CADNETOIDROLES.Add(relacion);
                    strMensaje = Resources.RecursoGeneral.MSN_ROL_ASOCIADO;
                }
                db.SaveChanges();
                resultados = Json(new { result = true, datos = strMensaje });
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
        [HttpPost]
        public ActionResult Eliminar(string strUsuario)
        {
            try
            {
                CADUSUARIO usuario = db.CADUSUARIOS.Find(strUsuario);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View("EliminarUsuario", null, usuario);
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return HttpNotFound();
            }
        }

        // POST: Elementos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string strUsuario)
        {
            try
            {
                Validador validador = new Validador();
                CADUSUARIO usuario = db.CADUSUARIOS.Find(strUsuario);
                if (usuario == null)
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_REGISTRO_NO_EXISTE;
                    return RedirectToAction("AdministrarUsuarios", "Account");
                }
                if (!validador.VerificarEliminarUsuario(usuario))
                {
                    GestorBase.MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strUsuario);
                    return RedirectToAction("AdministrarUsuarios", "Account");
                }
                db.CADUSUARIOS.Remove(usuario);
                await db.SaveChangesAsync();
                return RedirectToAction("AdministrarUsuarios", "Account", new { strMensaje = Resources.RecursoGeneral.MSN_REGISTRO_ELIMINADO.ToString() });
            }
            catch (DbUpdateException e)
            {
                var mensaje = e.Message;
                return RedirectToAction("AdministrarUsuarios", "Account", new { strMensaje = Resources.RecursoGeneral.MSN_EX_DBUPDATE.Replace("REG", strUsuario) });
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion Helpers
    }
}