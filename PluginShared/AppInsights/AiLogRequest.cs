using System.Runtime.Serialization;

[DataContract]
public class AiLogRequest
{
    [DataMember(Name = "name")]
    public string Name { get; set; }
    [DataMember(Name = "time")]
    public string Time { get; set; }
    [DataMember(Name = "iKey")]
    public string InstrumentationKey { get; set; }
    [DataMember(Name = "tags")]
    public AiTags Tags { get; set; }
    [DataMember(Name = "data")]
    public AiData Data { get; set; }
}