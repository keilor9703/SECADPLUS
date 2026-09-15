using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADANOTAGRALMetadata))]
    public partial class CADANOTAGRAL
    {
    }

    public class CADANOTAGRALMetadata
    {        
        [Display(Name = "Usuario")]
        public string USUARIO { get; set; }
        [Required]
        [Display(Name = "Anotacion")]
        public string ANOTACION { get; set; }        
        [Display(Name = "Consecutivo")]
        public int CONSECUTIVO { get; set; }        
        [Display(Name = "Sitio Graba")]
        public int SITIO_GRABA { get; set; }        
        [Display(Name = "Fecha Graba")]
        public DateTime FECHA_GRABACION { get; set; }
        [Display(Name = "Maquina")]
        public string MAQUINA { get; set; }
    }
}