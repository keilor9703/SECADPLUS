using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CADEVENTOS_FOTOSMetadata))]
    public partial class CADEVENTOS_FOTOS
    {
    }

    public class CADEVENTOS_FOTOSMetadata
    {
        [Required]
        [Display(Name = "Numero evento")]
        public long CADEVEN_NUMEEVENTO { get; set; }

        [Display(Name = "Sitio Graba")]
        public int CADEVEN_SITIOGRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public byte CONSECUTIVO { get; set; }

        [Display(Name = "Fecha Grabación")]
        public DateTime FECHA_GRABACION { get; set; }

        [Display(Name = "Foto")]
        public byte[] FOTO { get; set; }

        [StringLength(100), Required]
        [Display(Name = "Observación ")]
        public string OBSERVACION { get; set; }

        [Display(Name = "Usuario Grabación")]
        public string USUA_GRABA { get; set; }
    }
}