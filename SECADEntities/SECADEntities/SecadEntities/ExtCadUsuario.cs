using System;
using System.ComponentModel.DataAnnotations;
namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(UsuarioMetaData))]
    public partial class CADUSUARIO
    {
    }
    public class UsuarioMetaData
    {
        [Required]
        [StringLength(15)]
        [Display(Name = "Nombre de Usuario")]
        public string USUARIO { get; set; }
        [Required]
        [StringLength(12)]
        [Display(Name = "Número de Identificación")]
        public string IDENTIFICACION { get; set; }

        
        [Required]
        [Display(Name = "Número de fuerza de la unidad")]
        public byte CADUNDE_FUERZA { get; set; }
        [Required]
        [Display(Name = "Código de la unidad")]
        public int CADUNDE_CODIGO { get; set; }
        [StringLength(1)]
        public string CONDICION { get; set; }

        public Nullable<int> LUGE_CODIGO_CAD { get; set; }

        public Nullable<int> CADCANA_FUERZA_ID { get; set; }

        public Nullable<int> CADCANA_CODIGO { get; set; }
        [Required]
        [Display(Name = "Sitio de grabación")]
        public int SITIO_GRABACION { get; set; }
        
        [StringLength(128)]
        [Display(Name = "Usuario empresarial")]
        public string USUARIO_OID_ID { get; set; }
        [Required]
        [Display(Name = "Vigente")]
        public Nullable<bool> VIGENTE { get; set; }
        [Required]
        [Display(Name = "Autenticar por OID")]
        public Nullable<bool> AUTENTICA_OID { get; set; }

        public string USUARIO_OID { get; set; }
        public virtual CADNETOID CADNETOID { get; set; }
    }
}
