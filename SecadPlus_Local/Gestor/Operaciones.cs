using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SECADPLUS_Local.Gestor
{
    class Operaciones
    {
        private OracleConnection oracleConexion;
        private OracleDataAdapter adaptador;
        private string strCadenaCon;
        protected OracleCommand oracleCommand;
        protected OracleDataReader oracleReader;
        protected OracleParameter oracleOutput;
        protected OracleParameter oracleAuxiliar;
        protected OracleRefCursor cursor;
        protected OracleDataReader oracleDataReader;
        protected List<Tuple<string, string, Type>> paramsAvanzados;
        protected List<Tuple<string, object, Type>> paramsSalida;
        protected List<Tuple<string, object, Type>> paramsAux;
        protected bool blEstado;
        protected DataTable tabla;
        public static string EX_MENSAJE { get; set; }
        public static bool RESULTADO { get; set; }
        public string[] strArreglo;
        
        //private static Hashtable listaProcesos = new Hashtable();
        //private static System.Windows.Forms.Timer tmBuscarProcesos = new System.Windows.Forms.Timer();
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
        public static void EjecutarOperacion(Enumeraciones.SecadOperacion opcion, EventLog logger)
        {
            try
            {
                bool resultado = false;
                switch (opcion)
                {
                    case Enumeraciones.SecadOperacion.ActualizarNombreMaquina:
                        resultado = ActualizarNombreMaquina(logger);
                        break;
                    case Enumeraciones.SecadOperacion.ControlBD:
                        resultado = EjecutarControlDB(logger);
                        break;
                    case Enumeraciones.SecadOperacion.EjecutarMonitorTelefonia:
                        resultado = EjecutarMonitorTelefonia(logger);
                        break;
                    default:
                        resultado = false;
                        break;
                }
                if (resultado)
                {
                    logger.WriteEntry("Operación completada con exito");
                }
                else
                {
                    logger.WriteEntry("Operación completada de manera incorrecta.  | mensaje: " + EX_MENSAJE);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static bool ActualizarNombreMaquina(EventLog logger)
        {
            Operaciones operador = new Operaciones();
            try
            {

                string strNombre = "", strIp = "";
                IPHostEntry host = new IPHostEntry();


                host = Dns.GetHostEntry(Dns.GetHostName());
                //Obtiene la ip local
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIp = ip.ToString();
                    }
                }
                strNombre = Environment.MachineName;
                strNombre = Dns.GetHostEntry(strNombre).HostName;
                operador.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_ip", strIp, typeof(string)));
                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_maquina", strNombre, typeof(string)));
                operador.EjecutarProcedimiento("USR_CAD.pk_cadplus.p_registrar_maquina", SECADPLUSService.CADENA_CONEXION, "bool");
                //Registra los datos              
                return true;
            }
            catch (Exception ex)
            {
                EX_MENSAJE = ex.Message + " StackTrace: " + ex.StackTrace.ToString();
                logger.WriteEntry("Servicio Interrumpido.  | mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace.ToString());
                return false;
            }
            finally
            {

            }
        }
        private static bool EjecutarControlDB(EventLog logger)
        {
            Operaciones operador = new Operaciones();
            try
            {

                string strNombre = "", strIp = "";
                IPHostEntry host = new IPHostEntry();


                host = Dns.GetHostEntry(Dns.GetHostName());
                //Obtiene la ip local
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIp += " - || " + ip.ToString();
                    }
                }

                strNombre = Environment.MachineName;
                strNombre = Dns.GetHostEntry(strNombre).HostName;
                logger.WriteEntry("IP DE LA MÁQUINA ACTUAL: " + strIp + "-- NOMBRE DE LA MÁQUINA: " + strNombre);
                operador.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                operador.EjecutarProcedimiento("USR_CAD.pk_cadplus.P_CONTROL_DB", SECADPLUSService.CADENA_CONEXION, "bool");
                //Registra los datos              
                return true;
            }
            catch (Exception ex)
            {
                EX_MENSAJE = ex.Message + " StackTrace: " + ex.StackTrace.ToString();
                logger.WriteEntry("No se pudo completar la ejecucion del SP.  | mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace.ToString());
                return false;
            }
            finally
            {

            }
        }
        /// <summary>
        /// Creado por: Ing. Pablo Andrés Pinzón Fecha de cración : 24/10/2016 Permite ejecutar
        /// cualquier procedimiento almacenado de Oracle
        /// </summary>
        /// <param name="strNombre">Nombre del procedimiento almacenado</param>
        /// <param name="strConexion">Cadena de conexion a la base de datos</param>
        /// <param name="strQueryTipo">Tipo de query a ejecutar</param>
        /// <returns>Retorna un objeto datatable</returns>
        public DataTable EjecutarProcedimiento(string strNombre, string strConexion, string strQueryTipo = null, int? intTimeOut = null)
        {
            try
            {
                blEstado = false;
                tabla = new DataTable();
                strCadenaCon = strConexion;
                oracleConexion = new OracleConnection(strCadenaCon);
                //oracleCommand = new OracleCommand();
                oracleConexion.Open();
                oracleCommand = oracleConexion.CreateCommand();
                oracleCommand.CommandType = CommandType.StoredProcedure;

                oracleCommand.CommandText = strNombre;
                oracleCommand.InitialLONGFetchSize = -1;
                oracleCommand.InitialLOBFetchSize = -1;
                if (intTimeOut != null)
                {
                    oracleCommand.CommandTimeout = (int)intTimeOut;
                }

                if (ParamsAvanzados != null)
                {
                    //Adiciona los parametros y sus valores correspondientes
                    foreach (Tuple<string, string, Type> parameter in ParamsAvanzados)
                    {
                        if (parameter.Item3 == typeof(string))
                        {
                            oracleCommand.Parameters.Add(
                            new OracleParameter(parameter.Item1, parameter.Item2));
                        }
                        if (parameter.Item3 == typeof(decimal))
                        {
                            oracleCommand.Parameters.Add(
                            new OracleParameter(parameter.Item1, Convert.ToDecimal(parameter.Item2)));
                        }
                        if (parameter.Item3 == typeof(Int16))
                        {
                            oracleCommand.Parameters.Add(
                            new OracleParameter(parameter.Item1, Convert.ToInt16(parameter.Item2)));
                        }
                        if (parameter.Item3 == typeof(Int32))
                        {
                            oracleCommand.Parameters.Add(
                            new OracleParameter(parameter.Item1, Convert.ToInt32(parameter.Item2)));
                        }
                        if (parameter.Item3 == typeof(Int64))
                        {
                            oracleCommand.Parameters.Add(
                            new OracleParameter(parameter.Item1, Convert.ToInt64(parameter.Item2)));
                        }
                        if (parameter.Item3 == typeof(DateTime))
                        {
                            oracleAuxiliar = new OracleParameter(parameter.Item1, OracleDbType.Date);

                            oracleAuxiliar.Value = Convert.ToDateTime(parameter.Item2);
                            oracleCommand.Parameters.Add(oracleAuxiliar);
                        }
                        if (parameter.Item3 == typeof(OracleDbType))
                        {
                            oracleOutput = new OracleParameter();
                            oracleOutput.OracleDbType = OracleDbType.RefCursor;
                            oracleOutput.Direction = System.Data.ParameterDirection.Output;
                            oracleCommand.Parameters.Add(oracleOutput);
                        }
                        if (parameter.Item3 == typeof(string[]))
                        {
                            OracleParameter param = new OracleParameter(parameter.Item1, OracleDbType.Varchar2);
                            param.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                            param.Value = strArreglo;
                            oracleCommand.Parameters.Add(param);
                        }
                    }
                }
                if (ParamsSalida != null)
                {
                    //Adiciona los parametros y sus valores correspondientes
                    foreach (Tuple<string, object, Type> parameter in ParamsSalida)
                    {
                        if (parameter.Item3 == typeof(string))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
                        }
                        if (parameter.Item3 == typeof(decimal))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Decimal).Direction = ParameterDirection.Output;
                        }
                        if (parameter.Item3 == typeof(Int16))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Int16).Direction = ParameterDirection.Output;
                        }
                        if (parameter.Item3 == typeof(Int32))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Int32).Direction = ParameterDirection.Output;
                        }
                        if (parameter.Item3 == typeof(DateTime))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Date).Direction = ParameterDirection.Output;
                        }
                    }
                }
                switch (strQueryTipo)
                {
                    case "bool":
                        oracleCommand.ExecuteNonQuery();
                        blEstado = true;
                        break;

                    case "dataReader":
                        OracleDataReader dr = oracleCommand.ExecuteReader();
                        tabla.Rows.Add();
                        tabla.Columns.Add();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                tabla.Rows[0][0] = dr.GetString(0);
                            }
                        }
                        break;

                    case "MULTIPLE":
                        oracleCommand.ExecuteNonQuery();
                        paramsAux = new List<Tuple<string, object, Type>>();

                        foreach (Tuple<string, object, Type> parameter in ParamsSalida)
                        {
                            paramsAux.Add(parameter);
                        }
                        ParamsSalida.Clear();
                        //recorre los parametros de salida para obtener los valores
                        foreach (Tuple<string, object, Type> parameter in paramsAux)
                        {
                            paramsSalida.Add(new Tuple<string, object, Type>(parameter.Item1, oracleCommand.Parameters[parameter.Item1].Value.ToString(), parameter.Item3));
                        }
                        break;

                    default:
                        tabla.Clear();
                        tabla.Dispose();

                        oracleCommand.ExecuteNonQuery();
                        cursor = (OracleRefCursor)oracleOutput.Value;
                        oracleDataReader = cursor.GetDataReader();
                        adaptador = new OracleDataAdapter(oracleCommand);
                        adaptador.Fill(tabla);
                        adaptador.Dispose();
                        oracleDataReader.Dispose();

                        break;
                }
                return tabla;
            }
            catch (OracleException oex)
            {

                throw oex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
            }
        }

        private static bool EjecutarMonitorTelefonia(EventLog logger)
        {
            //Detectar la máquina y arrancar solo si coincide con el configurado
            string strNombre = "";
            IPHostEntry host = new IPHostEntry();

            strNombre = Environment.MachineName;
            strNombre = Dns.GetHostEntry(strNombre).HostName;

            if (!strNombre.StartsWith(SECADPLUSService.EQUIPO_MONITOR))
            {
                EX_MENSAJE = "Máquina no es " + SECADPLUSService.EQUIPO_MONITOR;
                return false;
            }

            try
            {
                SECADPLUSService.procesoLauncher = new Process();
                SECADPLUSService.procesoLauncher.StartInfo.FileName = "C:\\ITO\\Servicio\\LauncherMonitor.exe";
                SECADPLUSService.procesoLauncher.StartInfo.UseShellExecute = false;
                SECADPLUSService.procesoLauncher.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                SECADPLUSService.procesoLauncher.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al Ejecutar el launcher");
                logger.WriteEntry("Error al Ejecutar el launcher");
                EX_MENSAJE = ex.ToString();
                return false;
            }
            
            return true;
        }
    }
}
