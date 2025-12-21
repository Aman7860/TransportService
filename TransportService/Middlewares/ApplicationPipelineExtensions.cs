namespace TransportService.Middlewares
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

            return app;
        }
    }
}
