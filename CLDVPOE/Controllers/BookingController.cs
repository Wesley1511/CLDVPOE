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

        public IActionResult Index()    //returns a list of all the bookings in the database to the index view of the booking controller, this is the main view for the bookings where we can see all the bookings and also navigate to the create, edit and delete views
        {
            var bookings = _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .ToList();

            return View(bookings);
        }

        public IActionResult Create()       //this runs when we click on the create button on the index view of the booking controller and takes us to the create view where we can create a new booking, in this view we have dropdown lists for selecting the event and venue for the booking, these dropdown lists are populated with data from the database using the ViewBag and SelectList classes
        {
            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName");   //responsible for adding information to the dropdowns
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)        //this runs when we submit the form on the create view of the booking controller, it takes the data from the form and creates a new booking in the database, before saving the booking to the database it checks if the selected venue is already booked for the selected date, if it is then it adds a model error and returns the view with the error message, if the model state is valid then it saves the booking to the database and redirects to the index view of the booking controller
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

        public async Task<IActionResult> Edit(int id)   //this runs when we click on the edit button on the index view
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.Events = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)      //this runs when we submit the form on the edit view of the booking controller, it takes the data from the form and updates the existing booking in the database, before saving the changes to the database it checks if the selected venue is already booked for the selected date, excluding the current booking being edited, if it is then it adds a model error and returns the view with the error message, if the model state is valid then it saves the changes to the database and redirects to the index view of the booking controller
        {
            if (id != booking.BookingID) return NotFound();

            booking.BookingStatus = "Confirmed";
            ModelState.Remove("BookingStatus");

 
            bool venueAlreadyBooked = await _context.Bookings   //this checks for the double booking
                .AnyAsync(b => b.VenueID == booking.VenueID
                        && b.BookingDate.Date == booking.BookingDate.Date
                        && b.BookingID != booking.BookingID); 

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

        public IActionResult Delete(int id) //same as the venue and event delete methods
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
