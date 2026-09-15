using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CADEQUIPOSMetadata))]
    public partial class CADEQUIPOS
    {
    }

    public class CADEQUIPOSMetadata
    {
        [Display(Name = "Sitio de grabación")]
        public int SITIO_GRABA { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Identificación")]
        public string IDENTIFICACION { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Descripción")]
        public string DESCRIPCION { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Conexion Forms")]
        public string TNSNAMES60 { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Ubicación")]
        public string UBICACION { get; set; }

        [StringLength(80)]
        [Required]
        [Display(Name = "Serie")]
        public string SERIE { get; set; }
    }
}