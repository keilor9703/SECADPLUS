using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using SecadDatos.Context;
using SecadModelos.Areas.MinutaSivicc;
using SecadModelos.Areas.Recepcion;
using SecadModelos.Visor;

namespace SecadDatos.Areas.Recepcion
{
    public class DbRecepcion
    {
        public List<MdRecepcion.MdrecepcionM> F_GetDatosXCrimeCr(string V_Abonado, string V_Fecha, Int64 V_NumeroSecad)
        {
            List<MdRecepcion.MdrecepcionM> retornoM = new List<MdRecepcion.MdrecepcionM>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionXcrime");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();

                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_abonado", OracleDbType.Varchar2, 255)).Value = V_Abonado;
                objCommand.Parameters.Add(new OracleParameter("p_fecha", OracleDbType.Varchar2, 255)).Value = V_Fecha;                
                objCommand.Parameters.Add(new OracleParameter("p_numerosecad", OracleDbType.Int64)).Value = V_NumeroSecad;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadxcrime.F_GETDATOSXCRIME_CR";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdRecepcion.MdrecepcionM ObjR = new MdRecepcion.MdrecepcionM
                        {
                            SitioGrabacion = Convert.ToInt32(fila["sitiograbacion"].ToString()),
                            Idreg = Convert.ToInt32(fila["idreg"].ToString()),
                            FechaLlamada = fila["fechallamada"].ToString(),
                            TipoRegistro = fila["tiporegistro"].ToString(),
                            Calidad = fila["calidad"].ToString(),
                            CodiMotivo = fila["codimotivo"].ToString(),
                            Prioridad = fila["prioridad"].ToString(),
                            IdRegxCrime = Convert.ToInt32(fila["idregxcrime"].ToString()),
                            CodiXcrime = fila["codixcrime"].ToString(),
                            DescXcrime = fila["descxcrime"].ToString(),
                            TipoXcrime = fila["tipoxcrime"].ToString(),
                            Transcripcion = fila["transcripcion"].ToString(),
                            FechaIngbd = fila["fechaingbd"].ToString(),
                            CadpediSitioGraba = Convert.ToInt32(fila["SitioGraba"].ToString()),
                            CadpediNumeLlamada = Convert.ToInt64(fila["NumeSecad"].ToString()),
                            Observacion = fila["observacion"].ToString()
                        };
                        retornoM.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando pk_cadxcrime.f_getdatosxcrime_cr", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return retornoM;
        }
        public List<MdRecepcion.MdRecepcionD> F_GetDatosXCrimeCodigos(int V_SitioGrabacion, int V_IdregAnalisis)
        {
            List<MdRecepcion.MdRecepcionD> retornoD = new List<MdRecepcion.MdRecepcionD>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionXcrime");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();

                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_sitiograbacion", OracleDbType.Int32)).Value = V_SitioGrabacion;
                objCommand.Parameters.Add(new OracleParameter("p_idreganalisis", OracleDbType.Int32)).Value = V_IdregAnalisis;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadxcrime.F_GETDATOSXCRIMECODIGOS_CR";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdRecepcion.MdRecepcionD ObjR = new MdRecepcion.MdRecepcionD
                        {
                            SitioGrabacion = Convert.ToInt32(fila["SITIOGRABACION"].ToString()),
                            IdReganalisis = Convert.ToInt32(fila["IdReganalisis"].ToString()),
                            CodiMotivo = fila["CODIMOTIVO"].ToString(),
                            DescripcionCodigo = fila["DescripcionCodigo"].ToString(),
                            Orden = Convert.ToInt32(fila["Orden"].ToString())
                        };
                        retornoD.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando pk_cadxcrime.f_getdatosxcrime_cr", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }
            return retornoD;
        }


        public int Prueba(Int32 v_lineaid)
        {
            List<MdRecepcion.MdrecepcionM> retornoM = new List<MdRecepcion.MdrecepcionM>();
            int Res;
            ConDBOracle cnOracle = new ConDBOracle("strConexionXcrime");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();

                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("v_lineaid", OracleDbType.Int32)).Value = v_lineaid;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "pk_cadxcrime.f_getactaequipo";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                    //    MdRecepcion.MdrecepcionM ObjR = new MdRecepcion.MdrecepcionM
                    //    {
                    //        SitioGrabacion = Convert.ToInt32(fila["sitiograbacion"].ToString()),
                    //        Idreg = Convert.ToInt32(fila["idreg"].ToString()),
                    //        FechaLlamada = fila["fechallamada"].ToString(),
                    //        TipoRegistro = fila["tiporegistro"].ToString(),
                    //        Calidad = fila["calidad"].ToString(),
                    //        CodiMotivo = fila["codimotivo"].ToString(),
                    //        Prioridad = fila["prioridad"].ToString(),
                    //        IdRegxCrime = Convert.ToInt32(fila["idregxcrime"].ToString()),
                    //        CodiXcrime = fila["codixcrime"].ToString(),
                    //        DescXcrime = fila["descxcrime"].ToString(),
                    //        TipoXcrime = fila["tipoxcrime"].ToString(),
                    //        Transcripcion = fila["transcripcion"].ToString(),
                    //        FechaIngbd = fila["fechaingbd"].ToString(),
                    //        CadpediSitioGraba = Convert.ToInt32(fila["SitioGraba"].ToString()),
                    //        CadpediNumeLlamada = Convert.ToInt64(fila["NumeSecad"].ToString()),
                    //        Observacion = fila["observacion"].ToString()
                    //    };
                    //    retornoM.Add(ObjR);
                    Res = Convert.ToInt32(fila["Numero"].ToString());
                    }
                    
                }
                else
                {
                    Res = 0;
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando pk_cadxcrime.f_getdatosxcrime_cr", ex);
                Res = 0;
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
                Res = 0;
            }
            return Res;
        }

    }
}
