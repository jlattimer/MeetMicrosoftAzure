using System;

public class AiConfig
{
    public string InstrumentationKey { get; set; }
    public string AiEndpoint { get; set; }
    public Guid CorrelationId { get; set; }
    public string EntityName { get; set; }
    public Guid EntityId { get; set; }
    public string Message { get; set; }
    public int Stage { get; set; }
    public int Mode { get; set; }
    public int Depth { get; set; }
    public Guid InitiatingUserId { get; set; }
    public Guid UserId { get; set; }
    public bool LogTraces { get; set; }
    public bool LogMetrics { get; set; }
    public bool LogEvents { get; set; }
    public bool LogExceptions { get; set; }
}