using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.PlanesPrev
{
    public class MdPlanesPreventivos
    {
        public string Tipoplan { get; set; }
        public int IdTipoPlan { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Unidad { get; set; }
        public int UndeConsecutivo { get; set; }
        public string Jefe { get; set; }
        public string Dispositivo { get; set; }
        public Int64 Celular { get; set; }
        public string Inicio { get; set; }
        public string Modifica { get; set; }
        public int IdPLan { get; set; }
        public int IdMinuta { get; set; }
    }
}
