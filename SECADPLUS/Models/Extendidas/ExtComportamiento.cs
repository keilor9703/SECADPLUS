using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtComportamiento
    {
        public ExtComportamiento(DataRow fila, string strBoton) {
            Boton = strBoton;
            ID = fila["ID_COMPORTAMIENTO"].ToString();
            Dominio = fila["dominio"].ToString();
            Descripcion = fila["descripcion"].ToString();
            IdPadre = fila["ID_PAPA"].ToString();
        }
        public string Boton { get; set; }
        public string Dummy { get; set;}
        public string ID { get; set; }
        public string Dominio { get; set; }
        public string Descripcion { get; set; }
        public string IdPadre { get; set; }
    }
}