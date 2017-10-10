using System.Runtime.Serialization;

[DataContract]
public class GetSecretVersionsResponse
{
    [DataMember]
    public KvValue[] value { get; set; }
    [DataMember]
    public string nextLink { get; set; }
}