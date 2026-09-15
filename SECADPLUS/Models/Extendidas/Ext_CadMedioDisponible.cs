using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadMedioDisponible : CADMEDIOSDISPONIBLE
    {
        public Ext_CadMedioDisponible(DataRow fila, string strBoton)
        {
            Patrulla = (string.IsNullOrEmpty(fila["PATRULLA"].ToString()) ? "" : (fila["PATRULLA"].ToString()));
            Estado = (string.IsNullOrEmpty(fila["ESTADO"].ToString()) ? "" : (fila["ESTADO"].ToString()));
            Tipo = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            Canal = (string.IsNullOrEmpty(fila["CANAL"].ToString()) ? "" : (fila["CANAL"].ToString()));
            //CONSECUTIVO = (string.IsNullOrEmpty(fila["CONSECUTIVO"].ToString()) ? 0 : Convert.ToInt64(fila["CONSECUTIVO"].ToString()));
            Boton = strBoton;
            CuadranteNew = (string.IsNullOrEmpty(fila["CUADRANTENEW"].ToString()) ? "" : (fila["CUADRANTENEW"].ToString()));
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Patrulla { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public string Canal { get; set; }
        public string CuadranteNew { get; set; }
    }
}