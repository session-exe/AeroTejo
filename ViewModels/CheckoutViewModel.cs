using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "O nome no cartão é obrigatório")]
        public string NomeCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número do cartão é obrigatório")]
        [CreditCard(ErrorMessage = "Número de cartão inválido")]
        public string NumeroCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A validade é obrigatória")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Formato inválido (MM/AA)")]
        public string Validade { get; set; } = string.Empty; // Formato MM/AA

        [Required(ErrorMessage = "CVV obrigatório")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV inválido")]
        public string CVV { get; set; } = string.Empty;

        // --- DADOS DE FATURAÇÃO ---

        // CORREÇÃO: NIF agora é opcional (removemos o [Required])
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NIF deve ter 9 dígitos")]
        public string? NIF { get; set; }

        [Required(ErrorMessage = "A morada é obrigatória")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome para a fatura é obrigatório")]
        public string Nome { get; set; } = string.Empty;
    }
}