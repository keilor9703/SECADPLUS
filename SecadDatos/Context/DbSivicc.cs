using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using SecadModelos.Helpers;
using SecadModelos.Visor;

namespace SecadDatos.Context
{
    public class DbSivicc
    {
        public int DesacAlamarmas(int p_id, string p_user)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionRPC");
            OracleCommand objCommand = new OracleCommand();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = p_id;
                objCommand.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, 255)).Value = p_user;


                objCommand.Parameters.Add(new OracleParameter("P_RESULTADO", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "rpc_pk.DesactivarAlarma";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    ConexionDB.Desconectar();
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                objCommand = null;
                return 0;
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                objCommand = null;
            }
            return 1;
        }
        public string EnviarCasoSecad(MdPedidos obj)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionSecad");
            OracleCommand objCommand = new OracleCommand();
            string Numero_llamada = "0";

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("p_hora_caso", OracleDbType.Date)).Value = obj.Hora_caso;
                objCommand.Parameters.Add(new OracleParameter("p_disp_telefonico", OracleDbType.Varchar2, 255)).Value = obj.Disp_telefonico;
                objCommand.Parameters.Add(new OracleParameter("p_celda_marcacion", OracleDbType.Varchar2, 255)).Value = obj.Celda_marcacion;
                objCommand.Parameters.Add(new OracleParameter("p_nume_telefono", OracleDbType.Int64)).Value = obj.Nume_telefono;
                objCommand.Parameters.Add(new OracleParameter("p_prop_telefono", OracleDbType.Varchar2, 255)).Value = obj.Prop_telefono;
                objCommand.Parameters.Add(new OracleParameter("p_dire_telefono", OracleDbType.Varchar2, 255)).Value = obj.Dire_telefono;
                objCommand.Parameters.Add(new OracleParameter("p_luge_telefono", OracleDbType.Int32)).Value = obj.Luge_telefono;
                objCommand.Parameters.Add(new OracleParameter("p_barr_telefono", OracleDbType.Varchar2, 255)).Value = obj.Barr_telefono;
                objCommand.Parameters.Add(new OracleParameter("p_tipo_pedido", OracleDbType.Varchar2, 255)).Value = obj.Tipo_pedido;
                objCommand.Parameters.Add(new OracleParameter("p_cali_pedido", OracleDbType.Varchar2, 255)).Value = obj.Cali_pedido;
                objCommand.Parameters.Add(new OracleParameter("p_tipo_informacion", OracleDbType.Varchar2, 255)).Value = obj.Tipo_informacion;
                objCommand.Parameters.Add(new OracleParameter("p_codigo_cooperante", OracleDbType.Int32)).Value = obj.Codigo_cooperante;
                objCommand.Parameters.Add(new OracleParameter("p_nomb_llamante", OracleDbType.Varchar2, 255)).Value = obj.Nomb_llamante;
                objCommand.Parameters.Add(new OracleParameter("p_dire_llamante", OracleDbType.Varchar2, 255)).Value = obj.Dire_llamante;
                objCommand.Parameters.Add(new OracleParameter("p_luge_codigo", OracleDbType.Int32)).Value = obj.Luge_codigo;
                objCommand.Parameters.Add(new OracleParameter("p_dire_caso", OracleDbType.Varchar2, 255)).Value = obj.Dire_caso;
                objCommand.Parameters.Add(new OracleParameter("p_puca_codigo", OracleDbType.Int32)).Value = obj.Puca_codigo;
                objCommand.Parameters.Add(new OracleParameter("p_codi_barrio", OracleDbType.Int32)).Value = obj.Codi_barrio;
                objCommand.Parameters.Add(new OracleParameter("p_luge_barrio", OracleDbType.Int32)).Value = obj.Luge_barrio;
                objCommand.Parameters.Add(new OracleParameter("p_latitud_caso", OracleDbType.Varchar2, 255)).Value = obj.Latitud_caso;
                objCommand.Parameters.Add(new OracleParameter("p_longitud_caso", OracleDbType.Varchar2, 255)).Value = obj.Longitud_caso;
                objCommand.Parameters.Add(new OracleParameter("p_comentario", OracleDbType.Varchar2, 255)).Value = obj.Comentario;
                objCommand.Parameters.Add(new OracleParameter("p_codi_pedido", OracleDbType.Varchar2, 255)).Value = obj.Codi_pedido;
                objCommand.Parameters.Add(new OracleParameter("p_importancia", OracleDbType.Varchar2, 255)).Value = obj.Importancia;
                objCommand.Parameters.Add(new OracleParameter("p_prioridad", OracleDbType.Varchar2, 255)).Value = obj.Prioridad;
                objCommand.Parameters.Add(new OracleParameter("p_estado", OracleDbType.Varchar2, 255)).Value = obj.Estado;
                objCommand.Parameters.Add(new OracleParameter("p_enviar", OracleDbType.Varchar2, 255)).Value = obj.Enviar;
                objCommand.Parameters.Add(new OracleParameter("p_cadusua_usuario", OracleDbType.Varchar2, 255)).Value = obj.Cadusua_usuario;
                objCommand.Parameters.Add(new OracleParameter("p_cadpoli_cedu_empleado", OracleDbType.Varchar2, 255)).Value = obj.Cadpoli_cedu_empleado;
                objCommand.Parameters.Add(new OracleParameter("p_cadpedi_sitio_graba", OracleDbType.Int32)).Value = obj.Cadpedi_sitio_graba;
                objCommand.Parameters.Add(new OracleParameter("p_cadpedi_nume_llamada", OracleDbType.Int64)).Value = obj.Cadpedi_nume_llamada;
                objCommand.Parameters.Add(new OracleParameter("p_total_canales", OracleDbType.Varchar2, 255)).Value = obj.Total_canales;
                objCommand.Parameters.Add(new OracleParameter("p_tipo_genero", OracleDbType.Varchar2, 255)).Value = obj.Tipo_genero;

                objCommand.Parameters.Add(new OracleParameter("p_sitio_caso", OracleDbType.Int32)).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add(new OracleParameter("p_nume_llamada", OracleDbType.Int64)).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add(new OracleParameter("p_cod_salida", OracleDbType.Varchar2, 255)).Direction = ParameterDirection.Output;
                objCommand.Parameters.Add(new OracleParameter("p_desc_salida", OracleDbType.Varchar2, 255)).Direction = ParameterDirection.Output;


                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_SECADRPCP.P_INS_RPC_VINCULANTE";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Numero_llamada = Convert.ToString(objCommand.Parameters["p_nume_llamada"].Value);
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Numero_llamada;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                objCommand = null;
                objCommand.Dispose();
                Numero_llamada = "0";
                return Numero_llamada;
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                objCommand = null;
            }
            return Numero_llamada;
        }
        public string UdpNumeroCasoSecad(int p_id, Int64 p_caso, string p_dto, string p_municipio, string p_ciudad, string P_Barrio)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionRPC");
            OracleCommand objCommand = new OracleCommand();
            string Resultado = "0";

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = p_id;
                objCommand.Parameters.Add(new OracleParameter("p_caso", OracleDbType.Int64)).Value = p_caso;

                objCommand.Parameters.Add(new OracleParameter("p_postal", OracleDbType.Int64)).Value = p_caso;
                objCommand.Parameters.Add(new OracleParameter("p_dto", OracleDbType.Varchar2, 255)).Value = p_dto;
                objCommand.Parameters.Add(new OracleParameter("p_municipio", OracleDbType.Varchar2)).Value = p_municipio;
                objCommand.Parameters.Add(new OracleParameter("p_ciudad", OracleDbType.Varchar2, 255)).Value = p_ciudad;
                objCommand.Parameters.Add(new OracleParameter("p_barrio", OracleDbType.Varchar2, 255)).Value = P_Barrio;

                objCommand.Parameters.Add(new OracleParameter("P_RESULTADO", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "rpc_pk.NumCasoSecadAlarm";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Resultado = Convert.ToString(objCommand.Parameters["P_RESULTADO"].Value);
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                objCommand = null;
                objCommand.Dispose();
                Resultado = "0";
                return Resultado;
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                objCommand = null;
            }
            return Resultado;
        }
        public int F_CodigoSecad(int p_postal)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionRPC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();
            int CodigoSecad = 0;
            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_Id", OracleDbType.Int32)).Value = p_postal;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "rpc_pk.f_cons_lugesecad";

                if (ConexionDB.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    ConexionDB.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow row in resultado.Rows)
                    {
                        CodigoSecad = Convert.ToInt32(row["codigo_secad"].ToString()); ;
                    }
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando f_ListarDominio:", ex);
                CodigoSecad = 0;
            }
            finally
            {
                ConexionDB.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return CodigoSecad;
        }
        public List<MdAlarma> CosultaAlarmas(int v_id)
        {

            ConDBOracle ConexionDB = new ConDBOracle("strConexionRPC");

            List<MdAlarma> resultados = new List<MdAlarma>();

            Respuesta<List<MdAlarma>> resp = new Respuesta<List<MdAlarma>>();
            DataTable resultado = new DataTable();
            OracleCommand objCommand = new OracleCommand();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_Id", OracleDbType.Varchar2, 255)).Value = v_id;

                objCommand.CommandText = "rpc_pk.f_cons_rpc_alarmas";
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.Connection = ConexionDB.Conexion;

                if (ConexionDB.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    ConexionDB.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    resp.Data = new List<MdAlarma>();
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdAlarma ObjR = new MdAlarma();
                        ObjR.ID_ALARMA = Convert.ToInt32(fila["ID_ALARMA"].ToString());
                        ObjR.ID_TIPO = Convert.ToInt32(fila["ID_TIPO"].ToString());
                        ObjR.LATITUD = Convert.ToDecimal(fila["LATITUD"].ToString());
                        ObjR.LONGITUD = Convert.ToDecimal(fila["LONGITUD"].ToString());
                        ObjR.FECHA_CREACION = Convert.ToDateTime(fila["FECHA_CREACION"].ToString());
                        ObjR.ID_RPC_VINCULADO = Convert.ToInt32(fila["ID_RPC_VINCULADO"].ToString());
                        ObjR.CIUDADANO = fila["CIUDADANO"].ToString();
                        ObjR.DIRECCION_CIUDADANO = fila["DIRECCION_CIUDADANO"].ToString(); ;
                        ObjR.DTO_CIUDADANO = fila["DTO_CIUDADANO"].ToString();
                        ObjR.LUGAR_CIUDADANO = fila["LUGAR_CIUDADANO"].ToString();
                        ObjR.TELEFONO = fila["TELEFONO"].ToString();
                        ObjR.CELULAR = Convert.ToInt64(fila["CELULAR"].ToString());
                        ObjR.SEGMENTO = fila["SEGMENTO"].ToString();
                        ObjR.ID_MUNICIPIO = Convert.ToInt32(fila["ID_MUNICIPIO"].ToString());

                        resultados.Add(ObjR);
                    }
                    resp.Iserror = "false";
                    resp.CodigoExito = 1;
                }
                else
                {
                    resp.Iserror = "true";
                    resp.Codigoerror = 2;
                    resp.Mensaje = "No se encontraron datos";
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                resp.Iserror = "true";
                resp.Codigoerror = 3;
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando la consulta de dominios(): [ID NroSimcard:]", ex);
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                if (resultado.Rows.Count > 0)
                {
                    resp.CodigoExito = 1;
                    resp.Codigoerror = 0;
                }
                else
                {
                    resp.CodigoExito = 0;
                    resp.Codigoerror = 1;
                    resp.Iserror = "true";
                }
            }
            return resultados;
        }
        public List<MdCapas> F_GetCapaDinamica()
        {
            List<MdCapas> retorno = new List<MdCapas>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSIVICC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "usr_mnvcc.pk_secad.f_getcapasdinamicas";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdCapas ObjR = new MdCapas
                        {
                            CapaDinamicaID = Convert.ToInt32(fila["CAPA_DINAMICA_ID"].ToString()),
                            Nombre = fila["NOMBRE"].ToString(),
                            CapaActiva = Convert.ToInt32(fila["CAPA_ACTIVA"].ToString()),
                            Descripcion = fila["Descripcion"].ToString(),
                            Url = fila["URL"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                Context.LogException.Nivel = Context.LogException.TipoMensaje.M_ERROR;
                Context.LogException.err("Error Ejecutando F_GetComentIni:", ex);
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
        public List<MdUnidades> F_GetUnidades()
        {
            List<MdUnidades> retorno = new List<MdUnidades>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSIVICC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "usr_mnvcc.pk_secad.F_GetUnidades";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdUnidades ObjR = new MdUnidades
                        {
                            Consecutivo = Convert.ToInt32(fila["Consecutivo"].ToString()),
                            Sigla = fila["Sigla"].ToString(),
                            Area = fila["Area"].ToString(),
                            SiglaPapa = fila["SiglaPapa"].ToString(),
                            Dependencia = fila["Dependencia"].ToString(),
                            Unidad = fila["Unidad"].ToString(),
                            SiglaFisica = fila["SiglaFisica"].ToString(),
                            Regi = fila["Regi"].ToString(),
                            Orden = Convert.ToInt32(fila["Orden"].ToString()),
                            UndeConsecutivo = Convert.ToInt32(fila["UndeConsecutivo"].ToString()),
                            TiunCodigo = Convert.ToInt32(fila["TiunCodigo"].ToString()),
                            SiglaDepende = fila["SiglaDepende"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                Context.LogException.Nivel = Context.LogException.TipoMensaje.M_ERROR;
                Context.LogException.err("Error Ejecutando F_GetUnidades:", ex);
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
        public List<MdAlarmas> F_GetListadoAlarmas()
        {
            List<MdAlarmas> retorno = new List<MdAlarmas>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSIVICC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "usr_mnvcc.pk_secad.F_GetListadoAlarmas";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdAlarmas ObjR = new MdAlarmas
                        {
                            IdAlarma = Convert.ToInt32(fila["IdAlarma"].ToString()),
                            IdRpcVinculado = Convert.ToInt32(fila["IdRpcVinculado"].ToString()),
                            Latitud = Convert.ToDecimal(fila["Latitud"].ToString()),
                            Longitud = Convert.ToDecimal(fila["Longitud"].ToString()),
                            IdTipo = Convert.ToInt32(fila["IdTipo"].ToString()),
                            FechaCreacion = fila["FechaCreacion"].ToString(),
                            UsuarioCreacion = fila["UsuarioCreacion"].ToString(),
                            IdGpsActual = Convert.ToInt32(fila["IdGpsActual"].ToString()),
                            Ciudadano = fila["Ciudadano"].ToString(),
                            Identificacion = Convert.ToInt64(fila["Identificacion"].ToString()),
                            Direccion = fila["Direccion"].ToString(),
                            TelefonoFijo = fila["TelefonoFijo"].ToString(),
                            Celular = Convert.ToInt64(fila["Celular"].ToString()),
                            Correo = fila["Correo"].ToString(),
                            Segmento = fila["Segmento"].ToString(),
                            Dto = fila["Dto"].ToString(),
                            Municipio = fila["Municipio"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                Context.LogException.Nivel = Context.LogException.TipoMensaje.M_ERROR;
                Context.LogException.err("Error Ejecutando F_GetListadoAlarmas:", ex);
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
        public List<MdUbicacionPda> F_GetUbicacionPda()
        {
            List<MdUbicacionPda> retorno = new List<MdUbicacionPda>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSivicc");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "usr_mnvcc.pk_secad.F_GetUbicacionPda";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }
                if (resultado.Rows.Count > 0)
                {
                    foreach (DataRow fila in resultado.Rows)
                    {
                        MdUbicacionPda ObjR = new MdUbicacionPda
                        {
                            UbicacionGpsId = Convert.ToInt32(fila["UbicacionGpsId"].ToString()),
                            UidRegistro = fila["UidRegistro"].ToString(),
                            Fecha = Convert.ToDateTime(fila["Fecha"].ToString()),
                            Latitud = fila["Latitud"].ToString(),
                            Longitud = fila["Longitud"].ToString(),
                            Velocidad = Convert.ToInt32(fila["Velocidad"].ToString()),
                            TipoLectura = fila["TipoLectura"].ToString(),
                            UsuarioSesion = fila["UsuarioSesion"].ToString(),
                            SesionActiva = Convert.ToInt32(fila["SesionActiva"].ToString()),
                            Imei = fila["Imei"].ToString(),
                            SerialSim = fila["SerialSim"].ToString(),
                            NivelBateria = Convert.ToInt32(fila["NivelBateria"].ToString()),
                            CuadranteId = Convert.ToInt32(fila["CuadranteId"].ToString()),
                            VersionApk = fila["VersionApk"].ToString(),
                            FechaInstalacion = fila["FechaInstalacion"].ToString(),
                            DireccionIp = fila["DireccionIp"].ToString(),
                            EstaDentroCuadrante = Convert.ToInt32(fila["EstaDentroCuadrante"].ToString()),
                            Identificacion = Convert.ToInt64(fila["Identificacion"].ToString()),
                            NombresApellidos = fila["NombresApellidos"].ToString(),
                            CodigoCuadrante = fila["CodigoCuadrante"].ToString(),
                            NumCelCuadrante = Convert.ToInt64(fila["NumCelCuadrante"].ToString()),
                            DependenciaId = Convert.ToInt32(fila["DependenciaId"].ToString()),
                            SiglaUnidad = fila["SiglaUnidad"].ToString(),
                            DescripcionUnidad = fila["DescripcionUnidad"].ToString(),
                            SiglaPapaUnidad = fila["SiglaPapaUnidad"].ToString(),
                            DispositivosSerie = fila["DispositivosSerie"].ToString(),
                            DispMarcaModelo = fila["DispMarcaModelo"].ToString(),
                            Duracion = fila["Duracion"].ToString(),
                            Minutos = fila["Minutos"].ToString(),
                            DescRegion = fila["DescRegion"].ToString()
                        };
                        retorno.Add(ObjR);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                Context.LogException.Nivel = Context.LogException.TipoMensaje.M_ERROR;
                Context.LogException.err("Error Ejecutando F_GetUbicacionPda:", ex);
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
        public List<MdCuadrantes> F_GetCuadrantes()
        {
            List<MdCuadrantes> retorno = new List<MdCuadrantes>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSIVICC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;


                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "usr_mnvcc.pk_secad.F_GetCuadrantes";

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
                            CodigoSiedco = fila["CodigoSiedco"].ToString(),
                            NumCelCua = Convert.ToInt64(fila["NumCelCua"].ToString()),
                            PrmEstadoCuad = Convert.ToInt32(fila["PrmEstadoCuad"].ToString()),
                            SiglaFisica = fila["SiglaFisica"].ToString(),
                            UndeConsLevel1 = Convert.ToInt32(fila["UndeConsLevel1"].ToString()),
                            UndeConsec = Convert.ToInt32(fila["UndeConsec"].ToString()),
                            UndeCai = Convert.ToInt32(fila["UndeCai"].ToString()),
                            Nivel1 = fila["Nivel1"].ToString(),
                            Estacion = fila["Estacion"].ToString(),
                            Dependencia = fila["Dependencia"].ToString(),
                            Siedco = Convert.ToInt32(fila["Siedco"].ToString()),
                            CodCuaAnt = fila["CodCuaAnt"].ToString(),
                            Tipo = fila["Tipo"].ToString(),
                            Ordenamiento = Convert.ToInt32(fila["Ordenamiento"].ToString()),
                            RegiCodigo = Convert.ToInt32(fila["RegiCodigo"].ToString()),
                            DescRegion = fila["DescRegion"].ToString(),
                            Pda = fila["Pda"].ToString(),
                            Proveedor = fila["Proveedor"].ToString()
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
        public List<MdDominios> f_ListarDominio(int p_Dominio)
        {
            List<MdDominios> retorno = new List<MdDominios>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionSisec");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_Id", OracleDbType.Int32)).Value = p_Dominio;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "PK_RNRPC.F_Cons_Dominios";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    MdDominios registro;
                    foreach (DataRow row in resultado.Rows)
                    {
                        registro = new MdDominios();
                        registro.Id = long.Parse(row["ID"].ToString());
                        registro.Descripcion = row["DESCRIPCION"].ToString();
                        retorno.Add(registro);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando f_ListarDominio:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        public List<MdDominios> f_ListarDominio_rpc(int p_Dominio)
        {
            List<MdDominios> retorno = new List<MdDominios>();
            ConDBOracle cnOracle = new ConDBOracle("strConexionRPC");
            OracleCommand objCommand = new OracleCommand();
            DataTable resultado = new DataTable();

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("p_Id", OracleDbType.Int32)).Value = p_Dominio;

                objCommand.Connection = cnOracle.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "rpc_pk.F_Cons_Dominios_rpc";

                if (cnOracle.Conectar())
                {
                    resultado.Load(objCommand.ExecuteReader());
                    cnOracle.Desconectar();
                }

                if (resultado.Rows.Count > 0)
                {
                    MdDominios registro;
                    foreach (DataRow row in resultado.Rows)
                    {
                        registro = new MdDominios();
                        registro.Id = long.Parse(row["ID"].ToString());
                        registro.Descripcion = row["DESCRIPCION"].ToString();
                        retorno.Add(registro);
                    }
                }
            }
            catch (Exception ex)
            {
                cnOracle.Desconectar();
                LogException.Nivel = LogException.TipoMensaje.M_ERROR;
                LogException.err("Error Ejecutando f_ListarDominio:", ex);
            }
            finally
            {
                cnOracle.Desconectar();
                if (objCommand != null) { objCommand.Dispose(); }
            }

            return retorno;
        }
        //public string EnviarCasoSecad(MdPedidos obj)
        //{
        //    ConDBOracle ConexionDB = new ConDBOracle("strConexionSecad");
        //    OracleCommand objCommand = new OracleCommand();
        //    string Numero_llamada = "0";

        //    try
        //    {
        //        objCommand.Parameters.Clear();
        //        objCommand.Parameters.Add(new OracleParameter("p_hora_caso", OracleDbType.Date)).Value = obj.Hora_caso;
        //        objCommand.Parameters.Add(new OracleParameter("p_disp_telefonico", OracleDbType.Varchar2, 255)).Value = obj.Disp_telefonico;
        //        objCommand.Parameters.Add(new OracleParameter("p_celda_marcacion", OracleDbType.Varchar2, 255)).Value = obj.Celda_marcacion;
        //        objCommand.Parameters.Add(new OracleParameter("p_nume_telefono", OracleDbType.Int64)).Value = obj.Nume_telefono;
        //        objCommand.Parameters.Add(new OracleParameter("p_prop_telefono", OracleDbType.Varchar2, 255)).Value = obj.Prop_telefono;
        //        objCommand.Parameters.Add(new OracleParameter("p_dire_telefono", OracleDbType.Varchar2, 255)).Value = obj.Dire_telefono;
        //        objCommand.Parameters.Add(new OracleParameter("p_luge_telefono", OracleDbType.Int32)).Value = obj.Luge_telefono;
        //        objCommand.Parameters.Add(new OracleParameter("p_barr_telefono", OracleDbType.Varchar2, 255)).Value = obj.Barr_telefono;
        //        objCommand.Parameters.Add(new OracleParameter("p_tipo_pedido", OracleDbType.Varchar2, 255)).Value = obj.Tipo_pedido;
        //        objCommand.Parameters.Add(new OracleParameter("p_cali_pedido", OracleDbType.Varchar2, 255)).Value = obj.Cali_pedido;
        //        objCommand.Parameters.Add(new OracleParameter("p_tipo_informacion", OracleDbType.Varchar2, 255)).Value = obj.Tipo_informacion;
        //        objCommand.Parameters.Add(new OracleParameter("p_codigo_cooperante", OracleDbType.Int32)).Value = obj.Codigo_cooperante;
        //        objCommand.Parameters.Add(new OracleParameter("p_nomb_llamante", OracleDbType.Varchar2, 255)).Value = obj.Nomb_llamante;
        //        objCommand.Parameters.Add(new OracleParameter("p_dire_llamante", OracleDbType.Varchar2, 255)).Value = obj.Dire_llamante;
        //        objCommand.Parameters.Add(new OracleParameter("p_luge_codigo", OracleDbType.Int32)).Value = obj.Luge_codigo;
        //        objCommand.Parameters.Add(new OracleParameter("p_dire_caso", OracleDbType.Varchar2, 255)).Value = obj.Dire_caso;
        //        objCommand.Parameters.Add(new OracleParameter("p_puca_codigo", OracleDbType.Int32)).Value = obj.Puca_codigo;
        //        objCommand.Parameters.Add(new OracleParameter("p_codi_barrio", OracleDbType.Int32)).Value = obj.Codi_barrio;
        //        objCommand.Parameters.Add(new OracleParameter("p_luge_barrio", OracleDbType.Int32)).Value = obj.Luge_barrio;
        //        objCommand.Parameters.Add(new OracleParameter("p_latitud_caso", OracleDbType.Varchar2, 255)).Value = obj.Latitud_caso;
        //        objCommand.Parameters.Add(new OracleParameter("p_longitud_caso", OracleDbType.Varchar2, 255)).Value = obj.Longitud_caso;
        //        objCommand.Parameters.Add(new OracleParameter("p_comentario", OracleDbType.Varchar2, 255)).Value = obj.Comentario;
        //        objCommand.Parameters.Add(new OracleParameter("p_codi_pedido", OracleDbType.Varchar2, 255)).Value = obj.Codi_pedido;
        //        objCommand.Parameters.Add(new OracleParameter("p_importancia", OracleDbType.Varchar2, 255)).Value = obj.Importancia;
        //        objCommand.Parameters.Add(new OracleParameter("p_prioridad", OracleDbType.Varchar2, 255)).Value = obj.Prioridad;
        //        objCommand.Parameters.Add(new OracleParameter("p_estado", OracleDbType.Varchar2, 255)).Value = obj.Estado;
        //        objCommand.Parameters.Add(new OracleParameter("p_enviar", OracleDbType.Varchar2, 255)).Value = obj.Enviar;
        //        objCommand.Parameters.Add(new OracleParameter("p_cadusua_usuario", OracleDbType.Varchar2, 255)).Value = obj.Cadusua_usuario;
        //        objCommand.Parameters.Add(new OracleParameter("p_cadpoli_cedu_empleado", OracleDbType.Varchar2, 255)).Value = obj.Cadpoli_cedu_empleado;
        //        objCommand.Parameters.Add(new OracleParameter("p_cadpedi_sitio_graba", OracleDbType.Int32)).Value = obj.Cadpedi_sitio_graba;
        //        objCommand.Parameters.Add(new OracleParameter("p_cadpedi_nume_llamada", OracleDbType.Int64)).Value = obj.Cadpedi_nume_llamada;
        //        objCommand.Parameters.Add(new OracleParameter("p_total_canales", OracleDbType.Varchar2, 255)).Value = obj.Total_canales;
        //        objCommand.Parameters.Add(new OracleParameter("p_tipo_genero", OracleDbType.Varchar2, 255)).Value = obj.Tipo_genero;

        //        objCommand.Parameters.Add(new OracleParameter("p_sitio_caso", OracleDbType.Int32)).Direction = ParameterDirection.Output;
        //        objCommand.Parameters.Add(new OracleParameter("p_nume_llamada", OracleDbType.Int64)).Direction = ParameterDirection.Output;
        //        objCommand.Parameters.Add(new OracleParameter("p_cod_salida", OracleDbType.Varchar2, 255)).Direction = ParameterDirection.Output;
        //        objCommand.Parameters.Add(new OracleParameter("p_desc_salida", OracleDbType.Varchar2, 255)).Direction = ParameterDirection.Output;


        //        objCommand.Connection = ConexionDB.Conexion;
        //        objCommand.CommandType = CommandType.StoredProcedure;
        //        objCommand.CommandText = "PK_SECADRPCP.P_INS_RPC_VINCULANTE";

        //        if (ConexionDB.Conectar())
        //        {
        //            objCommand.ExecuteNonQuery();
        //            Numero_llamada = Convert.ToString(objCommand.Parameters["p_nume_llamada"].Value);
        //            ConexionDB.Desconectar();
        //            objCommand.Dispose();
        //            return Numero_llamada;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ConexionDB.Desconectar();
        //        objCommand = null;
        //        objCommand.Dispose();
        //        Numero_llamada = "0";
        //        return Numero_llamada;
        //    }
        //    finally
        //    {
        //        objCommand.Dispose();
        //        ConexionDB.Desconectar();
        //        objCommand = null;
        //    }
        //    return Numero_llamada;
        //}
        public string UdpNumeroCasoSecad(int p_id, Int64 p_caso)
        {
            ConDBOracle ConexionDB = new ConDBOracle("strConexionRPC");
            OracleCommand objCommand = new OracleCommand();
            string Resultado = "0";

            try
            {
                objCommand.Parameters.Clear();
                objCommand.Parameters.Add(new OracleParameter("p_id", OracleDbType.Int32)).Value = p_id;
                objCommand.Parameters.Add(new OracleParameter("p_caso", OracleDbType.Int64)).Value = p_caso;
                //objCommand.Parameters.Add(new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)).Direction = ParameterDirection.ReturnValue;
                objCommand.Parameters.Add(new OracleParameter("P_RESULTADO", OracleDbType.Int32)).Direction = ParameterDirection.Output;

                objCommand.Connection = ConexionDB.Conexion;
                objCommand.CommandType = CommandType.StoredProcedure;
                objCommand.CommandText = "rpc_pk.NumeroCasoSecad";

                if (ConexionDB.Conectar())
                {
                    objCommand.ExecuteNonQuery();
                    Resultado = Convert.ToString(objCommand.Parameters["P_RESULTADO"].Value);
                    ConexionDB.Desconectar();
                    objCommand.Dispose();
                    return Resultado;
                }
            }
            catch (Exception ex)
            {
                ConexionDB.Desconectar();
                objCommand = null;
                objCommand.Dispose();
                Resultado = "0";
                return Resultado;
            }
            finally
            {
                objCommand.Dispose();
                ConexionDB.Desconectar();
                objCommand = null;
            }
            return Resultado;
        }
    }
}
