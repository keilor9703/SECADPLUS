using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADMENSAJES_TEXTO_USUARIOSMetadata))]
    public partial class CADMENSAJES_TEXTO_USUARIOS
    {
    }

    public class CADMENSAJES_TEXTO_USUARIOSMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int CADMENTEX_MENS_SITIOGRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CADMENTEX_MENS_CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CADMENTEX_CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Usuario Destino")]
        public string CADUSUA_DESTINO { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string ESTADO { get; set; }

        [Required]
        [Display(Name = "Fecha Grabación")]
        public System.DateTime FECHA_GRABACION { get; set; }
    }
}