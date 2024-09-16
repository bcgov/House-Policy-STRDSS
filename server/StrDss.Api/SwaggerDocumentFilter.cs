using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StrDss.Api
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var filteredPaths = new OpenApiPaths();
            var referencedSchemas = new HashSet<string>();

            if (swaggerDoc.Info.Version == Common.ApiTags.Aps)
            {
                foreach (var path in swaggerDoc.Paths)
                {
                    var operation = path.Value?.Operations?.Values?.FirstOrDefault();

                    if (operation != null && operation.Tags.Any(t => Common.ApiTags.ApsTagList.Contains(t.Name)))
                    {
                        filteredPaths.Add(path.Key, path.Value);
                        TrackSchemasInOperations(operation, referencedSchemas);
                    }
                }
            }
            else
            {
                foreach (var path in swaggerDoc.Paths)
                {
                    if (path.Key != null && path.Value != null)
                    {
                        filteredPaths.Add(path.Key, path.Value);

                        foreach (var operation in path.Value.Operations.Values)
                        {
                            TrackSchemasInOperations(operation, referencedSchemas);
                        }
                    }
                }
            }

            swaggerDoc.Paths = filteredPaths;

            // Filter schemas in components to only include those that are referenced
            var filteredSchemas = new Dictionary<string, OpenApiSchema>();
            foreach (var schemaKey in swaggerDoc.Components.Schemas.Keys)
            {
                if (referencedSchemas.Contains(schemaKey))
                {
                    filteredSchemas.Add(schemaKey, swaggerDoc.Components.Schemas[schemaKey]);
                }
            }

            // Update the document's components with filtered schemas
            swaggerDoc.Components.Schemas = filteredSchemas;
        }

        /// <summary>
        /// Tracks the schemas used in the operation's parameters, request bodies, and responses.
        /// </summary>
        private void TrackSchemasInOperations(OpenApiOperation operation, HashSet<string> referencedSchemas)
        {
            // Track schemas from parameters
            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Schema?.Reference != null)
                {
                    referencedSchemas.Add(parameter.Schema.Reference.Id);
                }
            }

            // Track schemas from request bodies
            if (operation.RequestBody?.Content != null)
            {
                foreach (var content in operation.RequestBody.Content.Values)
                {
                    if (content.Schema?.Reference != null)
                    {
                        referencedSchemas.Add(content.Schema.Reference.Id);
                    }
                }
            }

            // Track schemas from responses
            foreach (var response in operation.Responses.Values)
            {
                foreach (var content in response.Content.Values)
                {
                    if (content.Schema?.Reference != null)
                    {
                        referencedSchemas.Add(content.Schema.Reference.Id);
                    }
                }
            }
        }


    }
}
