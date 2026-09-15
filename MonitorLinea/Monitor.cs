using MonitorLinea.Gestor;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MonitorLinea
{
    class Monitor
    {
        private static System.Windows.Forms.Timer tmProcesarCola = new System.Windows.Forms.Timer();
        public static string CADENA_CONEXION = "DATA SOURCE = SECAD12C; PASSWORD = SECADLOCAL; USER ID =APP_SECADLOCAL";
        public static int SITIO_GRABA = 200;
        private static string numExtension = "NONE";
        private static string servidorTel = "172.26.235.7";
        private static EventLog logger;

        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                numExtension = args[0];
            }

            logger = new EventLog();
            if (!EventLog.SourceExists("SecadPlusSource"))
            {
                EventLog.CreateEventSource(
                    "SecadPlusMonitor", "SecadPlusLog");
            }
            logger.Source = "SecadPlusSource";
            logger.Log = "SecadPlusLog";
            logger.WriteEntry("Iniciando monitor para la extensión " + numExtension);

            Thread thread = new Thread(MessagePumpThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            while (true)
            {
                Thread.Sleep(1000000);
            }
        }

        private static void MessagePumpThread()
        {
            AxCTITCONTROLLib.AxCTiTLine axCTiTLine1 = new AxCTITCONTROLLib.AxCTiTLine();
            AxCTITCONTROLLib.AxCTiTConnection axCTiTConnection1 = new AxCTITCONTROLLib.AxCTiTConnection();
            AxCTUTILSLib.AxCTUtils axCTUtils1 = new AxCTUTILSLib.AxCTUtils();

            axCTiTConnection1.CreateControl();
            axCTiTLine1.CreateControl();
            axCTUtils1.CreateControl();

            axCTiTConnection1.ApplicationName = "AplicacionDemo";
            axCTiTConnection1.Initialize = true;
            bool status = axCTiTConnection1.Initialize;
            if (!status)
            {
                Console.WriteLine("Error inicializando el dispositivo");
                logger.WriteEntry("Error inicializando el dispositivo");
                return;
            }
            int numDevices = axCTiTConnection1.DeviceCount;
            int idDevice = -1;
            for (int i = 0; i < numDevices; i++)
            {
                string deviceName = axCTiTConnection1.get_DeviceList(i);
                if (deviceName.Equals(numExtension))
                {
                    idDevice = i;
                }
            }
            if (idDevice == -1)
            {
                Console.WriteLine("Error, no se reconocieron los dispositivos de telefonía");
                logger.WriteEntry("Error, no se reconocieron los dispositivos de telefonía");
                return;
            }

            axCTiTLine1.deviceID = idDevice;
            axCTiTLine1.AddressID = 0;
            int mediaMode = axCTiTLine1.DeviceMediaModesAvailable;
            int openType = axCTUtils1.BitwiseNOT(2);
            mediaMode = axCTUtils1.BitwiseAND(mediaMode, openType);

            axCTiTLine1.ServerName = servidorTel;

            int callPrivileges = axCTUtils1.BitwiseOR(2, 4);
            axCTiTLine1.DeviceMediaModesActive = mediaMode;
            axCTiTLine1.DeviceOpen = true;
            int errorNumber = axCTiTLine1.ErrorNumber;
            if (errorNumber == 0)
            {
                Console.WriteLine("Dispositivo " + idDevice + " (ext. " + numExtension + ") abierto");
                logger.WriteEntry("Dispositivo " + idDevice + " (ext. " + numExtension + ") abierto");
            }
            else
            {
                Console.WriteLine("Error abriendo dispositivo");
                logger.WriteEntry("Error abriendo dispositivo");
                return;
            }
            //axCTiTLine1.OnCallerID += new AxCTITCONTROLLib._DCTiTLineEvents_OnCallerIDEventHandler(onCallerIdManageEvent);
            axCTiTLine1.OnCallStateConnected += new AxCTITCONTROLLib._DCTiTLineEvents_OnCallStateConnectedEventHandler(onCallstateConnectedEvent);
            tmProcesarCola.Interval = 5000;
            tmProcesarCola.Tick += new System.EventHandler(tmProcesarCola_Tick);
            tmProcesarCola.Enabled = true;
            Application.Run();

        }

        private static void tmProcesarCola_Tick(object sender, EventArgs e)
        {
            Operaciones.ProcesarColas(logger);
        }


        public static void onCallstateConnectedEvent(object sender, AxCTITCONTROLLib._DCTiTLineEvents_OnCallStateConnectedEvent e)
        {
            AxCTITCONTROLLib.AxCTiTLine axCTiTLine1 = (AxCTITCONTROLLib.AxCTiTLine)sender;
            int idFlags = axCTiTLine1.CallCalledIDFlags;
            if (idFlags == 8)
            {
                string numLlamante = axCTiTLine1.CallCallerIDNumber;
                if (numLlamante.Length > 10)
                {
                    numLlamante = numLlamante.Substring(2);
                }
                Console.WriteLine("Llamada para la extensión " + numExtension + " de la linea: " + numLlamante + " contestada");
                logger.WriteEntry("Llamada para la extensión " + numExtension + " de la linea: " + numLlamante + " contestada");
                Operaciones.RegistrarLlamadaEnCola(numExtension, numLlamante, logger);
            }
        }

    }
}
