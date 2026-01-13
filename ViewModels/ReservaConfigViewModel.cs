using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    /// <summary>
    /// ViewModel para configuração da reserva (seleção de assento e dados de passageiros)
    /// </summary>
    public class ReservaConfigViewModel
    {
        [Required(ErrorMessage = "Selecione um assento")]
        [Display(Name = "Assento")]
        public int AssentoId { get; set; }

        [Required(ErrorMessage = "A data de check-in é obrigatória")]
        [Display(Name = "Data de Check-in")]
        [DataType(DataType.Date)]
        public DateTime DataCheckIn { get; set; }

        [Required(ErrorMessage = "A data de check-out é obrigatória")]
        [Display(Name = "Data de Check-out")]
        [DataType(DataType.Date)]
        public DateTime DataCheckOut { get; set; }

        [Required(ErrorMessage = "Os dados dos passageiros são obrigatórios")]
        [StringLength(500, ErrorMessage = "Os dados não podem exceder 500 caracteres")]
        [Display(Name = "Dados dos Passageiros")]
        public string DadosPassageiros { get; set; } = string.Empty;
    }
}
