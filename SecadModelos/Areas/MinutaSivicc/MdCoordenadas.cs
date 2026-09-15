using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.MinutaSivicc
{
    public class MdCoordenadas
    {
        public int PSitioGraba { get; set; }

        public Int64 PMedioConsecutivo { get; set; }

        public Int64 Nume_Llamada { get; set; }

        public int PIdCudrante { get; set; }

        public string PCuadrante { get; set; }

        public string PLatitud { get; set; }

        public string PLongitud { get; set; }

        public DateTime PFechaGPS { get; set; }
        public string PEnCuadrante { get; set; }

    }
}
