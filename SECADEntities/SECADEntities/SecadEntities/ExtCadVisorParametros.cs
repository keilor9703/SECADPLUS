using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CADVISORPARAMETROMetadata))]
    public partial class CADVISORPARAMETRO
    {
    }
    public class CADVISORPARAMETROMetadata
    {
        [Required]
        [Display(Name = "Código")]
        public short CODIGO { get; set; }
        [Required]
        [Display(Name = "Usuario")]
        public string USUARIO { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public string TIPO { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string NOMBRE { get; set; }
        [Required]
        [StringLength(2000)]
        [Display(Name = "Parametro")]
        public string PARAMETRO { get; set; }
    }
}
