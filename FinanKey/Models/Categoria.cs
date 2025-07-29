namespace FinanKey.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public TipoCategoria tipoCategoria { get; set; }
    }
}
