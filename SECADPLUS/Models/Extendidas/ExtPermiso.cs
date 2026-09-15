using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtPermiso
    {
        public ExtPermiso(DataRow fila, string strBoton)
        {
            Elemento = (string.IsNullOrEmpty(fila["ELEMENTO_NOMBRE"].ToString()) ? "" : (fila["ELEMENTO_NOMBRE"].ToString()));
            Permiso = (string.IsNullOrEmpty(fila["PERMISO_NOMBRE"].ToString()) ? "" : (fila["PERMISO_NOMBRE"].ToString()));
            Perfil = (string.IsNullOrEmpty(fila["PERFIL_NOMBRE"].ToString()) ? "" : (fila["PERFIL_NOMBRE"].ToString()));
            if (fila["PEP_ESTADO"].ToString().Equals("1"))
            {
                Estado = "<span class='label label-success'>Activo</span>";
            }
            else
            {
                Estado = "<span class='label label-danger'>Inactivo</span>";
            }
            if (!string.IsNullOrEmpty(fila["PEP_FECHA_CREACION"].ToString())) FechaCreacion = Convert.ToDateTime(fila["PEP_FECHA_CREACION"]).ToShortDateString();
            Boton = strBoton;
        }

        public string Boton { get; set; }
        public string Dummy { set; get; }
        public string FechaCreacion { get; set; }
        public string Elemento { get; set; }
        public string Permiso { get; set; }
        public string Perfil { get; set; }
        public string Estado { get; set; }
    }
}