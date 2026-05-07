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

        public async Task<IActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return View(new SearchResultsViewModel());

            var query = q.ToLower();

            var venues = await _context.Venues
                .Where(v => v.VenueName.ToLower().Contains(query) ||
                            v.Location.ToLower().Contains(query))
                .ToListAsync();

            var events = await _context.Events
                .Where(e => e.EventName.ToLower().Contains(query) ||
                            (e.EventDescription != null && e.EventDescription.ToLower().Contains(query)))
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .Where(b => b.BookingStatus.ToLower().Contains(query) ||
                            (b.Venue != null && b.Venue.VenueName.ToLower().Contains(query)) ||
                            (b.Event != null && b.Event.EventName.ToLower().Contains(query)))
                .ToListAsync();

            var results = new SearchResultsViewModel
            {
                Query = q,
                Venues = venues,
                Events = events,
                Bookings = bookings
            };

            return View(results);
        }
    }
}