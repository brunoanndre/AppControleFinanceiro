using Dima.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Transaction
{
    public class CreateTransactionRequest : Request
    {
        [Required(ErrorMessage ="Título inválido")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Tipo inválido")]
        public ETransactionType Type { get; set; } = ETransactionType.Withdraw;
        [Required(ErrorMessage = "Quantidade inválida")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Categoria inválida")]
        public long CategoryId { get; set; }
        [Required(ErrorMessage = "Data inválida")]
        public DateTime? PaidOrReceivedAt { get; set; }

    }
}
