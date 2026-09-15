using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADPREGUNTAMetadata))]
    public partial class CADPREGUNTA
    {
    }

    public class CADPREGUNTAMetadata
    {
        [StringLength(60)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }
    }
}