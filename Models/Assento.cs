using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa um assento de um voo
    /// </summary>
    public class Assento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O número do assento é obrigatório")]
        [StringLength(10, ErrorMessage = "O número do assento não pode exceder 10 caracteres")]
        [Display(Name = "Número do Assento")]
        public string NumeroAssento { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ocupado")]
        public bool IsOccupied { get; set; } = false;

        // Chave estrangeira para Voo
        [Required]
        [ForeignKey("Voo")]
        public int VooId { get; set; }

        // Relacionamento
        public virtual Voo? Voo { get; set; }
    }
}
