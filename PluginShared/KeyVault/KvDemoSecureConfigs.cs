using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class KvDemoSecureConfigs
{
    [DataMember]
    public List<KvSecureConfig> KvSecureConfigs;
}