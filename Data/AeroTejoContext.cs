using Microsoft.EntityFrameworkCore;
using AeroTejo.Models;

namespace AeroTejo.Data
{
    /// <summary>
    /// Contexto da base de dados para a aplicação AeroTejo
    /// </summary>
    public class AeroTejoContext : DbContext
    {
        public AeroTejoContext(DbContextOptions<AeroTejoContext> options)
            : base(options)
        {
        }

        // DbSets para cada entidade
        public DbSet<User> Users { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Hotel> Hoteis { get; set; }
        public DbSet<Assento> Assentos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Faturacao> Faturacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de relacionamentos e restrições

            // User -> Reservas (1:N)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservas)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Voo -> Assentos (1:N)
            modelBuilder.Entity<Voo>()
                .HasMany(v => v.Assentos)
                .WithOne(a => a.Voo)
                .HasForeignKey(a => a.VooId)
                .OnDelete(DeleteBehavior.Cascade);

            // Voo -> Reservas (1:N)
            modelBuilder.Entity<Voo>()
                .HasMany(v => v.Reservas)
                .WithOne(r => r.Voo)
                .HasForeignKey(r => r.VooId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hotel -> Reservas (1:N)
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Reservas)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reserva -> Faturacao (1:1)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Faturacao)
                .WithOne(f => f.Reserva)
                .HasForeignKey<Faturacao>(f => f.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuração de precisão para campos decimais
            modelBuilder.Entity<Voo>()
                .Property(v => v.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Hotel>()
                .Property(h => h.PrecoPorNoite)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reserva>()
                .Property(r => r.ValorTotal)
                .HasPrecision(18, 2);

            // Índices para melhorar performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Voo>()
                .HasIndex(v => v.Destino);

            modelBuilder.Entity<Hotel>()
                .HasIndex(h => h.Cidade);
        }
    }
}
