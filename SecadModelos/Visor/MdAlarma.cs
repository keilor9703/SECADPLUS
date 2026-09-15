using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Visor
{
    public class MdAlarma
    {
        public int ID_ALARMA { get; set; }
        public int ID_TIPO { get; set; }
        public decimal LATITUD { get; set; }
        public decimal LONGITUD { get; set; }
        public DateTime FECHA_CREACION { get; set; }
        public int ID_RPC_VINCULADO { get; set; }
        public string CIUDADANO { get; set; }
        public string DIRECCION_CIUDADANO { get; set; }
        public string DTO_CIUDADANO { get; set; }
        public string LUGAR_CIUDADANO { get; set; }
        public string TELEFONO { get; set; }
        public Int64 CELULAR { get; set; }
        public string SEGMENTO { get; set; }
        public int ID_MUNICIPIO { get; set; }
    }
}
