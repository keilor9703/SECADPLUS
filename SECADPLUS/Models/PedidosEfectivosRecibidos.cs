using System.Data;

namespace SECADPLUS.Models
{
    public class PedidosEfectivosRecibidos
    {
        #region Propiedades

        public PedidosEfectivosRecibidos(DataRow datos)
        {
            Dummy = "";
            NUME_LLAMADA = (string.IsNullOrEmpty(datos["NUME_LLAMADA"].ToString()) ? "" : datos["NUME_LLAMADA"].ToString());
            NUME_TELEFONO = (string.IsNullOrEmpty(datos["NUME_TELEFONO"].ToString()) ? "" : datos["NUME_TELEFONO"].ToString());
            HORA = (string.IsNullOrEmpty(datos["HORA"].ToString()) ? "" : datos["HORA"].ToString());
            CODIGO1 = (string.IsNullOrEmpty(datos["CODIGO1"].ToString()) ? "" : datos["CODIGO1"].ToString());
            MPIO = (string.IsNullOrEmpty(datos["MPIO"].ToString()) ? "" : datos["MPIO"].ToString());
            DIRE_CASO = (string.IsNullOrEmpty(datos["DIRE_CASO"].ToString()) ? "" : datos["DIRE_CASO"].ToString());
            BARRIO = (string.IsNullOrEmpty(datos["BARRIO"].ToString()) ? "" : datos["BARRIO"].ToString());
            COMENTARIO = (string.IsNullOrEmpty(datos["COMENTARIO"].ToString()) ? "" : datos["COMENTARIO"].ToString());
            TIPO_PEDIDO = (string.IsNullOrEmpty(datos["TIPO_PEDIDO"].ToString()) ? "" : datos["TIPO_PEDIDO"].ToString());
            CALI_PEDIDO = (string.IsNullOrEmpty(datos["CALI_PEDIDO"].ToString()) ? "" : datos["CALI_PEDIDO"].ToString());
            ESTADO_LLAMADA = (string.IsNullOrEmpty(datos["ESTADO_LLAMADA"].ToString()) ? "" : datos["ESTADO_LLAMADA"].ToString());
            TIPO_GENERO = (string.IsNullOrEmpty(datos["TIPO_GENERO"].ToString()) ? "" : datos["TIPO_GENERO"].ToString());
            RECEPTOR = (string.IsNullOrEmpty(datos["RECEPTOR"].ToString()) ? "" : datos["RECEPTOR"].ToString());
        }

        public string Dummy { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string HORA { get; set; }
        public string CODIGO1 { get; set; }
        public string MPIO { get; set; }
        public string DIRE_CASO { get; set; }
        public string BARRIO { get; set; }
        public string COMENTARIO { get; set; }
        public string TIPO_PEDIDO { get; set; }
        public string CALI_PEDIDO { get; set; }
        public string ESTADO_LLAMADA { get; set; }
        public string TIPO_GENERO { get; set; }
        public string RECEPTOR { get; set; }

        #endregion Propiedades
    }
}