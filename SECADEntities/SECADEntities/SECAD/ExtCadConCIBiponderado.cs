using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADCONDCLBIPONDERADOMetadata))]
    public partial class CADCONDCLBIPONDERADO
    {
    }

    public class CADCONDCLBIPONDERADOMetadata
    {
        [Required]
        [Display(Name = "Código")]
        public string CADCONDCLBICONS { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public short CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Clase Ponderado")]
        public string CLASE_PONDERADO { get; set; }

        [Required]
        [Display(Name = "Vlr Minimo")]
        public long VLR_MINIMO { get; set; }

        [Required]
        [Display(Name = "Vlr Maximo")]
        public long VLR_MAXIMO { get; set; }

        [Required]
        [Display(Name = "Vlr Unidad")]
        public long VLR_UNIDAD { get; set; }

        [Required]
        [Display(Name = "Vlr Ponderado")]
        public int VLR_PONDERADO { get; set; }

        [Required]
        [Display(Name = "Modo Conducta")]
        public string MODOCONDUCTA { get; set; }
    }
}