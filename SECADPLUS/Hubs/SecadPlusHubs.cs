using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using SECADPLUS.Models;
using System.Threading;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using SECADEntities.SecadEntities;

namespace SECADPLUS.Hubs
{
    public class SecadPlusHubs
    {
    }
    //[HubName("NotificadorHub")]
    public class NotificadorHub : Hub
    {        
        public static bool NOTIFICAR = true;
        public static bool EJECUTAR = false;        
        public readonly static Thread procesoNotificador2;
        public static Timer procesoNotificador;
        private static string strConexion ="", strRangoConsulta = "0";
        private static bool Chequeando = false;
        private readonly static ConexionesNotificadorHub<string> _conexiones =new ConexionesNotificadorHub<string>();
        private readonly static IHubContext _contexto = GlobalHost.ConnectionManager.GetHubContext<NotificadorHub>();
        public void IniciarNotificador()
        {
            try
            {
                //Obtiene la conexión activa
                strConexion = Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.ConnectionString);
                int intIntervalo = 0;  
                //TimerCallback cb = new TimerCallback(Consultar);
                if (procesoNotificador==null) {
                    EJECUTAR = true;
                    intIntervalo = Convert.ToInt32(ConfigurationManager.AppSettings["RespuestaNotificaciones"].ToString());
                    strRangoConsulta = (ConfigurationManager.AppSettings["RangoConsultaNotificaciones"].ToString());
                    //Inicia el hilo para consultar mensajes despues de 1 segundo y se repite cada X segundos configurados en el webconfig
                    procesoNotificador = new Timer(Consultar, null, 1000, intIntervalo);                    
                }
            }
            catch (Exception ex)
            {
                GestorBase.GestionarExcepcion(ex, GetType().Name);
                GestorBase.BLS_NOTIFICACION = false;
            }
        }
        private static void Consultar(object obj)
        {
            if (!Chequeando)
            {
                Chequeando = true;

                try
                {
                    GestorGeneral objGestorGral;
                    DataTable dttNotificaciones;
                    List<NotificacionModel> mensajes = new List<NotificacionModel>();
                    bool blEmergente = true, blNoEmergente = true;
                    string strValor = "";
                    GestorBase.BL_NOTIFICACION = true;
                    using (DesRepMinEntities contextoRecepcion = new DesRepMinEntities(Integrador.ObtenerConexionActiva(SecadEnums.TipoConexion.Entity, "DespachosRecepcionMinuta", "SecadEntities")))
                    {
                        //Consulta el estado de las notificaciones
                        strValor = (from registro in contextoRecepcion.CADVISORPARAMETROS
                                    where registro.TIPO == "NOTIFICACIONES" && registro.NOMBRE == "EMERGENTE"
                                    select registro.PARAMETRO).FirstOrDefault();
                        if (!string.IsNullOrEmpty(strValor))
                        {
                            if (strValor.Equals("False"))
                            {
                                blEmergente = false;
                            }
                        }
                        strValor = "";
                        strValor = (from registro in contextoRecepcion.CADVISORPARAMETROS
                                    where registro.TIPO == "NOTIFICACIONES" && registro.NOMBRE == "NO_EMERGENTE"
                                    select registro.PARAMETRO).FirstOrDefault();
                        if (!string.IsNullOrEmpty(strValor))
                        {
                            if (strValor.Equals("False"))
                            {
                                blNoEmergente = false;
                            }
                        }
                    }
                    if (!blNoEmergente && !blEmergente)
                    {
                        //Especifica a los clientes que las notificaciones se encuentran deshabilitadas
                        NOTIFICAR = false;
                        //Limpiar los mensajes en todos los lcientes
                        Limpiar(_contexto);

                    }
                    else
                    {
                        NOTIFICAR = true;
                    }
                    //Verifica si el proceso se debe detener
                    if (!EJECUTAR)
                    {
                        procesoNotificador.Dispose();
                        return;
                    }
                    //Verifica si las notificaciones estan deshabilitadas
                    if (!NOTIFICAR)
                    {
                        return;
                    }
                    //Consulta los mensajes pendientes
                    objGestorGral = new GestorGeneral();
                    //Ordena la colección de conexiones y trae los usuarios
                    var conexiones = _conexiones.Coleccion.OrderByDescending(v => v.Key).Distinct();
                    foreach (var usuario in conexiones)
                    {
                        objGestorGral.ParamsAvanzados = new List<Tuple<string, string, Type>>();
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_usuario", usuario.Key.ToString(), typeof(string)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_minutos", strRangoConsulta, typeof(decimal)));
                        objGestorGral.ParamsAvanzados.Add(new Tuple<string, string, Type>("p_resultados", "", typeof(OracleDbType)));
                        dttNotificaciones = objGestorGral.EjecutarProcedimiento(Resources.RecursoGeneral.BaseDats_Usuario + ".PK_CADPLUS.p_generarmensajespendientes", strConexion, "Nonquery");
                        if (dttNotificaciones.Rows.Count > 0)
                        {
                            foreach (DataRow fila in dttNotificaciones.Rows)
                            {

                                mensajes.Add(new NotificacionModel { Mensaje = fila["TEXTO"].ToString(), Consecutivo = Convert.ToDecimal(fila["CONSECUTIVO"].ToString()) });
                            }
                            //Ejecuta el la notificacion a los clientes
                            Notificar(_contexto, usuario.Key.ToString(), mensajes, blEmergente, blNoEmergente);
                        }
                        else
                        {
                            LimpiarMensajesUsuario(_contexto, usuario.Key.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    GestorBase.GestionarExcepcion(ex, "SecadPLusHubs");
                    GestorBase.BLS_NOTIFICACION = false;
                }

                Chequeando = false;
            }
        }
        public static void Notificar(IHubContext contexto, string strUsuarioId, List<NotificacionModel> mensajes, bool blEmergente, bool blNoEmergente)
        {
            //Busca todos los clientes del usuarioId
            bool blVerificarNuevosMensajes = false;
            foreach (var connectionId in _conexiones.GetConnections(strUsuarioId))
            {
                contexto.Clients.Client(connectionId).notificar(mensajes, blEmergente, blNoEmergente, mensajes.Count);
                if (!blVerificarNuevosMensajes)
                {
                    contexto.Clients.Client(connectionId).nuevo();
                    blVerificarNuevosMensajes = true;
                }
            }
        }
        public static void LimpiarMensajesUsuario(IHubContext contexto, string strUsuarioId)
        {
            //Busca todos los clientes del usuarioId
            foreach (var connectionId in _conexiones.GetConnections(strUsuarioId))
            {
                contexto.Clients.Client(connectionId).limpiar();
            }            
        }
        public static void Limpiar(IHubContext contexto)
        {
            //Ordena la colección de conexiones y trae los usuarios
            var conexiones = _conexiones.Coleccion.OrderByDescending(v => v.Key).Distinct();
            foreach (var usuario in conexiones)
            {
                //Busca todos los clientes del usuarioId
                foreach (var connectionId in _conexiones.GetConnections(usuario.Key.ToString()))
                {
                    contexto.Clients.Client(connectionId).limpiar();
                }
            }            
        }
        public override Task OnConnected()
        {
            string strEstado= Context.QueryString["ESTADONOTIFICA"].ToString();            
            string strUsuarioId = Context.QueryString["USUARIO"].ToString();
            if (strEstado.Equals("TRUE"))
            {
                EJECUTAR = true;
            }else
            {
                EJECUTAR = false;
            }
            _conexiones.Add(strUsuarioId, Context.ConnectionId);

            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string strEstado = Context.QueryString["ESTADONOTIFICA"].ToString();
            string strUsuarioId = Context.QueryString["USUARIO"].ToString();

            _conexiones.Remove(strUsuarioId, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string strUsuarioId = Context.QueryString["USUARIO"].ToString();

            if (!_conexiones.GetConnections(strUsuarioId).Contains(Context.ConnectionId))
            {
                _conexiones.Add(strUsuarioId, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
        
        public void Notificacion(string name, string message)
        {
            Clients.All.notificar(name, message);
        }

    }
}