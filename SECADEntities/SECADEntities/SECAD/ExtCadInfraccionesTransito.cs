using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADINFRACCIONES_TRANSITOMetadata))]
    public partial class CADINFRACCIONES_TRANSITO
    {
    }

    public class CADINFRACCIONES_TRANSITOMetadata
    {
        [StringLength(10)]
        [Required]
        [Display(Name = "Código")]
        public string CODIGO { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }
    }
}