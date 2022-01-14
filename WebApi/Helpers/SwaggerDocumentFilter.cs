using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace WebApi.Helpers
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc
                .Paths
                .OrderBy(e => e.Key.Split("/")[1])
                .ThenBy(e=>e.Key.Split("/")[2])
                .ToList();
            
            swaggerDoc.Paths.Clear();
            foreach (var path in paths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }
        }
    }
}