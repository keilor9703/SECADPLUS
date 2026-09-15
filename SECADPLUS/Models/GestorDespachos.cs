using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{
    public class GestorDespachos : GestorBase
    {
        private string strCadena = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);

        #region métodos
        public Tuple<int, int, string> EjecutarCrearEvento(int idSitio, string idIdentificacion, decimal numeLlamada, int? idFuerza, int? idCanal, string idCodigoPedido, string claseTurno, string usuario)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_grabacion", idSitio.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_iden_usuario", idIdentificacion, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nume_llamada", numeLlamada.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadcana_fuerz_id", idFuerza.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_cadcana_codigo", idCanal.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codi_pedido", idCodigoPedido.ToString(), typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clase_turno", claseTurno, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", usuario, typeof(string)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_evento", null, typeof(int)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_control_crear", null, typeof(int)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_texto", null, typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_crear_evento", strCadena, "MULTIPLE");

                var evento = int.Parse(objGestorGral.ParamsSalida[0].Item2.ToString());
                var controlCrear = int.Parse(objGestorGral.ParamsSalida[1].Item2.ToString());
                string mensaje = objGestorGral.ParamsSalida[2].Item2.ToString();

                return Tuple.Create(evento, controlCrear, mensaje);
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                //return false;
            }
        }




        public DataTable ConsultarEventoMedios(int idSitio, int numeEvento)
        {
            try
            {
                string strQuery = " WHERE cadeven_sitiograba = " + idSitio + " AND cadeven_nume_evento = " + numeEvento + " ", strLimite = "";

                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_complemento", strQuery, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_limites", strLimite, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_recordset", "", typeof(OracleDbType)));
                return objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_lista_eventos_medios", strCadena, "Nonquery");
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public Boolean IngresarComentarioAdicionales(int idSitio, decimal numeLlamada, string codiCaso, string vigente, string usuario)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PSITIO_GRABA", idSitio.ToString(), typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("PNUME_LLAMADA", numeLlamada.ToString(), typeof(decimal)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_CODICASO", codiCaso, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_VIGENTE", vigente, typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("P_USUARIO", usuario, typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CAD.P_CARGARPREGUNTASADIC", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public Boolean ActualizarDireccionBarrio(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio_grabacion", parametros[0], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nume_llamada", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", parametros[2], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_direccion", parametros[3], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_barrio", parametros[4], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_luge_barrio", parametros[5], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codi_barrio", parametros[6], typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualizar_dire_barrio", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public Boolean ActualizarIdUbicacion(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_id_ubicacion", parametros[0], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_unidad", parametros[2], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_codigo", parametros[3], typeof(int)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualizar_id_ubicacion", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public string EjecutarMedioCaso(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_sitio_graba", parametros[0], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_sitio_graba", parametros[2], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_consecutivo", parametros[3], typeof(long)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_condicion", parametros[4], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", parametros[5], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_canal", parametros[6], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", parametros[7], typeof(string)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_texto", null, typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_insertar_evento_medio", strCadena, "MULTIPLE");

                string mensaje = objGestorGral.ParamsSalida[0].Item2.ToString();

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                //return false;
            }
        }

        public Tuple<string, string> EjecutarHorasMedioCaso(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_consecutivo", parametros[0], typeof(long)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_nom_hora", parametros[2], typeof(string)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_hora", null, typeof(string)));
                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_texto", null, typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_insertar_horas_evento_medio", strCadena, "MULTIPLE");

                string hora = objGestorGral.ParamsSalida[0].Item2.ToString();
                string mensaje = objGestorGral.ParamsSalida[1].Item2.ToString();

                return Tuple.Create(hora, mensaje);
            }
            catch (Exception ex)

            {
                throw GestionarExcepcion(ex, GetType().Name);
                //return false;
            }
        }

        public Boolean ActualizarNovedad(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio_consecutivo", parametros[0], typeof(long)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_valor", parametros[2], typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualizar_novedad", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public string ConsultarMedio(int idSitio, long idConsecutivo)
        {
            try
            {
                string strMedio;
                GestorGeneral gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_sitio", idSitio.ToString(), typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_medio", idConsecutivo.ToString(), typeof(long)));
                strMedio = gestorGeneral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_desc_recurso_patrulla", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                if (string.IsNullOrEmpty(strMedio))
                    return "No es posible mostrar el medio. Revise...";
                else
                    return strMedio;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public string TraerTurno(string idFuerza, string claseTurno, string funcion)
        {
            try
            {
                GestorGeneral gestor = new GestorGeneral();
                DateTime fecha = gestor.ObtenerTiempoServidor?? DateTime.Now;

                string turno;

                GestorGeneral gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(string)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_hora_inicia", fecha.ToString(), typeof(DateTime)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_fuerza", idFuerza, typeof(int)));
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_clasturno", claseTurno, typeof(string)));
                turno = gestorGeneral.EjecutarFuncion<string>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS." + funcion, Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));

                return turno;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }

        public Boolean ActualizarEventoEnSeguimiento(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_sitio_graba", parametros[0], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_valor", parametros[2], typeof(string)));

                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_actualizar_en_seguimiento", strCadena, "bool");

                return true;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
                
            }
        }

        public string EjecutarCerrarEvento(string[] parametros)
        {
            try
            {
                GestorGeneral objGestorGral;

                objGestorGral = new GestorGeneral();
                objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                objGestorGral.ParamsSalida = new List<Tuple<string, object, Type>>();

                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_caso_final", parametros[0], typeof(string)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_sitio_graba", parametros[1], typeof(int)));
                objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_evento_nume_evento", parametros[2], typeof(int)));

                objGestorGral.ParamsSalida.Add(new Tuple<string, object, Type>("p_texto", null, typeof(string)));
                objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_cerrar_evento", strCadena, "MULTIPLE");

                string mensaje = objGestorGral.ParamsSalida[0].Item2.ToString();

                return mensaje;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }
        public long ConsultaConsecAnotacionPatrulla()
        {
            try
            {
                long dcConsecutivo = 0;
                gestorGeneral = new GestorGeneral();
                gestorGeneral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                gestorGeneral.ParamsAvanzados.Add(new Tuple<string, string, Type>("f_resultado", "", typeof(decimal)));
                dcConsecutivo = gestorGeneral.EjecutarFuncion<long>(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.f_consultar_seq_anotapatrulla", Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString));
                return dcConsecutivo;
            }
            catch (Exception ex)
            {
                throw GestionarExcepcion(ex, GetType().Name);
            }
        }
        #endregion métodos
    }
}