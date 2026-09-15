using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace SECADPLUS.Models
{
    public class Integrador : GestorBase
    {
        #region Atributos

        private SecadEnums.TipoConexion tipoConexion;

        #endregion Atributos

        #region Propiedades

        public SecadEnums.TipoConexion TipoConexion
        {
            get { return tipoConexion; }
            set { tipoConexion = value; }
        }

        #endregion Propiedades

        #region methods

        public void Ejecutar()
        {
            strTemp = paramsSimples[0];
            if (strTemp.Equals(Resources.RecursoGeneral.Integrador_Opcion_ListarConexiones))
            {
                ObtenerListaDeConexiones();
            }
        }

        public void GestionarOrigenExterno()
        {
        }

        public void ConsultarSIATH()
        {
        }

        public void ConsultarCentral()
        {
        }

        public void ConsultarDijin()
        {
        }

        public void ConsultarSiedco()
        {
        }

        public void ConsultarSitoExtenero()
        {
        }

        public void ConsultarOID()
        {
        }

        public void ConsultarSivicc()
        {
        }

        public void ConsultarArcGis()
        {
        }

        public void ConsultarUbicag()
        {
        }

        public void ConsultarINPEC()
        {
        }

        /// <summary>
        /// Creado por: Ing Pablo Pinzón Fecha de creación=19/10/2016 Permite consultar todas las
        /// cadenas de conexion a base de datos disponibles en web.config
        /// </summary>
        private void ObtenerListaDeConexiones()
        {
            try
            {
                listaSimple = new List<SelectListItem>();
                //listaSimple.Add(new SelectListItem());
                foreach (ConnectionStringSettings conexion in ConfigurationManager.ConnectionStrings)
                {
                    //Omite la conexion de .NET por defecto
                    if (conexion.Name.Equals("LocalSqlServer") || conexion.Name.Equals("OraAspNetConString"))
                    {
                        continue;
                    }
                    listaSimple.Add(new SelectListItem { Text = conexion.Name, Value = conexion.Name });
                    //paramsSimples.Add(conexion.Name);

                    string connString = conexion.ConnectionString;
                    string provider = conexion.ProviderName;
                }
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Creado por: Ing. Pablo Andrés Pinzón Fecha de creación: 19/10/2016 Este método permite
        /// obtener la cadena de conexión activa seleccionada por el usuario en sesión.
        /// </summary>
        /// <param name="tipoConexion">Especifíca el formato de la cadena de conexión devuelta</param>
        /// <returns></returns>
        public static string ObtenerConexionActiva(SecadEnums.TipoConexion tipoConexion, string strEntityName = null, string strEntityNameSpace = null, string strConexion = null, [System.Runtime.CompilerServices.CallerMemberName] string strMetodo = "")
        {
            try
            {
                var nombre = strMetodo;
                string strCantidadDeconexiones = ConfigurationManager.AppSettings["CantidadDeConexiones"].ToString();
                string strCantidadDeconexionesMinimo = ConfigurationManager.AppSettings["CantidadDeConexionesMinimo"].ToString();
                string strConexionTiempo = ConfigurationManager.AppSettings["TiempoVidaConexion"].ToString();
                string strIncremento = ConfigurationManager.AppSettings["Incremento"].ToString();
                string strDecremento = ConfigurationManager.AppSettings["Decremento"].ToString();
                string strComplementoPoolConexiones = "Min Pool Size=" + strCantidadDeconexionesMinimo + ";Max Pool Size=" + strCantidadDeconexiones + ";Connection lifetime = "+ strConexionTiempo + ";Connection Timeout = 180;Incr Pool Size = "+ strIncremento + ";Decr Pool Size = "+ strDecremento + ";";
                string strConexionTSName = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=SERVIDOR)(PORT=PUERTO)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=SERVICIO)));User Id=USUARIO;Password=CLAVE;";
                string strConexionEntity = "metadata=res://*/COMPLEMENTO.csdl|res://*/COMPLEMENTO.ssdl|res://*/COMPLEMENTO.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string='DATA SOURCE=" + Resources.RecursoGeneral.Integrador_Host + ":" + Resources.RecursoGeneral.Integrador_Port + "/" + Resources.RecursoGeneral.Integrador_Service_Name + ";PASSWORD=CLAVE;USER ID=USUARIO;"+ strComplementoPoolConexiones + "'";                
                string strConexionString = "DATA SOURCE=COMPLEMENTO;PASSWORD=CLAVE;USER ID=USUARIO;"+ strComplementoPoolConexiones;                
                string strConexionActiva = "";
                string strCadenaConexion = "";
                string strPort = "";
                string strHost = "";
                string strServiceName = "";
                string strUser = "";
                string strPass = "";
                string[] strContenidoConexion;
                //verifica si se debe activar la conexión por TNSNAME
                string strControlTnsName = ConfigurationManager.AppSettings["CONEXIONPORTNSNAME"].ToString();
                
                if (BL_NOTIFICACION)
                {
                    strConexionActiva = ConfigurationManager.AppSettings["ConexionPorDefecto"].ToString();
                }
                else if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["ConexionActiva"] != null)
                {
                    strConexionActiva = System.Web.HttpContext.Current.Session["ConexionActiva"].ToString();
                }
                else if (!string.IsNullOrEmpty(strConexion))
                {
                    strConexionActiva = strConexion;
                }
                else
                {
                    strConexionActiva = ConfigurationManager.AppSettings["ConexionPorDefecto"].ToString();
                }
                if (strControlTnsName.Equals("SI"))
                {
                    strConexionEntity = "metadata=res://*/COMPLEMENTO.csdl|res://*/COMPLEMENTO.ssdl|res://*/COMPLEMENTO.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string='DATA SOURCE=" + strConexionActiva.Trim().TrimEnd().TrimStart() + ";PASSWORD=CLAVE;USER ID=USUARIO;" + strComplementoPoolConexiones + "'";
                    strConexionString = "DATA SOURCE="+strConexionActiva.Trim().TrimEnd().TrimStart()+";PASSWORD=CLAVE;USER ID=USUARIO;" + strComplementoPoolConexiones;
                }
                foreach (ConnectionStringSettings conexion in ConfigurationManager.ConnectionStrings)
                {
                    if (conexion.Name.Equals(strConexionActiva))
                    {
                        strCadenaConexion = conexion.ConnectionString;
                        break;
                    }
                }
                strContenidoConexion = strCadenaConexion.Split(';');
                ////Verifica que los parámetros de la candea de conexión esten completos
                if (strContenidoConexion.Length < 5)
                {
                    strUser = strContenidoConexion[0].Replace(Resources.RecursoGeneral.Integrador_User + "=", "");
                    strPass = strContenidoConexion[1].Replace(Resources.RecursoGeneral.Integrador_Password + "=", "");


                }else
                {
                    strHost = strContenidoConexion[0].Replace(Resources.RecursoGeneral.Integrador_Host + "=", "");
                    strPort = strContenidoConexion[1].Replace(Resources.RecursoGeneral.Integrador_Port + "=", "");
                    strServiceName = strContenidoConexion[2].Replace(Resources.RecursoGeneral.Integrador_Service_Name + "=", "");
                    strUser = strContenidoConexion[3].Replace(Resources.RecursoGeneral.Integrador_User + "=", "");
                    strPass = strContenidoConexion[4].Replace(Resources.RecursoGeneral.Integrador_Password + "=", "");
                }
                //Asigna el contenido de la conexión en variables temporales
                
                
                //Verifica el tipo de conexion que se solicita
                switch (tipoConexion)
                {
                    case SecadEnums.TipoConexion.ConnectionString:
                        strCadenaConexion = strConexionString.Replace("COMPLEMENTO", strHost + ":" + strPort + "/" + strServiceName);
                        strCadenaConexion = strCadenaConexion.Replace("USUARIO", strUser).Replace("CLAVE", strPass);
                        return strCadenaConexion;

                    case SecadEnums.TipoConexion.Entity:
                        if (strEntityName == null || strEntityNameSpace == null)
                            throw new Exception(Resources.RecursoGeneral.EX_Integrador_EntityString);
                        strCadenaConexion = strConexionEntity.Replace("COMPLEMENTO", strEntityNameSpace + "." + strEntityName).Replace(Resources.RecursoGeneral.Integrador_Port, strPort).Replace(Resources.RecursoGeneral.Integrador_Host, strHost);
                        strCadenaConexion = strCadenaConexion.Replace(Resources.RecursoGeneral.Integrador_Service_Name, strServiceName).Replace("USUARIO", strUser).Replace("CLAVE", strPass);
                        return strCadenaConexion;

                    case SecadEnums.TipoConexion.TSName:
                        strCadenaConexion = strConexionTSName.Replace("SERVIDOR", strHost).Replace("PUERTO", strPort).Replace("SERVICIO", strServiceName);
                        strCadenaConexion = strCadenaConexion.Replace("USUARIO", strUser).Replace("CLAVE", strPass);
                        return strCadenaConexion;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "Integrador");
            }
        }

        #endregion methods
    }
}