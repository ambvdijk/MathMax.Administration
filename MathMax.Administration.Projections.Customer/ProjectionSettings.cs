namespace MathMax.Administration.Projections.Customer;

public class ProjectionSettings
{
    public string NatsUrl { get; set; } = null!;
    public string StreamName { get; set; } = null!;
    public string ConsumerName { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string PostgresConnectionString { get; set; } = null!;
}
