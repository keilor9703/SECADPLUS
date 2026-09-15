using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Visor
{
    public class MdDominios
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public long PadreId { get; set; }
        public string Abreviatura { get; set; }
        public string Observacion { get; set; }
    }
}
