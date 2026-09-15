using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Areas.MinutaSivicc
{
    public class MdMinutaEmpl
    {
        public Int64 MinutaEmplId { get; set; }

        public string Funcionario { get; set; }

        public string Unidad { get; set; }

        public string Turno { get; set; }

        public string TipoMinuta { get; set; }

        public string Inicio { get; set; }

        public string Fin { get; set; }

        public string Fecha { get; set; }

        public string CodigoMinuta { get; set; }

        public string Cuadrante { get; set; }

        public Int32 UndeConsecutivo { get; set; }

        public Int32 UndeCai { get; set; }

        public int Arma { get; set; }

        public int Veh { get; set; }

        public int Comunica { get; set; }



    }

    public class MdMinutaEmplNew
    {
        public Int64 MinutaEmplId { get; set; }
        public string Funcionario { get; set; }
        public string Turno { get; set; }
        public string Unidad { get; set; }
        public string TipoMinuta { get; set; }
        public string Inicio { get; set; }
        public string Fin { get; set; }
        public string Cuadrante { get; set; }
        public int Arma { get; set; }
        public int Veh { get; set; }
        public int Comunica { get; set; }
        public string Asistencia { get; set; }
    }

    public class MdArmasEmpl
    {
        public Int64 IdArmamento { get; set; }
        public String TipoElemento { get; set; }
        public String Marca { get; set; }
        public String Calibre { get; set; }
        public String Serie { get; set; }
        public String Estado { get; set; }
        public Int64 IdInventario { get; set; }

    }

    public class MdComunicaEmpl
    {
        public String TipoElemento { get; set; }
        public String Marca { get; set; }
        public String Modelo { get; set; }
        public String Serie { get; set; }
        public String Estado { get; set; }
        public Int64 IdInventario { get; set; }

    }

    public class MdVehiEmpl
    {
        public String TipoElemento { get; set; }
        public String Marca { get; set; }
        public String Modelo { get; set; }
        public String Sigla { get; set; }
        public String Placa { get; set; }
        public String Estado { get; set; }
        public Int64 IdInventario { get; set; }

    }
}
