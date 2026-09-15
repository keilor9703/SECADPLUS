using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{
    public class GestorRecepcion : GestorBase

    {
        #region Propiedades

        public int Sitio { get; set; }
        public string NumeroLlamada { get; set; }

        #endregion Propiedades

        #region métodos

        public bool Ejecutar()
        {
            return true;
        }

        public bool InsertaHistoricoWs(string strSitio, string strusuario, string strNumeroLlamada, string strFuerzaRetorno, string strRespuestaWs)
        {
            String resp_ref_abreviada = "";
            try
            {
                strMensaje = "";
                gestorGeneral3 = new GestorGeneral();
                gestorGeneral3.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                gestorGeneral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_SITIO", strSitio, typeof(string)));
                gestorGeneral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_USUARIO", strusuario, typeof(string)));
                gestorGeneral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_NUMEROLLAMADA", strNumeroLlamada, typeof(string)));
                gestorGeneral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_IDFUERZAENTIDAD", strFuerzaRetorno, typeof(string)));
                gestorGeneral3.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_RESPUESTAWS", strRespuestaWs, typeof(string)));

                gestorGeneral3.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_INTEGRACION_SALUD.P_CADLOGHISTWS", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
                if (!gestorGeneral3.Estado)
                {
                    strMensaje = "No se pudo completar la actualización del estado del caso.";
                }
                GestorBase.MSN_ALERTA_COMUN = strMensaje;
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool EnviarLlamada(string strSitio, string strNumeroLlamada, string strTelefono, string strPedido, ref decimal idFuerzaEntidad, ref string resp_ref_abreviada)
        {
            decimal codigoFuerzaRetorno = 0;
            int cont = 0;
            try
            {
                strMensaje = "";
                gestorGeneral = new GestorGeneral();
                //gestorGeneral2 = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                //Verifica el numero de pedido
                if (strPedido.Equals("900"))
                {
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", strNumeroLlamada, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_abonado", strTelefono, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_estado", "C", typeof(string)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_enviar", "N", typeof(string)));
                    gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.p_cambio_estado_caso_new", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
                    if (!gestorGeneral.Estado)
                    {
                        strMensaje = "No se pudo completar la actualización del estado del caso.";
                    }
                }
                else
                {
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", strNumeroLlamada, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_abonado", strTelefono, typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_estado", "P", typeof(string)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_enviar", "S", typeof(string)));
                    gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.p_cambio_estado_caso_new", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
                    if (!gestorGeneral.Estado)
                    {
                        strMensaje = "No se pudo completar la actualización del estado del caso.";
                    }
                }
                //Cambia el estado de los pedidos del canal
                gestorGeneral.ParamsAvanzados.Clear();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", strNumeroLlamada, typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_estado", "P", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_enviar", "S", typeof(string)));
                gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.p_cambio_estado_pedido_canal", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
                if (strPedido.Equals("900"))
                {
                    GestorBase.MSN_ALERTA_COMUN = "";
                }
                if (!gestorGeneral.Estado)
                {
                    strMensaje += "\n No se completo la actualización del estado de los canales asignados al caso.";
                }
                //Procedimiento para consultar el codigo de la fuerza
                gestorGeneral2 = new GestorGeneral();
                gestorGeneral2.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral2.ParamsSalida = new List<Tuple<string, object, Type>>();
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", strNumeroLlamada, typeof(decimal)));
                gestorGeneral2.ParamsSalida.Add(new Tuple<string, object, Type>("p_codigo_fuerza", "", typeof(decimal)));
                gestorGeneral2.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADINTEGMINUTASECADSIVICC.p_obtener_fuerza", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in gestorGeneral2.ParamsSalida)
                {
                    codigoFuerzaRetorno = Convert.ToInt32(parameter.Item2.ToString());
                    idFuerzaEntidad = codigoFuerzaRetorno;
                }
                //funcion para validar si esta activo la consulta a web services del sem pereira 00 = ACTIVO , 01 = INACTIVO
                //string resp_ref_abreviada = "";
                /*
                gestorGeneral2 = new GestorGeneral();
                gestorGeneral2.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", strSitio, typeof(decimal)));
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nombre", "INTEGRACION_WEBSERVICES", typeof(string)));
                gestorGeneral2.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", "60169011", typeof(string)));
                resp_ref_abreviada = gestorGeneral2.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_referencias_abrev", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                */
                resp_ref_abreviada = "00";
                GestorBase.MSN_ALERTA_COMUN = strMensaje;
                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }

        }

        /// <summary>
        /// Permite consulta el consecutivo de llamada para un pedido
        /// </summary>
        /// <returns></returns>
        public decimal ObtenerConsecutivoLlamada()
        {
            try
            {
                decimal dcConsecutivo = 0;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                dcConsecutivo = gestorGeneral.EjecutarFuncion<decimal>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultar_seq_pedido", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                return dcConsecutivo;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        /// <summary>
        /// Permite consultar el medio de comunicación asignado a un abonado
        /// </summary>
        /// <param name="strNumero"></param>
        /// <param name="strNumeracion"></param>
        /// <returns></returns>
        public string ObtenerMedioCom(string strNumero, string strNumeracion)
        {
            try
            {
                string strMedio = "";
                gestorGeneral = new GestorGeneral();
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("vl_salida", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numero", strNumero, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numeracion", strNumeracion, typeof(string)));
                strMedio = gestorGeneral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.f_disp_telefonico", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                return strMedio;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string ObtenerCasoPreguntas(string strCaso)
        {
            try
            {
                string strPreguntas = "";
                DataTable dtDatos;
                gestorGeneral = new GestorGeneral();
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", strCaso, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                dtDatos = gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_preguntascaso", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "nonquery");
                foreach (DataRow fila in dtDatos.Rows)
                {
                    strPreguntas += fila["DESCRIPCION"].ToString() + "\n";
                }
                return strPreguntas;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public bool PedidoRepetido(string strFechaCaso, string strCodigoCaso, string strDireCaso, string strLugeCodigo)
        {
            try
            {
                DateTime fechaCaso = Convert.ToDateTime(strFechaCaso);
                blEstado = false;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsSalida = new List<Tuple<string, object, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fecha", fechaCaso.ToShortDateString(), typeof(DateTime)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codi_pedido", strCodigoCaso, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_calidad", "01", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_direccion", strDireCaso, typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_codigo", strLugeCodigo, typeof(decimal)));
                gestorGeneral.ParamsSalida.Add(new Tuple<string, object, Type>("p_sitio", "", typeof(decimal)));
                gestorGeneral.ParamsSalida.Add(new Tuple<string, object, Type>("p_llamada", "", typeof(decimal)));
                gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_CASO_EXISTE", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in gestorGeneral.ParamsSalida)
                {
                    //consulta el nombre de la ciudad si existe
                    if (!string.IsNullOrEmpty(parameter.Item2.ToString()))
                    {
                        blEstado = true;
                    }
                    if (parameter.Item1.Equals("p_sitio") && !string.IsNullOrEmpty(parameter.Item2.ToString()))
                    {
                        Sitio = Convert.ToInt32(parameter.Item2.ToString());
                    }
                    if (parameter.Item1.Equals("p_llamada"))
                    {
                        NumeroLlamada = parameter.Item2.ToString();
                    }
                }
                //Si existe una llamada la marca como base
                if (blEstado)
                {
                    gestorGeneral.ParamsAvanzados.Clear();
                    gestorGeneral.ParamsSalida.Clear();
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", Sitio.ToString(), typeof(decimal)));
                    gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_numellamada", NumeroLlamada, typeof(decimal)));
                    gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.p_marcallamadabase", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "bool");
                }
                return blEstado;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public long ConsultarLlamadaEntrante(int intSitio, string strACD)
        {
            try
            {
                long lgNumero = 0;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsSalida = new List<Tuple<string, object, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_graba", intSitio.ToString(), typeof(decimal)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_acd", strACD, typeof(string)));
                gestorGeneral.ParamsSalida.Add(new Tuple<string, object, Type>("p_numero_telefono", "", typeof(decimal)));
                gestorGeneral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_consultar_llamada", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString), "MULTIPLE");
                foreach (Tuple<string, object, Type> parameter in gestorGeneral.ParamsSalida)
                {
                    if (parameter.Item1.Equals("p_numero_telefono") && !string.IsNullOrEmpty(parameter.Item2.ToString()))
                    {
                        lgNumero = Convert.ToInt64(parameter.Item2.ToString());
                    }
                }
                return lgNumero;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        #endregion métodos
    }
}