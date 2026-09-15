using System;
using System.Collections.Specialized;

namespace SECADPLUS.Models
{
    public class DatatableForm
    {
        public DatatableForm(NameValueCollection formulario)
        {
            if (formulario["start"] == null)
            {
                return;
            }
            Inicio = 0;
            CantidadRegistros = 0;
            TotalRegistros = 0;
            TotalRegistroFiltrados = 0;
            Final = 0;
            IndiceOrdenamiento = 0;
            Inicio = Convert.ToInt32(formulario["start"].ToString());
            CantidadRegistros = Convert.ToInt32(formulario["length"].ToString());
            ConsultaValor = formulario["search[value]"].ToString();
            //if (!Regex.IsMatch(ConsultaValor, @"^[a-zA-Z'.\s]{1,40}$"))
            //{
            //    // Name does not match expression
            //    ConsultaValor = "";
            //}
            ConsultaRegular = Convert.ToBoolean(formulario["search[regex]"].ToString());
            TipoOrdenamiento = formulario["order[0][dir]"].ToString();
            IndiceOrdenamiento = Convert.ToInt32(formulario["order[0][column]"].ToString());
            Final = Inicio + CantidadRegistros;
        }

        public string ConsultaValor { get; set; }
        public bool ConsultaRegular { get; set; }
        public int IndiceOrdenamiento { get; set; }
        public string TipoOrdenamiento { get; set; }
        public int Draw { get; set; }
        public int Inicio { get; set; }
        public int CantidadRegistros { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalRegistroFiltrados { get; set; }
        public int Final { get; set; }
    }
}