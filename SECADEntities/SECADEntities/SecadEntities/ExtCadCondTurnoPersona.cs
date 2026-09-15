using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(TURNOPERSONAMetaData))]
    public partial class CADCONDTURNOPER
    {
    }

    public class TURNOPERSONAMetaData
    {
        public int SITIO_GRABA { get; set; }
        public long CONSECUTIVO { get; set; }
        public Nullable<int> CADCONDTURNSITGR { get; set; }
        public Nullable<long> CADCONDTURNTURNO { get; set; }
        public Nullable<long> CADCONDTURNCOND { get; set; }

        [Display(Name = "Modo")]
        public string MODOCONDUCTA { get; set; }

        [Required]
        [Display(Name = "Relaciones")]
        public string RELACONDUCTA { get; set; }

        [Display(Name = "Participación")]
        public string PARTCONDUCTA { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string TIPO_ID { get; set; }

        [Required]
        [Display(Name = "Identificación")]
        public long IDENTIFICACION { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        [StringLength(60)]
        public string APELLIDOS { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        [StringLength(60)]
        public string NOMBRES { get; set; }

        [Display(Name = "Genero")]
        public string GENERO { get; set; }

        [Display(Name = "Infracción")]
        public string INFRACCION { get; set; }

        [Display(Name = "Condición")]
        public Nullable<int> CONDCODIGO { get; set; }

        public Nullable<int> CADEVENSITIOGRABA { get; set; }

        [Display(Name = "Número Evento")]
        public Nullable<long> CADEVENNUMEEVENTO { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(2000)]
        public string OBSERVACIONES { get; set; }

        [Required]
        [Display(Name = "FechaConducta")]
        public System.DateTime FECHA_CONDUCTA { get; set; }

        public string USUA_TRANSACCION { get; set; }
        public Nullable<System.DateTime> FECHA_TRANSACCION { get; set; }
        public string MAQUINA_TRANSACCION { get; set; }
    }
}