using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل عقد بين الشركة والعميل
    /// </summary>
    public class Contract
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public Guid BranchId { get; set; }

        public Guid ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // 'active', 'pending', 'expired', 'terminated'

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } // 'security', 'personal', 'event', etc.

        public int GuardsCount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        public string Notes { get; set; }

        // العلاقات
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Guard> Guards { get; set; }
        
        public virtual ICollection<Accident> Accidents { get; set; }
    }
}
