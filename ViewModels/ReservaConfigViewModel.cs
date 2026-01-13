using AeroTejo.Models;
using System.ComponentModel.DataAnnotations;

namespace AeroTejo.ViewModels
{
    public class PassageiroInfo
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O documento é obrigatório")]
        public string NumeroDocumento { get; set; } = string.Empty;

        // CORREÇÃO: Assento agora é por passageiro
        [Required(ErrorMessage = "Escolha um assento")]
        public string AssentoSelecionado { get; set; } = string.Empty;
    }

    public class ReservaConfigViewModel
    {
        public int VooId { get; set; }
        public Voo? Voo { get; set; }

        public int? HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataCheckIn { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataCheckOut { get; set; }

        // Lista de assentos livres para preencher os dropdowns
        public List<string> AssentosDisponiveis { get; set; } = new();

        public List<PassageiroInfo> Passageiros { get; set; } = new List<PassageiroInfo> { new PassageiroInfo() };
    }
}