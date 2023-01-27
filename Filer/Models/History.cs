using System;
using System.ComponentModel.DataAnnotations;

namespace Filer.Models
{
    public class History
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string Path { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}