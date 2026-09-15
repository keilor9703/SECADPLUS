using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Visor
{
    public class MdAlarmas
    {
        public int IdAlarma { get; set; }
        public int IdRpcVinculado { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int IdTipo { get; set; }
        public string FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public int IdGpsActual { get; set; }
        public string Ciudadano { get; set; }
        public Int64 Identificacion { get; set; }
        public string Direccion { get; set; }
        public string TelefonoFijo { get; set; }
        public Int64 Celular { get; set; }
        public string Correo { get; set; }
        public string Segmento { get; set; }
        public string Dto { get; set; }
        public string Municipio { get; set; }
    }
}
