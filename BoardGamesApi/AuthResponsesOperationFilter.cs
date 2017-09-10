using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BoardGamesApi
{
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authAttributes = context.ApiDescription
                .ControllerAttributes()
                .Union(context.ApiDescription.ActionAttributes())
                .OfType<AuthorizeAttribute>()
                .ToArray();

            if (authAttributes.Any())
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });

            if (authAttributes.Any(x => x.Roles == "admin"))
                operation.Responses.Add("403", new Response { Description = "Forbidden" });
        }
    }
}
