using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadLugaresGeograficos
{
    [MetadataType(typeof(BannerImageneMetaData))]
    public partial class CADBANNERIMAGENE
    {
    }

    public class BannerImageneMetaData
    {
        [Display(Name = "Nombre del Archivo")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(50, ErrorMessage = "El campo admite solo 50 caracteres")]
        public string NOMBREARCHIVO { get; set; }

        [Display(Name = "Activo")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(0, 1, ErrorMessage = "Valor debería ser entre 0 hasta 1")]
        public int ACTIVO { get; set; }

        [Display(Name = "Orden")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int ORDEN { get; set; }
    }
}