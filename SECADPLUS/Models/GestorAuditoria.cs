using SECADEntities.SECAD;
using SECADEntities.SecadEntities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static SECADPLUS.Models.SecadEnums;

namespace SECADPLUS.Models
{
    public class GestorAuditoria : GestorBase 
    {
        #region métodos

        public static async Task<bool> RegistrarIntentoAutenticacion(bool blOID, string strConexion, Microsoft.AspNet.Identity.Owin.SignInStatus resultado, string strOidId, string strMensaje = null)
        {
            try
            {
                bool blResultado = false;
                string strCadenaBd = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "IngresoSistema", "SecadEntities", strConexion);
                string strDireccionIp = ObtenerDireccionIp();
                string strEsquema = Resources.RecursoGeneral.GestorAuditoria_ALTERNO;
                //Identifica si el ingreso al sistema fue exitoso
                if (resultado == Microsoft.AspNet.Identity.Owin.SignInStatus.Success)
                    blResultado = true;
                //Verifica el esquema de autenticación
                if (blOID)
                    strEsquema = Resources.RecursoGeneral.GestorAuditoria_ESQUEMAOID;
                using (IngresoSistemaEntities contexto = new IngresoSistemaEntities(strCadenaBd))
                {
                    var esquemaID = (from esquema in contexto.CADESQUEMA_AUTENTICACION
                                     where esquema.NOMBREESQUEMA == strEsquema
                                     select new { esquema.CADESQAUT_ID }).FirstOrDefault();
                    CADINGRESOSISTEMA_AUD ingresoSistema = new CADINGRESOSISTEMA_AUD();
                    ingresoSistema.ESQUEMA_ID = esquemaID.CADESQAUT_ID;
                    ingresoSistema.CADSITIO_GRABACION = Convert.ToInt32(ConfigurationManager.AppSettings["SitioGrabacion"].ToString());
                    ingresoSistema.CADNETUSER_ID = strOidId;
                    ingresoSistema.IP_EQUIPO = strDireccionIp;
                    ingresoSistema.RESULTADO = blResultado;
                    if (resultado != Microsoft.AspNet.Identity.Owin.SignInStatus.Success)
                    {
                        if (string.IsNullOrEmpty(strMensaje)) strMensaje = Resources.RecursoGeneral.MSG_DatosUsuarioIncorrectos;
                        ingresoSistema.OBSERVACION = strMensaje;
                    }

                    contexto.CADINGRESOSISTEMA_AUD.Add(ingresoSistema);
                    contexto.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "GestorAuditoria");
            }
        }

        public static bool RegistrarConexion(bool blVigente, string strNombreEquipo, CADUSUARIO usuarioSecad)
        {
            try
            {
                GestorGeneral gestor;
                CADCONEXION conexion;
                IPHostEntry host = new IPHostEntry();
                decimal dcValor = 0;
                string strNombreMaquinaCliente = strNombreEquipo;
                string strCadenaBd = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "SECAD", "SECAD");
                using (SecadEntities contexto = new SecadEntities(strCadenaBd))
                {
                    //Obtiene el consecutivo
                    gestor = new GestorGeneral();
                    gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                    gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                    dcValor = gestor.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultar_seq_conexion", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                    //strLog = "Consecutivo = " + dcValor.ToString() + "  booleano=" + blVigente.ToString() + " nombre equipo= " + strNombreMaquinaCliente+" \n";
                    //MSN_ALERTA_COMUN += strLog;
                    if (dcValor > 0)
                    {
                        //obtiene el nombre de la máquina si esta disponible
                        if (blVigente)
                        {
                            //Verifica si es bucaramanga
                            if (usuarioSecad.SITIO_GRABACION !=7243)
                            {
                                //Limpia las conexiones que aún permanezcan vigentes para el mismo usuario y sitio de grabación
                                LimpiarConexiones(usuarioSecad, strNombreMaquinaCliente, strCadenaBd);
                            }                            
                            //Registra la conexion
                            conexion = new CADCONEXION();
                            conexion.CONSECUTIVO = Convert.ToInt64(dcValor.ToString());
                            conexion.SITIO_GRABA = usuarioSecad.SITIO_GRABACION;
                            if (!string.IsNullOrEmpty(strNombreMaquinaCliente)) conexion.MAQUINA = strNombreMaquinaCliente;
                            conexion.SESION_ID = Convert.ToInt64(dcValor.ToString());
                            conexion.CADUSUA_USUARIO = usuarioSecad.USUARIO;
                            conexion.VIGENTE = "S";
                            contexto.CADCONEXIONs.Add(conexion);
                            //strLog = "SE CREO EL REGISTRO DE CONEXION";
                            //MSN_ALERTA_COMUN += strLog;
                        }
                        else
                        {
                            LimpiarConexiones(usuarioSecad, strNombreMaquinaCliente, strCadenaBd);
                            //Verifica si es bucaramanga
                            if (usuarioSecad.SITIO_GRABACION == 7243)
                            {
                                //Limpia las conexiones que aún permanezcan vigentes para el mismo usuario y sitio de grabación
                               
                            }
                            //CONSULTA Y ACTUALIZA EL REGISTRO SI EXISTE
                            conexion = (from registro in contexto.CADCONEXIONs
                                        where registro.SITIO_GRABA == usuarioSecad.SITIO_GRABACION && registro.CADUSUA_USUARIO == usuarioSecad.USUARIO && registro.MAQUINA == strNombreMaquinaCliente && registro.VIGENTE == "S"
                                        select registro).FirstOrDefault();
                            if (conexion != null)
                            {
                                conexion.VIGENTE = "N";
                                //strLog = "SE ACTUALILO EL REGISTRO DE CONEXION CON SONSECUTIVO =" + conexion.CONSECUTIVO;
                                //MSN_ALERTA_COMUN +=strLog;
                            }
                        }
                        contexto.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                
                throw GestionarExcepcion(ex, "GestorSeguridad", "Nombre de maquina strNombreEquipo"+ strNombreEquipo);
            }
        }

        private static void LimpiarConexiones(CADUSUARIO usuarioSecad, string strNombreCliente, string strCadena)
        {
            try
            {
                using (SecadEntities contexto = new SecadEntities(strCadena))
                {
                    //Consulta y cierra las conexiones existentes vigentes sin fecha de cierre para el usuario y sitio de grabación autenticado
                    var registros = (from registro in contexto.CADCONEXIONs
                                     where registro.SITIO_GRABA == usuarioSecad.SITIO_GRABACION && registro.CADUSUA_USUARIO == usuarioSecad.USUARIO && registro.MAQUINA == strNombreCliente && registro.VIGENTE == "S"
                                     && registro.FECHA_HASTA == null
                                     select registro).ToList();
                    foreach (CADCONEXION conexion in registros)
                    {
                        conexion.VIGENTE = "N";
                    }
                    contexto.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "GestorAuditoría");
            }
        }

        public void RegistrarAuditoria<T>(Object objeto, string strTabla, string strLlave, TipoAuditoria tipo, [System.Runtime.CompilerServices.CallerMemberName] string strFuncion = "")
        {
            try
            {
                CADUSUARIO usuario;
                string strTipo = "";
                //Si el usuario de secad no se encuentra creado en la sesión intenta crearlo
                if (HttpContext.Current.Session["USUARIO_SECAD"] == null)
                {
                    var valor = GestorBase.ConsultarUsuarioSecad();
                }
                usuario = (CADUSUARIO)HttpContext.Current.Session["USUARIO_SECAD"];
                CADNETAUDITORIA registro = new CADNETAUDITORIA();
                JavaScriptSerializer serializador = new JavaScriptSerializer();
                strTemp = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "Auditoria", "SecadEntities");
                registro.FUNCION = strFuncion;
                registro.IP = ObtenerDireccionIp();
                registro.NOMBRE_TABLA = strTabla;
                registro.USUARIO = usuario.USUARIO;
                registro.PK_REGISTRO = strLlave;
                switch (tipo)
                {
                    case TipoAuditoria.Eliminar:
                        strTipo = Resources.RecursoGeneral.AUDITORIA_ELIMINAR;
                        break;

                    case TipoAuditoria.Modificar:
                        strTipo = Resources.RecursoGeneral.AUDITORIA_MODIFICAR;
                        break;

                    case TipoAuditoria.Crear:
                        strTipo = Resources.RecursoGeneral.AUDITORIA_CREAR;
                        break;
                }
                registro.TIPO_EVENTO = strTipo;
                registro.DATOS_TRANSACCION = Newtonsoft.Json.JsonConvert.SerializeObject(objeto);

                EjecutarRegistro(registro, strTemp);
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        
        /// <summary>
        /// Permite ejecutar de manera asincrona el registro de auditoria
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task EjecutarRegistro(CADNETAUDITORIA registro, string strConexion)
        {
            try
            {
                await Task.Run(() =>
                {
                    Registrar(registro, strConexion);
                });
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Ejecuta de manera asíncrona el registro de un ingreso no autorizado al sistema
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task<bool> Registrar(CADNETAUDITORIA registro, string strConexion)
        {
            try
            {
                using (AuditoriaEntities contextoAudita = new AuditoriaEntities(strConexion))
                {
                    contextoAudita.CADNETAUDITORIAs.Add(registro);
                    contextoAudita.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        public void RegistrarAuditoriaAntecedente(List<Tuple<string, string, Type>> parametros)
        {
            try
            {

                strTemp = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                EjecutarRegistroAntecedente(parametros, strTemp);
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite ejecutar de manera asincrona el registro de auditoria
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task EjecutarRegistroAntecedente(List<Tuple<string, string, Type>> parametros, string strConexion)
        {
            try
            {
                await Task.Run(() =>
                {
                    RegistrarAntecedente(parametros, strConexion);
                });
                
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Ejecuta de manera asíncrona el registro de un ingreso no autorizado al sistema
        /// </summary>
        /// <param name="strOidId"></param>
        /// <param name="dcIdElemento"></param>
        /// <returns></returns>
        private async Task<bool> RegistrarAntecedente(List<Tuple<string, string, Type>> parametros, string strConexion)
        {
            try
            {
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = parametros;
                gestorGeneral.ParamsSalida = new List<Tuple<string, object, Type>>();
                gestorGeneral.ParamsSalida.Add(new Tuple<string, object, Type>("p_consecutivo", null, typeof(decimal)));
                gestorGeneral.ParamsSalida.Add(new Tuple<string, object, Type>("P_mensaje", null, typeof(string)));
                gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadantecedentes.p_cadcon_personas", strConexion, "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in gestorGeneral.ParamsSalida)
                {
                    var valor = parameter.Item2.ToString();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }
        }
        /// <summary>
        /// Genera un deep copy de un objeto
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public object GenerarCopiaAuditoria(object objSource)
        {

            try
            {
                //Obtener el tipo de objeto de origen y crear una nueva instancia de ese tipo
                Type typeSource = objSource.GetType();
                object objTarget = Activator.CreateInstance(typeSource);

                //Obtener todas las propiedades del tipo de objeto de origen
                PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                //Asignar toda la propiedad de origen a las propiedades del objeto de destino
                foreach (PropertyInfo property in propertyInfo)
                {
                    //Compruebe si la propiedad se puede escribir 
                    if (property.CanWrite)
                    {
                        //Compruebe si el tipo de propiedad es tipo de valor, enum o tipo de cadena
                        if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)) || property.PropertyType.IsArray)
                        {
                            property.SetValue(objTarget, property.GetValue(objSource, null), null);
                        }
                        else
                        {
                            object objPropertyValue = property.GetValue(objSource, null);
                            if (objPropertyValue == null)
                            {
                                property.SetValue(objTarget, null, null);
                            }
                            else
                            {
                                property.SetValue(objTarget, GenerarCopiaAuditoria(objPropertyValue), null);
                            }
                        }
                    }
                }
                return objTarget;
            }
            catch (Exception ex)
            {
                throw GestorBase.GestionarExcepcion(ex, GetType().Name);
            }

      
        }




        #endregion métodos
    }
}