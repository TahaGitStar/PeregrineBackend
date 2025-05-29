using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل فرع من فروع الشركة
    /// </summary>
    public class Branch
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        public string ManagerName { get; set; }

        [MaxLength(100)]
        public string ManagerEmail { get; set; }

        [MaxLength(20)]
        public string ManagerPhone { get; set; }

        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<Contract> Contracts { get; set; }
    }
}
