using System.Runtime.Serialization;

[DataContract]
public class GetSecretResponse
{
    [DataMember]
    public string value { get; set; }
    [DataMember]
    public string id { get; set; }
    [DataMember]
    public KvAttributes attributes { get; set; }
}