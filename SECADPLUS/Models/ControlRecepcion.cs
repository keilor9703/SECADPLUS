using System.Data;

namespace SECADPLUS.Models
{
    public class ControlRecepcion
    {
        #region Propiedades

        public ControlRecepcion(DataRow datos)
        {
            Dummy = "";
            USUARIO = (string.IsNullOrEmpty(datos["USUARIO"].ToString()) ? "" : datos["USUARIO"].ToString());
            NOMBRE = (string.IsNullOrEmpty(datos["NOMBRE"].ToString()) ? "" : datos["NOMBRE"].ToString());
            LOGIN = (string.IsNullOrEmpty(datos["LOGIN"].ToString()) ? "" : datos["LOGIN"].ToString());
            EQUIPO = (string.IsNullOrEmpty(datos["EQUIPO"].ToString()) ? "" : datos["EQUIPO"].ToString());
            UBICACION = (string.IsNullOrEmpty(datos["UBICACION"].ToString()) ? "" : datos["UBICACION"].ToString());
            TURNO = (string.IsNullOrEmpty(datos["TURNO"].ToString()) ? "" : datos["TURNO"].ToString());
            CLASE_TURNO = (string.IsNullOrEmpty(datos["CLASE_TURNO"].ToString()) ? "" : datos["CLASE_TURNO"].ToString());
            SITIO_GRABA = (string.IsNullOrEmpty(datos["SITIO_GRABA"].ToString()) ? "" : datos["SITIO_GRABA"].ToString());
            RECIBIDAS = (string.IsNullOrEmpty(datos["RECIBIDAS"].ToString()) ? "" : datos["RECIBIDAS"].ToString());
            EFECTIVAS = (string.IsNullOrEmpty(datos["EFECTIVAS"].ToString()) ? "" : datos["EFECTIVAS"].ToString());
            ULTIMA_FECHA = (string.IsNullOrEmpty(datos["ULTIMA_FECHA"].ToString()) ? "" : datos["ULTIMA_FECHA"].ToString());
            PEDIDO_SIN_ENVIO = (string.IsNullOrEmpty(datos["PEDIDO_SIN_ENVIO"].ToString()) ? "" : datos["PEDIDO_SIN_ENVIO"].ToString());
            TIEMPO_SEG = (string.IsNullOrEmpty(datos["TIEMPO_SEG"].ToString()) ? "" : datos["TIEMPO_SEG"].ToString());
        }

        public string Dummy { get; set; }
        public string USUARIO { get; set; }
        public string NOMBRE { get; set; }
        public string LOGIN { get; set; }
        public string EQUIPO { get; set; }
        public string UBICACION { get; set; }
        public string TURNO { get; set; }
        public string CLASE_TURNO { get; set; }
        public string SITIO_GRABA { get; set; }
        public string RECIBIDAS { get; set; }
        public string EFECTIVAS { get; set; }
        public string ULTIMA_FECHA { get; set; }
        public string PEDIDO_SIN_ENVIO { get; set; }
        public string TIEMPO_SEG { get; set; }

        #endregion Propiedades
    }
}