﻿using Newtonsoft.Json;

namespace ACMESharp.Providers.Dynu.Results
{
    internal class DnsResult
    {
        public DnsRecord[] Result { get; set; }
        [JsonProperty("result_info")]
        public ResultInfo ResultInfo { get; set; }
    }
}