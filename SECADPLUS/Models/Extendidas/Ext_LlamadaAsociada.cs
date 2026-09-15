using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_LlamadaAsociada
    {
        public Ext_LlamadaAsociada(DataRow fila, string strBoton)
        {
            Llamada = fila["NUME_LLAMADA"].ToString();
            HoraCaso = fila["HORA_CASO"].ToString();
            NumeTelefono = fila["NUME_TELEFONO"].ToString();
            Calidad = fila["CALI_PEDIDO"].ToString();
            Ciudad = fila["CIUDAD"].ToString();
            Llamante = fila["NOMB_LLAMANTE"].ToString();
            Dire_Caso = fila["DIRE_CASO"].ToString();
            CodiPedido = fila["CODI_PEDIDO"].ToString();
            Estado = fila["ESTADO"].ToString();
            Sitio = fila["SITIO_GRABA"].ToString();
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Llamada { get; set; }
        public string HoraCaso { get; set; }
        public string NumeTelefono { get; set; }
        public string Calidad { get; set; }
        public string Ciudad { get; set; }
        public string Llamante { get; set; }
        public string Dire_Caso { get; set; }
        public string CodiPedido { get; set; }
        public string Estado { get; set; }
        public string Sitio { get; set; }
    }
}