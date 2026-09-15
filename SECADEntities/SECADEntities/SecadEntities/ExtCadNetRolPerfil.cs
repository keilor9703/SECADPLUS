using System.ComponentModel.DataAnnotations;

namespace SECADEntities.SecadEntities
{
    [MetadataTypeAttribute(typeof(CadNetPerfilRolMetaData))]
    public partial class CADNETROLPERFIL
    {
    }

    public class CadNetPerfilRolMetaData
    {
        [Required]
        public decimal PERFIL_ID { get; set; }
        [Required]
        public string ROL_ID { get; set; }
        
    }
}