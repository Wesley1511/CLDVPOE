using System.ComponentModel.DataAnnotations;

namespace CLDVPOE.Models

{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        public string VenueName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; } = true;

    }
}
