using CLDVPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CLDVPOE.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .ToList();

            return View(bookings);
        }

        public IActionResult Create()
        {
            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName");
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            booking.BookingStatus = "Confirmed";

            bool venueAlreadyBooked = await _context.Bookings
            .AnyAsync(b => b.VenueID == booking.VenueID
                    && b.BookingDate.Date == booking.BookingDate.Date);

            if (venueAlreadyBooked)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date.");
            }

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName");
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName");

            return View(booking);
        }

        // ✅ Edit GET
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // ✅ Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            booking.BookingStatus = "Confirmed";
            ModelState.Remove("BookingStatus");

            // Check for double booking, excluding the current booking
            bool venueAlreadyBooked = await _context.Bookings
                .AnyAsync(b => b.VenueID == booking.VenueID
                        && b.BookingDate.Date == booking.BookingDate.Date
                        && b.BookingID != booking.BookingID); // ✅ Exclude itself

            if (venueAlreadyBooked)
                ModelState.AddModelError("", "This venue is already booked for the selected date.");

            if (ModelState.IsValid)
            {
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        public IActionResult Delete(int id)
        {
            var ev = _context.Bookings.Find(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ev = await _context.Bookings.FindAsync(id);
            if (ev != null)
            {
                _context.Bookings.Remove(ev);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
