using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADREFERENCIAMetadata))]
    public partial class CADREFERENCIA
    {
    }

    public class CADREFERENCIAMetadata
    {
        [Display(Name = "Sitio de grabación")]
        public int SITIO_GRABA { get; set; }

        [StringLength(60)]
        [Required]
        [Display(Name = "Nombre")]
        public string NOMBRE { get; set; }

        [StringLength(10)]
        [Required]
        [Display(Name = "Código")]
        public string CODIGO { get; set; }

        [StringLength(60)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Abreviatura")]
        public string ABREVIATURA { get; set; }
    }
}