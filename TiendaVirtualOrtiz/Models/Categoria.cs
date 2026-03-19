using System.ComponentModel.DataAnnotations;

namespace TiendaVirtualOrtiz.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Range(0, 1000000)]
        //luna
        public string Descripcion { get; set; }

        [Range(0, 1)]
        public string Estado { get; set; }
    }
}