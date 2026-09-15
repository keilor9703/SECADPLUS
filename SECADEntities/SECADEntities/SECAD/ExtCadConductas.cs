using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCONDUCTAMetadata))]
    public partial class CADCONDUCTA
    {
    }

    public class CADCONDUCTAMetadata
    {
        [Required]
        [Display(Name = "Código")]
        public int CODIGO { get; set; }

        [StringLength(200)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [StringLength(20)]
        [Required]
        [Display(Name = "Abreviatura")]
        public string ABREVIATURA { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string TIPO { get; set; }

        [Required]
        [Display(Name = "Nivel")]
        public byte NIVEL { get; set; }

        [Required]
        [Display(Name = "Vigente")]
        public string VIGENTE { get; set; }
    }
}