using System.ComponentModel.DataAnnotations;

namespace AeroTejo.Models
{
    /// <summary>
    /// Modelo que representa um voo
    /// </summary>
    public class Voo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A companhia aérea é obrigatória")]
        [StringLength(100, ErrorMessage = "O nome da companhia não pode exceder 100 caracteres")]
        [Display(Name = "Companhia Aérea")]
        public string Companhia { get; set; } = string.Empty;

        [Required(ErrorMessage = "A origem é obrigatória")]
        [StringLength(100, ErrorMessage = "A origem não pode exceder 100 caracteres")]
        [Display(Name = "Origem")]
        public string Origem { get; set; } = string.Empty;

        [Required(ErrorMessage = "O destino é obrigatório")]
        [StringLength(100, ErrorMessage = "O destino não pode exceder 100 caracteres")]
        [Display(Name = "Destino")]
        public string Destino { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data e hora são obrigatórias")]
        [Display(Name = "Data e Hora")]
        public DateTime DataHora { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, 100000, ErrorMessage = "O preço deve estar entre 0.01 e 100000")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O total de lugares é obrigatório")]
        [Range(1, 500, ErrorMessage = "O total de lugares deve estar entre 1 e 500")]
        [Display(Name = "Total de Lugares")]
        public int TotalLugares { get; set; }

        // Relacionamentos
        public virtual ICollection<Assento> Assentos { get; set; } = new List<Assento>();
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
