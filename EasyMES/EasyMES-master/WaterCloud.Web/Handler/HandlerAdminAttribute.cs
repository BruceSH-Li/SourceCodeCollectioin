using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WaterCloud.Code;

namespace WaterCloud.Web
{
	public class HandlerAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (OperatorProvider.Provider.GetCurrent() != null && OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                return;
            }
            else
            {
                //filterContext.HttpContext.Response.WriteAsync("<script>top.location.href ='" + filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=403" + "';if(document.all) window.event.returnValue = false;</script>");
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.PathBase + "/Home/Error?msg=403");
                return;
            }
        }
    }
}