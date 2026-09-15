using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SECADPLUS.Models
{
    public class UtilidadesDeMapeo
    {
        public static List<T> ConvertirDataTableAListaDto<T>(DataTable dt)
        {
            const BindingFlags bandera = BindingFlags.Public | BindingFlags.Instance;
            var NombreDeLasColumnas = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName.ToUpper())
                .ToList();
            var PropiedadesDelObjeto = typeof(T).GetProperties(bandera);
            var ListaDto = dt.AsEnumerable().Select(datosFila =>
            {
                var crearInstancia = Activator.CreateInstance<T>();

                foreach (var propiedad in PropiedadesDelObjeto.Where(propiedades => NombreDeLasColumnas.Contains(propiedades.Name) && datosFila[propiedades.Name] != DBNull.Value))
                {
                    propiedad.SetValue(crearInstancia, datosFila[propiedad.Name.ToUpper()], null);
                }
                return crearInstancia;
            }).ToList();



            return ListaDto;
        }
    }
}