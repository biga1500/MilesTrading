using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilesTrading.Models.Entities
{
    public class LoyaltyProgram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navegação
        public virtual ICollection<UserMiles> UserMiles { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
