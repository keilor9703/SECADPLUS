using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(TurnoBienMetaData))]
    public partial class CADCONDTURNOBIENE
    {
    }

    public class TurnoBienMetaData {
        [Required]
        public int SITIO_GRABA { get; set; }
        [Required]
        public long CONSECUTIVO { get; set; }

        public Nullable<int> CADCONDTURNSITGR { get; set; }
        public Nullable<long> CADCONDTURNTURNO { get; set; }
        public Nullable<long> CADCONDTURNCOND { get; set; }
        [Display(Name = "Modo")]
        public string MODOCONDUCTA { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public short CADCLBI_TIBI_CODIGO { get; set; }
        [Required]
        [Display(Name = "Clase")]
        public short CADCLBI_CODIGO { get; set; }
        [Display(Name = "Marca")]
        public Nullable<short> MARCA_CODIGO { get; set; }
        [Display(Name = "Linea")]
        public Nullable<short> LINEA_CODIGO { get; set; }
        [Display(Name = "Color")]
        public Nullable<short> COLO_CODIGO { get; set; }
        public string PLACA { get; set; }
        public string CHASIS { get; set; }
        public string MOTOR { get; set; }
        public string SERIE { get; set; }
        public Nullable<long> CANTIDAD { get; set; }
        public Nullable<long> AVALUO { get; set; }
        public Nullable<int> CADPARQ_SITGR { get; set; }
        [Display(Name = "Código Parqueadero")]
        public string CADPARQ_CODIGO { get; set; }
        [Display(Name = "Código Infracción")]
        public string CADINFR_CODIGO { get; set; }
        public Nullable<int> CADEVENSITIOGRABA { get; set; }
        public Nullable<long> CADEVENNUMEEVENTO { get; set; }
        public string OBSERVACIONES { get; set; }
        public Nullable<System.DateTime> FECHA_CONDUCTA { get; set; }
        public string USUA_TRANSACCION { get; set; }
        public Nullable<System.DateTime> FECHA_TRANSACCION { get; set; }
        public string MAQUINA_TRANSACCION { get; set; }
        public Nullable<short> CADUNID_MEDIDA { get; set; }
    }
}
