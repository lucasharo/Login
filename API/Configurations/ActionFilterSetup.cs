using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Configurations
{
    public static class ActionFilterSetup
    {
        public static IServiceCollection AddActionFilterSetup(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ActionFilter));
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return services;
        }
    }

    public class ActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            StringBuilder mensagens = new StringBuilder();

            if (!context.ModelState.IsValid)
            {
                int indice = 0;

                context.ModelState.Keys
                       .Select(key =>
                       {
                           context.ModelState[key].Errors.Select(x => x).ToList().ForEach(ModelError =>
                           {
                               indice++;

                               var field_obj = Regex.Match(ModelError.ErrorMessage, @"'((\w+\.\w+)|(\w+))(?!\w+)'").ToString().Replace("'", "");

                               if (field_obj.Length > 0)
                               {
                                   var cast_error = Regex.Match(ModelError.ErrorMessage, @"(\bstring\b)\s(\bto\b)\s\w+:").ToString().Replace(":", "");

                                   if (cast_error.Length > 0)
                                   {
                                       string[] cast_type = cast_error.Split(" ");

                                       mensagens.Append("Erro na conversão do campo: " + key + ", de: " + cast_type[0] + " para: " + cast_type[2] + ".");
                                   }
                                   else
                                   {
                                       mensagens.Append("(Inconsistência no Json) O campo: " + key + ", não pode ser vazio.");
                                   }
                               }
                               else
                               {
                                   var field_obj_invalidJson = Regex.Match(ModelError.ErrorMessage, @"(.*\bparsing property name\W Expected\s':'\s\b.*)").ToString().Replace("'", "");

                                   if (field_obj_invalidJson.Length > 0)
                                   {
                                       mensagens.Append("Json Inconsistente!");
                                   }
                                   else
                                   {
                                       mensagens.Append(ModelError.ErrorMessage + "; ");
                                   }
                               }
                           });

                           return key;
                       }).ToList();

                var result = new BaseResponse<string>
                {
                    Message = mensagens.ToString(),
                    StatusCode = (int)HttpStatusCode.BadRequest
                };

                context.Result = new OkObjectResult(result);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}