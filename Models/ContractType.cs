using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل نوع عقد أمني
    /// </summary>
    public class ContractType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string ArabicName { get; set; }

        [MaxLength(50)]
        public string IconName { get; set; }
    }
}
