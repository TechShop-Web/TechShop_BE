using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PaymentService.Repository.Interfaces;
using PaymentService.Repository.Models;
using PaymentService.Repository.Repositories;
using PaymentService.Service;
using PaymentService.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PaymentService API",
        Version = "v1"
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<VnPayModel>(builder.Configuration.GetSection("VNPay"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService.Service.Services.PaymentService>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

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

builder.Services.AddGrpcClient<OrderService.Grpc.OrderService.OrderServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:OrderServiceUrl"]);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("PaymentService is running"))
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=sqlserver;Database=TechShopPaymentDB;User Id=sa;Password=12345;TrustServerCertificate=True;",
        name: "database",
        timeout: TimeSpan.FromSeconds(30));

var app = builder.Build();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Starting Payment database migration...");
        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                Console.WriteLine("Payment database connection successful.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Payment database connection attempt {retryCount}/10 failed: {ex.Message}");
                await Task.Delay(5000);
            }
        }

        if (retryCount < 10)
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Payment database migration completed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to connect to Payment database after 10 attempts.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the Payment database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Health Check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            service = "PaymentService",
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
app.UseCors("AllowAllOrigins");

Console.WriteLine("PaymentService started successfully on Docker environment");
Console.WriteLine($"Swagger available at: http://localhost:8080/swagger");
Console.WriteLine($"Health check available at: http://localhost:8080/health");

app.Run();