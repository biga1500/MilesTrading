using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilesTrading.Models.Entities
{
    public enum OfferType
    {
        BUY,
        SELL
    }

    public enum OfferStatus
    {
        ACTIVE,
        COMPLETED,
        CANCELLED
    }

    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        [Required]
        public OfferType Type { get; set; }

        [Required]
        public int MilesAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PricePerThousand { get; set; }

        [Required]
        public OfferStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navegação
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ProgramId")]
        public virtual LoyaltyProgram LoyaltyProgram { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
