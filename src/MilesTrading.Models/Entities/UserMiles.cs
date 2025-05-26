using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilesTrading.Models.Entities
{
    public class UserMiles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        [Required]
        public int MilesBalance { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navegação
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ProgramId")]
        public virtual LoyaltyProgram LoyaltyProgram { get; set; }
    }
}
