using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    /// <summary>
    /// ViewModel para o registo de novos utilizadores
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A idade é obrigatória")]
        [Range(18, 120, ErrorMessage = "A idade deve estar entre 18 e 120 anos")]
        [Display(Name = "Idade")]
        public int Idade { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telemóvel é obrigatório")]
        [Phone(ErrorMessage = "Número de telemóvel inválido")]
        [Display(Name = "Telemóvel")]
        public string Telemovel { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A password deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação da password é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Password")]
        [Compare("Password", ErrorMessage = "As passwords não coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
