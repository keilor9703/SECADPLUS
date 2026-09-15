using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_RM_Mujer
    {
        public Ext_RM_Mujer(DataRow fila, string strBoton)
        {
            Dummy = "";
            Boton = strBoton;

            PERSATEN_SITIOGRABACION = fila["PERSATEN_SITIOGRABACION"].ToString();
            PERSATEN_CONSECUTIVO = fila["PERSATEN_CONSECUTIVO"].ToString();
            EVEN_SITIOGRABACION = fila["EVEN_SITIOGRABACION"].ToString();
            EVEN_NUMEEVENTO = fila["EVEN_NUMEEVENTO"].ToString();

            CONSECUTIVO_RM = fila["CONSECUTIVO"].ToString();
            TIPO_SERV_RM = fila["TIPO_SERV_RM"].ToString();
            OBSERVACION_RM = fila["OBSERVACION"].ToString();

            CADUSUARIO_RAE = fila["CADUSUARIO"].ToString();
            FECHA_TRANSACCION_RM = fila["FECHA_TRANSACCION"].ToString();
            DESC_TIPO_SERVICIO_RM = fila["DESC_TIPO_SERVICIO_RM"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string PERSATEN_SITIOGRABACION { get; set; }
        public string PERSATEN_CONSECUTIVO { get; set; }
        public string EVEN_SITIOGRABACION { get; set; }
        public string EVEN_NUMEEVENTO { get; set; }

        public string CONSECUTIVO_RM { get; set; }
        public string TIPO_SERV_RM { get; set; }
        public string OBSERVACION_RM { get; set; }

        public string CADUSUARIO_RAE { get; set; }
        public string FECHA_TRANSACCION_RM { get; set; }
        public string DESC_TIPO_SERVICIO_RM { get; set; }
    }
}