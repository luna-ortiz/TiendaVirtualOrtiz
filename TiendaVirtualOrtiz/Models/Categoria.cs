using System.ComponentModel.DataAnnotations;
using TiendaVirtualOrtiz.Models;

namespace TiendaVirtualOrtiz.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(1000)]
        public string Descripcion { get; set; }

        
        [StringLength(100)]
        public string Estado { get; set; }
    }
}