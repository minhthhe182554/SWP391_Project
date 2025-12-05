using Microsoft.AspNetCore.Mvc.Filters;
using SWP391_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace SWP391_Project.Helpers
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly Role[] _roles;

        public RoleAuthorizeAttribute(params Role[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleString = context.HttpContext.Session.GetString("Role");

            if(string.IsNullOrEmpty(roleString))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if(Enum.TryParse(roleString, out Role userRole) )
            {
                if(_roles.Length > 0 && !_roles.Contains(userRole))
                {
                    var controller = context.Controller as Controller;
                    if(controller != null)
                    {
                        controller.TempData["Error"] = "Ban khong co quyen truy cap trang nay";
                    }
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            } else
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
