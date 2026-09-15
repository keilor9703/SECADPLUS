using System.Data;

namespace SECADPLUS.Models
{
    public class Pedidos
    {
        #region Propiedades

        public Pedidos(DataRow datos)
        {
            Dummy = "";
            NUME_LLAMADA = (string.IsNullOrEmpty(datos["NUME_LLAMADA"].ToString()) ? "" : datos["NUME_LLAMADA"].ToString());
            HORA_CASO = (string.IsNullOrEmpty(datos["HORA_CASO"].ToString()) ? "" : datos["HORA_CASO"].ToString());
            NUME_TELEFONO = (string.IsNullOrEmpty(datos["NUME_TELEFONO"].ToString()) ? "" : datos["NUME_TELEFONO"].ToString());
            TIPO_PEDIDO = (string.IsNullOrEmpty(datos["TIPO_PEDIDO"].ToString()) ? "" : datos["TIPO_PEDIDO"].ToString());
            NOMB_LLAMANTE = (string.IsNullOrEmpty(datos["NOMB_LLAMANTE"].ToString()) ? "" : datos["NOMB_LLAMANTE"].ToString());
            LUGE_CODIGO = (string.IsNullOrEmpty(datos["LUGE_CODIGO"].ToString()) ? "" : datos["LUGE_CODIGO"].ToString());
            DIRE_CASO = (string.IsNullOrEmpty(datos["DIRE_CASO"].ToString()) ? "" : datos["DIRE_CASO"].ToString());
            COMENTARIO = (string.IsNullOrEmpty(datos["COMENTARIO"].ToString()) ? "" : datos["COMENTARIO"].ToString());
            CODI_PEDIDO = (string.IsNullOrEmpty(datos["CODI_PEDIDO"].ToString()) ? "" : datos["CODI_PEDIDO"].ToString());
            ESTADO = (string.IsNullOrEmpty(datos["ESTADO"].ToString()) ? "" : datos["ESTADO"].ToString());
            CADUSUA_USUARIO = (string.IsNullOrEmpty(datos["CADUSUA_USUARIO"].ToString()) ? "" : datos["CADUSUA_USUARIO"].ToString());
        }

        public Pedidos(DataRow datos, string strBoton)
        {
            NUME_LLAMADA = (string.IsNullOrEmpty(datos["NUME_LLAMADA"].ToString()) ? "" : datos["NUME_LLAMADA"].ToString());
            HORA_CASO = (string.IsNullOrEmpty(datos["HORACASO"].ToString()) ? "" : datos["HORACASO"].ToString());
            NUME_TELEFONO = (string.IsNullOrEmpty(datos["NUME_TELEFONO"].ToString()) ? "" : datos["NUME_TELEFONO"].ToString());
            Boton = strBoton;
            NOMB_LLAMANTE = (string.IsNullOrEmpty(datos["NOMB_LLAMANTE"].ToString()) ? "" : datos["NOMB_LLAMANTE"].ToString());
            CADUSUA_USUARIO = (string.IsNullOrEmpty(datos["CADUSUA_USUARIO"].ToString()) ? "" : datos["CADUSUA_USUARIO"].ToString());
            PROP_TELEFONO = (string.IsNullOrEmpty(datos["PROP_TELEFONO"].ToString()) ? "" : datos["PROP_TELEFONO"].ToString());
            SITIO_GRABA = (string.IsNullOrEmpty(datos["SITIO_GRABA"].ToString()) ? "" : datos["SITIO_GRABA"].ToString());
            CADPEDI_NUME_LLAMADA = (string.IsNullOrEmpty(datos["CADPEDI_NUME_LLAMADA"].ToString()) ? "" : datos["CADPEDI_NUME_LLAMADA"].ToString());
            CODI_PEDIDO = datos["CODI_PEDIDO"].ToString();
            COMENTARIO = datos["COMENTARIO"].ToString();
        }
        
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string HORA_CASO { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string TIPO_PEDIDO { get; set; }
        public string NOMB_LLAMANTE { get; set; }
        public string LUGE_CODIGO { get; set; }
        public string DIRE_CASO { get; set; }
        public string COMENTARIO { get; set; }
        public string CODI_PEDIDO { get; set; }
        public string ESTADO { get; set; }
        public string CADUSUA_USUARIO { get; set; }
        public string PROP_TELEFONO { get; set; }
        public string SITIO_GRABA { get; set; }
        public string CADPEDI_NUME_LLAMADA { get; set; }

        #endregion Propiedades
    }
}