using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCONDCLASESBIENMetadata))]
    public partial class CADCONDCLASESBIEN
    {
    }

    public class CADCONDCLASESBIENMetadata
    {
        [Required]
        [Display(Name = "Consecutivo")]
        public string CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Tipo Conducta")]
        public string TIPO_CONDUCTA { get; set; }

        [Required]
        [Display(Name = "Código")]
        public int COND_CODIGO { get; set; }
    }
}