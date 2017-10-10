using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class OauthHelper
{
    //Get an OAuth access token for O365/Azure services
    public static string GetToken(string clientId, string clientSecret, string tenantId, string resource)
    {
        var getTokenTask = Task.Run(async () =>
            await RequestToken(clientId, clientSecret, tenantId, resource));
        Task.WaitAll(getTokenTask);

        //Deserialize the token response to get the access token
        OauthTokenResponse oauthTokenResponse = Serialization.DeserializeResponse<OauthTokenResponse>(getTokenTask.Result);
        string token = oauthTokenResponse.access_token;

        return token;
    }

    private static async Task<string> RequestToken(string clientId, string clientSecret, string tenantId, string resource)
    {
        try
        {
            using (HttpClient client = HttpHelper.GetHttpClient())
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("resource", resource),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                HttpResponseMessage response = await client.PostAsync(
                    "https://login.windows.net/" + tenantId + "/oauth2/token", formContent);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidPluginExecutionException($"Error retrieving OAuth access token with response: {response.StatusCode.ToString()}");

                return response.Content.ReadAsStringAsync().Result;
            }
        }
        catch (Exception e)
        {
            throw new InvalidPluginExecutionException("Error retrieving OAuth access token", e);
        }
    }
}