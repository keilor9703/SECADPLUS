using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Globalization;
using SecadModelos.Areas.PlanesPrev;
using SecadDatos.Context;

namespace SecadDatos.Areas.PlanesPreventivos
{
    public class DbPlanesPrev
    {
        public List<MdPlanesPreventivos> F_GetPlanesGPS(int? V_IdUnidad)
        {
            List<MdPlanesPreventivos> retorno = new List<MdPlanesPreventivos>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSINAC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_IdUnidad", OracleDbType.Int32)).Value = V_IdUnidad;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "USR_SINAC.PK_SINAC.F_GetPlanesGPS";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdPlanesPreventivos ObjR = new MdPlanesPreventivos
                        {
                            Tipoplan = fila["Tipoplan"].ToString(),
                            IdTipoPlan = Convert.ToInt32(fila["IdTipoPlan"].ToString()),
                            Longitud = fila["Longitud"].ToString(),
                            Latitud = fila["Latitud"].ToString(),
                            Unidad = fila["Unidad"].ToString(),
                            UndeConsecutivo = Convert.ToInt32(fila["UndeConsecutivo"].ToString()),
                            Jefe = fila["Jefe"].ToString(),
                            Dispositivo = fila["Dispositivo"].ToString(),
                            Celular = Convert.ToInt64(fila["Celular"].ToString()),
                            Inicio = fila["Inicio"].ToString(),
                            Modifica = fila["Modifica"].ToString(),
                            IdPLan = Convert.ToInt32(fila["IdPLan"].ToString()),
                            IdMinuta = Convert.ToInt32(fila["IdMinuta"].ToString())
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetPlanesGPS:", ex);
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
