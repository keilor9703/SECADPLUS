using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CompMedMetaData))]
    public partial class CADCONDTURNOCOMPMEDISP
    {
    }
    public class CompMedMetaData {
        [Required]
        public int CADCOMPSITGR { get; set; }
        [Required]
        [Display(Name = "Comparendo")]
        public long CADCOMPCONSEC { get; set; }
        [Required]
        public int CADMEDISPSITGR { get; set; }
        [Required]
        public long CADMEDISPCONSEC { get; set; }
        [Required]
        [Display(Name = "Cédula Empleado")]
        public string CEDU_EMPLEADO { get; set; }
        [Required]
        [Display(Name = "Fecha Conducta")]
        public System.DateTime FECHA_CONDUCTA { get; set; }
        [Required]
        public string USUA_GRABA { get; set; }
        [Required]
        public System.DateTime FECHA_GRABACION { get; set; }
        [Required]
        public string MAQUINA_GRABA { get; set; }
        public string TIPO_ACCION { get; set; }
    }
}
