using ACMESharp.ACME;
using ACMESharp.Ext;
using System.Collections.Generic;
using System.Linq;

namespace ACMESharp.Providers.Dynu
{
    [ChallengeHandlerProvider(
        "Dynu",
        ChallengeTypeKind.DNS,
        isCleanUpSupported: true,
        Label = "DynuDDNS",
        Description = "Provider for handling challenges that manages DNS entries hosted in a DynuDDNS zone")]
    public class DynuChallengeHandlerProvider : IChallengeHandlerProvider
    {
        public static readonly ParameterDetail DomainName =
            new ParameterDetail(nameof(DynuChallengeHandler.DomainName), ParameterType.TEXT, isRequired: true,
                label: "Domain name", desc: "The domain name to operate against.");

        public static readonly ParameterDetail ApiId =
            new ParameterDetail(nameof(DynuChallengeHandler.ApiId), ParameterType.TEXT, isRequired: true,
                label: "API ID", desc: "The registered API ID.");

        public static readonly ParameterDetail ApiPassword =
            new ParameterDetail(nameof(DynuChallengeHandler.ApiPassword), ParameterType.TEXT, isRequired: true,
                label: "API Password", desc: "The password of the registered API ID.");

        private static readonly ParameterDetail[] Params =
        {
            DomainName,
            ApiId,
            ApiPassword
        };

        public IEnumerable<ParameterDetail> DescribeParameters()
        {
            return Params;
        }

        public bool IsSupported(Challenge c)
        {
            return c is DnsChallenge;
        }

        public IChallengeHandler GetHandler(Challenge c, IReadOnlyDictionary<string, object> initParams)
        {
            var handler = new DynuChallengeHandler();
            if (initParams == null)
            {
                initParams = new Dictionary<string, object>();
            }
            ValidateParameters(initParams);
            handler.DomainName = (string)initParams[DomainName.Name];
            handler.ApiId = (string)initParams[ApiId.Name];
            handler.ApiPassword = (string)initParams[ApiPassword.Name];
            return handler;
        }

        private void ValidateParameters(IReadOnlyDictionary<string, object> parameters)
        {
            foreach (ParameterDetail detail in Params.Where(x => x.IsRequired))
            {
                if (!parameters.ContainsKey(detail.Name))
                {
                    throw new KeyNotFoundException($"Missing required parameter [{detail.Name}]");
                }
            }
        }
    }
}
