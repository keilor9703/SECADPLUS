using System;
using System.ComponentModel.DataAnnotations;


namespace SECADEntities.SecadEntities
{
    [MetadataType(typeof(CNPMetaData))]
    public partial class CADEVENTOS_CNP_COMP
    {
    }
    public class CNPMetaData {
        [Key]
        [Required]
        [Display(Name ="CNP No. Orden Comparendo")]
        public string CNP_ORDEN_COMPARENDO { get; set; }
        [Required]
        public int CADEVEN_SITIOGRABA { get; set; }
        [Required]
        public long CADEVEN_NUMEEVENTO { get; set; }
        [Required]
        [Display(Name = "Id. Comportamiento")]
        [Range(0, 999999)]
        public int CNP_COMPORTAMIENTO_ID { get; set; }
        [Required]
        [Display(Name = "Tipo Doc Persona")]
        public string TIPO_IDPERSONA { get; set; }
        [Required]
        [Display(Name = "No. Doc")]
        [MaxLength(12)]
        public string IDPERSONA { get; set; }
        [Required]
        [Display(Name = "Nombres")]
        [MaxLength(80)]
        public string NOMBRE_PERSONA { get; set; }
        [Required]
        [Display(Name = "Apellidos")]
        [MaxLength(80)]
        public string APELLIDOS_PERSONA { get; set; }
        [Required]
        [Display(Name = "Cédula Policía")]
        [MaxLength(12)]
        public string CADPOLI_CEDULA { get; set; }
        [Required]
        [Display(Name = "Usuario grabación")]
        public string USUA_GRABA { get; set; }
        [Display(Name = "Fecha grabación")]
        public Nullable<System.DateTime> FECHA_GRABACION { get; set; }
        [Required]
        [Display(Name = "Máquina grabación")]
        public string MAQUINA_GRABACION { get; set; }
        [Display(Name = "Validado CNP")]
        public string VALIDADO_CNP { get; set; }
    }

}
