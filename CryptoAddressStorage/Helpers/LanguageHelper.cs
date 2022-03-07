using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Helpers
{
    public static class LanguageHelper
    {
        public static string LANG_KEY = "language";
        public static string RAW_ROUTE_KEY = "rawRoute";

        public static void SetLanguageAndRawRoute(HttpRequest req, ITempDataDictionary tempData, ISiteRepository repo)
        {
            RouteValueDictionary routeDict = req.RouteValues;

            string language = routeDict.GetValueOrDefault(LANG_KEY).ToString();
            string rawRoute = String.Format("{0}/{1}{2}", routeDict["controller"], routeDict["action"], (routeDict["id"] != null ? "/" + routeDict["id"] : String.Empty));

            tempData[LANG_KEY] = language;
            tempData[RAW_ROUTE_KEY] = rawRoute;

            repo.SetSessionLanguage(language);
            repo.SetRawRoute(rawRoute);
        }
    }
}
