using Oracle.ManagedDataAccess.Client;
using SecadDatos.Context;
using System;
using System.Collections.Generic;
using SecadModelos.Areas.Patrullas;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadDatos.Areas.Patrullas
{
    public class DbPatrulla
    {

        public List<MdCadPatrulla> F_GetConsCuadrante(int V_IntUnidadFuerza, int V_IntUnidadCodigo)
        {
            List<MdCadPatrulla> retorno = new List<MdCadPatrulla>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadFuerza", OracleDbType.Int32)).Value = V_IntUnidadFuerza;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = V_IntUnidadCodigo;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.F_GetConsCuadrante";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdCadPatrulla Patru = new MdCadPatrulla();
                        Patru.CadUniFuerza = Convert.ToInt32(fila["CadUniFuerza"].ToString());
                        Patru.CadUniCodigo = Convert.ToInt32(fila["CadUniCodigo"].ToString());
                        Patru.Codigo = Convert.ToInt32(fila["Codigo"].ToString());
                        //Patru.IdClaseTurno = fila["IdClaseTurno"].ToString();
                        Patru.DescPatrulla = fila["DescPatrulla"].ToString();
                        Patru.ClaseTurno = fila["ClaseTurno"].ToString();
                        Patru.Vigente = fila["Vigente"].ToString();
                        var Vige = fila["Vigente"].ToString();
                        if (Vige == "S")
                        {
                            Patru.Activos = true;
                        }
                        else
                        {
                            Patru.Activos = false;
                        }
                        Patru.FechaDesde = Convert.ToDateTime(fila["FechaDesde"].ToString());
                        Patru.FechaHastaStr = fila["FechaHasta"].ToString();
                        if (Patru.FechaHastaStr != "")
                            Patru.FechaHasta = Convert.ToDateTime(Patru.FechaHastaStr);
                        else
                            Patru.FechaHasta = null;
                        Patru.Observacion = fila["Observacion"].ToString();
                        Patru.UsuaTransaccion = fila["UsuaTransaccion"].ToString();
                        Patru.SitioGrabacion = Convert.ToInt32(fila["SitioGrabacion"].ToString());
                        Patru.IdRadio = fila["IdRadio"].ToString();
                        Patru.CuadranteId = Convert.ToInt32(fila["CuadranteId"].ToString());
                        Patru.Cuadrante = fila["Cuadrante"].ToString();
                        retorno.Add(Patru);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetConsCuadrante:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }

        public MdCadPatrulla F_GetCuadranteId(int V_IntUnidadFuerza, int V_IntUnidadCodigo, int V_IntCodigo)
        {
            MdCadPatrulla retorno = new MdCadPatrulla();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadFuerza", OracleDbType.Int32)).Value = V_IntUnidadFuerza;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = V_IntUnidadCodigo;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = V_IntCodigo;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.F_GetCuadranteId";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        retorno = new MdCadPatrulla
                        {
                            CadUniFuerza = Convert.ToInt32(fila["CadUniFuerza"].ToString()),
                            CadUniCodigo = Convert.ToInt32(fila["CadUniCodigo"].ToString()),
                            Codigo = Convert.ToInt32(fila["Codigo"].ToString()),
                            IdClaseTurno = fila["IdClaseTurno"].ToString(),
                            DescPatrulla = fila["DescPatrulla"].ToString(),
                            ClaseTurno = fila["ClaseTurno"].ToString(),
                            Vigente = fila["Vigente"].ToString(),
                            FechaDesde = Convert.ToDateTime(fila["FechaDesde"].ToString()),
                            FechaHastaStr = fila["FechaHasta"].ToString(),
                            Observacion = fila["Observacion"].ToString(),
                            UsuaTransaccion = fila["UsuaTransaccion"].ToString(),
                            SitioGrabacion = Convert.ToInt32(fila["SitioGrabacion"].ToString()),
                            IdRadio = fila["IdRadio"].ToString(),
                            CuadranteId = Convert.ToInt32(fila["CuadranteId"].ToString()),
                            Cuadrante = fila["Cuadrante"].ToString(),
                        };

                        if (retorno.FechaHastaStr != "")
                            retorno.FechaHasta = Convert.ToDateTime(retorno.FechaHastaStr);
                        else
                            retorno.FechaHasta = null;
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetCuadranteId:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }

        //public List<MdCadPatrulla> F_GetCuadranteId(int V_IntUnidadFuerza, int V_IntUnidadCodigo, int V_IntCodigo)
        //{
        //    List<MdCadPatrulla> retorno = new List<MdCadPatrulla>();
        //    ConDBOracle cnOracle = new ConDBOracle("strConexionMETUN");
        //    OracleCommand objCommand = new OracleCommand();
        //    DataTable resultado = new DataTable();

        //    try
        //    {
        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
        //        objCommand.Parameters.Add(new OracleParameter("p_IntUnidadFuerza", OracleDbType.Int32)).Value = V_IntUnidadFuerza;
        //        objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = V_IntUnidadCodigo;
        //        objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = V_IntCodigo;

        //        objCommand.Connection = cnOracle.Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "P_INTEGRACIONSIVICCSECAD.F_GetCuadranteId";

        //        if (cnOracle.Conectar())
        //        {
        //            resultado.Load(objCommand.ExecuteReader());
        //            cnOracle.Desconectar();
        //        }

        //        if (resultado.Rows.Count > 0)
        //        {
        //            foreach (DataRow fila in resultado.Rows)
        //            {
        //                MdCadPatrulla Patru = new MdCadPatrulla();
        //                Patru.CadUniFuerza = Convert.ToInt32(fila["CadUniFuerza"].ToString());
        //                Patru.CadUniCodigo = Convert.ToInt32(fila["CadUniCodigo"].ToString());
        //                Patru.Codigo = Convert.ToInt32(fila["Codigo"].ToString());
        //                Patru.IdClaseTurno = fila["IdClaseTurno"].ToString();
        //                Patru.DescPatrulla = fila["DescPatrulla"].ToString();
        //                Patru.ClaseTurno = fila["ClaseTurno"].ToString();
        //                Patru.Vigente = fila["Vigente"].ToString();
        //                var Vige = fila["Vigente"].ToString();
        //                if (Vige == "S")
        //                {
        //                    Patru.Activos = true;
        //                }
        //                else
        //                {
        //                    Patru.Activos = false;
        //                }
        //                Patru.FechaDesde = Convert.ToDateTime(fila["FechaDesde"].ToString());
        //                Patru.FechaHastaStr = fila["FechaHasta"].ToString();
        //                if (Patru.FechaHastaStr != "")
        //                    Patru.FechaHasta = Convert.ToDateTime(Patru.FechaHastaStr);
        //                else
        //                    Patru.FechaHasta = null;
        //                Patru.Observacion = fila["Observacion"].ToString();
        //                Patru.UsuaTransaccion = fila["UsuaTransaccion"].ToString();
        //                Patru.SitioGrabacion = Convert.ToInt32(fila["SitioGrabacion"].ToString());
        //                Patru.IdRadio = fila["IdRadio"].ToString();
        //                Patru.CuadranteId = Convert.ToInt32(fila["CuadranteId"].ToString());
        //                Patru.Cuadrante = fila["Cuadrante"].ToString();
        //                retorno.Add(Patru);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        cnOracle.Desconectar();
        //        LogException.Nivel = LogException.TipoMensaje.M_ERROR;
        //        LogException.err("Error Ejecutando F_GetCuadranteId:", ex);
        //    }
        //    finally
        //    {
        //        cnOracle.Desconectar();
        //        if (objCommand != null) { objCommand.Dispose(); }
        //    }

        //    return retorno;
        //}

        public int UpdateCuadrante(MdCadPatrulla ObjP)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            int Resultado = 0;

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadFuerza", OracleDbType.Int32)).Value = ObjP.CadUniFuerza;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = ObjP.CadUniCodigo;
                objCommand.Parameters.Add(new OracleParameter("p_IntUnidadCodigo", OracleDbType.Int32)).Value = ObjP.Codigo;
                objCommand.Parameters.Add(new OracleParameter("p_DescPatrulla", OracleDbType.Varchar2, 30)).Value = ObjP.DescPatrulla;
                objCommand.Parameters.Add(new OracleParameter("p_ClaseTurno", OracleDbType.Varchar2, 2)).Value = ObjP.ClaseTurno;
                objCommand.Parameters.Add(new OracleParameter("p_Vigente", OracleDbType.Varchar2, 1)).Value = ObjP.Vigente;
                objCommand.Parameters.Add(new OracleParameter("p_FechaDesde", OracleDbType.Date)).Value = ObjP.FechaDesde;
                objCommand.Parameters.Add(new OracleParameter("p_FechaHasta", OracleDbType.Date)).Value = ObjP.FechaHasta;
                objCommand.Parameters.Add(new OracleParameter("p_Observacion", OracleDbType.Varchar2, 1000)).Value = ObjP.Observacion;
                objCommand.Parameters.Add(new OracleParameter("p_UsuaTransaccion", OracleDbType.Varchar2, 30)).Value = ObjP.UsuaTransaccion;
                objCommand.Parameters.Add(new OracleParameter("p_SitioGrabacion", OracleDbType.Int32)).Value = ObjP.SitioGrabacion;
                objCommand.Parameters.Add(new OracleParameter("p_IdRadio", OracleDbType.Varchar2, 20)).Value = ObjP.IdRadio;
                objCommand.Parameters.Add(new OracleParameter("p_CuadranteId", OracleDbType.Int32)).Value = ObjP.CuadranteId;
                //objCommand.Parameters.Add(new OracleParameter("p_Cuadrante", OracleDbType.Varchar2, 25)).Value = ObjP.Cuadrante;

                objCommand.Parameters.Add(new OracleParameter("P_RESULTADO", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadintegminutasecadsivicc.UpdateCuadrante";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Resultado = int.Parse(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error al Eliminar : UpdateCuadrante ", ex);
                objCommand = null;
                objCommand.Dispose();
                Resultado = 0;
                return Resultado;
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                objCommand = null;
                ConexionDB.Dispose();
            }
            return Resultado;
        }
    }
}
