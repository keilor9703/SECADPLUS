using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADMENSAJEMetadata))]
    public partial class CADMENSAJE
    {
    }

    public class CADMENSAJEMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int SITIO_GRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Usuario Graba")]
        public string CADUSUA_INICIA { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public System.DateTime FECHA { get; set; }
    }
}