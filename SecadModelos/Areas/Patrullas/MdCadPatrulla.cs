using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.Patrullas
{
    public class MdCadPatrulla
    {

        public int CadUniFuerza { get; set; }
        public int CadUniCodigo { get; set; }
        public int Codigo { get; set; }
        public string DescPatrulla { get; set; }
        public string IdClaseTurno { get; set; }
        public string ClaseTurno { get; set; }
        public string Vigente { get; set; }

        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string FechaHastaStr { get; set; }
        public string Observacion { get; set; }
        public string UsuaTransaccion { get; set; }
        public int SitioGrabacion { get; set; }
        public string IdRadio { get; set; }
        public int CuadranteId { get; set; }
        public string Cuadrante { get; set; }

        public bool Activos { get; set; }
    }
}
