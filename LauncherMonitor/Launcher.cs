using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace LauncherMonitor
{
    class Launcher
    {
        static Hashtable listaProcesos = new Hashtable();
        private static System.Windows.Forms.Timer tmBuscarProcesos = new System.Windows.Forms.Timer();
        private static System.Diagnostics.EventLog logger;
        static Mutex mutex = new Mutex(true, "{8F6F0DA0-B9A1-45fd-A8CF-72AB1E6BD1DA}");
        private static bool finalizar = false;
        //private static string EQUIPO_MONITOR = "ASIESDEANT07";

        [STAThread]
        static void Main(string[] args)
        {
            //SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            logger = new EventLog();
            if (!EventLog.SourceExists("SecadPlusSource"))
            {
                EventLog.CreateEventSource(
                    "SecadPlusMonitor", "SecadPlusLog");
            }
            logger.Source = "SecadPlusSource";
            logger.Log = "SecadPlusLog";

            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Console.WriteLine("Solo se permite una instancia del launcher");
                logger.WriteEntry("Solo se permite una instancia del launcher");
                return;
            }

            Console.WriteLine("Iniciado launcher de monitor");
            logger.WriteEntry("Iniciado launcher de monitor");

            Thread thread = new Thread(MessagePumpThread);
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (!finalizar)
            {
                Thread.Sleep(1000);
            }
        }

        private static void MessagePumpThread()
        {
            AxCTITCONTROLLib.AxCTiTConnection axCTiTConnection1 = new AxCTITCONTROLLib.AxCTiTConnection();
            axCTiTConnection1.CreateControl();

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
            string carpetaActual = Directory.GetCurrentDirectory();
            logger.WriteEntry("Numero de dispositivos: " + numDevices);
            for (int i = 0; i < numDevices; i++)
            {
                string deviceName = axCTiTConnection1.get_DeviceList(i);
                logger.WriteEntry("dispositivo: " + i + " : " + deviceName);
                int numCaracteres = Regex.Matches(deviceName, @"[a-zA-Z]").Count;
                if (numCaracteres == 0)
                {
                    try
                    {
                        //Ejecutar Monitor
                        idDevice = i;
                        Process procesoMonitor = new Process();
                        procesoMonitor.StartInfo.FileName = "C:\\ITO\\Servicio\\MonitorLinea.exe";
                        procesoMonitor.StartInfo.Arguments = deviceName;
                        procesoMonitor.StartInfo.UseShellExecute = false;
                        procesoMonitor.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        listaProcesos.Add(deviceName, procesoMonitor);
                        procesoMonitor.Start();
                    }
                    catch (Exception ex)
                    {
                        logger.WriteEntry("Error al crear el proceso para la extensión " + deviceName + " carpeta: " + carpetaActual + " : " + ex.ToString());
                    }
                }
            }
            if (idDevice == -1)
            {
                Console.WriteLine("Error, no se reconocieron los dispositivos de telefonía");
                logger.WriteEntry("Error, no se reconocieron los dispositivos de telefonía");
                finalizar = true;
            }
            tmBuscarProcesos.Interval = 5000;
            tmBuscarProcesos.Tick += new System.EventHandler(tmBuscarProcesos_Tick);
            tmBuscarProcesos.Enabled = true;
            Application.Run();
        }

        private static void tmBuscarProcesos_Tick(object sender, EventArgs e)
        {
            List<string> diedExtensionProcList = new List<string>();
            foreach (string extension in listaProcesos.Keys)
            {
                Process procesoMonitor = (Process)listaProcesos[extension];
                if (procesoMonitor.HasExited)
                {
                    diedExtensionProcList.Add(extension);
                }
            }
            if (diedExtensionProcList.Count > 0)
            {
                string carpetaActual = Directory.GetCurrentDirectory();
                foreach (string extension in diedExtensionProcList)
                {
                    try
                    {
                        Console.WriteLine("Reiniciando monitoreo extensión " + extension);
                        logger.WriteEntry("Reiniciando monitoreo extensión " + extension);
                        listaProcesos.Remove(extension);
                        Process procesoMonitor = new Process();
                        procesoMonitor.StartInfo.FileName = "C:\\ITO\\Servicio\\MonitorLinea.exe";
                        procesoMonitor.StartInfo.Arguments = extension;
                        procesoMonitor.StartInfo.UseShellExecute = false;
                        procesoMonitor.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        listaProcesos.Add(extension, procesoMonitor);
                        procesoMonitor.Start();
                    }
                    catch (Exception ex)
                    {
                        logger.WriteEntry("Error al crear el proceso para la extensión " + extension + " : " + ex.ToString());
                    }
                }
            }
        }

        /*
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        //Método que se ejecuta al cerrar el programa
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            logger.WriteEntry("Kill invoked");
            tmBuscarProcesos.Enabled = false;
            return true;
        }

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        */
    }
}
