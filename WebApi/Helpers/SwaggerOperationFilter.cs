using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace WebApi.Helpers
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var paths = context.ApiDescription.RelativePath.Split('/');
            if (paths.Length > 2)
            {
                var microService = paths[1];
                var controller = paths[2];

                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag
                    {
                        Name = $"MicroService: {microService} / Section: {controller}"
                    }
                };
            }
        }
    }
}
