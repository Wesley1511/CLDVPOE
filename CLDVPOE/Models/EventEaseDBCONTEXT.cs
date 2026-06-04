using Microsoft.EntityFrameworkCore;

namespace CLDVPOE.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = 1, TypeName = "Conference" },
                new EventType { EventTypeID = 2, TypeName = "Wedding" },
                new EventType { EventTypeID = 3, TypeName = "Concert" },
                new EventType { EventTypeID = 4, TypeName = "Sports" },
                new EventType { EventTypeID = 5, TypeName = "Corporate" },
                new EventType { EventTypeID = 6, TypeName = "Private Party" }
            );
        }
    }
}