using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(ParqueaderoMetaData))]
    public partial class CADPARQUEADERO
    {
    }

    public class ParqueaderoMetaData
    {
        [Display(Name = "Sitio de Grabación")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int SITIO_GRABA { get; set; }

        [Display(Name = "Código")]
        [Required]
        [StringLength(12, ErrorMessage = "El campo admite solo 12 caracteres")]
        public string CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(60, ErrorMessage = "El campo admite solo 60 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Código Lugar Geográfico")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int LUGE_CODIGO { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string DIRECCION { get; set; }

        [Display(Name = "Código Barrio")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int CODI_BARRIO { get; set; }

        [Display(Name = "Barrio")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int LUGE_BARRIO { get; set; }

        [Display(Name = "Teléfono")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string TELEFONO { get; set; }

        [Display(Name = "Horario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string HORARIO { get; set; }

        [Display(Name = "Usuario de la Transacción")]
        [StringLength(20, ErrorMessage = "El campo admite solo 20 caracteres")]
        public string USUA_TRANSACCION { get; set; }
    }
}