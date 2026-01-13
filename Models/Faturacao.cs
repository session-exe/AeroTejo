using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa os dados de faturação para geração de PDF
    /// </summary>
    public class Faturacao
    {
        [Key]
        public int Id { get; set; }

        // Chave estrangeira para Reserva
        [Required]
        [ForeignKey("Reserva")]
        public int ReservaId { get; set; }

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
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Data de Emissão")]
        public DateTime DataEmissao { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Caminho do PDF")]
        public string? CaminhoPDF { get; set; }

        // Relacionamento
        public virtual Reserva? Reserva { get; set; }
    }
}
