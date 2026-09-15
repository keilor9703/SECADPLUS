using System;
using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadLugaresGeograficos
{
    [MetadataType(typeof(CadPoliciaMetaData))]
    public partial class CADPOLICIA
    { }

    public class CadPoliciaMetaData
    {
        [Required]
        [Display(Name = "Cédula Empleado")]
        [StringLength(13, ErrorMessage = "El campo admite solo 13 caracteres")]
        public String CEDU_EMPLEADO { get; set; }

        [Display(Name = "Nombre Empleado")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string NOMB_EMPLEADO { get; set; }

        [Required]
        [Display(Name = "Número de Placa")]
        [Range(0, 9999999, ErrorMessage = "Valor debería ser entre 0 hasta 9999999")]
        public int NUME_PLACA { get; set; }

        [Required]
        [Display(Name = "Dirección Residencia")]
        [StringLength(80, ErrorMessage = "El campo admite solo 80 caracteres")]
        public string DIRE_RESIDENCIA { get; set; }

        [Display(Name = "Teléfono Residencia")]
        [StringLength(25, ErrorMessage = "El campo admite solo 25 caracteres")]
        public string TELE_RESIDENCIA { get; set; }
        [Required]
        [Display(Name = "Sexo")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string TIPO_SEXO { get; set; }

        [Display(Name = "Grupo Sanguíneo")]
        [StringLength(6, ErrorMessage = "El campo admite solo 6 caracteres")]
        public string GRUP_SANGUINEO { get; set; }

        [Display(Name = "Estado Civil")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string ESTA_CIVIL { get; set; }
        [Required]
        [Display(Name = "Código Especialidad")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string CODI_ESPECIALIDAD { get; set; }
        [Required]
        [Display(Name = "Estado Laboral")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string ESTA_LABORAL { get; set; }

        [Display(Name = "Número de Sección")]
        [StringLength(1, ErrorMessage = "El campo admite solo un carácter")]
        public string NUME_SECCION { get; set; }
        [Required]
        [Display(Name = "Fuerza")]
        [Range(1, 99, ErrorMessage = "Valor debería ser entre 1 hasta 99")]
        public int CADUNID_FUERZA { get; set; }
        [Required]
        [Display(Name = "Unidad")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int CADUNID_CODIGO { get; set; }

        [Display(Name = "Grado")]
        [StringLength(6, ErrorMessage = "El campo admite solo 6 caracteres")]
        public string GRAD_ALFABETICO { get; set; }

        [Display(Name = "Fuerza Principal")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int CADFUERZ_ID { get; set; }
    }
}