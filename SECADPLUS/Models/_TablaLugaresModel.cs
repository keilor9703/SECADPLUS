using System.Collections.Generic;
using System.Data;

namespace SECADPLUS.Models
{
    public class _TablasModel
    {
        public List<_LugaresModel> Lugares { get; set; }
        public List<_SiathModel> CodigosSIATH { get; set; }
        public List<ExtUnidad> Unidades { get; set; }
    }

    public class _LugaresModel
    {
        public _LugaresModel(string strDummy, string strBoton, string strCodigo, string strDescripcion)
        {
            Dummy = strDummy;
            Boton = strBoton;
            Codigo = strCodigo;
            Descripcion = strDescripcion;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

    public class _SiathModel
    {
        public _SiathModel(DataRow fila, string strBoton)
        {
            Codigo = fila["SIATH"].ToString();
            Descripcion = fila["DESCRIPCION"].ToString();
            Vigente = fila["VIGENTE"].ToString();
            Boton = strBoton;
        }

        public string Dummy { get; set; }
        public string Boton { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Vigente { get; set; }
    }
}