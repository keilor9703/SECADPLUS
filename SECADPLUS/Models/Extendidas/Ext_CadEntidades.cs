using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class Ext_CadEntidades
    {
        public Ext_CadEntidades(DataRow fila, string strBoton, string strDestinoImagen)
        {
            Dummy = "";
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            ABREVIATURA = fila["ABREVIATURA"].ToString();
            ESCUDO = strDestinoImagen;
            ID = fila["ID"].ToString();
        }

        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string DESCRIPCION { get; set; }
        public string ABREVIATURA { get; set; }
        public string ESCUDO { get; set; }
        public string ID { get; set; }
    }
}