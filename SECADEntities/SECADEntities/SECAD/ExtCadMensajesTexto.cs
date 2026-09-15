using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADMENSAJES_TEXTOMetadata))]
    public partial class CADMENSAJES_TEXTO
    {
    }

    public class CADMENSAJES_TEXTOMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int CADMENS_SITIOGRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CADMENS_CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Usuario Graba")]
        public string CADUSUA_GRABA { get; set; }

        [Required]
        [Display(Name = "Texto")]
        public string TEXTO { get; set; }
    }
}