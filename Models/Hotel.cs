using System.ComponentModel.DataAnnotations;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa um hotel/alojamento
    /// </summary>
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do hotel é obrigatório")]
        [StringLength(150, ErrorMessage = "O nome não pode exceder 150 caracteres")]
        [Display(Name = "Nome do Hotel")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(100, ErrorMessage = "A cidade não pode exceder 100 caracteres")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O preço por noite é obrigatório")]
        [Range(0.01, 10000, ErrorMessage = "O preço deve estar entre 0.01 e 10000")]
        [Display(Name = "Preço por Noite")]
        [DataType(DataType.Currency)]
        public decimal PrecoPorNoite { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        // Relacionamentos
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
