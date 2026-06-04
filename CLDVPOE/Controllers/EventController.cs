using Azure.Storage.Blobs;
using CLDVPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CLDVPOE.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var eventsList = _context.Events
                .Include(e => e.EventType)
                .ToList();
            return View(eventsList);
        }

        public IActionResult Create()
        {
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeID", "TypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event ev)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeID", "TypeName");
            return View(ev);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeID", "TypeName", ev.EventTypeID);
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev)
        {
            if (id != ev.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Events.Update(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeID", "TypeName", ev.EventTypeID);
            return View(ev);
        }

        public IActionResult Delete(int id) //runs when the delete button is clicked
        {
            var ev = _context.Events.Find(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)  //runs when someone confirms the deletion
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventID == id);

            if (hasBookings)
            {
                ModelState.AddModelError("", "Cannot delete this event because it has active bookings.");
                return View("Delete", ev);
            }

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}