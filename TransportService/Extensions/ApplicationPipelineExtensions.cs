using TransportService.Middlewares;

namespace TransportService.Extensions
{
    public static class ApplicationPipelineExtensions
    {
        public static IApplicationBuilder UseApplicationPipeline(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport Service API v1");
                });
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseMiddleware<JwtValidationMiddleware>();  // our custom validator
            app.UseAuthorization();

            return app;
        }
    }
}
