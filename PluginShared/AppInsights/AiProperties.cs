using System.Runtime.Serialization;

[DataContract]
public class AiProperties
{
    [DataMember(Name = "entityId")]
    public string EntityId { get; set; }
    [DataMember(Name = "entityName")]
    public string EntityName { get; set; }
    [DataMember(Name = "correlationId")]
    public string CorrelationId { get; set; }
    [DataMember(Name = "message")]
    public string Message { get; set; }
    [DataMember(Name = "stage")]
    public string Stage { get; set; }
    [DataMember(Name = "mode")]
    public string Mode { get; set; }
    [DataMember(Name = "depth")]
    public int Depth { get; set; }

    public static AiProperties GetPropertiesFromConfig(AiConfig aiconfig)
    {
        return new AiProperties
        {
            EntityId = aiconfig.EntityId.ToString(),
            EntityName = aiconfig.EntityName,
            CorrelationId = aiconfig.CorrelationId.ToString(),
            Message = aiconfig.Message,
            Stage = GetStageName(aiconfig.Stage),
            Mode = GetModeName(aiconfig.Mode),
            Depth = aiconfig.Depth
        };
    }

    private static string GetStageName(int stage)
    {
        switch (stage)
        {
            case 10:
                return "Pre-validation";
            case 20:
                return "Pre-operation";
            case 40:
                return "Post-operation";
            default:
                return "MainOperation";
        }
    }

    private static string GetModeName(int mode)
    {
        return mode == 0 ? "Synchronous" : "Asynchronous";
    }
}