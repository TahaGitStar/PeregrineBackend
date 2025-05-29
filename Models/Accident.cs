using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل حادث أمني أو تقرير حادث
    /// </summary>
    public class Accident
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        public string MediaUrls { get; set; } // JSON string

        public Guid GuardId { get; set; }

        public Guid ContractId { get; set; }

        // العلاقات
        [ForeignKey("GuardId")]
        public virtual Guard Guard { get; set; }

        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }

    /// <summary>
    /// يمثل تعليق على حادث أمني
    /// </summary>
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public Guid AccidentId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public bool IsAdminComment { get; set; }

        // العلاقات
        [ForeignKey("AccidentId")]
        public virtual Accident Accident { get; set; }
    }
}
