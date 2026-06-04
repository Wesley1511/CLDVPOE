using Microsoft.AspNetCore.Mvc;
using CLDVPOE.Models;
using Microsoft.EntityFrameworkCore;

namespace CLDVPOE.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string q, int? eventTypeID, DateTime? startDate, DateTime? endDate, bool? venueAvailable)
        {
            var eventTypes = await _context.EventTypes.ToListAsync();

            if (string.IsNullOrWhiteSpace(q) && eventTypeID == null && startDate == null && endDate == null && venueAvailable == null)
            {
                return View(new SearchResultsViewModel { EventTypes = eventTypes });
            }

            var query = q?.ToLower() ?? string.Empty;
            int.TryParse(q, out int searchId);

            var venuesQuery = _context.Venues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                venuesQuery = venuesQuery.Where(v => v.VenueName.ToLower().Contains(query) ||
                                                     v.Location.ToLower().Contains(query));

            if (venueAvailable.HasValue)
                venuesQuery = venuesQuery.Where(v => v.IsAvailable == venueAvailable.Value);

            var venues = await venuesQuery.ToListAsync();

            var eventsQuery = _context.Events
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                eventsQuery = eventsQuery.Where(e => e.EventName.ToLower().Contains(query) ||
                                                     (e.EventDescription != null && e.EventDescription.ToLower().Contains(query)));

            if (eventTypeID.HasValue)
                eventsQuery = eventsQuery.Where(e => e.EventTypeID == eventTypeID.Value);

            if (startDate.HasValue)
                eventsQuery = eventsQuery.Where(e => e.StartDate >= startDate.Value);

            if (endDate.HasValue)
                eventsQuery = eventsQuery.Where(e => e.EndDate <= endDate.Value);

            var events = await eventsQuery.ToListAsync();

            var bookingsQuery = _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                bookingsQuery = bookingsQuery.Where(b =>
                    b.BookingStatus.ToLower().Contains(query) ||
                    (b.Venue != null && b.Venue.VenueName.ToLower().Contains(query)) ||
                    (b.Event != null && b.Event.EventName.ToLower().Contains(query)) ||
                    (searchId > 0 && b.BookingID == searchId));

            if (startDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate.Value);

            if (endDate.HasValue)
                bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate.Value);

            var bookings = await bookingsQuery.ToListAsync();

            var results = new SearchResultsViewModel
            {
                Query = q,
                Venues = venues,
                Events = events,
                Bookings = bookings,
                EventTypes = eventTypes,
                SelectedEventTypeID = eventTypeID,
                StartDate = startDate,
                EndDate = endDate,
                VenueAvailable = venueAvailable
            };

            return View(results);
        }
    }
}