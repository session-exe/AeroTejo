using AeroTejo.Models;
using AeroTejo.Helpers;

namespace AeroTejo.Data
{
    /// <summary>
    /// Classe responsável por popular a base de dados com dados iniciais
    /// </summary>
    public static class DbSeeder
    {
        public static void Seed(AeroTejoContext context)
        {
            // Verificar se já existem dados
            if (context.Users.Any())
            {
                return; // Base de dados já foi populada
            }

            // Criar utilizador administrador
            string adminSalt = PasswordHelper.GenerateSalt();
            string adminHash = PasswordHelper.HashPassword("admin123", adminSalt);

            var admin = new User
            {
                NomeCompleto = "Administrador Sistema",
                Idade = 30,
                Email = "admin@aerotejo.pt",
                Telemovel = "+351 910 000 000",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Role = "Administrador"
            };

            context.Users.Add(admin);

            // Criar utilizador passageiro de teste
            string userSalt = PasswordHelper.GenerateSalt();
            string userHash = PasswordHelper.HashPassword("pass123", userSalt);

            var passageiro = new User
            {
                NomeCompleto = "João Silva",
                Idade = 28,
                Email = "joao.silva@email.pt",
                Telemovel = "+351 912 345 678",
                PasswordHash = userHash,
                PasswordSalt = userSalt,
                Role = "Passageiro"
            };

            context.Users.Add(passageiro);

            // Criar voos de exemplo
            var voos = new List<Voo>
            {
                new Voo
                {
                    Companhia = "TAP Air Portugal",
                    Origem = "Lisboa",
                    Destino = "Porto",
                    DataHora = DateTime.Now.AddDays(7).AddHours(10),
                    Preco = 89.99m,
                    TotalLugares = 180
                },
                new Voo
                {
                    Companhia = "Ryanair",
                    Origem = "Porto",
                    Destino = "Faro",
                    DataHora = DateTime.Now.AddDays(10).AddHours(14),
                    Preco = 49.99m,
                    TotalLugares = 189
                },
                new Voo
                {
                    Companhia = "EasyJet",
                    Origem = "Lisboa",
                    Destino = "Faro",
                    DataHora = DateTime.Now.AddDays(5).AddHours(8),
                    Preco = 59.99m,
                    TotalLugares = 156
                },
                new Voo
                {
                    Companhia = "TAP Air Portugal",
                    Origem = "Porto",
                    Destino = "Lisboa",
                    DataHora = DateTime.Now.AddDays(14).AddHours(16),
                    Preco = 79.99m,
                    TotalLugares = 180
                },
                new Voo
                {
                    Companhia = "Ryanair",
                    Origem = "Faro",
                    Destino = "Lisboa",
                    DataHora = DateTime.Now.AddDays(12).AddHours(12),
                    Preco = 54.99m,
                    TotalLugares = 189
                }
            };

            context.Voos.AddRange(voos);
            context.SaveChanges();

            // Criar assentos para cada voo
            foreach (var voo in voos)
            {
                for (int i = 1; i <= voo.TotalLugares; i++)
                {
                    var assento = new Assento
                    {
                        VooId = voo.Id,
                        NumeroAssento = $"{(char)('A' + (i - 1) / 6)}{((i - 1) % 6) + 1}",
                        IsOccupied = false
                    };
                    context.Assentos.Add(assento);
                }
            }

            // Criar hotéis de exemplo
            var hoteis = new List<Hotel>
            {
                new Hotel
                {
                    Nome = "Hotel Tejo Palace",
                    Cidade = "Lisboa",
                    PrecoPorNoite = 120.00m,
                    Descricao = "Hotel de luxo no centro de Lisboa com vista para o Tejo."
                },
                new Hotel
                {
                    Nome = "Porto Riverside Hotel",
                    Cidade = "Porto",
                    PrecoPorNoite = 95.00m,
                    Descricao = "Hotel moderno junto ao rio Douro com excelentes comodidades."
                },
                new Hotel
                {
                    Nome = "Algarve Beach Resort",
                    Cidade = "Faro",
                    PrecoPorNoite = 150.00m,
                    Descricao = "Resort à beira-mar com piscinas e spa."
                },
                new Hotel
                {
                    Nome = "Lisboa Central Inn",
                    Cidade = "Lisboa",
                    PrecoPorNoite = 75.00m,
                    Descricao = "Hotel económico no coração de Lisboa."
                },
                new Hotel
                {
                    Nome = "Porto Vintage Hotel",
                    Cidade = "Porto",
                    PrecoPorNoite = 110.00m,
                    Descricao = "Hotel boutique com decoração tradicional portuguesa."
                },
                new Hotel
                {
                    Nome = "Faro Marina Hotel",
                    Cidade = "Faro",
                    PrecoPorNoite = 85.00m,
                    Descricao = "Hotel junto à marina com vista para o mar."
                }
            };

            context.Hoteis.AddRange(hoteis);
            context.SaveChanges();

            Console.WriteLine("Base de dados populada com sucesso!");
            Console.WriteLine("Utilizador Admin: admin@aerotejo.pt / admin123");
            Console.WriteLine("Utilizador Passageiro: joao.silva@email.pt / pass123");
        }
    }
}
