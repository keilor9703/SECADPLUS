using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models
{
    public class VistaCuadrantesCivic
    {
        public VistaCuadrantesCivic(DataRow fila) {
            Sitio = fila["SITIO_GRABA"].ToString();
            Consecutivo = fila["CONSECUTIVO"].ToString();
            FuerzaUnidad = fila["CADUNID_FUERZA"].ToString();
            CodigoUnidad = fila["CADUNID_CODIGO"].ToString();
            NroCuadrante = fila["NRO_CUADRANTE"].ToString();
            CapaCuadrante = fila["CAPA_CUADRANTE"].ToString();
            Ambito = fila["AMBITO"].ToString();
            if (fila["VIGENTE"].ToString().ToUpper().Equals("S"))
            {
                Vigente = "<span class='label label-success'>SI</span>";
            }else
            {
                Vigente = "<span class='label label-danger'>NO</span>";
            }            
            FechaDesde = fila["FECHA_DESDE"].ToString();
            FechaHasta = fila["FECHA_HASTA"].ToString();
            Descripcion = fila["DESCRIPCION"].ToString();
            UsuarioTrans = fila["USUA_TRANSACCION"].ToString();
            NivelPeligrosidad = fila["NIVEL_PELIGROSIDAD"].ToString();
            CentroideX = fila["CENTROIDEX"].ToString();
            CentroideY = fila["CENTROIDEY"].ToString();
            CAD1CUADSITGR_DIJIN = fila["CAD1CUADSITGR_DIJIN"].ToString();
            CAD1CUADCONS_DIJIN = fila["CAD1CUADCONS_DIJIN"].ToString();
            CAD1CUADCONS_DIPOL = fila["CAD1CUADCONS_DIPOL"].ToString();
            CAD1CUADSITGR_DIPOL = fila["CAD1CUADSITGR_DIPOL"].ToString();
            SiglaDpto = fila["SIGLA_DPTO"].ToString();
            MotivoCierre = fila["MOTIVO_CIERRE"].ToString();
        }
        public string Dummy { get; set; }
        public string Sitio { get; set; }
        public string Consecutivo { get; set; }
        public string FuerzaUnidad { get; set; }
        public string CodigoUnidad { get; set; }
        public string NroCuadrante { get; set; }
        public string CapaCuadrante { get; set; }
        public string Ambito { get; set; }
        public string Vigente { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Descripcion { get; set; }
        public string UsuarioTrans { get; set; }
        public string NivelPeligrosidad { get; set; }
        public string CentroideX { get; set; }
        public string CentroideY { get; set; }
        public string CAD1CUADSITGR_DIJIN { get; set; }
        public string CAD1CUADCONS_DIJIN { get; set; }
        public string CAD1CUADSITGR_DIPOL { get; set; }
        public string CAD1CUADCONS_DIPOL { get; set; }
        public string SiglaDpto { get; set; }
        public string MotivoCierre { get; set; }
    }
}