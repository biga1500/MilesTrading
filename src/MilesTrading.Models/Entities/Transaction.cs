using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilesTrading.Models.Entities
{
    public enum TransactionStatus
    {
        PENDING,
        COMPLETED,
        CANCELLED
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OfferId { get; set; }

        [Required]
        public int BuyerId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalValue { get; set; }

        [Required]
        public TransactionStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navegação
        [ForeignKey("OfferId")]
        public virtual Offer Offer { get; set; }

        [ForeignKey("BuyerId")]
        public virtual User Buyer { get; set; }

        [ForeignKey("SellerId")]
        public virtual User Seller { get; set; }
    }
}
