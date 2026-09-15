using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCASOS_CANALMetadata))]
    public partial class CADCASOS_CANAL
    {
    }

    public class CADCASOS_CANALMetadata
    {
        [Required]
        [Display(Name = "Caso")]
        public string CADCASO_CODIGO { get; set; }

        [Required]
        [Display(Name = "Fuerza")]
        public int CADCANA_FUERZ_ID { get; set; }

        [Required]
        [Display(Name = "Canal")]
        public int CADCANA_CODIGO { get; set; }
    }
}