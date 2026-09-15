using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SECAD
{
    [MetadataType(typeof(CanalesMetaData))]
    public partial class CADCANALE
    {
    }

    public class CanalesMetaData
    {
        [Display(Name = "Fuerza")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int CADFUERZ_ID { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(60, ErrorMessage = "El campo admite solo 60 caracteres")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Abreviatura")]
        [StringLength(10, ErrorMessage = "El campo admite solo 10 caracteres")]
        public string ABREVIATURA { get; set; }

        [Display(Name = "Vigente")]
        [StringLength(1, ErrorMessage = "El campo admite solo 1 caracteres")]
        public string VIGENTE { get; set; }

        [Display(Name = "Desde")]
        public DateTime DESDE { get; set; }

        [Display(Name = "Hasta")]
        public DateTime HASTA { get; set; }

        [Display(Name = "Observación")]
        [StringLength(200, ErrorMessage = "El campo admite solo 200 caracteres")]
        public string OBSERVACION { get; set; }

        [Display(Name = "Disponible")]
        [StringLength(1, ErrorMessage = "El campo admite solo 1 caracteres")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string DISPONIBLE { get; set; }

        [Display(Name = "Tipo Canal")]
        [StringLength(1, ErrorMessage = "El campo admite solo 1 caracteres")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string TIPO_CANAL { get; set; }

        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string CLASE_TURNO { get; set; }

        [Display(Name = "Tipo de Casos")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string TIPO_ATENCION { get; set; }

        [Display(Name = "Autorización Medios")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string AUTORIZACION_MEDIOS { get; set; }

        [Display(Name = "Autorización Atenciones")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string AUTORIZACION_ANOTACIONES { get; set; }
    }
}