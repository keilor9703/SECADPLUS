using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCASOMetadata))]
    public partial class CADCASO
    {
    }

    public class CADCASOMetadata
    {
        [StringLength(6)]
        [Required]
        [Display(Name = "Código")]
        public string CODIGO { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }
    }
}