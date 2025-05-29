using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل حارس أمن
    /// </summary>
    public class Guard
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string BadgeNumber { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string ProfileImageUrl { get; set; }

        [MaxLength(100)]
        public string Specialization { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid ContractId { get; set; }

        // العلاقات
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }

        public virtual ICollection<WorkSchedule> Schedule { get; set; }
        
        public virtual ICollection<LeaveDay> LeaveDays { get; set; }
        
        public virtual ICollection<Accident> Accidents { get; set; }
    }

    /// <summary>
    /// يمثل جدول عمل لحارس أمن
    /// </summary>
    public class WorkSchedule
    {
        [Key]
        public int Id { get; set; }

        public Guid GuardId { get; set; }

        [Required]
        public int DayOfWeek { get; set; } // 1 = Monday, 7 = Sunday

        [Required]
        [MaxLength(10)]
        public string StartTime { get; set; }

        [Required]
        [MaxLength(10)]
        public string EndTime { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        // العلاقات
        [ForeignKey("GuardId")]
        public virtual Guard Guard { get; set; }
    }

    /// <summary>
    /// يمثل إجازة لحارس أمن
    /// </summary>
    public class LeaveDay
    {
        [Key]
        public int Id { get; set; }

        public Guid GuardId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // 'approved', 'pending', 'rejected'

        public Guid? ReplacementId { get; set; }

        // العلاقات
        [ForeignKey("GuardId")]
        public virtual Guard Guard { get; set; }

        [ForeignKey("ReplacementId")]
        public virtual Guard ReplacementGuard { get; set; }
    }
}
