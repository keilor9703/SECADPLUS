using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(MEDIOPERMetaData))]
    public partial class CADCONDTURNOPERSMEDISP
    {
    }
    public class MEDIOPERMetaData
    {
        [Required]
        [Display(Name ="Sitio Grabación")]
        public int CADPERSITGR { get; set; }
        [Required]
        [Display(Name = "Consecutivo Persona")]
        public long CADPERCONSEC { get; set; }
        [Required]
        [Display(Name = "Sitio Grabación del Medio")]
        public int CADMEDISPSITGR { get; set; }
        [Required]
        [Display(Name = "Consecutivo del Medio")]
        public long CADMEDISPCONSEC { get; set; }
        [Required]
        [Display(Name = "Cédula")]
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
