using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataTypeAttribute(typeof(CadNetPerfilMetaData))]
    public partial class CADNETPERFIL
    {
    }

    public class CadNetPerfilMetaData
    {
        [Required]
        [StringLength(256)]
        [Display(Name = "Nombre")]
        public string PERFIL_NOMBRE { get; set; }
    }
}