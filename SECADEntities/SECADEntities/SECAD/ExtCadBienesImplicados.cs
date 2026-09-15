using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADBIENES_IMPLICADOSMetadata))]
    public partial class CADBIENES_IMPLICADOS
    {
    }

    public class CADBIENES_IMPLICADOSMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int CADBIEN_SITIOGRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo del Bien")]
        public long CADBIEN_CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Código del Caso")]
        public string CASO_CODIGO { get; set; }

        [Required]
        [Display(Name = "Fecha Novedad")]
        public System.DateTime FECHA_NOVEDAD { get; set; }
    }
}