using SECADEntities.SecadEntities;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtPedidoComentario : CADPEDIDOS_INFORMACION
    {
        public ExtPedidoComentario(DataRow fila, string strBoton)
        {
            ANOTACION = fila["ANOTACION"].ToString();
            USUA_USUARIO = fila["USUA_USUARIO"].ToString();
            Boton = strBoton;
            Fecha = fila["FECHA_GRABACION"].ToString();
        }

        public string Dummy { get; set; }
        public string Fecha { get; set; }
        public string Boton { get; set; }
    }
}