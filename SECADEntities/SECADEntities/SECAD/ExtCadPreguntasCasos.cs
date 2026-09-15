using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADPREGUNTASCASOMetadata))]
    public partial class CADPREGUNTASCASO
    {
    }

    public class CADPREGUNTASCASOMetadata
    {
        [Required]
        [Display(Name = "Descripción")]
        public short CADPREG_CODIGO { get; set; }
    }
}