using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilesTrading.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal FinancialBalance { get; set; }

        public int TrustLevel { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navegação
        public virtual ICollection<UserMiles> UserMiles { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Transaction> BuyerTransactions { get; set; }
        public virtual ICollection<Transaction> SellerTransactions { get; set; }
    }
}
