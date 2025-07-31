using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using UserService.API.Services.Grpc;
using UserService.Repository.Models;
using UserService.Repository.Repositories;
using UserService.Service.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
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

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("Default"))
           .ConfigureWarnings(warnings =>
               warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService.Service.Services.UserService>();

builder.Services.AddGrpc();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("UserService is running"))
    .AddSqlServer(
        config.GetConnectionString("Default") ?? "Server=sqlserver;Database=TechShop_UserDb;User Id=sa;Password=12345;TrustServerCertificate=True;",
        name: "database",
        timeout: TimeSpan.FromSeconds(30));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User-Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        Console.WriteLine("Starting User database migration...");
        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                await dbContext.Database.CanConnectAsync();
                Console.WriteLine("User database connection successful.");
                break;
            }
            catch (Exception ex)
            {
                retryCount++;
                Console.WriteLine($"User database connection attempt {retryCount}/10 failed: {ex.Message}");
                await Task.Delay(5000);
            }
        }

        if (retryCount < 10)
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("User database migration completed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to connect to User database after 10 attempts.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the User database: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
            service = "UserService",
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

app.MapGrpcService<UserGrpcService>();
app.MapControllers();


app.Run();