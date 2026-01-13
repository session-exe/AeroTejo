using System.ComponentModel.DataAnnotations;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa um utilizador do sistema
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

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
        [StringLength(100, ErrorMessage = "O e-mail não pode exceder 100 caracteres")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telemóvel é obrigatório")]
        [Phone(ErrorMessage = "Número de telemóvel inválido")]
        [StringLength(20, ErrorMessage = "O telemóvel não pode exceder 20 caracteres")]
        [Display(Name = "Telemóvel")]
        public string Telemovel { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required(ErrorMessage = "O papel do utilizador é obrigatório")]
        [StringLength(50)]
        [Display(Name = "Papel")]
        public string Role { get; set; } = "Passageiro"; // "Administrador" ou "Passageiro"

        // Relacionamentos
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
