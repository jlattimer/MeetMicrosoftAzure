using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginShared.Tests
{
    [TestClass]
    public class OauthHelperTests
    {
        private string _clientId = "{Your Value}";
        private string _clientSecret = "{Your Value}";
        private string _tenantId = "{Your Value}";
        private string _resource = "https://vault.azure.net";

        [TestMethod]
        public void Get_token()
        {
            var result = OauthHelper.GetToken(_clientId, _clientSecret, _tenantId, _resource);

            Assert.IsNotNull(result);
        }
    }
}