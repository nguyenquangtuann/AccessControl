using Autofac.Extensions.DependencyInjection;
using Autofac;
using AccessControl.Data.Repositories;
using AccessControl.Service;
using AccessControl.WebApi.Infrastructure.Extentsions;
using OfficeOpenXml.Style;
using AccessControl.WebApi.Services;
using AccessControl.WebApi.Common.Loggings;
using AccessControl.Data.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new ContainerModule());
    builder.RegisterInstance(AutoMapperConfig.Initialize()).SingleInstance();

    builder.RegisterAssemblyTypes(typeof(AppGroupRepository).Assembly)
          .Where(t => t.Name.EndsWith("Repository"))
           .As(serviceMapping: x => x.GetInterfaces().FirstOrDefault(t => t.Name.EndsWith("Repository")));
    builder.RegisterAssemblyTypes(typeof(AppGroupService).Assembly)
       .Where(t => t.Name.EndsWith("Service"))
       .As(x => x.GetInterfaces().FirstOrDefault(t => t.Name.EndsWith("Service")));
}).ConfigureServices(services =>
{
    services.AddAutofac();
});

RoundTheCodeFileLoggerOption loggerOption = new RoundTheCodeFileLoggerOption();
builder.Services.Configure<RoundTheCodeFileLoggerOption>(builder.Configuration.GetSection("Logging").GetSection("RoundTheCodeFile").GetSection("Options"));
builder.Services.AddSingleton<ILoggerProvider, RoundTheCodeFileLoggerProvider>();

// add worker services
builder.Services.AddHostedService<Worker>();
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddIdentity();
builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
        c.RoutePrefix = "swagger";
    });
}
app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader())
                .UseAuthentication();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseStaticFiles();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
    c.RoutePrefix = "swagger";
});
app.UseStaticFiles();
app.MapControllers();
app.UseStaticFiles();
app.Run();
