using CryptoAddressStorage.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CryptoAddressStorage.Services
{
    public class GlobalAction : IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;

            ISiteRepository _repository = (ISiteRepository)filterContext.HttpContext.RequestServices.GetService(typeof(ISiteRepository));

            LanguageHelper.SetLanguageAndRawRoute(controller.Request, controller.TempData, _repository);
        }
    }
}
