using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    /// <summary>
    /// ViewModel para o checkout (dados de pagamento e faturação)
    /// </summary>
    public class CheckoutViewModel
    {
        // Dados de Pagamento (Mock)
        [Required(ErrorMessage = "O número do cartão é obrigatório")]
        [StringLength(19, MinimumLength = 16, ErrorMessage = "Número de cartão inválido")]
        [Display(Name = "Número do Cartão")]
        public string NumeroCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome no cartão é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Display(Name = "Nome no Cartão")]
        public string NomeCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de validade é obrigatória")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Formato: MM/AA")]
        [Display(Name = "Validade (MM/AA)")]
        public string Validade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CVV é obrigatório")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV inválido")]
        [Display(Name = "CVV")]
        public string CVV { get; set; } = string.Empty;

        // Dados de Faturação
        [Required(ErrorMessage = "O NIF é obrigatório")]
        [StringLength(20, ErrorMessage = "O NIF não pode exceder 20 caracteres")]
        [Display(Name = "NIF")]
        public string NIF { get; set; } = string.Empty;

        [Required(ErrorMessage = "A morada é obrigatória")]
        [StringLength(250, ErrorMessage = "A morada não pode exceder 250 caracteres")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Display(Name = "Nome para Faturação")]
        public string Nome { get; set; } = string.Empty;
    }
}
