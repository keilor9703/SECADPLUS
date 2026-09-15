using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadTurnos : CADTURNO
    {
        public Ext_CadTurnos(DataRow fila, string strBoton)
        {
            if (!string.IsNullOrEmpty(fila["HORA_INICIA"].ToString())) HoraInicia = fila["HORA_INICIA"].ToString();
            if (!string.IsNullOrEmpty(fila["HORA_TERMINA"].ToString())) HoraTermina = (fila["HORA_TERMINA"]).ToString();
            if (!string.IsNullOrEmpty(fila["DIA_INICIA"].ToString())) DiaInicia = fila["DIA_INICIA"].ToString();
            if (!string.IsNullOrEmpty(fila["DIA_TERMINA"].ToString())) DiaTermina = (fila["DIA_TERMINA"]).ToString();
            SITIO_GRABA = (string.IsNullOrEmpty(fila["SITIO_GRABA"].ToString()) ? 0 : Convert.ToInt32(fila["SITIO_GRABA"].ToString()));
            TURNO = (string.IsNullOrEmpty(fila["TURNO"].ToString()) ? 0 : Convert.ToInt64(fila["TURNO"].ToString()));
            ClaseTurno = (string.IsNullOrEmpty(fila["CLASE"].ToString()) ? "" : (fila["CLASE"].ToString()));
            TipoTurno = (string.IsNullOrEmpty(fila["TIPO"].ToString()) ? "" : (fila["TIPO"].ToString()));
            CONSIGNAS = (string.IsNullOrEmpty(fila["CONSIGNAS"].ToString()) ? "" : (fila["CONSIGNAS"].ToString()));
            Boton = strBoton;
        }

        public string ClaseTurno { get; set; }
        public string TipoTurno { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string HoraInicia { get; set; }
        public string HoraTermina { get; set; }
        public string DiaInicia { get; set; }
        public string DiaTermina { get; set; }
    }
}