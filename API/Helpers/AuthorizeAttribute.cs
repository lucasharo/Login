using API.Models;
using Entities.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace API.Helpers
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.Admin))
            {
                context.Result = new ForbidResult();
            }
        }
    }

    public class AuthorizeUserAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.Admin) && !user.IsInRole(Role.User))
            {
                context.Result = new ForbidResult();
            }
        }
    }

    public class AuthorizeAlterarSenhaAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.IsInRole(Role.AlterarSenha))
            {
                context.Result = new ForbidResult();
            }
        }
    }

    internal static class Result
    {
        public static JsonResult ForbiddenResult => new JsonResult(HttpStatusCode.OK)
        {
            Value = new BaseResponse<string>
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Message = "Não autorizado"
            }
        };
    }
}