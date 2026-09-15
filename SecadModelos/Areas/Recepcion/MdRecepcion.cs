using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.Recepcion
{
    public class MdRecepcion
    { 
        public class MdrecepcionM
        {
            public Int64 Abonado { get; set; }
            public string Fecha { get; set; }
            public Int64 NumeroSecad { get; set; }
            public int SitioGrabacion { get; set; }
            public int Idreg { get; set; }
            public string FechaLlamada { get; set; }
            public string TipoRegistro { get; set; }
            public string Calidad { get; set; }
            public string CodiMotivo { get; set; }
            public string Prioridad { get; set; }
            public int IdRegxCrime { get; set; }
            public string CodiXcrime { get; set; }
            public string DescXcrime { get; set; }
            public string TipoXcrime { get; set; }
            public string Transcripcion { get; set; }
            public string FechaIngbd { get; set; }
            public int CadpediSitioGraba { get; set; }
            public Int64 CadpediNumeLlamada { get; set; }
            public string Observacion { get; set; }
        }

        public class MdRecepcionD
        {
            public int SitioGrabacion { get; set; }
            public int IdReganalisis { get; set; }
            public string CodiMotivo { get; set; }
            public string DescripcionCodigo { get; set; }
            public int Orden { get; set; }
        }
    }
}
