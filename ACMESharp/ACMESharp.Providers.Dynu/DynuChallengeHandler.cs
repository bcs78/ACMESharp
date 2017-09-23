using ACMESharp.ACME;
using System;
using System.Text.RegularExpressions;

namespace ACMESharp.Providers.Dynu
{
    public class DynuChallengeHandler : IChallengeHandler
    {
        public string ApiId { get; set; }
        public string ApiPassword { get; set; }
        public string DomainName { get; set; }

        public void Handle(ChallengeHandlingContext ctx)
        {
            AssertNotDisposed();
            DnsChallenge challenge = (DnsChallenge)ctx.Challenge;
            var helper = new DynuHelper(ApiId, ApiPassword, DomainName);
            helper.AddDnsRecord(challenge.RecordName, GetCleanedRecordValue(challenge.RecordValue));

            ctx.Out.WriteLine("DNS record created of type [TXT] with name [{0}]", challenge.RecordName);
        }

        public void CleanUp(ChallengeHandlingContext ctx)
        {
            AssertNotDisposed();
            DnsChallenge challenge = (DnsChallenge)ctx.Challenge;
            var helper = new DynuHelper(ApiId, ApiPassword, DomainName);
            helper.DeleteDnsRecord(challenge.RecordName);

            ctx.Out.WriteLine("DNS record deleted of type [TXT] with name [{0}]", challenge.RecordName);
        }

        private string GetCleanedRecordValue(string recordValue)
        {
            var dnsValue = recordValue;
            return dnsValue;
            // var dnsValue = Regex.Replace(recordValue, "\\s", "");
            // var dnsValues = string.Join("\" \"", Regex.Replace(dnsValue, "(.{100,100})", "$1\n").Split('\n'));
            //return dnsValues;
        }

        private void AssertNotDisposed()
        {
            if (IsDisposed)
                throw new InvalidOperationException("Dynu Challenge Handler is disposed");
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

    }
}
