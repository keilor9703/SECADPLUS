using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Visor
{
    public class MdUbicacionPda
    {
        public int UbicacionGpsId { get; set; }

        public string UidRegistro { get; set; }
        public DateTime? Fecha { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int Velocidad { get; set; }
        public string TipoLectura { get; set; }
        public string UsuarioSesion { get; set; }
        public int SesionActiva { get; set; }
        public string Imei { get; set; }
        public string SerialSim { get; set; }
        public int NivelBateria { get; set; }
        public int CuadranteId { get; set; }
        public string VersionApk { get; set; }
        public string FechaInstalacion { get; set; }
        public string DireccionIp { get; set; }
        public int EstaDentroCuadrante { get; set; }
        public Int64 Identificacion { get; set; }
        public string NombresApellidos { get; set; }
        public string CodigoCuadrante { get; set; }
        public Int64 NumCelCuadrante { get; set; }
        public int DependenciaId { get; set; }
        public string SiglaUnidad { get; set; }
        public string DescripcionUnidad { get; set; }
        public string SiglaPapaUnidad { get; set; }
        public string DispositivosSerie { get; set; }
        public string DispMarcaModelo { get; set; }
        public string Duracion { get; set; }
        public string Minutos { get; set; }
        public string DescRegion { get; set; }
    }
}