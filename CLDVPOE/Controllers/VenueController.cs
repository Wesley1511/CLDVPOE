using Microsoft.AspNetCore.Mvc;
using CLDVPOE.Models;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

/*--------------------------
 The code on lines 36 to 53 was generated with the assistance of Claude 
 This is recycled from lines 87 to 109
 ---------------------------*/

namespace CLDVPOE.Controllers
{
    public class VenueController : Controller       //controller for the venue view
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenueController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()    //simply returns a list of all the venues in the database to the index view of the venue controller, this is the main view for the venues where we can see all the venues and also navigate to the create, edit and delete views
        {
            return View(_context.Venues.ToList());
        }

        public IActionResult Create()   //this runs when we click on the create button on the index view of the venue controller and takes us to the create view where we can create a new venue and also upload an image for the venue
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)   //this runs when we click on the save button on the create view of the venue controller
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

        public IActionResult Edit(int id)       //this brings us to the edit view where we can edit the details of the venue and also upload a new image if needed
        {
            var venue = _context.Venues.Find(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost]  //this is the post method for the edit view where we update the details of the venue and also upload a new image if needed, runs when we click save
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

        public IActionResult Delete(int id) //this runs when we hit delete on the venue and takes us the deletion confirmation screen
        {
            var venue = _context.Venues.Find(id);
            if (venue == null) return NotFound();
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)    //this runs when we confirm the deletion of the venue, it checks if there are any active bookings for the venue and if there are it does not allow the deletion and shows an error message, if there are no active bookings it deletes the venue
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
