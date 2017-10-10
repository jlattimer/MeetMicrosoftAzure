using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginShared.Tests
{
    [TestClass]
    public class KeyVaultTests
    {
        [TestMethod]
        public void Get_static_secret_value()
        {
            KvSecureConfig kvSecureConfig = new KvSecureConfig
            {
                ApiVersion = "2016-10-01",
                ClientId = "{Your Value}",
                ClientSecret = "{Your Value}",
                TenantId = "{Your Value}",
                Resource = "https://vault.azure.net",
                VaultUrl = "https://{Your Value}.vault.azure.net",
                SecretId = "{Your Value}",
                SecretName = "{Your Value}"
            };

            var result = KeyVault.GetSecretByUrl(kvSecureConfig);

            Assert.AreEqual(result, "HelloWorld");
        }

        [TestMethod]
        public void Get_versioned_secret_value()
        {
            KvSecureConfig kvSecureConfig = new KvSecureConfig
            {
                ApiVersion = "2016-10-01",
                ClientId = "{Your Value}",
                ClientSecret = "{Your Value}",
                TenantId = "{Your Value}",
                Resource = "https://vault.azure.net",
                VaultUrl = "https://{Your Value}.vault.azure.net",
                SecretId = "",
                SecretName = "{Your Value}"
            };

            var result = KeyVault.GetSecretByName(kvSecureConfig);

            Assert.AreEqual(result, "Version2");
        }
    }
}