using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Completada")]
        public bool Completada { get; set; } = false;

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de realización")]
        [DataType(DataType.Date)]
        public DateTime? FechaRealizacion { get; set; }

        public byte[]? ImagenDatos { get; set; }

        [StringLength(100)]
        public string? ImagenContentType { get; set; }
    }
}