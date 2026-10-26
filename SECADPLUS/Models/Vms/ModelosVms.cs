using System;

namespace SECADPLUS.Models.Vms
{
    /// <summary>
    /// Cámara tal como la entrega el VMS, reducida a los datos que SECAD necesita.
    /// El resto de la configuración de la cámara (incluidas credenciales) nunca se guarda.
    /// </summary>
    public class CamaraVms
    {
        public string IdVms { get; set; }
        public string Nombre { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        /// <summary>Estado reportado por el VMS (ARMED, DISARMED, ALARMED, DETACHED, DISARM_DETACHED).</summary>
        public string Estado { get; set; }
        public bool Habilitada { get; set; }
        public bool Ptz { get; set; }
    }

    /// <summary>
    /// Cámara del censo local con su distancia al caso.
    /// </summary>
    public class CamaraCercana
    {
        public string IdVms { get; set; }
        public string Nombre { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Estado { get; set; }
        public bool EnLinea { get; set; }
        public int DistanciaMetros { get; set; }
    }

    /// <summary>
    /// Contenido de un token de reproducción ya validado.
    /// </summary>
    public class TokenVideo
    {
        public string IdVms { get; set; }
        public string Usuario { get; set; }
        public DateTime Vence { get; set; }
    }
}
