namespace CLDVPOE.Models
{
    public class SearchResultsViewModel
    {
        public string? Query { get; set; }
        public List<Venue> Venues { get; set; } = new();
        public List<Event> Events { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}
