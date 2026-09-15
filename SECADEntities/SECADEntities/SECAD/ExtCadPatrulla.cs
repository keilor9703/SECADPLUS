using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CapPatrullaMetaData))]
    public partial class CADPATRULLA
    {
        //public string turnoDesc;
        //public string TurnoDesc {
        //    get { return ""; }
        //    set { turnoDesc = value; } }
    }

    public class CapPatrullaMetaData
    {
        [Display(Name = "Descripción")]
        [Required]
        [StringLength(30, ErrorMessage = "El campo admite un máximo de 30 caracteres")]
        public string DESC_PATRULLA { get; set; }

        [Display(Name = "Clase de Turno")]
        [Required]
        public string CLASE_TURNO { get; set; }

        [Display(Name = "Vigente")]
        [Required]
        public string VIGENTE { get; set; }

        [Display(Name = "Observación")]
        [StringLength(1000)]
        public string OBSERVACION { get; set; }

        [Display(Name = "Id Radio")]
        [StringLength(20)]
        public string ID_RADIO { get; set; }

        [Display(Name = "Fecha desde")]
        public Nullable<System.DateTime> FECHA_DESDE { get; set; }

        [Display(Name = "Fecha Hasta")]
        public Nullable<System.DateTime> FECHA_HASTA { get; set; }

        [Display(Name = "Usuario")]
        public string USUA_TRANSACCION { get; set; }

        [Display(Name = "Sitio Grabación")]
        public Nullable<int> SITIO_GRABA { get; set; }

        [Display(Name = "ID Cuadrante Sivicc")]
        public int CUADRANTEID { get; set; }

        [Display(Name = "Cuadrante")]
        [StringLength(20)]
        public string CUADRANTE { get; set; }
    }
}