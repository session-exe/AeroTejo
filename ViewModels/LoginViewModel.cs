using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    /// <summary>
    /// ViewModel para o login de utilizadores
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
