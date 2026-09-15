using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CadSitioMetaData))]
    public partial class CADSITIO
    {
    }

    public class CadSitioMetaData
    {
        [Display(Name = "Sitio Grabación")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int SITIO_GRABA { get; set; }

        [Display(Name = "Consecutivo")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public string CONSECUTIVO { get; set; }

        [Display(Name = "Clase de Sitio")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string CLSI_CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(60, ErrorMessage = "El campo admite solo 60 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(60, ErrorMessage = "El campo admite solo 60 caracteres")]
        public string DIRECCION { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(12, ErrorMessage = "El campo admite solo 12 caracteres")]
        public string TELEFONO { get; set; }

        [Display(Name = "Código Barrio")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int BARR_CODIGO { get; set; }

        [Display(Name = "Código Barrio")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int BARR_LUGE_CODIGO { get; set; }

        [Display(Name = "Latitud")]
        [StringLength(6, ErrorMessage = "El campo admite solo 6 caracteres")]
        public string LATITUD { get; set; }

        [Display(Name = "Longitud")]
        [StringLength(6, ErrorMessage = "El campo admite solo 6 caracteres")]
        public string LONGITUD { get; set; }

        [Display(Name = "Comentario")]
        [StringLength(500, ErrorMessage = "El campo admite solo 500 caracteres")]
        public string COMENTARIO { get; set; }

        [Display(Name = "Código Lugar Geográfico")]
        [Range(1, 99999999, ErrorMessage = "Valor debería ser entre 1 hasta 99999999")]
        public int LUGE_CODIGO { get; set; }
    }
}