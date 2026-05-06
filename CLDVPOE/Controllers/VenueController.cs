using Microsoft.AspNetCore.Mvc;
using CLDVPOE.Models;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace CLDVPOE.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenueController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View(_context.Venues.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var connectionString = _configuration.GetConnectionString("AzureStorage");

                    var blobServiceClient = new BlobServiceClient(connectionString);

                    var containerClient = blobServiceClient.GetBlobContainerClient("venue-images");
                    await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }

                    venue.ImagePath = blobClient.Uri.ToString();
                }

                venue.IsAvailable = true;

                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = _context.Venues.Find(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {

                if (imageFile != null && imageFile.Length > 0)
                {
                    var connectionString = _configuration.GetConnectionString("AzureStorage");
                    var blobServiceClient = new BlobServiceClient(connectionString);
                    var containerClient = blobServiceClient.GetBlobContainerClient("venue-images");
                    await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }

                    venue.ImagePath = blobClient.Uri.ToString();
                }
                else
                {
                    var existing = await _context.Venues.FindAsync(id);
                    venue.ImagePath = existing?.ImagePath;
                    _context.Entry(existing).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }

                _context.Venues.Update(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(venue);
        }

        public IActionResult Delete(int id)
        {
            var venue = _context.Venues.Find(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.VenueID == id);

            if (hasBookings)
            {
                ModelState.AddModelError("", "Cannot delete this venue because it has active bookings.");
                return View("Delete", venue);
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
