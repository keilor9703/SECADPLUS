namespace SECADPLUS.Models
{
    public class Menu
    {
        public int Nivel { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Tipo { get; set; }
        public string Icono { get; set; }
        public int Id { get; set; }
        public int Padre_Id { get; set; }
    }
}