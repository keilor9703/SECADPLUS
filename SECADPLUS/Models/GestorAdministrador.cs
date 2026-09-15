using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{
    public class GestorAdministrador : GestorBase
    {
        private string strConsultaSql;
        private OracleCommand oracleComando;
        private OracleConnection oracleConexion;
        private OracleDataAdapter adaptador;

        #region métodos

        public DataTable ConsultarCiudadesDepartamento(int id, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT CODIGO, DESCRIPCION FROM CADLUGARES_GEOGRAFICOS" +
                                 " CONNECT BY PRIOR CODIGO = LUGE_CODIGO" +
                                 " START WITH CODIGO = :id";

                oracleComando = new OracleCommand(strConsultaSql);
                oracleComando.Parameters.Add(new OracleParameter("id", id));
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "lugaresgeograficos");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarMunicipios(string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT LG.CODIGO, LG.DESCRIPCION||' - '||" +
                                 " (SELECT DESCRIPCION FROM CADLUGARES_GEOGRAFICOS LG2 WHERE LG2.CODIGO = LG.LUGE_CODIGO) DESCRIPCION " +
                                 " FROM CADLUGARES_GEOGRAFICOS LG" +
                                 " CONNECT BY PRIOR LG.CODIGO = LG.LUGE_CODIGO" +
                                 " START WITH LG.CODIGO = 10";

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "municipios");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarRegional(int idFuerza, int idCodigo, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT r.CONSECUTIVO, r.DESCRIPCION" +
                                 " From CADUNIDADES u Inner Join CADREGIONALES r" +
                                 " On r.CONSECUTIVO = u.CONSECUTIVO_REGIONAL" +
                                 " Where (u.FUERZA = :fuerza And u.CODIGO = :codigo)";

                oracleComando = new OracleCommand(strConsultaSql);
                oracleComando.Parameters.Add(new OracleParameter("fuerza", idFuerza));
                oracleComando.Parameters.Add(new OracleParameter("codigo", idCodigo));

                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "regional");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarMensajeTextoUsuarios(int? idSitio, long? idCons, long? idText, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT CADMENTEX_MENS_SITIOGRABA, CADMENTEX_MENS_CONSECUTIVO, CADMENTEX_CONSECUTIVO, " +
                                 "CADUSUA_DESTINO, ESTADO, FECHA_NOTIFICACION, FECHA_GRABACION, " +
                                 "CADFUER_DESTINO, PK_CAD.f_desc_fuerza(CADFUER_DESTINO) ENTIDAD, " +
                                 "CADCANA_DESTINO, PK_CAD.f_desc_canal(CADFUER_DESTINO,CADCANA_DESTINO) CANAL " +
                                 "FROM cadmensajes_texto_usuarios " +
                                 "WHERE CADMENTEX_MENS_SITIOGRABA = " + idSitio + " AND CADMENTEX_MENS_CONSECUTIVO = " + idCons + " AND CADMENTEX_CONSECUTIVO = " + idText;

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "cadmensajes_texto_usuarios");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarUsuariosMensajeA(long? idFuerza, long? idCanal, string strCadena)
        {
            try
            {
                strConsultaSql = "select cadusuarios.usuario, " +
                                 "cadpolicias.grad_alfabetico||' '||cadpolicias.nomb_empleado NOMBRE, " +
                                 "cadfuerzas.descripcion DESC_FUERZA, " +
                                 "cadsitios_grabacion.descripcion DESC_SITIO " +
                                 "from cadusuarios, cadpolicias, cadfuerzas, cadsitios_grabacion " +
                                 "where cadpolicias.cedu_empleado = cadusuarios.identificacion " +
                                 "and cadfuerzas.id = cadpolicias.cadfuerz_id " +
                                 "and cadsitios_grabacion.consecutivo = cadfuerzas.sitio_graba  " +
                                 "and cadusuarios.cadcana_fuerza_id = " + idFuerza +
                                 "and cadusuarios.cadcana_codigo =  " + idCanal +
                                 "union select '0' usuario, " +
                                 "'TODOS LOS USUARIOS' NOMBRE, " +
                                 "' ' DESC_FUERZA, " +
                                 "' ' DESC_SITIO " +
                                 "from DUAL ";

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "usuarios_destino");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarPersonasIncidentes(string idUsuario, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT A.SITIO_GRABA, A.ANO_CONSULTA, A.CONSECUTIVO, " +
                                 "'CEDULA: '||A.CEDULA||'. FECHA: '||TO_CHAR(FECHA_CONSULTA,'DD/MM/YYYY:HH24:MI:SS')||'. DATO: '|| A.CONSULTA_DIJIN DATO " +
                                 "FROM USR_CADAUDITA.CADCON_PERSONAS A  " +
                                 "WHERE A.USUARIO = '" + idUsuario + "' AND CONSULTA_DIJIN = 'V' " +
                                 "ORDER BY FECHA_CONSULTA DESC ";

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "personas");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarVehiculosIncidentes(string idUsuario, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT A.SITIO_GRABA, A.ANO_CONSULTA, A.CONSECUTIVO, A.CAMPO_BUSQUEDA||': '||A.CRITERIO_BUSQUEDA" +
                                 "||'. FECHA: '||TO_CHAR(FECHA_CONSULTA,'DD/MM/YYYY:HH24:MI:SS')||'. DATO: '|| A.RESULTADO_CONSULTA DATO " +
                                 "FROM USR_CADAUDITA.CADCON_VEHICULOS A " +
                                 "WHERE A.USUARIO = '" + idUsuario + "' ORDER BY FECHA_CONSULTA DESC ";

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "vehiculos");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarCasosCanal(int? idFuerza, int? idCanal, string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT cc. cadcaso_codigo, cc.cadcana_fuerz_id, cc.cadcana_codigo, c.descripcion " +
                                 "FROM CADCASOS c INNER JOIN CADCASOS_CANAL cc ON c.codigo = cc.cadcaso_codigo  " +
                                 "WHERE cc.cadcana_fuerz_id = " + idFuerza + "AND cc.cadcana_codigo = " + idCanal;

                oracleComando = new OracleCommand(strConsultaSql);
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;
                return gestorGeneral.LlenarDataSet(oracleComando, "casos_canal");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarLineasMarca(int? id)
        {
            try
            {
                strConsultaSql = "select t.* from cadlineas_marca t where t.marc_codigo = :id";

                oracleComando = new OracleCommand(strConsultaSql);
                oracleComando.Parameters.Add(new OracleParameter("id", id));
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                return gestorGeneral.LlenarDataSet(oracleComando, "lineasmarcas");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public DataTable ConsultarClaseNovedad(int? id)
        {
            try
            {
                strConsultaSql = "select t.* from USR_CAD.CADTIPOSNOVEDADES t where t.CADNOVBIN = :id";

                oracleComando = new OracleCommand(strConsultaSql);
                oracleComando.Parameters.Add(new OracleParameter("id", id));
                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                return gestorGeneral.LlenarDataSet(oracleComando, "claseNovedad");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public long ConsultarConsecutivoBienes(string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT SECU_BIENESIMPL.NEXTVAL FROM SYS.DUAL";

                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;

                oracleConexion = new OracleConnection(strCadena);
                oracleComando = new OracleCommand(strConsultaSql, oracleConexion);

                oracleConexion.Open();
                long id = Convert.ToInt64(oracleComando.ExecuteScalar());
                oracleConexion.Close();
                oracleConexion.Dispose();
                return id;
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

        public long ConsultarConsecutivoMensaje(string strCadena)
        {
            try
            {
                strConsultaSql = "SELECT secu_mensajes.nextval FROM SYS.DUAL";

                gestorGeneral = new GestorGeneral();
                gestorGeneral.CadenaConexion = strCadena;

                oracleConexion = new OracleConnection(strCadena);
                oracleComando = new OracleCommand(strConsultaSql, oracleConexion);

                oracleConexion.Open();
                long id = Convert.ToInt64(oracleComando.ExecuteScalar());
                oracleConexion.Close();
                oracleConexion.Dispose();
                return id;
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

        public Boolean EjecutarEnviarMensaje(int idSitio, long idCons, string strCadena)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", idSitio.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", idCons.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_estado", "E", typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_CAMBIO_ESTADO_MENSAJE", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public Boolean EjecutarCopiarReferencias(int origen, int destino, string descCiudad, int codCiudad, string strCadena)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_origen_sitio_grabacion", origen.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_destino_sitio_grabacion", destino.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_desc_ciudad", descCiudad, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_ciudad", codCiudad.ToString(), typeof(int)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_copiar_referencias", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public Boolean EjecutarCasosCanal(int idFuerza, int idCanal, string casoDesde, string casoHasta, string strCadena)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadcana_fuerz_id", idFuerza.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadcana_codigo", idCanal.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_caso_desde", casoDesde, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_caso_hasta", casoHasta, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_procesar_casos_canal", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public DataTable GetFuerza(string strCadena)
        {
            tabla = new DataTable();
            try
            {
                oracleConexion = new OracleConnection();
                oracleConexion.ConnectionString = strCadena;
                oracleConexion.Open();
                oracleComando = oracleConexion.CreateCommand();
                oracleComando.CommandType = System.Data.CommandType.StoredProcedure;
                oracleComando.CommandText = Resources.RecursoGeneral.BaseDats_Usuario + ".pk_cadplus.p_fuerza";
                OracleParameter par1 = new OracleParameter();
                par1.OracleDbType = OracleDbType.RefCursor;
                par1.Direction = System.Data.ParameterDirection.Output;
                oracleComando.Parameters.Add(par1);
                oracleComando.ExecuteNonQuery();
                OracleRefCursor cursor = (OracleRefCursor)par1.Value;
                OracleDataReader dtr = cursor.GetDataReader();
                adaptador = new OracleDataAdapter(oracleComando);
                adaptador.Fill(tabla);
                oracleConexion.Close();
                oracleConexion.Dispose();
                return tabla;
            }
            catch (Exception ex)
            {
                oracleConexion.Close();
                oracleConexion.Dispose();
                throw GestionarExcepcion(ex, GetType().Name);
            }
            finally {
                oracleConexion.Close();
                oracleConexion.Dispose();
            }
        }

        #endregion métodos
    }
}