using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CLDVPOE.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? EventDescription { get; set; }
    }
}