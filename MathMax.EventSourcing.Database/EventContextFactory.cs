using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MathMax.EventSourcing.Entity;

public class EventContextFactory : IDesignTimeDbContextFactory<EventContext>
{
    public EventContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EventContext>();
        
        // Use a dummy connection string for design-time (migrations don't need a real database)
        optionsBuilder.UseNpgsql("Host=localhost;Database=design_time_dummy;Username=dummy;Password=dummy");
        
        return new EventContext(optionsBuilder.Options);
    }
}
