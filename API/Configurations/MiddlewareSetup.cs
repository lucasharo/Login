using API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace API.Configurations
{
    public static class MiddlewareSetup
    {
        public class CustomMiddleware
        {
            private readonly RequestDelegate _next;

            public CustomMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext httpContext)
            {
                await _next(httpContext);

                if (httpContext.Response.StatusCode != (int)HttpStatusCode.OK)
                {
                    var result = new BaseResponse<string>
                    {
                        StatusCode = httpContext.Response.StatusCode
                    };

                    if (httpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    {
                        result.Message = "Não autorizado";
                    }
                    else if (httpContext.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                    {
                        result.Message = "Não possui permissão";
                    }

                    httpContext.Response.StatusCode = (int)HttpStatusCode.OK;

                    httpContext.Response.ContentType = "application/json";

                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Formatting = Formatting.Indented
                    }));
                }
            }
        }

        public static IApplicationBuilder UseMiddlewareSetup(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }
}
