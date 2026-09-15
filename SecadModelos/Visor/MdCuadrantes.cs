using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Visor
{
    public class MdCuadrantes
    {
        public int CuadranteId { get; set; }
        public string CodigoCuadrante { get; set; }
        public string CodigoSiedco { get; set; }
        public Int64 NumCelCua { get; set; }
        public int PrmEstadoCuad { get; set; }
        public string SiglaFisica { get; set; }
        public int UndeConsLevel1 { get; set; }
        public int UndeConsec { get; set; }
        public int UndeCai { get; set; }
        public string Nivel1 { get; set; }
        public string Estacion { get; set; }
        public string Dependencia { get; set; }
        public int Siedco { get; set; }
        public string CodCuaAnt { get; set; }
        public string Tipo { get; set; }
        public int Ordenamiento { get; set; }
        public int RegiCodigo { get; set; }
        public string DescRegion { get; set; }
        public string Pda { get; set; }
        public string Proveedor { get; set; }
    }
}
