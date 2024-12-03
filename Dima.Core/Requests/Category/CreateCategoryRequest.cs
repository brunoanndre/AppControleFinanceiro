using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Category
{
    public class CreateCategoryRequest : Request
    {
        [Required(ErrorMessage = "Título inválido")]
        [MaxLength(50,ErrorMessage = "O título deve conter até 50 caracteres.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Descrição inválida")]
        public string Description { get; set; } = string.Empty;
    }
}
