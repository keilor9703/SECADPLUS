using Oracle.ManagedDataAccess.Client;
using System;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecadDatos.Context
{
    public class ConDBOracle : IDisposable
    {
        #region "Variables"
        public OracleConnection Conexion;
        private OracleTransaction Transaccion;
        #endregion
        public OracleDataReader ora_DataReader;

        private static string strConexion(string nameCnn)
        {
            try
            {
                return System.Web.Configuration.WebConfigurationManager.ConnectionStrings[nameCnn].ConnectionString;
            }
            catch (Exception ex)
            {
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando strConexion:", ex);
                return null;
            }
        }

        private struct stConnDB
        {
            public string CadenaConexion;
        }


        private stConnDB info;
        //Indica el numero de intentos de conectar a la BD sin exito.
        //0
        public byte ora_intentos = 2;

        //Public Sub New(ByVal Servidor As String, ByVal Usuario As String, ByVal Password As String)
        public ConDBOracle(string nameCnn, bool WebConfig = true) : base()
        {
            // Creamos la cadena de conexion de la base de datos.
            if (!WebConfig)
            {
                //info.CadenaConexion = String.Format("Data Source={0};User Id={1};Password={2};", Servidor, Usuario, Password);
                info.CadenaConexion = nameCnn;
            }
            else
            {
                info.CadenaConexion = strConexion(nameCnn);
            }

            // Instanciamos objeto conecction.
            Conexion = new OracleConnection();
        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose de la clase.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberamos objetos manejados.
            }
            try
            {
                // Liberamos los obtetos no manejados.
                if ((((ora_DataReader) != null)))
                {
                    ora_DataReader.Close();
                    ora_DataReader.Dispose();
                }
                // Cerramos la conexi�n a DB.
                if (!Desconectar())
                {
                    // Grabamos Log de Error...
                }
            }
            catch (Exception ex)
            {
                // Asignamos error.
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando Dispose:", ex);
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        private ConDBOracle() : base()
        {
            Dispose(false);
        }

        #region "Conectar-Desconectar"
        /// <summary>
        /// Se conecta a una base de datos de Oracle.
        /// </summary>
        /// <returns>True si se conecta bien.</returns>
        public bool Conectar()
        {
            bool ok = false;
            if (!this.IsConected())
            {
                try
                {
                    if ((((Conexion) != null)))
                    {
                        // Fijamos la cadena de conexi�n de la base de datos.
                        Conexion.ConnectionString = info.CadenaConexion;
                        Conexion.Open();
                        ok = true;
                    }
                }
                catch (Exception ex)
                {
                    // Desconectamos y liberamos memoria.
                    Desconectar();
                    // Asignamos error.
                    LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                    LogException.err("Error Ejecutando Conectar: " + info.CadenaConexion, ex);
                    // Asignamos error de funci�n
                    ok = false;
                }
            }
            return ok;
        }

        /// <summary>
        /// Se conecta a la base de datos de Oracle.
        /// </summary>
        /// <param name="cnConexion">Cadena de conexion</param>
        /// <returns>True si se conecta bien.</returns>
        /// <remarks></remarks>
        public bool Conectar(string cnConexion)
        {
            bool ok = false;
            try
            {
                if ((((Conexion) != null)))
                {
                    // Fijamos la cadena de conexi�n de la base de datos.
                    Conexion.ConnectionString = cnConexion;
                    Conexion.Open();
                    ok = true;
                }
            }
            catch (Exception ex)
            {
                // Desconectamos y liberamos memoria.
                Desconectar();
                // Asignamos error.
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando Conectar: " + Conexion.ConnectionString, ex);
                // Asignamos error de funci�n
                ok = false;
            }
            return ok;
        }

        /// <summary>
        /// Cierra la conexion de BBDD.
        /// </summary>
        public bool Desconectar()
        {
            try
            {
                // Cerramos la conexion
                if ((((Conexion) != null)))
                {
                    if ((Conexion.State != ConnectionState.Closed))
                    {
                        Conexion.Close();
                    }
                }
                // Liberamos su memoria.
                Conexion.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando Desconectar:", ex);
                return false;
            }
        }

        /// <summary>
        /// Devuelve el estado de la base de datos
        /// </summary>
        /// <returns>True si esta conectada.</returns>
        public bool IsConected()
        {
            bool ok = false;
            try
            {
                // Si el objeto conexion ha sido instanciado
                if ((((Conexion) != null)))
                {
                    // Segun el estado de la Base de Datos.
                    switch ((Conexion.State))
                    {
                        case ConnectionState.Closed:
                        case ConnectionState.Broken:
                        case ConnectionState.Connecting:
                            ok = false;
                            break;
                        case ConnectionState.Open:
                        case ConnectionState.Fetching:
                        case ConnectionState.Executing:
                            ok = true;
                            break;
                    }
                }
                else
                {
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando IsConected:", ex);
                ok = false;
            }
            return ok;
        }

        #endregion

        #region "Ejecutar SPs y SQLs"

        /// <summary>
        /// Ejecuta un procedimiento almacenado de Oracle.
        /// </summary>
        /// <param name="oraCommand">Objeto Command con los datos del procedimiento.</param>
        /// <param name="SpName">Nombre del procedimiento almacenado.</param>
        /// <returns>True si el procedimiento se ejecuto bien.</returns>
        public bool EjecutarSP(ref OracleCommand OraCommand, string SpName)
        {
            bool ok = true;
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }
                if (ok)
                {
                    OraCommand.Connection = Conexion;
                    OraCommand.CommandText = SpName;
                    OraCommand.CommandType = CommandType.StoredProcedure;
                    OraCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando EjecutarSP: " + OraCommand.CommandText, ex);
                ok = false;
            }
            return ok;
        }

        /// <summary>
        /// Ejecuta una sql que rellenar un DataReader (sentencia select).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <returns></returns> 
        public DataTable EjecutarSQL(string SqlQuery)
        {
            bool ok = true;
            OracleCommand ora_Command = new OracleCommand();
            DataTable objRetorno = new DataTable();
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }
                if (ok)
                {
                    // Cerramos cursores abiertos, para evitar el error ORA-1000
                    if ((((ora_DataReader) != null)))
                    {
                        ora_DataReader.Close();
                        ora_DataReader.Dispose();
                    }
                    ora_Command.Connection = Conexion;
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;

                    // Ejecutamos sql.
                    ora_DataReader = ora_Command.ExecuteReader();
                    objRetorno.Load(ora_DataReader);
                }
            }
            catch (Exception ex)
            {
                Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando EjecutarSQL:", ex);
                ok = false;
            }
            finally
            {
                Desconectar();
                ora_Command.Dispose();
            }

            //Return ok
            return objRetorno;
        }

        /// <summary>
        /// Ejecuta una sql que no devuelve datos (Update, Delete, Insert).
        /// </summary>
        /// <param name="SqlQuery">sentencia sql a ejecutar</param>
        /// <param name="FilasAfectadas">Fila afectadas por la sentencia SQL</param>
        /// <returns></returns>
        public bool EjecutarSQL(string SqlQuery, ref int FilasAfectadas)
        {
            bool ok = true;
            OracleCommand ora_Command = new OracleCommand();
            try
            {
                // Si no esta conectado, se conecta.
                if (!IsConected())
                {
                    ok = Conectar();
                }
                if (ok)
                {
                    Transaccion = Conexion.BeginTransaction();
                    ora_Command = Conexion.CreateCommand();
                    ora_Command.CommandType = CommandType.Text;
                    ora_Command.CommandText = SqlQuery;
                    FilasAfectadas = ora_Command.ExecuteNonQuery();
                    Transaccion.Commit();
                }
            }
            catch (Exception ex)
            {
                // Hacemos rollback.
                Transaccion.Rollback();
                Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando EjecutarSQL:", ex);
                ok = false;
            }
            finally
            {
                Desconectar();
                // Recolectamos objetos para liberar su memoria.
                if ((((ora_Command) != null)))
                {
                    ora_Command.Dispose();
                }
            }
            return ok;
        }
        #endregion
    }
}
