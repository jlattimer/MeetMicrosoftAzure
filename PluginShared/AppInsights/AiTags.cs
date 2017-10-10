using System.Runtime.Serialization;

[DataContract]
public class AiTags
{
    [DataMember(Name = "ai.operation.name")]
    public string OperationName { get; set; }
    [DataMember(Name = "ai.cloud.roleInstance")]
    public string RoleInstance { get; set; }
}