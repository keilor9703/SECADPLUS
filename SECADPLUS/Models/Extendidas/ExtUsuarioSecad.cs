using SECADEntities.SecadEntities;
using System;
using System.Data;

namespace SECADPLUS.Models.Extendidas
{
    public class ExtUsuarioSecad : CADUSUARIO
    {
        public ExtUsuarioSecad(DataRow fila, string strBoton = null)
        {
            CADCANA_FUERZA_ID = 0;
            CADCANA_CODIGO = 0;
            UsuarioOid = string.IsNullOrEmpty(fila["USUARIOID"].ToString()) ? "" : fila["USUARIOID"].ToString();
            USUARIO = string.IsNullOrEmpty(fila["USUARIO"].ToString()) ? "" : fila["USUARIO"].ToString();
            IDENTIFICACION = string.IsNullOrEmpty(fila["IDENTIFICACION"].ToString()) ? "" : fila["IDENTIFICACION"].ToString();
            Grado = string.IsNullOrEmpty(fila["GRAD_ALFABETICO"].ToString()) ? "" : fila["GRAD_ALFABETICO"].ToString();
            Departamento = string.IsNullOrEmpty(fila["DEPARTAMENTO"].ToString()) ? "" : fila["DEPARTAMENTO"].ToString();
            Ciudad = string.IsNullOrEmpty(fila["CIUDAD"].ToString()) ? "" : fila["CIUDAD"].ToString();
            if (!string.IsNullOrEmpty(fila["FECHA_DESDE"].ToString()))
            {
                FECHA_DESDE = Convert.ToDateTime(fila["FECHA_DESDE"].ToString());
                FechaDesde = Convert.ToDateTime(fila["FECHA_DESDE"].ToString()).ToShortDateString();
            }
            if (!string.IsNullOrEmpty(fila["FECHA_HASTA"].ToString()))
            {
                FECHA_HASTA = Convert.ToDateTime(fila["FECHA_HASTA"].ToString());
                FechaHasta = Convert.ToDateTime(fila["FECHA_HASTA"]).ToShortDateString();
            }
            CONDICION = string.IsNullOrEmpty(fila["CONDICIONID"].ToString()) ? "" : fila["CONDICIONID"].ToString();
            Sitio = string.IsNullOrEmpty(fila["SITIO"].ToString()) ? "" : fila["SITIO"].ToString();
            if (!string.IsNullOrEmpty(fila["SITIOID"].ToString())) SITIO_GRABACION = Convert.ToInt32(fila["SITIOID"]);
            CondicionDesc = string.IsNullOrEmpty(fila["CONDICION"].ToString()) ? "" : fila["CONDICION"].ToString();
            Fuerza = string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? "" : fila["FUERZA"].ToString();
            if (!string.IsNullOrEmpty(fila["ENTIDADID"].ToString())) CADCANA_FUERZA_ID = Convert.ToInt32(fila["ENTIDADID"].ToString());
            CanalDes = string.IsNullOrEmpty(fila["CANAL"].ToString()) ? "" : fila["CANAL"].ToString();
            if (!string.IsNullOrEmpty(fila["CANALID"].ToString())) CADCANA_CODIGO = Convert.ToInt32(fila["CANALID"].ToString());
            NombreEmpleado = string.IsNullOrEmpty(fila["NOMB_EMPLEADO"].ToString()) ? "" : fila["NOMB_EMPLEADO"].ToString();
            if (!string.IsNullOrEmpty(strBoton))
            {
                Boton = strBoton.Replace("_USUARIO", USUARIO).Replace("_SITIO", SITIO_GRABACION.ToString()).Replace("_FUERZAID", CADCANA_FUERZA_ID.ToString()).Replace("_CANALID", CADCANA_CODIGO.ToString()).Replace("_CONDICION", CONDICION);
            }
        }

        public ExtUsuarioSecad(DataRow fila, bool blAdmin, string strBoton = null)
        {
            CADCANA_FUERZA_ID = 0;
            CADCANA_CODIGO = 0;
            UsuarioOid = string.IsNullOrEmpty(fila["USUARIOID"].ToString()) ? "" : fila["USUARIOID"].ToString();
            USUARIO = string.IsNullOrEmpty(fila["USUARIO"].ToString()) ? "" : fila["USUARIO"].ToString();
            IDENTIFICACION = string.IsNullOrEmpty(fila["IDENTIFICACION"].ToString()) ? "" : fila["IDENTIFICACION"].ToString();
            Grado = string.IsNullOrEmpty(fila["GRAD_ALFABETICO"].ToString()) ? "" : fila["GRAD_ALFABETICO"].ToString();
            Departamento = string.IsNullOrEmpty(fila["DEPARTAMENTO"].ToString()) ? "" : fila["DEPARTAMENTO"].ToString();
            Ciudad = string.IsNullOrEmpty(fila["CIUDAD"].ToString()) ? "" : fila["CIUDAD"].ToString();
            if (!string.IsNullOrEmpty(fila["FECHA_DESDE"].ToString()))
            {
                FECHA_DESDE = Convert.ToDateTime(fila["FECHA_DESDE"].ToString());
                FechaDesde = Convert.ToDateTime(fila["FECHA_DESDE"].ToString()).ToShortDateString();
            }
            if (!string.IsNullOrEmpty(fila["FECHA_HASTA"].ToString()))
            {
                FECHA_HASTA = Convert.ToDateTime(fila["FECHA_HASTA"].ToString());
                FechaHasta = Convert.ToDateTime(fila["FECHA_HASTA"]).ToShortDateString();
            }
            CONDICION = string.IsNullOrEmpty(fila["CONDICIONID"].ToString()) ? "" : fila["CONDICIONID"].ToString();
            Sitio = string.IsNullOrEmpty(fila["SITIO"].ToString()) ? "" : fila["SITIO"].ToString();
            if (!string.IsNullOrEmpty(fila["SITIOID"].ToString())) SITIO_GRABACION = Convert.ToInt32(fila["SITIOID"]);
            CondicionDesc = string.IsNullOrEmpty(fila["CONDICION"].ToString()) ? "" : fila["CONDICION"].ToString();
            Fuerza = string.IsNullOrEmpty(fila["FUERZA"].ToString()) ? "" : fila["FUERZA"].ToString();
            if (!string.IsNullOrEmpty(fila["ENTIDADID"].ToString())) CADCANA_FUERZA_ID = Convert.ToInt32(fila["ENTIDADID"].ToString());
            CanalDes = string.IsNullOrEmpty(fila["CANAL"].ToString()) ? "" : fila["CANAL"].ToString();
            if (!string.IsNullOrEmpty(fila["CANALID"].ToString())) CADCANA_CODIGO = Convert.ToInt32(fila["CANALID"].ToString());
            NombreEmpleado = string.IsNullOrEmpty(fila["NOMB_EMPLEADO"].ToString()) ? "" : fila["NOMB_EMPLEADO"].ToString();
            Boton = strBoton;
        }

        public string UsuarioOid { get; set; }
        public string Grado { get; set; }
        public string Departamento { get; set; }
        public string Ciudad { get; set; }
        public string Sitio { get; set; }
        public string CondicionDesc { get; set; }
        public string Fuerza { get; set; }
        public string CanalDes { get; set; }
        public string NombreEmpleado { get; set; }
        public string Boton { get; set; }
        public string Dummy { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
    }
}