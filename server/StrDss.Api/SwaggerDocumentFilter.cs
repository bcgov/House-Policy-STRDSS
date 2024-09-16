using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StrDss.Api
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Key is read-only so make a copy of the Paths property
            var pathsPerConsumer = new OpenApiPaths();
            var referencedSchemas = new HashSet<string>();

            if (swaggerDoc.Info.Version == Common.ApiTags.Aps)
            {
                foreach (var path in swaggerDoc.Paths)
                {
                    // If there are any tags (all methods are decorated with "SwaggerOperation(Tags = new[]...") with the current consumer name
                    var p = path.Value?.Operations?.Values?.FirstOrDefault();
                    if (p != null && p.Tags
                            .Where(t => Common.ApiTags.ApsTagList.Contains(t.Name)).Any())
                    {
                        // Add the path to the collection of paths for current consumer
                        pathsPerConsumer.Add(path.Key, path.Value);
                    }
                }


            }
            else
            {
                foreach (var path in swaggerDoc.Paths)
                {
                    if (path.Key != null && path.Value != null) pathsPerConsumer.Add(path.Key, path.Value);
                }
            }

            swaggerDoc.Paths = pathsPerConsumer;
        }
    }
}
