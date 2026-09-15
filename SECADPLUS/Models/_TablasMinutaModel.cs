using System.Data;

namespace SECADPLUS.Models
{
    public class _TablasMinutaModel
    {
    }

    public class EventoConducta
    {
        public EventoConducta(DataRow fila, string strBoton)
        {
            Boton = strBoton;
            Sitio = (string.IsNullOrEmpty(fila["cadeven_sitiograba"].ToString()) ? "" : (fila["cadeven_sitiograba"].ToString()));
            Evento = (string.IsNullOrEmpty(fila["cadeven_nume_evento"].ToString()) ? "" : (fila["cadeven_nume_evento"].ToString()));
            Llamada = (string.IsNullOrEmpty(fila["cadpedca_numellamada"].ToString()) ? "" : (fila["cadpedca_numellamada"].ToString()));
            LlamadaSitio = (string.IsNullOrEmpty(fila["cadpedca_sitiograba"].ToString()) ? "" : (fila["cadpedca_sitiograba"].ToString()));
            HoraCaso = (string.IsNullOrEmpty(fila["hora_caso"].ToString()) ? "" : (fila["hora_caso"].ToString()));
            DireccionCaso = (string.IsNullOrEmpty(fila["dire_caso"].ToString()) ? "" : (fila["dire_caso"].ToString()));
            Ciudad = (string.IsNullOrEmpty(fila["ciudad"].ToString()) ? "" : (fila["ciudad"].ToString()));
            CodigoPedido = (string.IsNullOrEmpty(fila["codi_pedido"].ToString()) ? "" : (fila["codi_pedido"].ToString()));
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string Sitio { get; set; }
        public string Evento { get; set; }
        public string Llamada { get; set; }
        public string LlamadaSitio { get; set; }
        public string HoraCaso { get; set; }
        public string DireccionCaso { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPedido { get; set; }
    }

    public class Infraccion
    {
        public Infraccion(DataRow fila, string strBoton)
        {
            Codigo = fila["CODIGO"].ToString() != string.Empty ? fila["CODIGO"].ToString() : "";
            Descripcion = fila["DESCRIPCION"].ToString() != string.Empty ? fila["DESCRIPCION"].ToString() : "";
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }
}