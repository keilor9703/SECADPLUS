using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Net;

namespace SECADPLUS.Models
{
    public class GestorSeguridad : GestorBase
    {
        #region atributos

        private SecadEnums.OIDRespuesta oidRespuesta = SecadEnums.OIDRespuesta.SinConexion;//Por defecto asumimos que no existe conexion

        #endregion atributos

        #region Propiedades

        public SecadEnums.OIDRespuesta OidRespuesta
        {
            get
            {
                return oidRespuesta;
            }
            set { oidRespuesta = value; }
        }

        #endregion Propiedades

        #region métodos
        
        /// <summary>
        /// Permite consultar el menú asociado al usuario autenticado
        /// </summary>
        /// <returns></returns>
        public List<Menu> ObtenerMenu()
        {
            try
            {
                List<Menu> menu = new List<Models.Menu>();
                Menu menuContenido;
                //Ejecuta el procedimiento almacenado para obtener el menú asociado al usuario
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new System.Collections.Generic.List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario_id", ObtenerIdUsuario(), typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_permiso", Resources.RecursoGeneral.GestorSeguridad_Permiso, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento("USR_CAD.PK_CADPLUS.p_obtener_menu", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "Nonquery");
                foreach (DataRow fila in tabla.Rows)
                {
                    menuContenido = new Menu();
                    menuContenido.Nombre = fila["ELEMENTO_NOMBRE"].ToString();
                    menuContenido.Ruta = fila["ELEMENTO_RUTA"].ToString();
                    menuContenido.Tipo = fila["TIPOELE_NOMBRE"].ToString();
                    menuContenido.Nivel = Convert.ToInt32(fila["LEVEL"].ToString());
                    menuContenido.Icono = fila["ELEMENTO_ICONO"].ToString();
                    menuContenido.Id = Convert.ToInt32(fila["CADELEMENTO_ID"].ToString());
                    menuContenido.Padre_Id = (string.IsNullOrEmpty(fila["ELEMENTO_PADRE_ID"].ToString())) ? 0 : Convert.ToInt32(fila["ELEMENTO_PADRE_ID"].ToString());
                    menu.Add(menuContenido);
                }
                return menu;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }
              
        public bool Autenticar()
        {
            return true;
        }        
        /// <summary>
        /// Permite realizar la autenticación contra el Directorio activo de Oracle
        /// </summary>
        /// <param name="strUserName">Nombre de usuario</param>
        /// <param name="strUserPassword">Contraseña de usuario</param>
        /// <returns></returns>
        public bool AutenticarOID(string strUserName, string strUserPassword)
        {
            try
            {
                LdapConnection conexion;
                NetworkCredential credencial;
                string strServidorOID = ConfigurationManager.AppSettings["OIDServidor"].ToString();
                string strUserCredential = Resources.RecursoGeneral.UserOidCadenaString.Replace("USERNAME", strUserName);
                using (conexion = new LdapConnection(strServidorOID))
                {
                    conexion.AuthType = AuthType.Basic;
                    conexion.Timeout = TimeSpan.FromSeconds(15);
                    credencial = new NetworkCredential(strUserCredential, strUserPassword);
                    conexion.Bind(credencial);
                    conexion.Dispose();
                }
                return true;
            }
            catch (LdapException ldapEx)
            {
                switch (ldapEx.ErrorCode)
                {
                    case 49:
                        oidRespuesta = SecadEnums.OIDRespuesta.DatosIncorrectos;
                        break;

                    default:
                        oidRespuesta = SecadEnums.OIDRespuesta.SinConexion;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite Verificar el estado del empleado o policía asociado a un usuario específico
        /// </summary>
        /// <param name="strEmpleadoCedula">Numero de identificación del empleado o policía</param>
        /// <returns></returns>
        public bool ConsultarEstadoEmpleado(string strOidId)
        {
            try
            {
                blEstado = false;
                strTemp = "";
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_oidid", strOidId, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_info_personal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                if (!BL_CONEXION_PERPONAL)
                {
                    return true;//Al no tener conexion a la vista PERPONAL el sistema omite esta validación
                }
                foreach (DataRow fila in tabla.Rows)
                {
                    strTemp = fila["ESTADO_EMPLEADO"].ToString();
                }
                if ((strTemp.Equals("LABORANDO")))
                {
                    blEstado = true;
                }
                return blEstado;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite verificar si el vambio de canal se puede realizar
        /// </summary>
        /// <returns></returns>
        public bool VerificarUsuarioCambioCanal()
        {
            try
            {
                blValor = true;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario_id", ObtenerIdUsuario(), typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_valida_cambio_canal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                if (tabla.Rows.Count <= 0) blValor = !blValor;
                return blValor;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite Verificars si el usuario de SECAD activo asociado al usuario institucional debe
        /// hautenticarse por OID
        /// </summary>
        /// <param name="strUsuarioOid"></param>
        /// <returns></returns>
        public bool VerificarEsquemaOid(string strUsuarioOid)
        {
            try
            {
                decimal dcValor = 0;

                dblTemp = 0;
                blEstado = false;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_oid_id", strUsuarioOid, typeof(string)));
                dcValor = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultar_oid_usuario", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                if (dcValor > 0) blEstado = true;
                return blEstado;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        #endregion métodos
    }
}