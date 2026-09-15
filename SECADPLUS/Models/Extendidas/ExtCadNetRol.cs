using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCadNetRol : CADNETROLE
    {
        public ExtCadNetRol(DataRow fila)
        {
            Id = (string.IsNullOrEmpty(fila["Id"].ToString()) ? "" : (fila["Id"].ToString()));
            Name = (string.IsNullOrEmpty(fila["Name"].ToString()) ? "" : (fila["Name"].ToString()));
            Asociado = (string.IsNullOrEmpty(fila["Id"].ToString()) ? (byte)0 : Convert.ToByte(fila["ASOCIADO"].ToString()));
        }

        public byte Asociado { get; set; }
    }
}