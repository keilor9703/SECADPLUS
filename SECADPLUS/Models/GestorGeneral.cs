using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Types;
//using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;
using SECADPLUS.Models.Extendidas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace SECADPLUS.Models
{
    public class GestorGeneral : GestorBase
    {
        #region Atributos

        private OracleConnection oracleConexion;
        private OracleDataAdapter adaptador;
        private string strCadenaCon;
        protected OracleCommand oracleCommand;
        protected OracleDataReader oracleReader;
        protected OracleParameter oracleOutput;
        protected OracleParameter oracleAuxiliar;
        protected OracleRefCursor cursor;
        protected OracleDataReader oracleDataReader;
        public string[] strArreglo;

        #endregion Atributos

        public string CadenaConexion
        {
            get { return strCadenaCon; }
            set { strCadenaCon = value; }
        }

        #region métodos

        /// <summary>
        /// Permite ontener el listado de usuarios en la base de datos
        /// </summary>
        /// <returns></returns>
        public List<ExtUsuarioSecad> ListarUsuarios()
        {
            try
            {
                List<ExtUsuarioSecad> lstUsuarios = new List<ExtUsuarioSecad>();
                paramsAvanzados = new List<Tuple<string, string, Type>>();
                paramsAvanzados.Add(new Tuple<string, string, Type>("OPCION", "USUARIOS", typeof(string)));
                paramsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", "", typeof(string)));
                paramsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_generarLista", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NonQuery");
                foreach (DataRow fila in tabla.Rows)
                {
                    lstUsuarios.Add(new ExtUsuarioSecad(fila));
                }
                return lstUsuarios;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable LlenarDataSet(OracleCommand cmd, string strTabla)
       // public DataTable LlenarDataSet(OracleCommand cmd, string strTabla)
        {
            tabla = new DataTable();
            oracleConexion = new OracleConnection(strCadenaCon);
            cmd.Connection = oracleConexion;
            adaptador = new OracleDataAdapter(cmd);
            try
            {
                oracleConexion.Open();
                adaptador.Fill(tabla);
                oracleConexion.Close();
                oracleConexion.Dispose();
                return tabla;
            }
            catch (Exception e)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                throw new ApplicationException("Error: " + e);
            }
            finally
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
            }
        }

        public T EjecutarFuncion<T>(string strNombre, string strConexion)
        {
            try
            {
                int RETURN_VALUE_BUFFER_SIZE = 32767;
                strCadenaCon = strConexion;
                oracleConexion = new OracleConnection(strCadenaCon);
                //oracleCommand = new OracleCommand();
                oracleConexion.Open();
                oracleCommand = oracleConexion.CreateCommand();
                oracleCommand.CommandType = CommandType.StoredProcedure;
                oracleCommand.CommandText = strNombre;
                //Adiciona los parametros y sus valores correspondientes
                foreach (Tuple<string, string, Type> parameter in ParamsAvanzados)
                {
                    if (parameter == ParamsAvanzados.First())
                    {
                        if (parameter.Item3 == typeof(DateTime))
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Date, RETURN_VALUE_BUFFER_SIZE);
                        }
                        else
                        {
                            oracleCommand.Parameters.Add(parameter.Item1, OracleDbType.Varchar2, RETURN_VALUE_BUFFER_SIZE);
                        }

                        oracleCommand.Parameters[parameter.Item1].Direction = ParameterDirection.ReturnValue;
                        continue;
                    }
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
                        oracleCommand.Parameters.Add(
                        new OracleParameter(parameter.Item1, Convert.ToDateTime(parameter.Item2)));
                    }
                }

                oracleCommand.ExecuteNonQuery();
                oracleConexion.Close();
                oracleConexion.Dispose();
                return (T)Convert.ChangeType(oracleCommand.Parameters[ParamsAvanzados[0].Item1].Value.ToString(), typeof(T));
            }
            catch (OracleException oex)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                throw oex;

            }
            catch (Exception ex)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                throw GestionarExcepcion(ex, GetType().Name);
            }
            finally
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
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

                        ////prueba daniel
                        if (parameter.Item3 == typeof(string[]))
                        {
                            OracleParameter param = new OracleParameter(parameter.Item1, OracleDbType.Varchar2);
                            param.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                            param.Value = strArreglo;
                            //oracleCommand.ArrayBindCount = prueba.Length;
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
                oracleConexion.Close();
                oracleConexion.Dispose();
                return tabla;
            }
            catch (OracleException oex)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                intTemp = oex.Errors[0].Number;
                if (intTemp == 12170)//Problemas al utilizar el tsnName de OFITE
                {
                    GestionarExcepcion(oex, GetType().Name);
                    BL_CONEXION_PERPONAL = false;
                    return tabla;
                }
                if (oex.Number == 1403)
                {
                    MSN_ALERTA_COMUN = Resources.RecursoGeneral.MSN_NO_DATOS;
                    tabla = new DataTable();
                    blEstado = true;
                    return tabla;
                }
                Exception exTemporal = GestionarExcepcion(oex, GetType().Name);
                throw exTemporal;
            }
            catch (Exception ex)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                throw GestionarExcepcion(ex, GetType().Name);
            }
            finally
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
            }
        }

        /// <summary>
        /// Permite obtener el consecutivo para la inserción de unidades
        /// </summary>
        /// <param name="strCodigo"></param>
        /// <returns></returns>
        public int ObtenerMaximoUnidad(string strCodigo)
        {
            try
            {
                intTemp = 0;
                paramsAvanzados = new List<Tuple<string, string, Type>>();
                paramsAvanzados.Add(new Tuple<string, string, Type>("OPCION", "UNIDADES", typeof(string)));
                paramsAvanzados.Add(new Tuple<string, string, Type>("CODIGO", strCodigo, typeof(string)));
                paramsAvanzados.Add(new Tuple<string, string, Type>("UNIDADES", "", typeof(OracleDbType)));
                tabla = EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.obtener_maximo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                foreach (DataRow fila in tabla.Rows)
                {
                    intTemp = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString())) ? 0 : Convert.ToInt32(fila["CONSECUTIVO"].ToString());
                }
                return intTemp;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite consultar los empleados del sistema
        /// </summary>
        /// <returns></returns>
        public List<Empleado> ObtenerEmpleados()
        {
            try
            {
                Empleado empleado;
                List<Empleado> listEmpleados = new List<Empleado>();
                //OBTENEMOS LISTA DE EMPELADOS
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_listar_empleados", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "nonquery");
                foreach (DataRow fila in tabla.Rows)
                {
                    empleado = new Empleado(fila);
                    listEmpleados.Add(empleado);
                }
                return listEmpleados;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite realizar la consulta de patrullas en el sitio de grabación
        /// </summary>
        /// <param name="intSitio">código del sitio de grabación</param>
        /// <param name="strPatrulla">nomre de la patrulla</param>
        /// <returns></returns>
        public string ConsultarPatrullas(int intSitio, string strPatrulla)
        {
            try
            {
                string strHtmlSalto = "\n";
                intTemp = 0;
                strTemp = "";
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_grabacion", intSitio.ToString(), typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_patrulla", strPatrulla, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_patrullas", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                foreach (DataRow fila in tabla.Rows)
                {
                    if (Convert.ToInt32(fila["CODIGO"].ToString()) != intTemp)
                    {
                        strTemp += "El medio de atención " + fila["DESC_PATRULLA"].ToString();
                        if (!string.IsNullOrEmpty(fila["VIGENTE"].ToString()))
                        {
                            if (fila["VIGENTE"].ToString().Equals("S"))
                                strTemp += strHtmlSalto + "Se encuentra activo desde " + fila["FECHA_DESDE"].ToString();
                            if (!fila["VIGENTE"].ToString().Equals("S"))
                                strTemp += strHtmlSalto + "Estuvo activo desde " + fila["FECHA_DESDE"].ToString() + " hasta " + fila["FECHA_HASTA"].ToString();
                        }
                        strTemp += strHtmlSalto + "Pertenece a:" + fila["UNIDADES"].ToString();
                        strTemp += strHtmlSalto + "Id dispositivos:";
                        intTemp = Convert.ToInt32(fila["CODIGO"].ToString());
                    }
                    strTemp += strHtmlSalto + (string.IsNullOrEmpty(fila["GPS_CODE"].ToString()) ? "" : fila["GPS_CODE"].ToString()) +
                            " Vehículo: " + (string.IsNullOrEmpty(fila["SIGLA_VEHICULO"].ToString()) ? "" : fila["SIGLA_VEHICULO"].ToString());
                }
                return strTemp;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite consultar los vehiculos de un sitio de grabación
        /// </summary>
        /// <param name="strSigla"></param>
        /// <returns></returns>
        public string ConsultarVehiculo(string strSigla)
        {
            try
            {
                string strHtmlSalto = "\n";
                string strControl = "";
                strTemp = "";
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_vehiculo_sigla", strSigla, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_vehiculos", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                foreach (DataRow fila in tabla.Rows)
                {
                    if (!fila["NUME_SIGLA"].ToString().Equals(strControl))
                    {
                        strTemp += "El Vehículo: " + fila["DESC_CLASE"].ToString() + ", " + fila["DESC_MARCA"].ToString();
                        strTemp += strHtmlSalto + "De sigla: " + fila["NUME_SIGLA"].ToString() + " y placa: " + fila["NUME_PLACA"].ToString();
                        strTemp += strHtmlSalto + "Pertenece a:" + fila["UNIDADES"].ToString();
                        strTemp += strHtmlSalto + "Id dispositivos rastreo:";
                        strControl = (fila["NUME_SIGLA"].ToString());
                    }
                    strTemp += strHtmlSalto + (string.IsNullOrEmpty(fila["GPS_CODE"].ToString()) ? "" : fila["GPS_CODE"].ToString());
                }
                return strTemp;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string ConsultarUnidadRastreo(string strSigla)
        {
            try
            {
                string strHtmlSalto = "\n";
                string strControl = "";
                strTemp = "";
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_vehiculo_sigla", strSigla, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_unidad_rastreo", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                foreach (DataRow fila in tabla.Rows)
                {
                    if (!fila["GPS_CODE"].ToString().Equals(strControl))
                    {
                        strTemp += "La Unidad de rastreo: " + fila["GPS_CODE"].ToString();
                        strTemp += strHtmlSalto + "Esta asocaida al sitio de grabación: " + fila["SITIO_DESC"].ToString();
                        strTemp += strHtmlSalto + "Patrulla: " + fila["DESC_PATRULLA"].ToString();
                        strTemp += strHtmlSalto + "Vehículo: " + fila["SIGLA_VEHICULO"].ToString();
                        strTemp += strHtmlSalto + "Pertenece a:" + fila["UNIDADES"].ToString();
                        strTemp += strHtmlSalto + "Id dispositivos rastreo:";
                        strControl = (fila["GPS_CODE"].ToString());
                    }
                }
                return strTemp;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public void ActualizarUsuariosOfite()
        {
            try
            {
                paramsAvanzados = new List<Tuple<string, string, Type>>();
                EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_actualizar_usuarios_ofite", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Actualizar los empleados del desde la vista vempleados a cad_policias
        /// </summary>
        /// <returns></returns>
        public DataTable ActualizaEmpleados(int iFuerzaId)
        {
            try
            {
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_id_fuerza", iFuerzaId.ToString(), typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                tabla = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualiza_personal_ponal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "NONQUERY");
                string resultado = string.Empty;
                return tabla;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite realiar la cosnulta de los padres de una unidad
        /// </summary>
        /// <param name="intUnidadFuerza"></param>
        /// <param name="intUnidadCodigo"></param>
        /// <returns></returns>
        public static string ConsultarPadresUnidad(int intUnidadFuerza, int intUnidadCodigo)
        {
            try
            {
                GestorGeneral gestor = new GestorGeneral();
                gestor.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", intUnidadFuerza.ToString(), typeof(string)));
                gestor.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", intUnidadCodigo.ToString(), typeof(string)));
                string strValor = gestor.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.F_PADRES_UNIDAD", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                return strValor;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, "GestorGeneral");
            }
        }

        
        #endregion métodos
    }

}