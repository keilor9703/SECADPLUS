using System.Data;

namespace SECADPLUS.Models
{
    public class Comunicados
    {
        #region Propiedades

        public Comunicados(string strBoton, DataRow datos)
        {
            Dummy = "";
            BOTON = strBoton;
            SITIO_GRABA = (string.IsNullOrEmpty(datos["SITIO_GRABA"].ToString()) ? "" : datos["SITIO_GRABA"].ToString());
            CONSECUTIVO = (string.IsNullOrEmpty(datos["CONSECUTIVO"].ToString()) ? "" : datos["CONSECUTIVO"].ToString());
            CADFUERZ_ID = (string.IsNullOrEmpty(datos["CADFUERZ_ID"].ToString()) ? "" : datos["CADFUERZ_ID"].ToString());
            NUMERO = (string.IsNullOrEmpty(datos["NUMERO"].ToString()) ? "" : datos["NUMERO"].ToString());
            ORIGEN = (string.IsNullOrEmpty(datos["ORIGEN"].ToString()) ? "" : datos["ORIGEN"].ToString());
            AÑO = (string.IsNullOrEmpty(datos["AÑO"].ToString()) ? "" : datos["AÑO"].ToString());
            DESTINO = (string.IsNullOrEmpty(datos["DESTINO"].ToString()) ? "" : datos["DESTINO"].ToString());
            FECHA = (string.IsNullOrEmpty(datos["FECHA"].ToString()) ? "" : datos["FECHA"].ToString());
            TEXTO = (string.IsNullOrEmpty(datos["TEXTO"].ToString()) ? "" : datos["TEXTO"].ToString());
            FIRMA = (string.IsNullOrEmpty(datos["FIRMA"].ToString()) ? "" : datos["FIRMA"].ToString());
            CARGO_FIRMA = (string.IsNullOrEmpty(datos["CARGO_FIRMA"].ToString()) ? "" : datos["CARGO_FIRMA"].ToString());
            TRANSMITE = (string.IsNullOrEmpty(datos["TRANSMITE"].ToString()) ? "" : datos["TRANSMITE"].ToString());
            RECIBE = (string.IsNullOrEmpty(datos["RECIBE"].ToString()) ? "" : datos["RECIBE"].ToString());
            USUARIO = (string.IsNullOrEmpty(datos["USUARIO"].ToString()) ? "" : datos["USUARIO"].ToString());
        }

        public string Dummy { get; set; }
        public string BOTON { get; set; }
        public string SITIO_GRABA { get; set; }
        public string CONSECUTIVO { get; set; }
        public string CADFUERZ_ID { get; set; }
        public string NUMERO { get; set; }
        public string ORIGEN { get; set; }
        public string AÑO { get; set; }
        public string DESTINO { get; set; }
        public string FECHA { get; set; }
        public string TEXTO { get; set; }
        public string FIRMA { get; set; }
        public string CARGO_FIRMA { get; set; }
        public string TRANSMITE { get; set; }
        public string RECIBE { get; set; }
        public string USUARIO { get; set; }

        #endregion Propiedades
    }
}