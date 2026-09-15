using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CADREVISTAMetadata))]
    public partial class CADREVISTA
    {
    }
    public class CADREVISTAMetadata
    {
        [Required]
        [Display(Name = "Patrulla")]
        public byte CADPATR_FUERZA { get; set; }
        [Required]
        [Display(Name = "Patrulla")]
        public int CADPATR_UNDE { get; set; }
        [Required]
        [Display(Name = "Patrulla")]
        public int CADPATR_CODIGO { get; set; }
        [Required]
        [Display(Name = "Fecha")]
        public System.DateTime FECHA { get; set; }
        [Required]
        [Display(Name = "Sitio")]
        public int CADSITI_CODIGO { get; set; }
    }
}
