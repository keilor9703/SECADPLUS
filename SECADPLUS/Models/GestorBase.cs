using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SECADPLUS.Models
{
    public abstract class GestorBase
    {
        #region Atributos

        protected List<Tuple<string, string, Type>> paramsAvanzados;
        protected List<Tuple<string, object, Type>> paramsSalida;
        protected List<Tuple<string, object, Type>> paramsAux;
        protected string strTemp;
        protected double dblTemp;
        protected int intTemp;
        protected bool blValor, blEstado;
        protected List<string> paramsSimples;
        protected List<SelectListItem> listaSimple;
        protected GestorAuditoria gestorAuditoria;
        protected GestorRecepcion gestorRecepcion;
        protected GestorDespachos gestorDespachos;
        protected GestorSeguridad gestorSeguridad;        
        protected GestorMinuta gestorMinuta;
        protected GestorGeneral gestorGeneral;
        protected GestorGeneral gestorGeneral2;
        protected GestorGeneral gestorGeneral3;
        protected DataTable tabla;
        protected string strMensaje;
        public static string strExCodigo = "", strExCodigoInterno = "", strMensajeComun = "", strCodigoNotificacion = "", strBreadCrumb="";
        public static bool BL_CONEXION_PERPONAL = true, BL_PrimeraEntrada = true, BL_NOTIFICACION = false;//Variable que permite determinar si la conexion de la vista perponal funciona

        #endregion Atributos

        #region Propiedades

        //Propiedades para gestión de Excepciones
        public static string EX_CODIGO
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["EX_CODIGO"] != null)
                {
                    return HttpContext.Current.Session["EX_CODIGO"].ToString();
                }
                else
                {
                    return strExCodigo;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["EX_CODIGO"] != null)
                {
                    HttpContext.Current.Session["EX_CODIGO"] = value;
                }
                else
                {
                    strExCodigo = value;
                }
            }
        }

        public static string EX_CODIGO_INTERNO
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["EX_CODIGO_INTERNO"] != null)
                {
                    return HttpContext.Current.Session["EX_CODIGO_INTERNO"].ToString();
                }
                else
                {
                    return strExCodigoInterno;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["EX_CODIGO_INTERNO"] != null)
                {
                    HttpContext.Current.Session["EX_CODIGO_INTERNO"] = value;
                }
                else
                {
                    strExCodigoInterno = value;
                }
            }
        }

        //Propiedad estatica para gestionar los mensajes de advertencias
        public static string MSN_ALERTA_COMUN
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MSN_ALERTA_COMUN"] != null)
                {
                    return HttpContext.Current.Session["MSN_ALERTA_COMUN"].ToString();
                }
                else
                {
                    return strMensajeComun;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["MSN_ALERTA_COMUN"] != null)
                {
                    HttpContext.Current.Session["MSN_ALERTA_COMUN"] = value;
                }
                else
                {
                    strMensajeComun = value;
                }
            }
        }
        //Propiedad estatica para manejar la miga de pan
        public static string BREADCRUMB
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["BREADCRUMB"] != null)
                {
                    return HttpContext.Current.Session["BREADCRUMB"].ToString();
                }
                else
                {
                    return strBreadCrumb;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["BREADCRUMB"] != null)
                {
                    HttpContext.Current.Session["BREADCRUMB"] = value;
                }
                else
                {
                    strBreadCrumb = value;
                }
            }
        }
        //Propiedad estatica para gestionar las notificaciones
        public static bool BLS_PrimeraEntrada
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_PRIEMERA_ENTRADA"] != null)
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["S_PRIEMERA_ENTRADA"]);
                }
                else
                {
                    return BL_PrimeraEntrada;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_PRIEMERA_ENTRADA"] != null)
                {
                    HttpContext.Current.Session["S_PRIEMERA_ENTRADA"] = value;
                }
                else
                {
                    BL_PrimeraEntrada = value;
                }
            }
        }

        //Propiedad estatica para gestionar las notificaciones
        public static bool BLS_NOTIFICACION
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_BL_NOTIFICACION"] != null)
                {
                    return Convert.ToBoolean(HttpContext.Current.Session["S_BL_NOTIFICACION"]);
                }
                else
                {
                    return BL_NOTIFICACION;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_BL_NOTIFICACION"] != null)
                {
                    HttpContext.Current.Session["S_BL_NOTIFICACION"] = value;
                }
                else
                {
                    BL_NOTIFICACION = value;
                }
            }
        }

        //Propiedad estatica para gestionar las notificaciones
        public static string BLS_CodigoNotificacion
        {
            get
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_CodigoNotificacion"] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session["S_CodigoNotificacion"]);
                }
                else
                {
                    return strCodigoNotificacion;
                }
            }
            set
            {
                //Verifica que exista un contexto http, sesión y variable de sesión
                if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["S_CodigoNotificacion"] != null)
                {
                    HttpContext.Current.Session["S_CodigoNotificacion"] = value;
                }
                else
                {
                    strCodigoNotificacion = value;
                }
            }
        }

        public DataTable Tabla
        {
            get { return tabla; }
            set { tabla = value; }
        }

        public bool Estado
        {
            get { return blEstado; }
            set { blEstado = value; }
        }

        public List<SelectListItem> ListaSimple
        {
            get { return listaSimple; }
            set { listaSimple = value; }
        }

        #endregion Propiedades

        #region Métodos

        public List<Tuple<string, string, Type>> ParamsAvanzados
        {
            get
            {
                return paramsAvanzados;
            }
            set
            {
                paramsAvanzados = value;
            }
        }

        public List<Tuple<string, object, Type>> ParamsSalida
        {
            get
            {
                return paramsSalida;
            }
            set
            {
                paramsSalida = value;
            }
        }

        public List<string> ParamsSimples
        {
            get
            {
                return paramsSimples;
            }
            set
            {
                paramsSimples = value;
            }
        }

        public bool SolicitudPrincipal()
        {
            try
            {
                return false;
            }
            catch (Exception Ex)
            {
                var mensaje = Ex.Message;
                return true;
            }
        }

        /// <summary>
        /// Elaborado por: Ing. Pablo Pinzón Fecha de creación: 10/10/2016 Este método permite
        /// gestionar cualquier excepción que suceda en el sistema generando por cada excepción un
        /// registro en la base de datos.
        /// </summary>
        /// <param name="e">
        /// Objeto tipo Exception que ha sido disparado desde algún metódo en el sistema.
        /// </param>
        /// <param name="strClase">Nombre de la clase en donde sucedio la excepción</param>
        /// <param name="strMethod">Función que ha disparado la excepción dentro del sistema</param>
        /// <returns></returns>
        public static Exception GestionarExcepcion(Exception e, string strClase, [System.Runtime.CompilerServices.CallerMemberName] string strMetodo = "", string strAdicional = null)
        {
            try
            {
                //Gestiona el control por variable estatica
                //Verifica que no se repita la generación de exepciones en la base de datos
                if (!EX_CODIGO.Equals(""))
                    return e;
                SECADExcepcionEntities exContext;
                CADEXCEPCION excepcion;
                
                string strIp = ObtenerDireccionIp();
                string strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECADExcepcion", "SecadEntities");
                //string con = "metadata=res://*/Models.SecadEntities.SECADExcepcion.csdl|res://*/Models.SecadEntities.SECADExcepcion.ssdl|res://*/Models.SecadEntities.SECADExcepcion.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string='DATA SOURCE=172.28.18.61:1521/SECAD123;PASSWORD=SECAD2015;USER ID=USR_CAD'";
                EX_CODIGO = "";
                EX_CODIGO_INTERNO = "";
                using (exContext = new SECADExcepcionEntities(strConexion))
                {
                    excepcion = new CADEXCEPCION();
                    if (e.InnerException == null)
                    {
                        excepcion.EX_EXINTERNA = "";
                    }
                    else
                    {
                        excepcion.EX_EXINTERNA = e.InnerException.ToString();
                    }
                    excepcion.EX_EXINTERNA = e.ToString();
                    excepcion.EX_CLASE = strClase;
                    excepcion.EX_ORIGEN = e.Source.ToString();
                    if (string.IsNullOrEmpty(strAdicional))
                    {
                        excepcion.EX_MENSAJE = e.Message.ToString();
                    }
                    else
                    {
                        excepcion.EX_MENSAJE = e.Message.ToString() + strAdicional;
                    }

                    excepcion.EX_TRAZA = e.StackTrace.ToString();
                    excepcion.EX_USER_ID = ObtenerIdUsuario();
                    //Verifica que exista la sesión y el objeto del usuario SECAD
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["USUARIO_SECAD"] != null)
                    {
                        excepcion.EX_SITIO_GRABACION = ((CADUSUARIO)HttpContext.Current.Session["USUARIO_SECAD"]).SITIO_GRABACION;
                    }
                    else
                    {
                        excepcion.EX_SITIO_GRABACION = Convert.ToInt32(ConfigurationManager.AppSettings["SitioGrabacion"].ToString());
                    }
                    excepcion.EX_FUNCION = strMetodo;
                    excepcion.EX_IPEQUIPO = strIp;
                    //excepcion.EX_FECHA = (DateTime)gestor.ObtenerTiempoServidor();
                    exContext.CADEXCEPCIONs.Add(excepcion);
                    exContext.SaveChanges();
                    EX_CODIGO = "CODIGO: " + excepcion.EX_ID.ToString() + "";
                    EX_CODIGO_INTERNO = "CODIGO: " + e.HResult;
                    return new Exception();
                }
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
                // throw ex;
                throw;
            }
        }

        /// <summary>
        /// Creado Por: Pablo Andrés Pinzón Fecha de creación: 20/10/2016 Retorna el id asociado al
        /// usuario autenticado en el sistema
        /// </summary>
        /// <returns></returns>
        public static string ObtenerIdUsuario()
        {
            try
            {
                //if (HttpContext.Current.User.Identity.IsAuthenticated)
                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    HttpContext contexto = HttpContext.Current;
                    return HttpContext.Current.User.Identity.GetUserId();
                }
                else
                {
                    //return null;
                    return "Sin Usuario";
                }
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "GestorBase");
            }
        }

        /// <summary>
        /// Permite obtener la hora actual del servidor de base de datos
        /// </summary>
        /// <returns></returns>
        public DateTime? ObtenerTiempoServidor
        {
            get
            {
                try
                {
                    strTemp = "SELECT to_char(SYSDATE,'DD/MM/YYYY HH24:MI:SS') FECHA FROM DUAL";

                    OracleCommand comando = new OracleCommand(strTemp);
                    //Oracle.DataAccess.Client.OracleCommand comando = new Oracle.DataAccess.Client.OracleCommand(strTemp);
                    gestorGeneral = new GestorGeneral();
                    gestorGeneral.CadenaConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                    tabla = gestorGeneral.LlenarDataSet(comando, "FECHA");
                    foreach (DataRow fila in tabla.Rows)
                    {
                        if (!string.IsNullOrEmpty(fila["FECHA"].ToString()))
                        {
                            //return Convert.ToDateTime(fila["FECHA"].ToString());
                            string fechaString = fila["FECHA"].ToString();
                            DateTime fecha;
                            if (DateTime.TryParseExact(fechaString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                            {
                                return fecha;
                            }
                            else
                            {
                                // Manejar el caso donde la conversión falla
                                // Puedes lanzar una excepción o devolver null, según sea necesario
                                return null;
                            }
                        }
                    }
                    return new DateTime();
                }
                catch (Exception ex)
                {
                    throw GestionarExcepcion(ex, GetType().Name);
                }
            }
        }

        /// <summary>
        /// Permite obtener la hora actual del servidor de base de datos
        /// </summary>
        /// <returns></returns>
        public short ObtenerDocumentoConsecutivo(string strSitio, string strLlamada)
        {
            try
            {
                strTemp = "SELECT (NVL(MAX(CONSECUTIVO),0) + 1) CONSECUTIVO FROM USR_CAD.CADPEDIDOS_DOCUMENTOS WHERE CADPEDI_SITIOGRABA = " + strSitio + " AND CADPEDI_NUMELLAMADA = " + strLlamada;

                OracleCommand comando = new OracleCommand(strTemp);
               // Oracle.DataAccess.Client.OracleCommand comando = new Oracle.DataAccess.Client.OracleCommand(strTemp);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                tabla = gestorGeneral.LlenarDataSet(comando, "CONSECUTIVO");
                foreach (DataRow fila in tabla.Rows)
                {
                    if (!string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()))
                    {
                        return Convert.ToInt16(fila["CONSECUTIVO"].ToString());
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        protected void serializeObject()
        {
            string Json = new JavaScriptSerializer().Serialize(gestorAuditoria);
        }

        public string ObtenerMensaje()
        {
            return strMensaje;
        }

        /// <summary>
        /// Este método permite obtener la dirección IP del usuario que esta consultando el sistema.
        /// </summary>
        /// <returns></returns>
        public static string ObtenerDireccionIp()
        {
            try
            {
                string strIp = "";
                //Verifica que el dato de la Ip no se encuentre ya creado en las variables de sesión
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["IPUSUARIO"] != null)
                {
                    strIp = HttpContext.Current.Session["IPUSUARIO"].ToString();
                }
                else
                {
                    strIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                //Verifica que la Ip se ha identificado de manera correcta
                if (string.IsNullOrEmpty(strIp))
                {
                    strIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                //Asigna o actualiza el valor de la ip identificada
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["IPUSUARIO"] = strIp;
                }
                return strIp;
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message;
                return "::1";
            }
        }

        /// <summary>
        /// Permite realizar la consulta del usuario de secad asociado al usuario institucional que
        /// se ha identificado en el sistema
        /// </summary>
        /// <param name="strClienteNombre">
        /// parámetro opcional para identificar el nombre del equipo del usuario final
        /// </param>
        /// <returns></returns>
        public static async Task<bool> ConsultarUsuarioSecad(string strId = null)
        {
            try
            {
                HttpContext contexto = HttpContext.Current;
                CADUSUARIO usuarioSecad = null;
                string strIdOid = "";
                if (!string.IsNullOrEmpty(strId))
                {
                    strIdOid = strId;
                }else {
                    strIdOid = ObtenerIdUsuario();
                }
                
                using (PermisosEntities contextoDb = new PermisosEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "PermisosModel", "SecadEntities")))
                {
                    usuarioSecad = (from registro in contextoDb.CADUSUARIOS
                                    where registro.USUARIO_OID_ID == strIdOid && registro.VIGENTE == true
                                    select registro).FirstOrDefault();

                    if (usuarioSecad != null)
                    {
                        contexto.Session["USUARIO_SECAD"] = usuarioSecad;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "GestorBase");
            }
        }

        #endregion Métodos
    }
}