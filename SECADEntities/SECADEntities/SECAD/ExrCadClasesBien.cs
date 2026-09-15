using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCLASES_BIENMetadata))]
    public partial class CADCLASES_BIEN
    {
    }

    public class CADCLASES_BIENMetadata
    {
        [StringLength(40)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [Required]
        [Display(Name = "Unidad")]
        public byte UNME_CODIGO { get; set; }
    }
}