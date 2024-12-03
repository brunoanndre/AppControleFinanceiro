using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Category
{
    public class UpdateCategoryRequest : Request
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Título inválido")]
        [MaxLength(50, ErrorMessage = "O título deve conter até 50 caracteres.")]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; } 
    }
}
