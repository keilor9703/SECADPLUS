using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SECADPLUS.Models
{
   

    public class ImagenService
    {
        public static List<ImagenModel> ObtenerImagenes()  //ObtenerImagenes( int cantidad = 5) Para obtener las útimas 5 fotos agregadas a la carpeta
        {
            var imagenes = new List<ImagenModel>();
            var rutaCarpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Carrusel");

            if (Directory.Exists(rutaCarpeta))
            {
                var archivos = Directory.GetFiles(rutaCarpeta).Where(f =>
                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)

                // .OrderByDescending(f => new FileInfo(f).LastWriteTime).Take(cantidad); // Para obtener las útimas 5 fotos agregadas a la carpeta
                );

                foreach (var archivo in archivos)
                {
                    var nombreArchivo = Path.GetFileName(archivo);
                    imagenes.Add(new ImagenModel
                    {
                        Ruta = $"~/Images/Carrusel/{nombreArchivo}",
                        Alt = $"Imagen: {nombreArchivo}"
                    });
                }
            }

            return imagenes;
        }
    }
}