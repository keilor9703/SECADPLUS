using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtUniTurno : CADUNITURNO
    {
        public ExtUniTurno(DataRow fila, string strBoton, bool blDiponible)
        {
            CADUNID_FUERZA = (string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? 0 : Convert.ToInt32(fila["FUERZA"].ToString()));
            CADUNIP_CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            Medio = (string.IsNullOrEmpty(fila["DESCRIPCION"].ToString()) ? "" : (fila["DESCRIPCION"].ToString()));
            strBoton = strBoton.Replace("_DESCRIPCION", Medio).Replace("_FUERZA", CADUNID_FUERZA.ToString());
            Boton = strBoton;
        }

        public ExtUniTurno(DataRow fila, string strBoton)
        {
            CADTURN_SITIOGRABA = (string.IsNullOrEmpty(fila["SITIO"].ToString()) ? 0 : Convert.ToInt32(fila["SITIO"].ToString()));
            CADTURN_TURNO = (string.IsNullOrEmpty(fila["TURNO"].ToString()) ? 0 : Convert.ToInt64(fila["TURNO"].ToString()));
            CADUNID_FUERZA = (string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? 0 : Convert.ToInt32(fila["FUERZA"].ToString()));
            CADUNIP_CODIGO = (string.IsNullOrEmpty(fila["CODIGO"].ToString()) ? 0 : Convert.ToInt32(fila["CODIGO"].ToString()));
            Medio = (string.IsNullOrEmpty(fila["MEDIO"].ToString()) ? "" : (fila["MEDIO"].ToString()));
            CONSIGNAS = (string.IsNullOrEmpty(fila["CONSIGNAS"].ToString()) ? "" : (fila["CONSIGNAS"].ToString()));
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Medio { get; set; }
    }
}