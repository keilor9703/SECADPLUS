using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_ROI_Mujer
    {
        public Ext_ROI_Mujer(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;

            PERSATEN_SITIOGRABACION = fila["PERSATEN_SITIOGRABACION"].ToString();
            PERSATEN_CONSECUTIVO = fila["PERSATEN_CONSECUTIVO"].ToString();
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();

            CONSECUTIVO_ROI = fila["CONSECUTIVO"].ToString();
            TIPO_SERV_ROI = fila["TIPO_SERV_ROI"].ToString();
            OBSERVACION_ROI = fila["OBSERVACION"].ToString();

            CADUSUARIO_ROI = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION_ROI = fila["FECHA_TRANSACCION"].ToString();
            DESC_TIPO_SERVICIO_ROI = fila["DESC_TIPO_SERVICIO_ROI"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string PERSATEN_SITIOGRABACION { get; set; }
        public string PERSATEN_CONSECUTIVO { get; set; }
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }

        public string CONSECUTIVO_ROI { get; set; }
        public string TIPO_SERV_ROI { get; set; }
        public string OBSERVACION_ROI { get; set; }

        public string CADUSUARIO_ROI { get; set; }
        public string FECHA_TRANSACCION_ROI { get; set; }
        public string DESC_TIPO_SERVICIO_ROI { get; set; }
    }
}