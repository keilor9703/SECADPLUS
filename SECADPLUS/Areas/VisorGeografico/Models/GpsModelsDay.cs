using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECADPLUS.Areas.VisorGeografico.Models
{
    public class GpsModelsDay
    {
        public int UBICACION_GPS_ID { get; set; }
        public string UID_REGISTRO { get; set; }
        public DateTime FECHA { get; set; }
        public string LATITUD { get; set; }
        public string LONGITUD { get; set; }
        public int VELOCIDAD { get; set; }
        public string TIPO_LECTURA { get; set; }
        public string USUARIO_SESION { get; set; }
        public bool SESION_ACTIVA { get; set; }
        public string IMEI { get; set; }
        public string SERIAL_SIM { get; set; }
        public int NIVEL_BATERIA { get; set; }
        public int CUADRANTE_ID { get; set; }
        public string VERSION_APK { get; set; }
        public DateTime FECHA_INSTALACION { get; set; }
        public string DIRECCION_IP { get; set; }
        public int ESTA_DENTRO_CUADRANTE { get; set; }
        public Int64 IDENTIFICACION { get; set; }
        public string NOMBRES_APELLIDOS { get; set; }
        public string CODIGO_CUADRANTE { get; set; }
        public string CELULAR_CUADRANTE { get; set; }
        public int DEPENDENCIA_ID { get; set; }
        public string SIGLA_UNIDAD { get; set; }
        public string DESCRIPCION_UNIDAD { get; set; }
        public string SIGLA_PAPA_UNIDAD { get; set; }
        public string DISPOSITIVO_SERIE { get; set; }
        public string DISPOSITIVO_MARCA_MODELO { get; set; }
        public decimal DURACION { get; set; }
        public string MINUTOS { get; set; }
        public string DESC_REGION { get; set; }
    }
}