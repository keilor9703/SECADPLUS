using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadPedidosDocs : CADPEDIDOS_DOCUMENTOS
    {
        public Ext_CadPedidosDocs(DataRow fila, string strBoton, string strBotonImagen)
        {
            CONSECUTIVO = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()) ? (short)0 : Convert.ToInt16(fila["CONSECUTIVO"].ToString()));
            FechaGrabacion = fila["FECHA_GRABACION"].ToString();
            FechaModifica = fila["FECHA_MODIFICACION"].ToString();
            USUA_MODIFICA = fila["USUA_MODIFICA"].ToString();
            RUTA = fila["RUTA"].ToString();
            NOMBRE_ARCHIVO = fila["NOMBRE_ARCHIVO"].ToString();
            Boton = strBoton;
            BotonImagen = strBotonImagen;
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string BotonImagen { get; set; }
        public string FechaGrabacion { get; set; }
        public string FechaModifica { get; set; }
    }
}