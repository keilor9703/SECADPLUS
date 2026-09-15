using System.Data;

namespace SECADPLUS.Models
{
    public class MapaPedidosUbicacion
    {
        #region Propiedades

        public MapaPedidosUbicacion(DataRow datos)
        {
            TIME_ = (string.IsNullOrEmpty(datos["FECHA_HORA"].ToString()) ? "" : datos["FECHA_HORA"].ToString());
            REG_CONSECUTIVO = (string.IsNullOrEmpty(datos["REG_CONSECUTIVO"].ToString()) ? "" : datos["REG_CONSECUTIVO"].ToString());
            ESTADO = (string.IsNullOrEmpty(datos["ESTADO"].ToString()) ? "" : datos["ESTADO"].ToString());
            DESC_ESTADO = (string.IsNullOrEmpty(datos["DESC_ESTADO"].ToString()) ? "" : datos["DESC_ESTADO"].ToString());
            USUARIO_GRABA = (string.IsNullOrEmpty(datos["USUARIO_GRABA"].ToString()) ? "" : datos["USUARIO_GRABA"].ToString());
            CADPEDI_SITIOGRABA = (string.IsNullOrEmpty(datos["CADPEDI_SITIOGRABA"].ToString()) ? "" : datos["CADPEDI_SITIOGRABA"].ToString());
            CADPEDI_NUMELLAMADA = (string.IsNullOrEmpty(datos["CADPEDI_NUMELLAMADA"].ToString()) ? "" : datos["CADPEDI_NUMELLAMADA"].ToString());
            IMPORTANCIA = datos["IMPORTANCIA"].ToString();
            PRIORIDAD = datos["PRIORIDAD"].ToString();
            TIEMPO_ASIGNA = datos["TIEMPO_ASIGNA"].ToString();            
        }
        public string TIME_ { get; set; }
        public string REG_CONSECUTIVO { get; set; }
        public string ESTADO { get; set; }
        public string DESC_ESTADO { get; set; }
        public string USUARIO_GRABA { get; set; }
        public string CADPEDI_SITIOGRABA { get; set; }
        public string CADPEDI_NUMELLAMADA { get; set; }
        public string IMPORTANCIA { get; set; }
        public string PRIORIDAD { get; set; }
        public string TIEMPO_ASIGNA { get; set; }

        #endregion Propiedades
    }
}