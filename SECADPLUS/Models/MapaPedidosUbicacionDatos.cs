using System.Data;

namespace SECADPLUS.Models
{
    public class MapaPedidosUbicacionDatos
    {
        #region Propiedades

        public MapaPedidosUbicacionDatos(DataRow datos)
        {
            CADPEDI_SITIOGRABA = (string.IsNullOrEmpty(datos["CADPEDI_SITIOGRABA"].ToString()) ? "" : datos["CADPEDI_SITIOGRABA"].ToString());
            CADPEDI_NUMELLAMADA = (string.IsNullOrEmpty(datos["CADPEDI_NUMELLAMADA"].ToString()) ? "" : datos["CADPEDI_NUMELLAMADA"].ToString());
            REG_CONSECUTIVO = (string.IsNullOrEmpty(datos["REG_CONSECUTIVO"].ToString()) ? "" : datos["REG_CONSECUTIVO"].ToString());
            MSID = (string.IsNullOrEmpty(datos["MSID"].ToString()) ? "" : datos["MSID"].ToString());
            TIME_ = (string.IsNullOrEmpty(datos["TIME_"].ToString()) ? "" : datos["TIME_"].ToString());
            SHAPE = (string.IsNullOrEmpty(datos["SHAPE"].ToString()) ? "" : datos["SHAPE"].ToString());
            COORD_X = (string.IsNullOrEmpty(datos["COORD_X"].ToString()) ? "" : datos["COORD_X"].ToString());
            COORD_Y = (string.IsNullOrEmpty(datos["COORD_Y"].ToString()) ? "" : datos["COORD_Y"].ToString());
            RADIUS = (string.IsNullOrEmpty(datos["RADIUS"].ToString()) ? "" : datos["RADIUS"].ToString());
            INRADIUS = (string.IsNullOrEmpty(datos["INRADIUS"].ToString()) ? "" : datos["INRADIUS"].ToString());
            OUTRADIUS = (string.IsNullOrEmpty(datos["OUTRADIUS"].ToString()) ? "" : datos["OUTRADIUS"].ToString());
            STARTANGLE = (string.IsNullOrEmpty(datos["STARTANGLE"].ToString()) ? "" : datos["STARTANGLE"].ToString());
            STOPANGLE = (string.IsNullOrEmpty(datos["STOPANGLE"].ToString()) ? "" : datos["STOPANGLE"].ToString());
            ANGLE = (string.IsNullOrEmpty(datos["ANGLE"].ToString()) ? "" : datos["ANGLE"].ToString());
            SEMIMAJOR = (string.IsNullOrEmpty(datos["SEMIMAJOR"].ToString()) ? "" : datos["SEMIMAJOR"].ToString());
            SEMIMINOR = (string.IsNullOrEmpty(datos["SEMIMINOR"].ToString()) ? "" : datos["SEMIMINOR"].ToString());
            ANGULARUNIT = (string.IsNullOrEmpty(datos["ANGULARUNIT"].ToString()) ? "" : datos["ANGULARUNIT"].ToString());
            OPERADOR = (string.IsNullOrEmpty(datos["OPERADOR"].ToString()) ? "" : datos["OPERADOR"].ToString());
            TIPO_REGISTRO = (string.IsNullOrEmpty(datos["TIPO_REGISTRO"].ToString()) ? "" : datos["TIPO_REGISTRO"].ToString());
            LONGITUD_XDEC = (string.IsNullOrEmpty(datos["LONGITUD_XDEC"].ToString()) ? "" : datos["LONGITUD_XDEC"].ToString());
            LATITUD_YDEC = (string.IsNullOrEmpty(datos["LATITUD_YDEC"].ToString()) ? "" : datos["LATITUD_YDEC"].ToString());
            USUARIO_GRABA = (string.IsNullOrEmpty(datos["USUARIO_GRABA"].ToString()) ? "" : datos["USUARIO_GRABA"].ToString());
            FECHA_GRABACION = (string.IsNullOrEmpty(datos["FECHA_GRABACION"].ToString()) ? "" : datos["FECHA_GRABACION"].ToString());
            ESTADO = (string.IsNullOrEmpty(datos["ESTADO"].ToString()) ? "" : datos["ESTADO"].ToString());
            FECHA_MODIFICACION = (string.IsNullOrEmpty(datos["FECHA_MODIFICACION"].ToString()) ? "" : datos["FECHA_MODIFICACION"].ToString());

            CONSECUTIVO = (string.IsNullOrEmpty(datos["CONSECUTIVO"].ToString()) ? "" : datos["CONSECUTIVO"].ToString());
            P_LONGITUD_XDEC = (string.IsNullOrEmpty(datos["LONGITUD_XDEC"].ToString()) ? "" : datos["P_LONGITUD_XDEC"].ToString());
            P_LATITUD_YDEC = (string.IsNullOrEmpty(datos["LATITUD_YDEC"].ToString()) ? "" : datos["P_LATITUD_YDEC"].ToString());

            DESC_ESTADO = (string.IsNullOrEmpty(datos["DESC_ESTADO"].ToString()) ? "" : datos["DESC_ESTADO"].ToString());
            LATLON = (string.IsNullOrEmpty(datos["LATLON"].ToString()) ? "" : datos["LATLON"].ToString());
            COMENTARIO = (string.IsNullOrEmpty(datos["COMENTARIO"].ToString()) ? "" : datos["COMENTARIO"].ToString());
            DIRE_TELEFONO = (string.IsNullOrEmpty(datos["DIRE_TELEFONO"].ToString()) ? "" : datos["DIRE_TELEFONO"].ToString());
            BARRIO = (string.IsNullOrEmpty(datos["BARRIO"].ToString()) ? "" : datos["BARRIO"].ToString());
            NOMBRECASO = (string.IsNullOrEmpty(datos["NOMBRECASO"].ToString()) ? "" : datos["NOMBRECASO"].ToString());
            AREA = (string.IsNullOrEmpty(datos["AREA"].ToString()) ? "" : datos["AREA"].ToString());
        }

        public string CADPEDI_SITIOGRABA { get; set; }
        public string CADPEDI_NUMELLAMADA { get; set; }
        public string REG_CONSECUTIVO { get; set; }
        public string MSID { get; set; }
        public string TIME_ { get; set; }
        public string SHAPE { get; set; }
        public string COORD_X { get; set; }
        public string COORD_Y { get; set; }
        public string RADIUS { get; set; }
        public string INRADIUS { get; set; }
        public string OUTRADIUS { get; set; }
        public string STARTANGLE { get; set; }
        public string STOPANGLE { get; set; }
        public string ANGLE { get; set; }
        public string SEMIMAJOR { get; set; }
        public string SEMIMINOR { get; set; }
        public string ANGULARUNIT { get; set; }
        public string OPERADOR { get; set; }
        public string TIPO_REGISTRO { get; set; }
        public string LONGITUD_XDEC { get; set; }
        public string LATITUD_YDEC { get; set; }
        public string USUARIO_GRABA { get; set; }
        public string FECHA_GRABACION { get; set; }
        public string ESTADO { get; set; }
        public string FECHA_MODIFICACION { get; set; }

        public string CONSECUTIVO { get; set; }
        public string P_LONGITUD_XDEC { get; set; }
        public string P_LATITUD_YDEC { get; set; }

        public string DESC_ESTADO { get; set; }
        public string LATLON { get; set; }
        public string COMENTARIO { get; set; }
        public string DIRE_TELEFONO { get; set; }
        public string BARRIO { get; set; }
        public string NOMBRECASO { get; set; }
        public string AREA { get; set; }

        #endregion Propiedades
    }
}