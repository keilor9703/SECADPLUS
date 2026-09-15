using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADTIPOS_BIENMetadata))]
    public partial class CADTIPOS_BIEN
    {
    }

    public class CADTIPOS_BIENMetadata
    {
        [StringLength(40)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }
    }
}