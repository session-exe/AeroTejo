using AeroTejo.Models;
using AeroTejo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AeroTejo.ViewModels;

namespace AeroTejo.Data
{
    public static class DbSeeder
    {
        public static void Seed(AeroTejoContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any()) return;

            Console.WriteLine("A iniciar Seeding...");

            // 1. UTILIZADORES
            string salt = PasswordHelper.GenerateSalt();
            var admin = new User { NomeCompleto = "Administrador", Idade = 35, Email = "admin@aerotejo.pt", Telemovel = "910000000", PasswordHash = PasswordHelper.HashPassword("admin123", salt), PasswordSalt = salt, Role = "Administrador" };
            var joao = new User { NomeCompleto = "João Silva", Idade = 28, Email = "joao@email.pt", Telemovel = "912345678", PasswordHash = PasswordHelper.HashPassword("pass123", salt), PasswordSalt = salt, Role = "Passageiro" };
            var ana = new User { NomeCompleto = "Ana Pereira", Idade = 32, Email = "ana@email.pt", Telemovel = "966554433", PasswordHash = PasswordHelper.HashPassword("pass123", salt), PasswordSalt = salt, Role = "Passageiro" };

            context.Users.AddRange(admin, joao, ana);
            context.SaveChanges();

            // 2. VOOS
            var voos = new List<Voo>
            {
                new Voo { Companhia = "TAP", Origem = "Lisboa", Destino = "Paris", DataHora = DateTime.Now.AddDays(2), Preco = 150.00m, TotalLugares = 180 },
                new Voo { Companhia = "British Airways", Origem = "Lisboa", Destino = "Londres", DataHora = DateTime.Now.AddDays(5), Preco = 120.50m, TotalLugares = 180 },
                new Voo { Companhia = "Iberia", Origem = "Porto", Destino = "Madrid", DataHora = DateTime.Now.AddDays(10), Preco = 85.00m, TotalLugares = 150 },
                new Voo { Companhia = "Lufthansa", Origem = "Faro", Destino = "Berlim", DataHora = DateTime.Now.AddDays(15), Preco = 200.00m, TotalLugares = 180 },
                new Voo { Companhia = "Emirates", Origem = "Lisboa", Destino = "Dubai", DataHora = DateTime.Now.AddDays(20), Preco = 600.00m, TotalLugares = 300 },
                new Voo { Companhia = "Ryanair", Origem = "Porto", Destino = "Roma", DataHora = DateTime.Now.AddDays(8), Preco = 45.99m, TotalLugares = 180 },
                new Voo { Companhia = "Ryanair", Origem = "Faro", Destino = "Lisboa", DataHora = DateTime.Now.AddDays(30), Preco = 29.99m, TotalLugares = 180 }
            };

            context.Voos.AddRange(voos);
            context.SaveChanges();

            // 3. ASSENTOS (CORREÇÃO: 1A, 1B, 1C...)
            var letras = new[] { "A", "B", "C", "D", "E", "F" };
            var assentos = new List<Assento>();

            foreach (var voo in voos)
            {
                int filas = voo.TotalLugares / 6;
                for (int fila = 1; fila <= filas; fila++)
                {
                    foreach (var letra in letras)
                    {
                        assentos.Add(new Assento
                        {
                            VooId = voo.Id,
                            NumeroAssento = $"{fila}{letra}", // Gera: 1A, 1B... 10A... 30F
                            IsOccupied = false
                        });
                    }
                }
            }
            context.Assentos.AddRange(assentos);
            context.SaveChanges();

            // 4. HOTÉIS
            var hoteis = new List<Hotel>
            {
                new Hotel { Nome = "Eiffel Tower Stay", Cidade = "Paris", PrecoPorNoite = 250.00m, Descricao = "Vista para a torre." },
                new Hotel { Nome = "London City Inn", Cidade = "Londres", PrecoPorNoite = 180.00m, Descricao = "No centro da cidade." },
                new Hotel { Nome = "Madrid Plaza", Cidade = "Madrid", PrecoPorNoite = 90.00m, Descricao = "Económico e central." },
                new Hotel { Nome = "Berlin Wall Hotel", Cidade = "Berlim", PrecoPorNoite = 120.00m, Descricao = "Histórico." },
                new Hotel { Nome = "Algarve Beach Resort", Cidade = "Faro", PrecoPorNoite = 150.00m, Descricao = "Resort à beira-mar." },
                new Hotel { Nome = "Hotel Tejo Palace", Cidade = "Lisboa", PrecoPorNoite = 120.00m, Descricao = "Luxo no centro." }
            };
            context.Hoteis.AddRange(hoteis);
            context.SaveChanges();

            // 5. RESERVAS DE EXEMPLO
            var vooParis = voos.First(v => v.Destino == "Paris");

            // Ocupar 1A
            var assentoOcupado = context.Assentos.First(a => a.VooId == vooParis.Id && a.NumeroAssento == "1A");
            assentoOcupado.IsOccupied = true;

            var listaPax = new List<PassageiroInfo> { new PassageiroInfo { NomeCompleto = "João Silva", NumeroDocumento = "123", AssentoSelecionado = "1A" } };

            var reserva = new Reserva
            {
                UserId = joao.Id,
                VooId = vooParis.Id,
                HotelId = null,
                DataCheckIn = vooParis.DataHora.Date,
                DataCheckOut = vooParis.DataHora.Date,
                DataReserva = DateTime.Now,
                NumeroAssento = "1A",
                DadosPassageiros = JsonSerializer.Serialize(listaPax),
                ValorTotal = vooParis.Preco
            };
            context.Reservas.Add(reserva);
            context.SaveChanges();

            context.Faturacoes.Add(new Faturacao { ReservaId = reserva.Id, Nome = "João Silva", NIF = "999999999", Morada = "Lisboa", DataEmissao = DateTime.Now });
            context.SaveChanges();

            Console.WriteLine("Dados populados com sucesso!");
        }
    }
}