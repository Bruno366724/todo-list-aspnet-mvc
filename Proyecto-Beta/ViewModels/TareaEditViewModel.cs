using System.ComponentModel.DataAnnotations;

namespace TodoListApp.ViewModels
{
    public class TareaEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        public bool TieneImagenActual { get; set; }

        [Display(Name = "Nueva imagen")]
        public IFormFile? Imagen { get; set; }
    }
}