using ACMESharp.Providers.Dynu.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ACMESharp.Providers.Dynu
{
    /// <summary>
    /// Helper class to interface with DynuDDNS API.
    /// </summary>
    /// <remarks>
    /// See <see cref="https://www.dynu.com/en-US/Resources/API"/>
    /// for more details.
    /// </remarks>
    public class DynuHelper
    {
        private readonly string _apiId;
        private readonly string _apiPassword;
        private readonly string _domainName;
        private string _authToken;
        private const string BaseUrl = "https://api.dynu.com/v1/";
        private const string RequestTokenUrl = BaseUrl + "oauth2/token";
        private const string ListRecordsUrl = BaseUrl + "dns/records/<domain>";
        private const string CreateRecordUrl = BaseUrl + "dns/record/add";
        private const string DeleteRecordUrl = BaseUrl + "dns/record/delete/<id>";

        public DynuHelper(string apiId, string apiPassword, string domainName)
        {
            _apiId = apiId;
            _apiPassword = apiPassword;
            _domainName = domainName;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("Authorization", _authToken);
            request.Headers.Add("Content-Type", "application/json");
            return request;
        }

        public void GetAccessToken()
        {
            HttpClient client = new HttpClient();
            var encodedApiCredetials = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_apiId + ":" + _apiPassword));
            var request = new HttpRequestMessage(HttpMethod.Post, RequestTokenUrl);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept-Language", "en_US");
            request.Headers.Add("Authorization", encodedApiCredetials);

            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });
//            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });

            request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var result = client.SendAsync(request).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                TokenResult accessToken = JsonConvert.DeserializeObject<TokenResult>(content);
                _authToken = accessToken.AccessToken.ToString();
            }
            else
            {
                throw new Exception("Could not get Authorization Token. {result.StatusCode}");
            }
        }

        public void DeleteDnsRecord(string name)
        {
            HttpClient client = new HttpClient();
            GetAccessToken();
        }

        public void AddDnsRecord(string name, string value)
        {
            HttpClient client = new HttpClient();
            GetAccessToken();
        }

        private List<DnsRecord> GetDnsRecords(string zoneId)
        {
            GetAccessToken();
            List<DnsRecord> records = new List<DnsRecord>();
            bool finishedPaginating = false;
            int page = 1;
            HttpClient client = new HttpClient();
            while (!finishedPaginating)
            {
                var request = CreateRequest(HttpMethod.Get, $"{string.Format(ListRecordsUrl, zoneId)}?page={page}");
                var result = client.SendAsync(request).GetAwaiter().GetResult();
                if (result.IsSuccessStatusCode)
                {
                    var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var dnsResult = JsonConvert.DeserializeObject<DnsResult>(content);
                    records.AddRange(dnsResult.Result);
                    if (dnsResult.ResultInfo.Page == dnsResult.ResultInfo.TotalPages)
                    {
                        finishedPaginating = true;
                    }
                    else
                    {
                        page = page + 1;
                    }
                }
                else
                {
                    throw new Exception($"Could not get DNS records for zone {zoneId}. Result: {result.StatusCode} - {result.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                }
            }
            return records;
        }


    }
}
