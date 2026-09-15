using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtUsuario : CADNETOID
    {
        public ExtUsuario(DataRow fila)
        {
            //Asigna los valores a sus pripiedades
            UserName = string.IsNullOrEmpty(fila["USERNAME"].ToString()) ? "" : fila["USERNAME"].ToString();
            Id = string.IsNullOrEmpty(fila["USUARIOID"].ToString()) ? "" : fila["USUARIOID"].ToString();
            if (!string.IsNullOrEmpty(fila["FECHA_CREACION"].ToString())) fecha_creacion = Convert.ToDateTime(fila["FECHA_CREACION"].ToString());
        }
    }
}