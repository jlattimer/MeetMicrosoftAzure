using Microsoft.Xrm.Sdk;

namespace KeyVaultPlugin
{
    public class RetrieveAzureSecret : PluginBase
    {
        #region Secure/Unsecure Configuration

        private readonly string _secureConfig;
        private string _unsecureConfig;

        public RetrieveAzureSecret(string unsecureConfig, string secureConfig) : base(typeof(RetrieveAzureSecret))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }

        #endregion

        [Time]
        protected override void ExecutePlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new InvalidPluginExecutionException("localContext");

            KvDemoSecureConfigs kvDemoSecureConfigs = Serialization.DeserializeResponse<KvDemoSecureConfigs>(_secureConfig);
            localContext.Trace($"Configuration Count: {kvDemoSecureConfigs.KvSecureConfigs.Count}");

            Entity target = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];

            //string clientId = "00000000-0000-0000-0000-000000000000";
            //string clientSecret = "00000000000000000000000000000000000000000000";
            //string tenantId = "00000000-0000-0000-0000-000000000000";
            //string resource = "https://vault.azure.net";
            //string apiVersion = "2016-10-01";
            //string secretUrl = "https://myvaulttest.vault.azure.net/secrets/MyPassword/00000000000000000000000000000000";
            //string vaultName = "https://myvaulttest.vault.azure.net";
            //string secretName = "MyPassword";

            string value1 = KeyVault.GetSecretByUrl(kvDemoSecureConfigs.KvSecureConfigs[0]);
            localContext.Trace($"SecretByUrl: {value1}");
            target["lat_secretbyurl"] = value1;



            string value2 = KeyVault.GetSecretByName(kvDemoSecureConfigs.KvSecureConfigs[1]);
            localContext.Trace($"SecretByName: {value2}");
            target["lat_secretbyname"] = value2;

        }
    }
}