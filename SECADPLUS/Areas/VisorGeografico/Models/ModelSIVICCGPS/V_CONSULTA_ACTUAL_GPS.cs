using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SECADPLUS.Areas.VisorGeografico.Models.ModelSIVICCGPS
{
    [Serializable()]
    [Table(nameof(V_CONSULTA_ACTUAL_GPS), Schema = "USR4DBLSECADPLUS2CUADRANTES")]
    public class V_CONSULTA_ACTUAL_GPS
    {
        #region Propiedades mapeadas
        /// <summary>
        /// Consecutivo (se consolida en el servidor y se ignora el del dispositivo)
        /// </summary>
        [Column("UBICACION_GPS_ID"), Key]
        public long? UbicacionGpsId { get; set; }

        /// <summary>
        /// UID del registro móvil compuesto por el IMEI y el Id del registro en el móvil, unidos por underscore, cuando el registro se guarda de inmediato en servidor, el ID se reemplaza por DateTime.Now.Ticks ya que no viene ID de la base de datos
        /// </summary>
        [Column("UID_REGISTRO")]
        public string UidRegistro { get; set; }

        /// <summary>
        /// Fecha en la que se almacenó el registro (captura de posición)
        /// </summary>
        [Column("FECHA")]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Latitud
        /// </summary>
        [Column("LATITUD")]
        public string Latitud { get; set; }

        /// <summary>
        /// Longitud
        /// </summary>
        [Column("LONGITUD")]
        public string Longitud { get; set; }

        /// <summary>
        /// Velocidad
        /// </summary>
        [Column("VELOCIDAD")]
        public int Velocidad { get; set; }

        /// <summary>
        /// Tipo de lectura (GPS o AGPS)
        /// </summary>
        [Column("TIPO_LECTURA")]
        public string TipoLectura { get; set; }

        /// <summary>
        /// Usuario empresarial que realizó el último inicio de sesión
        /// </summary>
        [Column("USUARIO_SESION")]
        public string UsuarioSesion { get; set; }

        /// <summary>
        /// Indica si el usuario tenía una sesión activa al momento de la captura de posición
        /// </summary>
        [Column("SESION_ACTIVA")]
        public bool SesionActiva { get; set; }

        /// <summary>
        /// Indica el IMEI del equipo
        /// </summary>
        [Column("IMEI")]
        public string Imei { get; set; }

        /// <summary>
        /// Indica el serial de la SIM Card del equipo
        /// </summary>
        [Column("SERIAL_SIM")]
        public string SerialSim { get; set; }

        /// <summary>
        /// Indica el serial de la SIM Card del equipo
        /// </summary>
        [Column("NIVEL_BATERIA")]
        public int NivelBateria { get; set; }

        /// <summary>
        /// Identificador del cuadrante en que operaba el móvil cuando se registró la ubicación
        /// </summary>
        [Column("CUADRANTE_ID")]
        public int? CuadranteId { get; set; }

        /// <summary>
        /// Versión del aplicativo instalado en el dispositivo móvil
        /// </summary>
        [Column("VERSION_APK")]
        public string VersionApk { get; set; }

        /// <summary>
        /// Fecha en la que se instaló el aplicativo
        /// </summary>
        [Column("FECHA_INSTALACION")]
        public DateTime? FechaInstalacion { get; set; }

        /// <summary>
        /// Dirección IP desde donde se realizó la sincronización (detectada desde el servidor, no desde el dispositivo)
        /// </summary>
        [Column("DIRECCION_IP")]
        public string DireccionIP { get; set; }

        /// <summary>
        /// Identificación del empleado en sesión
        /// </summary>
        [Column("IDENTIFICACION")]
        public long? Identificacion { get; set; }

        /// <summary>
        /// Nombres y apellidos del empleado en sesión
        /// </summary>
        [Column("NOMBRES_APELLIDOS")]
        public string NombresApellidos { get; set; }

        /// <summary>
        /// Código de cuadrante del empleado en sesión
        /// </summary>
        [Column("CODIGO_CUADRANTE")]
        public string CodigoCuadrante { get; set; }

        /// <summary>
        /// Número celular del cuadrante del empleado en sesión
        /// </summary>
        [Column("NUMERO_CELULAR_CUADRANTE")]
        public long? NumeroCelularCuadrante { get; set; }

        /// <summary>
        /// Identificador de la unidad del cuadrante (asignado en la minuta digital)
        /// </summary>
        [Column("DEPENDENCIA_ID")]
        public int? DependenciaId { get; set; }

        /// <summary>
        /// Sigla de la estación de policía
        /// </summary>
        [Column("SIGLA_UNIDAD")]
        public string DependenciaSigla { get; set; }

        /// <summary>
        /// Nombre de la estación de policía
        /// </summary>
        [Column("DESCRIPCION_UNIDAD")]
        public string DependenciaDescripcion { get; set; }

        /// <summary>
        /// SiglaPapa de la unidad
        /// </summary>
        [Column("SIGLA_PAPA_UNIDAD")]
        public string DependenciaSiglaPapa { get; set; }

        /// <summary>
        /// Número de serie del dispositivo
        /// </summary>
        [Column("DISPOSITIVO_SERIE")]
        public string DispositivoSerie { get; set; }

        /// <summary>
        /// Marca y modelo del dispositivo
        /// </summary>
        [Column("DISPOSITIVO_MARCA_MODELO")]
        public string DispositivoMarcaModelo { get; set; }


        /// <summary>
        /// Región
        /// </summary>
        [Column("DESC_REGION")]
        public string DescRegion { get; set; }

        /// <summary>
        /// Campo que determina si el punto está dentro del cuadrante (true) o no (false), si está null, el procesador no ha realizado la consulta
        /// </summary>
        [Column("ESTA_DENTRO_CUADRANTE")]
        public bool? EstaDentroCuadrante { get; set; } = null;

        /// <summary>
        /// Duracion
        /// </summary>
        [Column("DURACION")]
        public int Duracion { get; set; }

        /// <summary>
        /// Duracion
        /// </summary>
        [Column("MINUTOS")]
        public string Minutos { get; set; }
        #endregion

    }
}