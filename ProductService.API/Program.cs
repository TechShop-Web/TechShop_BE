using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductService.API.Services.Grpc;
using ProductService.Repository.Frameworks;
using ProductService.Repository.Models;
using ProductService.Repository.Repositories;
using ProductService.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ProductService.Repository")));

// Register Repository + UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register Service
builder.Services.AddScoped<IProductService, ProductService.Service.Services.ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();

builder.Services.AddGrpc();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("ProductService is running"))
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=sqlserver;Database=TechShopProductDB;User Id=sa;Password=12345;TrustServerCertificate=True;",
        name: "database",
        timeout: TimeSpan.FromSeconds(30));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ProductService API",
        Version = "v1"
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Starting Product database migration...");
        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                Console.WriteLine("Product database connection successful.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Product database connection attempt {retryCount}/10 failed: {ex.Message}");
                await Task.Delay(5000);
            }
        }

        if (retryCount < 10)
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Product database migration completed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to connect to Product database after 10 attempts.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the Product database: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<ProductGrpcService>();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            service = "ProductService",
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                exception = x.Value.Exception?.Message,
                duration = x.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapControllers();

app.Run();