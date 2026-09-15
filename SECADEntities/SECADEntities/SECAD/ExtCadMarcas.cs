using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADMARCAMetadata))]
    public partial class CADMARCA
    {
    }

    public class CADMARCAMetadata
    {
        [Required]
        [StringLength(60)]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }
    }
}