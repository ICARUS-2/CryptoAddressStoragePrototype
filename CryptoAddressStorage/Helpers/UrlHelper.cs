using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Helpers
{
    public static class UrlHelper
    {
        public static string Generate(string lang, string controller, string action)
        {
            return String.Format("/{0}/{1}/{2}", lang, controller, action);
        }

        public static string Generate(string lang, string controller, string action, string id)
        {
            return String.Format("/{0}/{1}/{2}/{3}", lang, controller, action, id);
        }
        
        public static string Generate(string lang, string rawRoute)
        {
            return String.Format("/{0}/{1}", lang, rawRoute);
        }
    }
}
