using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtUsuarioOid : SECADEntities.SecadEntities.CADNETOID
    {
        public ExtUsuarioOid(DataRow fila, string strBoton = null)
        {
            Id = (string.IsNullOrEmpty(fila["OIDID"].ToString()) ? "" : fila["OIDID"].ToString());
            UserName = (string.IsNullOrEmpty(fila["OID_USUARIO"].ToString()) ? "" : fila["OID_USUARIO"].ToString());
            if (string.IsNullOrEmpty(fila["VIGENTE"].ToString()))
            {
                Vigente = "<span class='label label-success'>SI</span>";
            }
            else
            {
                Vigente = "<span class='label label-danger'>NO</span>";
            }
            UsuarioSecad = (string.IsNullOrEmpty(fila["NOMBRE_USUARIO"].ToString()) ? "" : fila["NOMBRE_USUARIO"].ToString());
            Boton = strBoton;
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string UsuarioSecad { get; set; }
        public string Vigente { get; set; }
    }
}