using Oracle.ManagedDataAccess.Client;
using SecadDatos.Context;
using SecadModelos.Areas.MinutaSivicc;
using SecadModelos.Visor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadDatos.Areas.MinutaSivicc
{
    public class DbMinuta
    {
        public List<MdMinutaEmpl> F_GetMinuta(int V_UnidadID)
        {
            List<MdMinutaEmpl> retorno = new List<MdMinutaEmpl>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_UnidadID", OracleDbType.Int32)).Value = V_UnidadID;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetMinuta";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdMinutaEmpl ObjR = new MdMinutaEmpl
                        {
                            MinutaEmplId = Convert.ToInt64(fila["MinutaEmplId"].ToString()),
                            Funcionario = fila["Funcionario"].ToString(),
                            Unidad = fila["Unidad"].ToString(),
                            Turno = fila["Turno"].ToString(),
                            TipoMinuta = fila["TipoMinuta"].ToString(),
                            Inicio = fila["Inicio"].ToString(),
                            Fin = fila["Fin"].ToString(),
                            Fecha = fila["Fecha"].ToString(),
                            CodigoMinuta = fila["CodigoMinuta"].ToString(),
                            Cuadrante = fila["Cuadrante"].ToString(),
                            UndeConsecutivo = Convert.ToInt32(fila["UndeConsecutivo"].ToString()),
                            UndeCai = Convert.ToInt32(fila["UndeCai"].ToString()),
                            Arma = Convert.ToInt32(fila["Arma"].ToString()),
                            Veh = Convert.ToInt32(fila["Veh"].ToString()),
                            Comunica = Convert.ToInt32(fila["Comunica"].ToString())
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetMinuta:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        public List<MdMinutaEmplNew> F_GetMinutaNew(int V_UnidadID, string vFechaInicial, string vfechaFinal)
        {
            List<MdMinutaEmplNew> retorno = new List<MdMinutaEmplNew>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_UnidadID", OracleDbType.Int32)).Value = V_UnidadID;
                objCommand.Parameters.Add(new OracleParameter("P_Fecha_Inicio", OracleDbType.Varchar2)).Value = vFechaInicial;
                objCommand.Parameters.Add(new OracleParameter("P_Fecha_Fin", OracleDbType.Varchar2)).Value = vfechaFinal;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetMinutaNew";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdMinutaEmplNew ObjR = new MdMinutaEmplNew
                        {
                            MinutaEmplId = Convert.ToInt64(fila["MinutaEmplId"].ToString()),
                            Funcionario = fila["Funcionario"].ToString(),
                            Turno = fila["Turno"].ToString(),
                            Unidad = fila["Unidad"].ToString(),
                            TipoMinuta = fila["TipoMinuta"].ToString(),
                            Inicio = fila["Inicio"].ToString(),
                            Fin = fila["Fin"].ToString(),
                            Cuadrante = fila["Cuadrante"].ToString(),
                            Arma = Convert.ToInt32(fila["Arma"].ToString()),
                            Veh = Convert.ToInt32(fila["Veh"].ToString()),
                            Comunica = Convert.ToInt32(fila["Comunica"].ToString()),
                            Asistencia = fila["Asistencia"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetMinutaNew:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        public List<MdEstaciones> F_GetUnidades()
        {
            List<MdEstaciones> retorno = new List<MdEstaciones>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetUnidades";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdEstaciones ObjR = new MdEstaciones
                        {
                            Id = Convert.ToInt32(fila["ID"].ToString()),
                            Descripcion = fila["Descripcion"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetUnidades:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        public List<MdEstaciones> F_GetUnidades2(int V_CadturnSitiograba, int V_CadturnTurno)
        {
            List<MdEstaciones> retorno = new List<MdEstaciones>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("V_CadturnSitiograba", OracleDbType.Int32)).Value = V_CadturnSitiograba;
                objCommand.Parameters.Add(new OracleParameter("V_CadturnTurno", OracleDbType.Int32)).Value = V_CadturnTurno;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetUnidadesNew";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdEstaciones ObjR = new MdEstaciones
                        {
                            Id = Convert.ToInt32(fila["ID"].ToString()),
                            Descripcion = fila["Descripcion"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetMinutaNew:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        ///Obtener Armas
        public List<MdArmasEmpl> F_GetArmasEmpl(int P_MinEmplId)
        {
            List<MdArmasEmpl> retorno = new List<MdArmasEmpl>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("P_MinEmplId", OracleDbType.Int32)).Value = P_MinEmplId;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetArmasEmpl";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdArmasEmpl ObjR = new MdArmasEmpl
                        {
                            IdArmamento = Convert.ToInt64(fila["id_armamento"].ToString()),
                            TipoElemento = fila["tipo_elemento"].ToString(),
                            Marca = fila["marca"].ToString(),
                            Calibre = fila["calibre"].ToString(),
                            Serie = fila["serie"].ToString(),
                            Estado = fila["estado"].ToString(),
                            IdInventario = Convert.ToInt64(fila["id_inventario"].ToString())
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetArmasEmpl:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        ///Obtener Comunicaciones
        public List<MdComunicaEmpl> F_GetComunicaEmpl(int P_MinEmplId)
        {
            List<MdComunicaEmpl> retorno = new List<MdComunicaEmpl>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("P_MinEmplId", OracleDbType.Int32)).Value = P_MinEmplId;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetComunicaEmpl";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdComunicaEmpl ObjR = new MdComunicaEmpl
                        {
                            TipoElemento = fila["tipo_elemento"].ToString(),
                            Marca = fila["marca"].ToString(),
                            Modelo = fila["modelo"].ToString(),
                            Serie = fila["serie"].ToString(),
                            Estado = fila["estado"].ToString(),
                            IdInventario = Convert.ToInt64(fila["id_inventario"].ToString())
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetComunicaEmpl:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        ///Obtener Vehiculos Asignados
        public List<MdVehiEmpl> F_GetVehiEmpl(int P_MinEmplId)
        {
            List<MdVehiEmpl> retorno = new List<MdVehiEmpl>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("P_MinEmplId", OracleDbType.Int32)).Value = P_MinEmplId;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECAD_MINUTAS.F_GetVehiEmpl";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdVehiEmpl ObjR = new MdVehiEmpl
                        {
                            TipoElemento = fila["tipo_elemento"].ToString(),
                            Marca = fila["marca"].ToString(),
                            Modelo = fila["modelo"].ToString(),
                            Sigla = fila["sigla"].ToString(),
                            Placa = fila["placa"].ToString(),
                            Estado = fila["estado"].ToString(),
                            IdInventario = Convert.ToInt64(fila["id_inventario"].ToString())
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando F_GetVehiEmpl:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }

        public List<MdCoordenadas> F_GetCoordenada(int PSitioGraba, Int64 PMedioConsecutivo)
        {
            List<MdCoordenadas> retorno = new List<MdCoordenadas>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("PSITIOGRABA", OracleDbType.Int32)).Value = PSitioGraba;
                objCommand.Parameters.Add(new OracleParameter("PMEDISPCONSECUTIVO", OracleDbType.Int64)).Value = PMedioConsecutivo;
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                objCommand.Connection = cnOracle.Conexion;

                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.P_UBICACIONCUADRANTES";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdCoordenadas ObjR = new MdCoordenadas
                        {
                            PSitioGraba = Convert.ToInt32(fila["SITIO_GRABA"].ToString()),
                            PMedioConsecutivo = Convert.ToInt64(fila["CONSECUTIVO"].ToString()),
                            PIdCudrante = Convert.ToInt32(fila["CUADRANTE_ID"].ToString()),
                            PCuadrante = fila["CUADRANTE"].ToString(),
                            PLatitud = fila["LATITUD"].ToString(),
                            PLongitud = fila["LONGITUD"].ToString(),
                            PFechaGPS = Convert.ToDateTime(fila["FECHA"].ToString()),
                            PEnCuadrante = fila["ENCUADRANTE"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando P_UBICACIONCUADRANTE:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }

        public List<MdCuadrantes> F_GetCuadrantes()
        {
            List<MdCuadrantes> retorno = new List<MdCuadrantes>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetCuadrantes";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdCuadrantes ObjR = new MdCuadrantes
                        {
                            CuadranteId = Convert.ToInt32(fila["CuadranteId"].ToString()),
                            CodigoCuadrante = fila["CodigoCuadrante"].ToString(),                            
                            Estacion = fila["Estacion"].ToString(),
                            Dependencia = fila["Dependencia"].ToString()                            
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                Context.LogException.Nivel = Context.LogException.TipoMensaje.M_ERROR;
                Context.LogException.err("Error Ejecutando F_GetCuadrantes:", ex);
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

        public List<MdReferencias> F_GetClaseTurno()
        {
            List<MdReferencias> retorno = new List<MdReferencias>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetClaseTurno";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdReferencias ObjR = new MdReferencias
                        {
                            ReferenciaId = fila["ReferenciaId"].ToString(),
                            Descripcion = fila["Descripcion"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADINTEGMINUTASECADSIVICC.F_GetClaseTurno:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }


        public List<MdReferencias> F_GetModalidad()
        {
            List<MdReferencias> retorno = new List<MdReferencias>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetModalidad";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdReferencias ObjR = new MdReferencias
                        {
                            ReferenciaId = fila["ReferenciaId"].ToString(),
                            Descripcion = fila["Descripcion"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADINTEGMINUTASECADSIVICC.F_GetModalidad:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }

        public List<MdReferencias> F_GetConductas()
        {
            List<MdReferencias> retorno = new List<MdReferencias>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetConductas";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdReferencias ObjR = new MdReferencias
                        {
                            ReferenciaId = fila["ReferenciaId"].ToString(),
                            Descripcion = fila["Descripcion"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADINTEGMINUTASECADSIVICC.F_GetConductas:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }

        public List<MdReferencias> F_GetTipoArma()
        {
            List<MdReferencias> retorno = new List<MdReferencias>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetTipoArma";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdReferencias ObjR = new MdReferencias
                        {
                            ReferenciaId = fila["ReferenciaId"].ToString(),
                            Descripcion = fila["Descripcion"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADINTEGMINUTASECADSIVICC.F_GetTipoArma:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }

        public List<MdReferencias> F_GetTipoMoviliza()
        {
            List<MdReferencias> retorno = new List<MdReferencias>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.F_GetTipoMoviliza";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdReferencias ObjR = new MdReferencias
                        {
                            ReferenciaId = fila["ReferenciaId"].ToString(),
                            Descripcion = fila["Descripcion"].ToString(),
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando PK_CADINTEGMINUTASECADSIVICC.F_GetTipoMoviliza:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                objCommand.Dispose();
            }
            return retorno;
        }

        public decimal Ins_Coordenadas(Int32 SitioGraba, Int64 IdLlamada, double V_Latitud, double V_Longitud, string V_Modalidad, string V_Conducta, string V_TipoArma, string V_TipoMoviliza)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            decimal Resultado = 0;

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("P_SitioGraba", OracleDbType.Int32)).Value = SitioGraba;
                objCommand.Parameters.Add(new OracleParameter("P_NumeLlamada", OracleDbType.Int64)).Value = IdLlamada;
                objCommand.Parameters.Add(new OracleParameter("P_Latitud", OracleDbType.Double)).Value = V_Latitud;
                objCommand.Parameters.Add(new OracleParameter("P_Longitud", OracleDbType.Double)).Value = V_Longitud;
                objCommand.Parameters.Add(new OracleParameter("P_Modalidad", OracleDbType.Varchar2, 255)).Value = V_Modalidad;
                objCommand.Parameters.Add(new OracleParameter("P_Conducta", OracleDbType.Varchar2, 255)).Value = V_Conducta;
                objCommand.Parameters.Add(new OracleParameter("P_TipoArma", OracleDbType.Varchar2, 255)).Value = V_TipoArma;
                objCommand.Parameters.Add(new OracleParameter("P_TipoMoviliza", OracleDbType.Varchar2, 255)).Value = V_TipoMoviliza;

                objCommand.Parameters.Add(new OracleParameter("P_RESULTADO", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.Ins_Coordenadas";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Resultado = decimal.Parse(objCommand.Parameters["P_RESULTADO"].Value.ToString());
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error al Insertar : Ins_Coordenadas ", ex);
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

        public int update_evento(Int32 P_SITIO_GRABA, Int32 P_EVENTO, string P_CADCODI_FINAL, string P_CADCLCI_CODIGO)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionSECAD");
            OracleCommand objCommand = new OracleCommand();
            int Resultado = 0;

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("P_SITIO_GRABA", OracleDbType.Int32)).Value = P_SITIO_GRABA;
                objCommand.Parameters.Add(new OracleParameter("P_EVENTO", OracleDbType.Int32)).Value = P_EVENTO;
                objCommand.Parameters.Add(new OracleParameter("P_CADCODI_FINAL", OracleDbType.Varchar2, 255)).Value = P_CADCODI_FINAL;
                objCommand.Parameters.Add(new OracleParameter("P_CADCLCI_CODIGO", OracleDbType.Varchar2, 255)).Value = P_CADCLCI_CODIGO;
                
                objCommand.Parameters.Add(new OracleParameter("P_SALIDA", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_CADINTEGMINUTASECADSIVICC.P_UPDATE_EVENTO";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Resultado = int.Parse(objCommand.Parameters["P_SALIDA"].Value.ToString());
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error al Insertar : Ins_Coordenadas ", ex);
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
