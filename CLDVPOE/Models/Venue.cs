using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CLDVPOE.Models

{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public int Capacity { get; set; }

        public string? ImagePath { get; set; }

        public bool IsAvailable { get; set; }
    }
}
