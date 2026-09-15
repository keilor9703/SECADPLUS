using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(PatrullaMetaData))]
    public partial class ExtPatrulla
    {
    }

    public class PatrullaMetaData
    {
        [Required]
        [StringLength(30)]
        [Display(Name = "Descripción")]
        public string DESC_PATRULLA { get; set; }

        [Required]
        [Display(Name = "Turno")]
        public string CLASE_TURNO { get; set; }

        [Required]
        [Display(Name = "Vigente")]
        public string VIGENTE { get; set; }

        [Display(Name = "Fecha desde")]
        public Nullable<System.DateTime> FECHA_DESDE { get; set; }

        [Display(Name = "Fecha Hasta")]
        public Nullable<System.DateTime> FECHA_HASTA { get; set; }
    }
}