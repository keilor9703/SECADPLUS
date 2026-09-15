using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADBIENES_NOVEDADESMetadata))]
    public partial class CADBIENES_NOVEDADES
    {
    }

    public class CADBIENES_NOVEDADESMetadata
    {
        [Required]
        [Display(Name = "Tipo Inf.")]
        public string TIIN_CODIGO { get; set; }

        [Required]
        [Display(Name = "Clase Inf.")]
        public string CLIN_CODIGO { get; set; }
        [Required]
        [Display(Name = "Fecha")]
        public DateTime FECHA_NOVEDAD { get; set; }
    }
}
