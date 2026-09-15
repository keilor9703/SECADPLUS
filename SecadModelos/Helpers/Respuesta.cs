using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecadModelos.Helpers
{
    public class Respuesta<T>
    {
        private string _Iserror;
        private string _Mensaje;
        private int _Codigoerror;
        private int _CodigoExito;
        private string _Operacion;
        private T _Data;

        /// <summary>
        /// Error que se presenta en el evento
        /// </summary>
        public string Iserror
        {
            get
            {
                return _Iserror;
            }
            set
            {
                _Iserror = value;
            }
        }

        /// <summary>
        /// Descripción mensaje que se presenta en el evento
        /// </summary>
        public string Mensaje
        {
            get
            {
                return _Mensaje;
            }
            set
            {
                _Mensaje = value;
            }
        }

        /// <summary>
        /// Código de error que se presenta en el evento
        /// </summary>
        public int Codigoerror
        {
            get
            {
                return _Codigoerror;
            }
            set
            {
                _Codigoerror = value;
            }
        }

        /// <summary>
        /// Operación que se presenta en el evento
        /// </summary>
        public string Operacion
        {
            get
            {
                return _Operacion;
            }
            set
            {
                _Operacion = value;
            }
        }

        /// <summary>
        /// Colección de datos de la respuesta
        /// </summary>
        public T Data
        {
            get
            {
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }

        /// <summary>
        /// Codigo de exito que se presenta en el evento, representa el ID de almacenamientoen base de datos
        /// </summary>
        public int CodigoExito
        {
            get
            {
                return _CodigoExito;
            }
            set
            {
                _CodigoExito = value;
            }
        }
    }
}
