using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadLugaresGeograficos
{
    [MetadataType(typeof(CadLugaresGeograficosMD))]
    public partial class CADLUGARES_GEOGRAFICOS
    { }

    public class CadLugaresGeograficosMD
    {
        [Display(Name = "Código")]
        [Range(1, 999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999")]
        public int CODIGO { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(50, ErrorMessage = "El campo admite solo 50 caracteres")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string DESCRIPCION { get; set; }

        [Display(Name = "Tipo")]
        [StringLength(3, ErrorMessage = "El campo admite solo 3 caracteres")]
        [Required(ErrorMessage = "Este campo es necesario")]
        public string TIPO { get; set; }

        [Display(Name = "Código Dane")]
        [StringLength(8, ErrorMessage = "El campo admite solo 8 caracteres")]
        public string CODIGO_DANE { get; set; }

        [Display(Name = "Código Registraduría")]
        [StringLength(9, ErrorMessage = "El campo admite solo 9 caracteres")]
        public string CODIGO_REGISTRADURIA { get; set; }

        [Display(Name = "Código Lugar Geográfico")]
        [Range(0, 999999, ErrorMessage = "Valor debería ser entre 0 hasta 999999")]
        public int LUGE_CODIGO { get; set; }

        [Display(Name = "Nivel")]
        [Range(1, 99, ErrorMessage = "Valor debería ser entre 1 hasta 99")]
        public int NIVEL { get; set; }

        [Display(Name = "Departamento")]
        [StringLength(2, ErrorMessage = "El campo admite solo 2 caracteres")]
        public string DPTO { get; set; }

        [Display(Name = "Vigente")]
        [StringLength(1, ErrorMessage = "El campo admite solo un carácter")]
        public string VIGENTE { get; set; }

        [Display(Name = "Código Siedco")]
        [Range(1, 999999999, ErrorMessage = "Valor debería ser entre 1 hasta 999999999")]
        public int CODIGO_SIEDCO { get; set; }

        [Display(Name = "Código Siath")]
        [Range(0, 999999999, ErrorMessage = "Valor debería ser entre 0 hasta 999999999")]
        public int CODIGO_SIATH { get; set; }

        [Display(Name = "Nivel 1")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_1 { get; set; }

        [Display(Name = "Nivel 2")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_2 { get; set; }

        [Display(Name = "Nivel 3")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_3 { get; set; }

        [Display(Name = "Nivel 4")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_4 { get; set; }

        [Display(Name = "Nivel 5")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_5 { get; set; }

        [Display(Name = "Nivel 6")]
        [Range(1, 9999999, ErrorMessage = "Valor debería ser entre 1 hasta 9999999")]
        public int NIVEL_6 { get; set; }
    }
}