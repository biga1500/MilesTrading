using MilesTrading.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace MilesTrading.Data
{
    public class MilesTradingContext : DbContext
    {
        public MilesTradingContext(DbContextOptions<MilesTradingContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LoyaltyProgram> LoyaltyPrograms { get; set; }
        public DbSet<UserMiles> UserMiles { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas para User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configurações específicas para UserMiles
            modelBuilder.Entity<UserMiles>()
                .HasIndex(um => new { um.UserId, um.ProgramId })
                .IsUnique();

            // Configurações específicas para Offer
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.User)
                .WithMany(u => u.Offers)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.LoyaltyProgram)
                .WithMany(lp => lp.Offers)
                .HasForeignKey(o => o.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurações específicas para Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Offer)
                .WithMany(o => o.Transactions)
                .HasForeignKey(t => t.OfferId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.BuyerTransactions)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.SellerTransactions)
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrição de quantidade mínima de milhas
            modelBuilder.Entity<Offer>()
                .HasCheckConstraint("CK_Offer_MinMilesAmount", "MilesAmount >= 10000");

            // Seed de dados para programas de fidelidade
            modelBuilder.Entity<LoyaltyProgram>().HasData(
                new LoyaltyProgram { Id = 1, Name = "Latam Pass", Description = "Programa de fidelidade da companhia aérea Latam", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new LoyaltyProgram { Id = 2, Name = "Smiles", Description = "Programa de fidelidade da companhia aérea Gol", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new LoyaltyProgram { Id = 3, Name = "Azul Fidelidade", Description = "Programa de fidelidade da companhia aérea Azul", IsActive = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            );
        }
    }
}
