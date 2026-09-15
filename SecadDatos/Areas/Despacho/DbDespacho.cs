using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SecadDatos.Context;
using SecadModelos.Areas.Patrullas;
using System.Data;
using SecadModelos.Areas.Despacho;

namespace SecadDatos.Areas.Despacho
{
    public class DbDespacho
    {
        public List<MdDespacho> F_GetTime(Int64 P_IdNumEvento)
        {
            List<MdDespacho> retorno = new List<MdDespacho>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_IdNumEvento", OracleDbType.Int64)).Value = P_IdNumEvento;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADXCRIME.F_GetTime";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdDespacho ObjR = new MdDespacho
                        {
                            HoraAsigna = fila["HoraAsigna"].ToString(),
                            HoraLLega = fila["HoraLLega"].ToString(),
                            HoraTermina = fila["HoraTermina"].ToString(),
                            HoraRuta = fila["HoraRuta"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetDocumentPreRuta:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
                cnOracle.Dispose();
                resultado.Dispose();
            }
            return retorno;
        }

        public List<MdHash> F_HashMapa(Int64 V_Fuerza, Int64 V_CANAL)
        {
            List<MdHash> retorno = new List<MdHash>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionXcrime");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("P_FUERZA", OracleDbType.Int64)).Value = V_Fuerza;
                objCommand.Parameters.Add(new OracleParameter("P_CANAL", OracleDbType.Int64)).Value = V_CANAL;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADXCRIME.F_HASHMAPACANAL";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdHash ObjR = new MdHash
                        {
                            Hash = fila["Ruta"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADXCRIME.F_HASHMAPACANAL:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
                cnOracle.Dispose();
                resultado.Dispose();
            }
            return retorno;
        }

        public async Task<string> ConsultaRutasSistemas(string _local, string _sistema, string _pruebas)
        {
            string retorno = "";
            ConDBOracle cnOracle = new ConDBOracle("ConnectionSisec");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            bool pruebas = Convert.ToBoolean(_pruebas);
            bool local = Convert.ToBoolean(_local);
            int localint = 0;
            int pruebaint = 0;

            if (pruebas)
            {
                pruebaint = 1;
            }
            else { pruebaint = 0; }

            if (local)
            {
                localint = 1;
            }
            else { localint = 0; }

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("P_Sistema", OracleDbType.Varchar2, 255)).Value = _sistema;
                objCommand.Parameters.Add("P_Local", OracleDbType.Int32, ParameterDirection.Input).Value = localint;
                objCommand.Parameters.Add("P_Pruebas", OracleDbType.Int32, ParameterDirection.Input).Value = pruebaint;
                objCommand.Parameters.Add(new OracleParameter("P_Descripcion", OracleDbType.Varchar2, 255)).Direction = ParameterDirection.Output;



                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SISTEMA.F_GetSistemas";



                if (cnOracle.Conectar())
                {
                    await objCommand.ExecuteReaderAsync();
                    resultado.Load(await objCommand.ExecuteReaderAsync());
                    cnOracle.Desconectar();
                    objCommand.Dispose();
                }
                retorno = objCommand.Parameters["P_Descripcion"].Value.ToString();



            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                objCommand.Connection.Close();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_FUNCIONARIOS.F_GetFuncAntUnidad:", ex);
                objCommand = null;

            }
            finally
            {
                cnOracle.Desconectar();
                cnOracle.Dispose();
                objCommand = null;
                resultado.Dispose();
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return retorno;
        }
    }
}
