using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CADNOVEDADESPATRULLAMetadata))]
    public partial class CADNOVEDADESPATRULLA
    {
    }

    public class CADNOVEDADESPATRULLAMetadata
    {

        public int CADFUERZ_ID { get; set; }

        [Required]
        [Display(Name = "Medio")]
        public long CADMEDISP_CONSECUTIVO { get; set; }

        public int CADMEDISP_SITIOGRABA { get; set; }

        public int CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Hora Inicia")]
        public DateTime? HORA_INICIA { get; set; }

        [Display(Name = "Hora Termina")]
        public DateTime? HORA_TERMINA { get; set; }

        [StringLength(100)]
        [Display(Name = "Observación ")]
        public string OBSERVACION { get; set; }

        [Required]
        [Display(Name = "Tipo Novedad")]
        public string TIPO_NOVEDAD { get; set; }

        public string USUA_USUARIO { get; set; }
    }
}
