using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PermissionManager.API.BackgroundServices;
using PermissionManager.Application;
using PermissionManager.Application.Interfaces;
using PermissionManager.Application.Mappings;
using PermissionManager.Core.Utils;
using PermissionManager.Domain.Interfaces;
using PermissionManager.Infrastructure.Elasticsearch;
using PermissionManager.Infrastructure.Kafka;
using PermissionManager.Persistence.Contexts;
using PermissionManager.Persistence.Data;
using PermissionManager.Persistence.Repositories;
using PermissionManager.Persistence.UnitOfWork;


/* ****************************************
 * SERVICES CONFIGURATION
 *****************************************/
var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories and Unit Of Work
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// MediatR, Mapper y Validation
builder.Services.AddMediatR(opt => opt.RegisterServicesFromAssembly(typeof(PermissionApplication).Assembly));
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<PermissionApplication>();
ValidatorOptions.Global.DisplayNameResolver = (_, memberInfo, _) => memberInfo == null ? null : memberInfo.Name.ToSnakeCase();

// Kafka y Elasticsearch
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<ElasticsearchOptions>(builder.Configuration.GetSection("Elasticsearch"));
builder.Services.AddSingleton<IProducerService, KafkaProducerService>();
builder.Services.AddSingleton<IFullTextSearchService, ElasticsearchFullTextSearchService>();

// Add a hosted service to test Kafka consumers easily
builder.Services.AddHostedService<KafkaConsumer>();

// Controllers, CORS and Swagger
// Allow any origin (development purposes)
builder.Services.AddProblemDetails();
builder.Services.AddCors(opt => opt.AddDefaultPolicy(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddControllers(opt =>
{
    opt.SuppressAsyncSuffixInActionNames = false;
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo{Title = "PermissionManager", Version = "v1"});
    opt.EnableAnnotations();
});

/* ****************************************
 * APPLICATION PIPELINE CONFIGURATION
 *****************************************/
var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.EnsureCreated();
        ApplicationDbSeeder.SeedAsync(db).Wait();
    }
    catch
    {
        // ignore
    }
}

if (app.Environment.IsDevelopment())
{
    // app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error");

}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

// For IntegrationTests to see this class
namespace PermissionManager.API
{
    public partial class Program { }
}