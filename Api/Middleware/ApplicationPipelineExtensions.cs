using System;
using Microsoft.AspNetCore.Builder;

namespace Automobile.Api.Middleware
{
    /// <summary>
    /// Registers the application-wide middleware pipeline.
    /// Call __UseApplicationPipeline__ in Program.cs as early as possible in the pipeline.
    /// </summary>
    public static class ApplicationPipelineExtensions
    {
        public static IApplicationBuilder UseApplicationPipeline(this IApplicationBuilder app)
        {
            if (app is null) throw new ArgumentNullException(nameof(app));

            // Register global exception handling middleware first so it can catch downstream exceptions.
            app.UseGlobalExceptionHandling();

            // Additional global middleware (e.g. routing, static files, CORS) can be added here if needed.

            return app;
        }
    }
}