using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADSITIOS_GRABACIONMetadata))]
    public partial class CADSITIOS_GRABACION
    {
    }

    public class CADSITIOS_GRABACIONMetadata
    {
        [Display(Name = "Consecutivo")]
        public int CONSECUTIVO { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [Required]
        [Display(Name = "Ciduad")]
        public int LUGE_CIUDAD { get; set; }

        [Required]
        [Display(Name = "Fuerza")]
        public int CADUNDE_FUERZA { get; set; }

        [Required]
        [Display(Name = "Unidad")]
        public int CADUNDE_CODIGO { get; set; }
    }
}