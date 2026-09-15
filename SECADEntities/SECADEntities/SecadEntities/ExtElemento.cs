using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataTypeAttribute(typeof(ElementoMetaData))]
    public partial class CADNETELEMENTO
    {
    }

    public class ElementoMetaData
    {
        public decimal CADELEMENTO_ID { get; set; }

        [Display(Name = "Tipo de Elemento")]
        public decimal ELEMENTO_TIPO_ID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string ELEMENTO_NOMBRE { get; set; }

        [StringLength(1000)]
        [Display(Name = "Ruta")]
        public string ELEMENTO_RUTA { get; set; }

        [Display(Name = "Estado")]
        public bool ELEMENTO_ESTADO { get; set; }

        [Required]
        [Display(Name = "Posición")]
        public Nullable<byte> ELEMENTO_POSICION { get; set; }

        [Display(Name = "Elemento Padre")]
        public Nullable<decimal> ELEMENTO_PADRE_ID { get; set; }

        [Display(Name = "Fecha de Creación")]
        public Nullable<System.DateTime> ELEMENTO_FECHA_CREACION { get; set; }

        [Display(Name = "Icono")]
        public string ELEMENTO_ICONO { get; set; }
    }
}