using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_PedidosEfectivos
    {
        public Ext_PedidosEfectivos(DataRow fila, string strBoton)
        {
            Dummy = "";
            NUME_LLAMADA = fila["NUME_LLAMADA"].ToString();
            HORA = fila["HORA"].ToString();
            NUME_TELEFONO = fila["NUME_TELEFONO"].ToString();
            ESTADO_LLAMADA = fila["ESTADO_LLAMADA"].ToString();
            MPIO = fila["MPIO"].ToString();
            DIRE_CASO = fila["DIRE_CASO"].ToString();
            BARRIO = fila["BARRIO"].ToString();
            CODIGO1 = fila["CODIGO1"].ToString();
            PRIORIDAD = fila["PRIORIDAD"].ToString();

            COMENTARIO = fila["COMENTARIO"].ToString();
            RECEPTOR = fila["RECEPTOR"].ToString();
            ENVIAR = fila["ENVIAR"].ToString();
            ESTADO = fila["ESTADO"].ToString();
            HORA_CASO = fila["HORA_CASO"].ToString();
            MINUTOS_RESPUESTA = fila["MINUTOS_RESPUESTA"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string NUME_LLAMADA { get; set; }
        public string HORA { get; set; }
        public string NUME_TELEFONO { get; set; }
        public string ESTADO_LLAMADA { get; set; }
        public string MPIO { get; set; }
        public string DIRE_CASO { get; set; }
        public string BARRIO { get; set; }
        public string CODIGO1 { get; set; }
        public string PRIORIDAD { get; set; }

        public string COMENTARIO { get; set; }
        public string RECEPTOR { get; set; }
        public string ENVIAR { get; set; }
        public string ESTADO { get; set; }
        public string HORA_CASO { get; set; }
        public string MINUTOS_RESPUESTA { get; set; }
    }
}