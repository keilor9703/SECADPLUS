using System;
using System.Collections.Generic;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Collections;
using System.Diagnostics;

namespace MonitorLinea.Gestor
{
    class Operaciones
    {
        protected bool blEstado;
        protected DataTable tabla;
        private string strCadenaCon;
        private OracleConnection oracleConexion;
        protected OracleCommand oracleCommand;
        public List<Tuple<string, string, Type>> ParamsAvanzados;
        protected List<Tuple<string, object, Type>> paramsSalida;
        protected OracleParameter oracleOutput;
        protected OracleParameter oracleAuxiliar;
        protected List<Tuple<string, object, Type>> paramsAux;
        protected OracleRefCursor cursor;
        protected OracleDataReader oracleDataReader;
        private OracleDataAdapter adaptador;
        public string[] strArreglo;
        private static Hashtable hashColas = new Hashtable();
        private static int segundosLlamada = 10;


        public static void RegistrarLlamadaEnCola(string extension, string numeroLlamadaEntrante, EventLog logger)
        {
            if (string.IsNullOrEmpty(extension))
            {
                Console.WriteLine("Número vacío");
                logger.WriteEntry("Número vacío");
                return;
            }
            //Buscamos la cola y la creamos si no existe
            Queue<Tuple<string, string, DateTime>> colaLlamadas = (Queue<Tuple<string, string, DateTime>>)hashColas[extension];
            if (colaLlamadas == null)
            {
                colaLlamadas = new Queue<Tuple<string, string, DateTime>>();
                hashColas.Add(extension, colaLlamadas);
            }
            //Buscamos si el número existe en la cola y es reciente
            bool existeReciente = false;
            if (colaLlamadas.Count > 0)
            {
                foreach (Tuple<string, string, DateTime> llamada in colaLlamadas)
                {
                    if (extension.Equals(llamada.Item1.ToString()) && numeroLlamadaEntrante.Equals(llamada.Item2.ToString()))
                    {
                        TimeSpan difTiempo = DateTime.Now - (DateTime)llamada.Item3;
                        Console.WriteLine("Tiempo: " + difTiempo.Seconds + " seg.");
                        logger.WriteEntry("Tiempo: " + difTiempo.Seconds + " seg.");
                        if (difTiempo.Seconds <= segundosLlamada)
                        {
                            existeReciente = true;
                            break;
                        }
                    }
                }
            }

            if (!existeReciente)
            {
                colaLlamadas.Enqueue(new Tuple<string, string, DateTime>(extension, numeroLlamadaEntrante, DateTime.Now));
                Console.WriteLine("Llamada " + extension + " - " + numeroLlamadaEntrante + " en cola");
                logger.WriteEntry("Llamada " + extension + " - " + numeroLlamadaEntrante + " en cola");
            }
        }

        public static void ProcesarColas(EventLog logger)
        {
            foreach (Object key in hashColas.Keys)
            {
                Queue<Tuple<string, string, DateTime>> colaLlamadas = (Queue<Tuple<string, string, DateTime>>)hashColas[key];
                if (colaLlamadas.Count == 0)
                {
                    return;
                }
                Tuple<string, string, DateTime> llamada = colaLlamadas.Peek();
                string extension = llamada.Item1.ToString();
                if (!HayLlamadaPendiente(extension, logger))
                {
                    bool esRegistrada = RegistrarLlamadaEnBd(extension, llamada.Item2.ToString(), logger);
                    if (esRegistrada)
                    {
                        colaLlamadas.Dequeue();
                        Console.WriteLine("Llamada " + extension + " - " + llamada.Item2.ToString() + " eliminada de la cola");
                        logger.WriteEntry("Llamada " + extension + " - " + llamada.Item2.ToString() + " eliminada de la cola");
                    }
                }
            }
        }

        public static bool RegistrarLlamadaEnBd(string extension, string numeroLlamadaEntrante, EventLog logger)
        {
            Operaciones operador = new Operaciones();
            try
            {
                operador.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_extension", extension, typeof(string)));
                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeroLlamadaEntrante", numeroLlamadaEntrante, typeof(string)));
                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", Monitor.SITIO_GRABA.ToString(), typeof(string)));
                operador.EjecutarProcedimiento("PK_CADPLUS.p_registrar_llamada", Monitor.CADENA_CONEXION, "bool");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logger.WriteEntry(ex.ToString());
                return false;
            }
        }

        public static bool HayLlamadaPendiente(string extension, EventLog logger)
        {
            bool hayLlamadaPendiente = true;
            Operaciones operador = new Operaciones();
            try
            {
                operador.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                operador.ParamsSalida = new List<Tuple<string, object, Type>>();

                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_extension", extension, typeof(string)));
                operador.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitioGraba", Monitor.SITIO_GRABA.ToString(), typeof(string)));
                operador.ParamsSalida.Add(new Tuple<string, object, Type>("p_pendiente", null, typeof(string)));
                operador.EjecutarProcedimiento("PK_CADPLUS.p_llamada_pendiente", Monitor.CADENA_CONEXION, "MULTIPLE");
                Tuple<string, object, Type> parameter = operador.ParamsSalida.First();
                if (parameter.Item2.ToString().Equals("N"))
                {
                    hayLlamadaPendiente = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logger.WriteEntry(ex.ToString());
            }
            return hayLlamadaPendiente;
        }


        public DataTable EjecutarProcedimiento(string strNombre, string strConexion, string strQueryTipo = null, int? intTimeOut = null)
        {
            try
            {
                blEstado = false;
                tabla = new DataTable();
                strCadenaCon = strConexion;
                oracleConexion = new OracleConnection(strCadenaCon);
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

    }
}
