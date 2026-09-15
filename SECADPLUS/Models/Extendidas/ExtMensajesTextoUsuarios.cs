using SECADEntities.SECAD;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtMensajesTextoUsuarios : CADMENSAJES_TEXTO_USUARIOS
    {
        public ExtMensajesTextoUsuarios(DataRow fila)
        {
            CADMENTEX_MENS_SITIOGRABA = int.Parse(fila["CADMENTEX_MENS_SITIOGRABA"].ToString());
            CADMENTEX_MENS_CONSECUTIVO = long.Parse(fila["CADMENTEX_MENS_CONSECUTIVO"].ToString());
            CADMENTEX_CONSECUTIVO = long.Parse(fila["CADMENTEX_CONSECUTIVO"].ToString());
            CADUSUA_DESTINO = fila["CADUSUA_DESTINO"].ToString();
            ESTADO = fila["ESTADO"].ToString();

            if (string.IsNullOrEmpty(fila["FECHA_NOTIFICACION"].ToString()))
            {
                FECHA_NOTIFICACION = null;
            }
            else
            {
                FECHA_NOTIFICACION = DateTime.Parse(fila["FECHA_NOTIFICACION"].ToString());
            }



            if (!string.IsNullOrEmpty(fila["FECHA_GRABACION"].ToString()))         
            {
                FECHA_GRABACION = DateTime.Parse(fila["FECHA_GRABACION"].ToString());
            }
            CADFUER_DESTINO = int.Parse(fila["CADFUER_DESTINO"].ToString());
            CADCANA_DESTINO = int.Parse(fila["CADCANA_DESTINO"].ToString());
            ENTIDAD = fila["ENTIDAD"].ToString();
            CANAL = fila["CANAL"].ToString();
        }

        public string ENTIDAD { get; set; }
        public string CANAL { get; set; }
    }
}