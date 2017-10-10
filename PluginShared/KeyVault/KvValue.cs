using System.Runtime.Serialization;

[DataContract(Name = "Value")]
public class KvValue
{
    [DataMember]
    public string id { get; set; }
    [DataMember]
    public KvAttributes attributes { get; set; }
}