using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadLugaresGeograficos
{
    [MetadataType(typeof(CadComunaMetaData))]
    public partial class CADCOMUNA
    { }

    public class CadComunaMetaData
    {
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int CODIGO { get; set; }

        [Display(Name = "Código Lugar Geográfico")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int LUGE_CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Vigente")]
        [StringLength(1, ErrorMessage = "El campo admite solo un carácter")]
        public string VIGENTE { get; set; }

        [Display(Name = "Identificación")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string IDENTIFICACION { get; set; }
    }
}