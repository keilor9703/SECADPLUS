using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(TurnoComparendoMetaData))]
    public partial class CADCONDTURNOCOMP
    {
    }
    public class TurnoComparendoMetaData {
        [Required]
        [Display(Name = "Sitio")]
        public int SITIO_GRABA { get; set; }
        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }
        public Nullable<int> CADCONDTURNSITGR { get; set; }
        public Nullable<long> CADCONDTURNTURNO { get; set; }
        public Nullable<long> CADCONDTURNCOND { get; set; }
        [Display(Name = "Código de la infracción")]
        public string CODI_INFRACCION { get; set; }
        [Display(Name = "Cantidad")]
        [Range(0, short.MaxValue, ErrorMessage = "El valor ingresado para la cantidad es incorrecto")]
        public Nullable<short> CANTIDAD { get; set; }
        public Nullable<int> CADEVENSITIOGRABA { get; set; }
        public Nullable<long> CADEVENNUMEEVENTO { get; set; }
        [Display(Name = "Observaciones")]
        [MaxLength(2000)]
        public string OBSERVACIONES { get; set; }
        [Required]
        [Display(Name = "Fecha Conducta")]
        public System.DateTime FECHA_CONDUCTA { get; set; }
        public string USUA_TRANSACCION { get; set; }
        public Nullable<System.DateTime> FECHA_TRANSACCION { get; set; }
        public string MAQUINA_TRANSACCION { get; set; }
    }
}
