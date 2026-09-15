using Oracle.ManagedDataAccess.Client;
using SecadDatos.Context;
using SecadModelos.Areas.RNMC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadDatos.Areas.RNMC
{
   public class DbRnmc
    {
        public List<MdRnmc> F_GetArticulos()
        {
            List<MdRnmc> retorno = new List<MdRnmc>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.F_GetArticulos";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdRnmc ObjR = new MdRnmc
                        {
                            Id_Comportamiento = Convert.ToInt32(fila["ID_COMPORTAMIENTO"].ToString()),
                            Descripcion = fila["DESCRIPCION"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetArticulos:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }

            return retorno;
        }

        public List<MdRnmc> F_GetNumeral_Id(int V_Articulo)
        {
            List<MdRnmc> retorno = new List<MdRnmc>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_Articulo", OracleDbType.Int32)).Value = V_Articulo;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.F_GetNumeral_Id";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdRnmc ObjR = new MdRnmc
                        {
                            Id_Comportamiento = Convert.ToInt32(fila["ID_COMPORTAMIENTO"].ToString()),
                            Descripcion = fila["DESCRIPCION"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetNumeral_Id:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }

        public List<MdRnmc> F_GetLiteral_Id(int V_Numeral)
        {
            List<MdRnmc> retorno = new List<MdRnmc>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_Numeral", OracleDbType.Int32)).Value = V_Numeral;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.F_GetLiteral_Id";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdRnmc ObjR = new MdRnmc
                        {
                            Id_Comportamiento = Convert.ToInt32(fila["ID_COMPORTAMIENTO"].ToString()),
                            Descripcion = fila["DESCRIPCION"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetLiteral_Id:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }
    }
}
