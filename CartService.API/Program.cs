using CartService.API.Middleware;
using CartService.Repository.Implementations;
using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using CartService.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddGrpcClient<ProductService.ProductService.ProductServiceClient>(options =>
{
    options.Address = new Uri(config["GrpcSettings:ProductServiceUrl"]);
    Console.WriteLine("GRPC ProductServiceUrl: " + config["GrpcSettings:ProductServiceUrl"]);

})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true
    };
});


builder.Services.AddGrpcClient<UserService.UserService.UserServiceClient>(options =>
{
    options.Address = new Uri(config["GrpcSettings:UserServiceUrl"]);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true
    };
});

builder.Services.AddDbContext<TechShopCartServiceDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// Dependency injection
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICartService, CartService.Service.Implementations.CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var message = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "Invalid input";

        var result = new
        {
            errorCode = "HB40001",
            message
        };

        return new BadRequestObjectResult(result);
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured")))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var roles = context.Principal?.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);
                Console.WriteLine($"Validated token with roles: {string.Join(", ", roles ?? new string[] { "None" })}");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("CartService is running"))
    .AddSqlServer(
        config.GetConnectionString("DefaultConnection") ?? "Server=sqlserver;Database=TechShop_CartServiceDB;User Id=sa;Password=12345;TrustServerCertificate=True;",
        name: "database",
        timeout: TimeSpan.FromSeconds(30));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CartService API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
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
    var dbContext = scope.ServiceProvider.GetRequiredService<TechShopCartServiceDbContext>();
    try
    {
        Console.WriteLine("Starting Cart database migration...");
        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                Console.WriteLine("Cart database connection successful.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"Cart database connection attempt {retryCount}/10 failed: {ex.Message}");
                await Task.Delay(5000);
            }
        }

        if (retryCount < 10)
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Cart database migration completed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to connect to Cart database after 10 attempts.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the Cart database: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            service = "CartService",
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