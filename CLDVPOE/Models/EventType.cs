using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CLDVPOE.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        public string TypeName { get; set; } = string.Empty;
    }
}