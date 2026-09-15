using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models
{

    public class ConexionesNotificadorHub<T>
    {
        private readonly Dictionary<T, HashSet<string>> _conexiones =
            new Dictionary<T, HashSet<string>>();

        public Dictionary<T, HashSet<string>> Coleccion {
            get { return _conexiones; }
        }
        public int Count
        {
            get
            {
                return _conexiones.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_conexiones)
            {
                HashSet<string> connections;
                if (!_conexiones.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _conexiones.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_conexiones.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_conexiones)
            {
                HashSet<string> connections;
                if (!_conexiones.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _conexiones.Remove(key);
                    }
                }
            }
        }
    }
    public class NotificacionModel {
        public string Mensaje { get; set; }
        public decimal Consecutivo { get; set; }

    }
}