using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa uma reserva de voo e hotel
    /// </summary>
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        // Chave estrangeira para User
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Chave estrangeira para Voo
        [Required]
        [ForeignKey("Voo")]
        public int VooId { get; set; }

        // Chave estrangeira para Hotel (opcional)
        [ForeignKey("Hotel")]
        public int? HotelId { get; set; }

        [Required(ErrorMessage = "A data de check-in é obrigatória")]
        [Display(Name = "Data de Check-in")]
        [DataType(DataType.Date)]
        public DateTime DataCheckIn { get; set; }

        [Required(ErrorMessage = "A data de check-out é obrigatória")]
        [Display(Name = "Data de Check-out")]
        [DataType(DataType.Date)]
        public DateTime DataCheckOut { get; set; }

        [Required(ErrorMessage = "O valor total é obrigatório")]
        [Range(0.01, 1000000, ErrorMessage = "O valor total deve ser maior que zero")]
        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal ValorTotal { get; set; }

        [Required]
        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva { get; set; } = DateTime.Now;

        [StringLength(10)]
        [Display(Name = "Número do Assento")]
        public string? NumeroAssento { get; set; }

        [StringLength(500)]
        [Display(Name = "Dados dos Passageiros")]
        public string? DadosPassageiros { get; set; }

        // Relacionamentos
        public virtual User? User { get; set; }
        public virtual Voo? Voo { get; set; }
        public virtual Hotel? Hotel { get; set; }
        public virtual Faturacao? Faturacao { get; set; }
    }
}
