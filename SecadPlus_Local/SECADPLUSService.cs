using SECADPLUS_Local.Gestor;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace SECADPLUS_Local
{
    public partial class SECADPLUSService : ServiceBase
    {

        private System.Diagnostics.EventLog logger;
        public static string CADENA_CONEXION { get; set; }
        //public static int SITIO_GRABA { get; set; }
        public static string EQUIPO_MONITOR { get; set; }

        public static Process procesoLauncher;
        public SECADPLUSService()
        {

            InitializeComponent();
            logger = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("SecadPlusSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SecadPlusSource", "SecadPlusLog");
            }
            logger.Source = "SecadPlusSource";
            logger.Log = "SecadPlusLog";
        }

        protected override void OnStart(string[] args)
        {
            Timer tarea = new Timer();
            try
            {
                logger.WriteEntry("Inicia el servicio de SECADPLUS");
                CADENA_CONEXION = "DATA SOURCE = SECAD12C; PASSWORD = SECADLOCAL; USER ID =APP_SECADLOCAL";
                EQUIPO_MONITOR = "ASIESDEANT07";
                //logger.WriteEntry(CADENA_CONEXION);
                //Ejecuta la primera vez
                /*Operaciones.EjecutarOperacion(Enumeraciones.SecadOperacion.ControlBD, logger);*/
                Operaciones.EjecutarOperacion(Enumeraciones.SecadOperacion.EjecutarMonitorTelefonia, logger);
                //Genera el timer
                /*tarea.Enabled = true;
                tarea.Interval = 14400000;//Intervalo de 4 horas (valor en milisegundos)
                //tarea.Interval = 900000;//Intervalo de 15 minutos                
                tarea.Elapsed += delegate { Operaciones.EjecutarOperacion(Enumeraciones.SecadOperacion.ActualizarNombreMaquina, logger); };
                tarea.Start();
                */
            }
            catch (Exception ex)
            {
                logger.WriteEntry("Servicio Interrumpido.  | mensaje: " + ex.Message + " StackTrace: " + ex.StackTrace.ToString());
            }
        }

        protected override void OnStop()
        {
            if (!procesoLauncher.HasExited)
            {
                procesoLauncher.Kill();
                logger.WriteEntry("Launcher Asesinado");
            }
            //Buscamos los procesos monitor
            Process[] procesosMonitor = Process.GetProcessesByName("MonitorLinea");
            if(procesosMonitor != null && procesosMonitor.Length > 0)
            {
                foreach(Process procesoMonitor in procesosMonitor)
                {
                    procesoMonitor.Kill();
                    logger.WriteEntry("Proceso monitor Asesinado");
                }
            }
            logger.WriteEntry("In onStop.");
        }
    }
}
