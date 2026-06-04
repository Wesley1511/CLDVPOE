using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CLDVPOE.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? EventDescription { get; set; }

        public int? EventTypeID { get; set; }

        [ForeignKey("EventTypeID")]
        public EventType? EventType { get; set; }
    }
}