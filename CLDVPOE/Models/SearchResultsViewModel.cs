namespace CLDVPOE.Models
{
    public class SearchResultsViewModel
    {
        public string? Query { get; set; }
        public List<Venue> Venues { get; set; } = new();
        public List<Event> Events { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();

        // filter stuff
        public List<EventType> EventTypes { get; set; } = new();
        public int? SelectedEventTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? VenueAvailable { get; set; }
    }
}