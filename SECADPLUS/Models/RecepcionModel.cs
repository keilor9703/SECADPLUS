using SECADEntities.SecadEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SECADPLUS.Models
{
    public class RecepcionModel : CADPEDIDO
    {
        [Display(Name = "Sitios de grabación")]
        public List<System.Web.Mvc.SelectListItem> ListaCanales { get; set; }

        public string Condicion { get; set; }
        public System.Web.Mvc.SelectList Medios { get; set; }
        public System.Web.Mvc.SelectList Tipos { get; set; }
        public System.Web.Mvc.SelectList Calidad { get; set; }
        public System.Web.Mvc.SelectList Coordenadas { get; set; }
        public string Consecutivo { get; set; }
        public string FechaHora { get; set; }
        public string Conexiones { get; set; }
        public string ACD { get; set; }
        public int? LugarSitio { get; set; }
        public string Usuario { get; set; }
        public string Identificacion { get; set; }
        public int? LugeCad { get; set; }
        public string Numeracion { get; set; }
        public string ConexionDijin { get; set; }
        public string ConexionTelepol { get; set; }
        public int? LugeDpto { get; set; }
        public int? Fuerza { get; set; }
        public int? Canal { get; set; }
        public string Ciudad { get; set; }
        public string Barrio { get; set; }
        public long? lgCiudaCodigo { get; set; }
        public string CasoDesc { get; set; }
        public string Caso2Desc { get; set; }
        public string CasoRealDesc { get; set; }
        public string SitioOriginal { get; set; }
        public string LlamadaOriginal { get; set; }
        public string Modo { get; set; }
        public string CodDane { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        //rmr 05-12-2020
        public string ConexionMapa { get; set; }
        //fin rmr 05-12-2020
    }
}