using System.Runtime.Serialization;

[DataContract(Name = "Attributes")]
public class KvAttributes
{
    [DataMember]
    public bool enabled { get; set; }
    [DataMember]
    public int created { get; set; }
    [DataMember]
    public int updated { get; set; }
}