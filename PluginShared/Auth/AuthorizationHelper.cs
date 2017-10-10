using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class AuthorizationHelper
{
    public static string CreateSasToken(string resourceUri, string keyName, string key)
    {
        TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + 3600); //EXPIRES in 1h 
        string stringToSign = HttpUtility.UrlEncode(resourceUri.ToLower()) + "\n" + expiry;
        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));

        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
        var sasToken = String.Format(CultureInfo.InvariantCulture,
            "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
            HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);

        return sasToken;
    }


    public static string CreateAzureStorageAuthHeader(string requestDateString, string storageAccountKey, string storageAccountName)
    {
        Uri requestUri = new Uri("https://JLServiceBusDemo.servicebus.windows.net/pdfconversionin");
        var canonicalizedStringToBuild =
            $"{requestDateString}\n/{storageAccountName}/{requestUri.AbsolutePath.TrimStart('/')}";
        string signature;
        using (var hmac = new HMACSHA256(Convert.FromBase64String(storageAccountKey)))
        {
            byte[] dataToHmac = Encoding.UTF8.GetBytes(canonicalizedStringToBuild);
            signature = Convert.ToBase64String(hmac.ComputeHash(dataToHmac));
        }

        return $"{storageAccountName}:{signature}";
        //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedKeyLite", authorizationHeader);


        // String dateInRfc1123Format = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
        // String contentMD5 = String.Empty;
        // String contentType = "application/json";
        // String canonicalizedResource = String.Format("/{0}/{1}", storageAccountName, "pdfconversionin");
        // String stringToSign = String.Format(
        //     "{0}\n{1}\n{2}\n{3}\n{4}",
        //     "POST",
        //     contentMD5,
        //     contentType,
        //     dateInRfc1123Format,
        //     canonicalizedResource);
        //return CreateAuthorizationHeader(stringToSign, storageAccountKey, storageAccountName);

    }

    //public static String CreateAuthorizationHeader(String canonicalizedString, string storageAccountKey, string storageAccountName)
    //{
    //    String signature = String.Empty;

    //    using (HMACSHA256 hmacSha256 = new HMACSHA256(Convert.FromBase64String(storageAccountKey)))
    //    {
    //        Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
    //        signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));
    //    }

    //    String authorizationHeader = String.Format(
    //        CultureInfo.InvariantCulture,
    //        "{0}:{1}",
    //        storageAccountName,
    //        signature
    //    );

    //    return authorizationHeader;
    //}
}