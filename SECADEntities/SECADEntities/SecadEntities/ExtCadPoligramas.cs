using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CADPOLIGRAMAMetadata))]
    public partial class CADPOLIGRAMA
    {
    }

    public class CADPOLIGRAMAMetadata
    {
        [Required]
        [Display(Name = "Sitio de Grabación")]
        public int SITIO_GRABA { get; set; }

        [Required]
        [Display(Name = "Consecutivo")]
        public long CONSECUTIVO { get; set; }

        [Required]
        [Display(Name = "Fuerza")]
        public int CADFUERZ_ID { get; set; }

        [Required]
        [Display(Name = "Número")]
        public long NUMERO { get; set; }

        [StringLength(120)]
        [Required]
        [Display(Name = "Origen")]
        public string ORIGEN { get; set; }

        [Required]
        [Display(Name = "Año")]
        public short AÑO { get; set; }

        [StringLength(120)]
        [Required]
        [Display(Name = "Destino")]
        public string DESTINO { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public DateTime FECHA { get; set; }

        [StringLength(4000)]
        [Required]
        [Display(Name = "Texto")]
        public string TEXTO { get; set; }

        [StringLength(120)]
        [Required]
        [Display(Name = "Firma")]
        public string FIRMA { get; set; }

        [StringLength(120)]
        [Required]
        [Display(Name = "Cargo Firma")]
        public string CARGO_FIRMA { get; set; }

        [StringLength(60)]
        [Required]
        [Display(Name = "Transmite")]
        public string TRANSMITE { get; set; }

        [StringLength(60)]
        [Required]
        [Display(Name = "Recibe")]
        public string RECIBE { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string USUARIO { get; set; }
    }
}