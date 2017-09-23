using ACMESharp.ACME;
using ACMESharp.Providers.Dynu;
using ACMESharp.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ACMESharp.Providers
{
    [TestClass]
    public class DynuTests
    {
        public static readonly IReadOnlyDictionary<string, object> EMPTY_PARAMS =
                new Dictionary<string, object>()
        {
            ["ApiId"] = "",
            ["ApiPassword"] = "",
            ["DomainName"] = "",
        };

        private static IReadOnlyDictionary<string, object> _handlerParams = EMPTY_PARAMS;

        private static IReadOnlyDictionary<string, object> GetParams()
        {
            return _handlerParams;
        }

        [ClassInitialize]
        public static void Init(TestContext tctx)
        {
            var file = new FileInfo("Config\\DynuHandlerParams.json");
            if (file.Exists)
            {
                using (var fs = new FileStream(file.FullName, FileMode.Open))
                {
                    _handlerParams = JsonHelper.Load<Dictionary<string, object>>(fs);
                }
            }
        }

        public static DynuChallengeHandlerProvider GetProvider()
        {
            return new DynuChallengeHandlerProvider();
        }

        public static DynuChallengeHandler GetHandler(Challenge challenge)
        {
            return (DynuChallengeHandler)GetProvider().GetHandler(challenge, null);
        }

        public static DynuHelper GetHelper()
        {
            var p = GetParams();
            var h = new DynuHelper(
                    (string)p["ApiId"],
                    (string)p["ApiPassword"],
                    (string)p["DomainName"]
                );
            return h;
        }


    }
}
