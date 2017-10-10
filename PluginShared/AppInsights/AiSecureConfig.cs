using System.Runtime.Serialization;

[DataContract]
public class AiSecureConfig
{
    [DataMember]
    public string InstrumentationKey { get; set; }
    [DataMember]
    public string AiEndpoint { get; set; }
    [DataMember]
    public bool LogTraces { get; set; }
    [DataMember]
    public bool LogMetrics { get; set; }
    [DataMember]
    public bool LogEvents { get; set; }
    [DataMember]
    public bool LogExceptions { get; set; }
}