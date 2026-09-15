using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADTIPOSNOVEDADEMetadata))]
    public partial class CADTIPOSNOVEDADE
    {
    }

    public class CADTIPOSNOVEDADEMetadata
    {
        [Required]
        [Display(Name = "Código")]
        public string CODIGO { get; set; }

        [StringLength(60)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [Required]
        [Display(Name = "Nivel")]
        public byte NIVEL { get; set; }
    }
}