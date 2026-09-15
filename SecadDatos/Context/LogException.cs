using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadDatos.Context
{
    public class LogException
    {
        public enum TipoMensaje
        {
            /// <summary>
            /// Tipo de mensaje de depuración.
            /// </summary>
            M_DEBUG,
            /// <summary>
            /// Tipo de mensaje de información.
            /// </summary>
            M_INFO,
            /// <summary>
            /// Tipo de mensaje de adventencia.
            /// </summary>
            M_WARNING,
            /// <summary>
            /// Tipo de mensaje de error.
            /// </summary>
            M_ERROR,
        }

        private const Int32 TAM_FICHERO = 2048000;

        private const string EXTENSION = ".log";

        private static TipoMensaje eNivel = TipoMensaje.M_DEBUG;

        /// <summary>Nivel de registro.</summary>
        /// <value>Establece el nivel de registro.</value>
        /// <returns>Obtiene el nivel de registro.</returns>
        public static TipoMensaje Nivel
        {
            get { return eNivel; }
            set { eNivel = value; }
        }


        /// <summary>
        /// Añade un mensaje al registro, indicando el tipo de mensaje y pudiendo indicar de forma
        /// opcional el nombre del archivo y la línea donde se lanzó el mensaje.
        /// </summary>
        /// <param name="eTipo">Tipo de mensaje a guardar en el registro.</param>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        private static void add(TipoMensaje eTipo, string sMensaje)
        {
            if (eTipo >= eNivel)
            {
                // Nombre de archivo destino.
                string sDestino = getFileFecha();

                // Comprueba si el archivo está lleno, en cuyo caso hace una copia del
                // archivo existente y comienza a rellenar uno nuevo.
                verificar(sDestino);

                // archivo en el que almacenar el registro.
                FileStream objFichero = new FileStream(sDestino, FileMode.Append, FileAccess.Write);

                if ((objFichero != null) & objFichero.CanWrite)
                {
                    // Mensaje completo a guardar.
                    string sDeb = string.Empty;
                    // Fecha actua.
                    System.DateTime objFecha = System.DateTime.Now;

                    // Composición y guardado en el archivo de la fecha y tipo
                    sDeb = objFecha.ToString("yyyyMMdd HH:mm:ss") + " [" + eTipo.ToString() + "] - " + sMensaje;

                    // Escribe y cierra
                    StreamWriter objStream = new StreamWriter(objFichero);
                    objStream.WriteLine(sDeb);
                    objStream.Close();
                    objFichero.Close();
                }
            }
        }


        /// <summary>Obtiene el nombre de archivo actual.</summary>
        /// <returns>Cadena con el nombre del archivo.</returns>
        private static string getFileFecha()
        {
            System.DateTime objFecha = System.DateTime.Now;
            string sCadena = string.Empty;
            sCadena = "Log_";

            // Compone el nombre de archivo de la forma AAAAMMDD.log
            sCadena += objFecha.Year;
            if (objFecha.Month < 10)
            {
                sCadena += "0";
            }
            sCadena += objFecha.Month;
            if (objFecha.Day < 10)
            {
                sCadena += "0";
            }
            sCadena += objFecha.Day;
            sCadena += EXTENSION;

            //Solo retorna nombre del archivo de Log
            //Return sCadena


            //Obtiene ruta de ejecucion de la aplicacion
            //Dim dirLogs As String = My.Application.Info.DirectoryPath & "/Logs/"
            //If Not My.Computer.FileSystem.DirectoryExists(path) Then
            //    My.Computer.FileSystem.CreateDirectory(path)
            //End If
            //Return dirLogs & sCadena


            //Permite Guardar el archivo en el mismo directorio de la aplicacion
            string dirLogs = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/");
            if (!Directory.Exists(dirLogs))
            {
                try
                {
                    Directory.CreateDirectory(dirLogs);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return dirLogs + sCadena;

        }

        public bool CreateDirectoryRecursively(string path)
        {
            try
            {
                string[] pathParts = path.Split('\\');
                for (var i = 0; i < pathParts.Length; i++)
                {
                    // Correct part for drive letters
                    if (i == 0 && pathParts[i].Contains(":"))
                    {
                        pathParts[i] = pathParts[i] + "\\";
                    } // Do not try to create last part if it has a period (is probably the file name)
                    else if (i == pathParts.Length - 1 && pathParts[i].Contains("."))
                    {
                        return true;
                    }
                    if (i > 0)
                    {
                        pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);
                    }
                    if (!Directory.Exists(pathParts[i]))
                    {
                        Directory.CreateDirectory(pathParts[i]);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        /// <summary>Analiza si el archivo es demasiado grande y hace una copia de seguridad.</summary>
        /// <param name="sFichero">archivo a analizar.</param>
        private static void verificar(string sFichero)
        {
            try
            {
                FileInfo Info = new FileInfo(sFichero);
                if (Info.Length >= TAM_FICHERO)
                {
                    File.Move(sFichero, sFichero + ".old");
                }
            }
            catch (IOException ex)
            {
            }
        }


        /// <summary>Inserta un mensaje de error en el registro.</summary>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        public static void err(string sMensaje, Exception objExcepcion)
        {
            add(TipoMensaje.M_ERROR, sMensaje + Environment.NewLine + "\t#2\t#3" + objExcepcion.Message + Environment.NewLine + "\t#2\t#3" + objExcepcion.ToString());
        }


        /// <summary>Inserta un mensaje de error en el registro.</summary>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        public static void err(string sMensaje)
        {
            add(TipoMensaje.M_ERROR, sMensaje);
        }


        /// <summary>Inserta un mensaje de advertencia en el registro.</summary>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        public static void warning(string sMensaje)
        {
            add(TipoMensaje.M_WARNING, sMensaje);
        }


        /// <summary>Inserta un mensaje de información en el registro.</summary>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        public static void inf(string sMensaje)
        {
            add(TipoMensaje.M_INFO, sMensaje);
        }


        /// <summary>Inserta un mensaje de depuración en el registro.</summary>
        /// <param name="sMensaje">Mensaje a guardar en el registro.</param>
        public static void debug(string sMensaje)
        {
            add(TipoMensaje.M_DEBUG, sMensaje);
        }
    }
}