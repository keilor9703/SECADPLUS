namespace SECADPLUS.Models
{
    public class SecadEnums
    {
        #region Propiedades

        public enum OIDRespuesta
        {
            SinConexion = 1,
            DatosIncorrectos = 2
        }

        public enum TipoConexion
        {
            Entity = 1,
            TSName = 2,
            ConnectionString = 3
        }

        public enum TipoAuditoria
        {
            Modificar = 1,
            Eliminar = 2,
            Crear = 3
        }

        #endregion Propiedades
    }
}