using Newtonsoft.Json;

namespace ACMESharp.Providers.Dynu.Results
{
    internal class TokenResult
    {
//        public TokenResult[] Result { get; set; }
        [JsonProperty("access_token")]
        public int AccessToken { get; set; }
    }
}