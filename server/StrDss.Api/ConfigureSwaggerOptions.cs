using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StrDss.Api
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="options">The options specified by <see cref="SwaggerGenOptions"/> object</param>
        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {

            // Create Swagger documents per version and consumer
            options.SwaggerDoc(Common.ApiTags.Default, CreateInfoForApiVersion(Common.ApiTags.Default, "StrData"));
            options.SwaggerDoc(Common.ApiTags.Aps, CreateInfoForApiVersion(Common.ApiTags.Aps, $"StrData {Common.ApiTags.Aps}"));

            // Include all paths
            options.DocInclusionPredicate((name, api) => true);

            // Filter endpoints based on consumer
            options.DocumentFilter<SwaggerDocumentFilter>();

            // Take first description on any conflict
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        }

        static OpenApiInfo CreateInfoForApiVersion(string version, string title)
        {
            var info = new OpenApiInfo()
            {
                Title = title,
                Version = version
            };

            return info;
        }
    }
}
