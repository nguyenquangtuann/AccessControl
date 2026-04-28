using AccessControl.WebApi.Infrastructure.Extentsions;

namespace AccessControl.WebApi.Infrastructure.Extentsions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
            => app.UseSwaggerUI(options =>
             {
                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "User manager API");
                 options.RoutePrefix = string.Empty;
             });

        //public static void ApplyMigrations(this IApplicationBuilder app)
        //{
        //    using var services = app.ApplicationServices.CreateScope();

        //    var dbContext = services.ServiceProvider.GetService<ProjectDbContext>();

        //    dbContext.Database.Migrate();
        //}
    }
}