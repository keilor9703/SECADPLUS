using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(TurnoMetaData))]
    public partial class CADTURNO
    {
    }
    public class TurnoMetaData
    {
        [Required]
        [Display(Name = "Sitio")]
        public int SITIO_GRABA { get; set; }        
        [Display(Name = "Turno")]
        public long TURNO { get; set; }
        [Required]
        [Display(Name = "Hora Inicia")]
        public System.DateTime HORA_INICIA { get; set; }
        [Required]
        [Display(Name = "Hora Termina")]
        public System.DateTime HORA_TERMINA { get; set; }
        [Required]
        [Display(Name = "Tipo Turno")]
        public string TIPO_TURNO { get; set; }
        [Required]
        [Display(Name = "Día Inicia")]
        public string DIA_INICIA { get; set; }
        [Required]
        [Display(Name = "Día Termina")]
        public string DIA_TERMINA { get; set; }
        [Display(Name = "Clase Turno")]
        public string CLASE_TURNO { get; set; }
    }
}
