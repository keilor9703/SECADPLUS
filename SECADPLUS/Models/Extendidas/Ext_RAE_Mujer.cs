using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_RAE_Mujer
    {
        public Ext_RAE_Mujer(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;

            PERSATEN_SITIOGRABACION = fila["PERSATEN_SITIOGRABACION"].ToString();
            PERSATEN_CONSECUTIVO = fila["PERSATEN_CONSECUTIVO"].ToString();
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();

            CONSECUTIVO_RAE = fila["CONSECUTIVO"].ToString();
            TIPO_SERV_RAE = fila["TIPO_SERV_RAE"].ToString();
            OBSERVACION_RAE = fila["OBSERVACION"].ToString();

            CADUSUARIO_RAE = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION_RAE = fila["FECHA_TRANSACCION"].ToString();
            DESC_TIPO_SERVICIO_RAE = fila["DESC_TIPO_SERVICIO_RAE"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string PERSATEN_SITIOGRABACION { get; set; }
        public string PERSATEN_CONSECUTIVO { get; set; }
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }

        public string CONSECUTIVO_RAE { get; set; }
        public string TIPO_SERV_RAE { get; set; }
        public string OBSERVACION_RAE { get; set; }

        public string CADUSUARIO_RAE { get; set; }
        public string FECHA_TRANSACCION_RAE { get; set; }
        public string DESC_TIPO_SERVICIO_RAE { get; set; }
    }
}