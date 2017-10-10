using System;
using System.Runtime.Serialization;

[DataContract]
public class KvSecureConfig
{
    [DataMember]
    public string ClientId { get; set; }
    [DataMember]
    public string ClientSecret { get; set; }
    [DataMember]
    public string TenantId { get; set; }
    [DataMember]
    public string Resource { get; set; }
    [DataMember]
    public string ApiVersion { get; set; }
    [DataMember]
    public string VaultUrl { get; set; }
    [DataMember]
    public string SecretName { get; set; }
    [DataMember]
    public string SecretId { get; set; }

    public Uri SecretUri => new Uri($"{VaultUrl}/secrets/{SecretName}/{SecretId}?api-version={ApiVersion}");

    public Uri SecretVersionsUri => new Uri($"{VaultUrl}/secrets/{SecretName}/versions?api-version={ApiVersion}");
}