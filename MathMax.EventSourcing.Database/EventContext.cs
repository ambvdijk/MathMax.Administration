using Microsoft.EntityFrameworkCore;

namespace MathMax.EventSourcing.Entity;

public class EventContext(DbContextOptions<EventContext> options) : DbContext(options)
{
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");

            // Use UniqueId as primary key
            entity.HasKey(e => e.UniqueId);

            entity.Property(e => e.UniqueId)
                  .HasColumnName("unique_id")
                  .IsRequired();

            entity.Property(e => e.ReadableId)
                  .HasColumnName("readable_id")
                  .IsRequired();

            entity.Property(e => e.EventType)
                  .HasColumnName("event_type")
                  .IsRequired();

            entity.Property(e => e.Payload)
                  .HasColumnName("payload")
                  .HasColumnType("jsonb")  // PostgreSQL JSONB type
                  .IsRequired();

            entity.Property(e => e.AggregateId)
                  .HasColumnName("aggregate_id")
                  .IsRequired(false);

            entity.Property(e => e.Version)
                  .HasColumnName("version")
                  .IsRequired(false);

            entity.Property(e => e.Timestamp)
                  .HasColumnName("timestamp")
                  .HasColumnType("timestamptz")  // Explicitly use timestamptz
                  .IsRequired();

            // Unique constraint on aggregate_id + version
            entity.HasIndex(e => new { e.AggregateId, e.Version }).IsUnique();
        });
    }
}