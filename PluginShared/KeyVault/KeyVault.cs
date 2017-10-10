using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class KeyVault
{
    //Get the Secret value from the Key Vault by url - api-version is required
    public static string GetSecretByUrl(KvSecureConfig kvSecureConfig)
    {
        //Retrieve a secret by its url
        var getKeyByUrlTask = Task.Run(async () => await GetSecretByUrlRequest(kvSecureConfig));
        Task.WaitAll(getKeyByUrlTask);

        //Deserialize the vault response to get the secret
        GetSecretResponse getSecretResponse1 =
            Serialization.DeserializeResponse<GetSecretResponse>(getKeyByUrlTask.Result);

        //returnedValue is the Azure Key Vault Secret
        string returnedValue = getSecretResponse1.value;

        return returnedValue;
    }

    //Get the most recent, enabled Secret value by name  - api-version is required
    public static string GetSecretByName(KvSecureConfig kvSecureConfig)
    {
        var getKeyByNameTask = Task.Run(async () => await GetSecretByNameRequest(kvSecureConfig));
        Task.WaitAll(getKeyByNameTask);

        var retrievedSecretUrl = getKeyByNameTask.Result;

        //The full secret url is returned, just need the id now
        kvSecureConfig.SecretId = retrievedSecretUrl.Substring(retrievedSecretUrl.Length - 32);

        // Retrieve a secret by its url
        var getKeyByUrlTask = Task.Run(async () => await GetSecretByUrlRequest(kvSecureConfig));
        Task.WaitAll(getKeyByUrlTask);

        //Deserialize the vault response to get the secret
        GetSecretResponse getSecretResponse =
            Serialization.DeserializeResponse<GetSecretResponse>(getKeyByUrlTask.Result);

        //returnedValue is the Azure Key Vault Secret
        string returnedValue = getSecretResponse.value;

        return returnedValue;
    }

    //Retrieves a single secret based on its unique url
    private static async Task<string> GetSecretByUrlRequest(KvSecureConfig kvSecureConfig)
    {
        try
        {
            string token = OauthHelper.GetToken(kvSecureConfig.ClientId, kvSecureConfig.ClientSecret,
                kvSecureConfig.TenantId, kvSecureConfig.Resource);

            using (HttpClient client = HttpHelper.GetHttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, kvSecureConfig.SecretUri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidPluginExecutionException($"Error retrieving secret value from key vault with response: {response.StatusCode.ToString()}");

                return response.Content.ReadAsStringAsync().Result;
            }
        }
        catch (Exception e)
        {
            throw new InvalidPluginExecutionException("Error retrieving secret value from key vault ", e);
        }
    }

    //Retrieves a collection of secrets based on its value & name
    public static async Task<string> GetSecretByNameRequest(KvSecureConfig kvSecureConfig)
    {
        try
        {
            string token = OauthHelper.GetToken(kvSecureConfig.ClientId, kvSecureConfig.ClientSecret,
                kvSecureConfig.TenantId, kvSecureConfig.Resource);

            Uri nextLink = kvSecureConfig.SecretVersionsUri;
            List<KvValue> values = new List<KvValue>();

            using (HttpClient client = HttpHelper.GetHttpClient())
            {
                while (nextLink != null)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, nextLink);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        throw new InvalidPluginExecutionException($"Failed retrieving secret versions from key vault with response: {response.StatusCode.ToString()}");

                    var versions =
                        Serialization.DeserializeResponse<GetSecretVersionsResponse>(response.Content
                            .ReadAsStringAsync().Result);
                    values.AddRange(versions.value);

                    nextLink = string.IsNullOrEmpty(versions.nextLink) ?
                        null :
                        new Uri(versions.nextLink);
                }
            }

            KvValue mostRecentValue =
                values.Where(a => a.attributes.enabled)
                    .OrderByDescending(a => TimeHelper.UnixTimeToUtc(a.attributes.created))
                    .FirstOrDefault();

            return mostRecentValue?.id;
        }
        catch (Exception e)
        {
            throw new InvalidPluginExecutionException("Error retrieving secret versions from key vault ", e);
        }
    }
}