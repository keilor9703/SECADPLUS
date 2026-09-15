using SECADEntities.SECAD;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtCasosCanal : CADCASOS_CANAL
    {
        public ExtCasosCanal(DataRow fila)
        {
            CADCASO_CODIGO = fila["CADCASO_CODIGO"].ToString();
            CADCANA_FUERZ_ID = int.Parse(fila["CADCANA_FUERZ_ID"].ToString());
            CADCANA_CODIGO = int.Parse(fila["CADCANA_CODIGO"].ToString());

            DESCRIPCION = fila["DESCRIPCION"].ToString();
        }

        public ExtCasosCanal(DataRow fila, string strBoton)
        {
            CADCASO_CODIGO = fila["CODIGO"].ToString();
            DESCRIPCION = fila["DESCRIPCION"].ToString();
            Boton = strBoton;
        }

        public string DESCRIPCION { get; set; }
        public string Dummy { get; set; }
        public string Boton { get; set; }
    }
}