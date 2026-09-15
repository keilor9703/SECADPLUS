using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADBIENEMetadata))]
    public partial class CADBIENE
    {
      
    }

    public class CADBIENEMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int SITIO_GRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Tipo de Bien")]
        public short CADCLBI_TIBI_CODIGO { get; set; }

        [Required]
        [Display(Name = "Clase de Bien")]
        public short CADCLBI_CODIGO { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public short MARCA_CODIGO { get; set; }

        [Required]
        [Display(Name = "Línea")]
        public short LINEA_CODIGO { get; set; }

        [Required]
        [Display(Name = "Placa")]
        public string PLACA { get; set; }

        [Required]
        [Display(Name = "Chasis")]
        public string CHASIS { get; set; }

        [Required]
        [Display(Name = "Motor")]
        public string MOTOR { get; set; }
    }
}