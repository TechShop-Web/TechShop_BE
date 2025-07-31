using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.API.Middleware;
using OrderService.API.Services;
using OrderService.Repository.ApplicationContext;
using OrderService.Repository.Implementations;
using OrderService.Repository.Interfaces;
using OrderService.Service.Interfaces;
using OrderService.Service.Mapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsEnvironment("Docker"))
{
    builder.Configuration.AddJsonFile("appsettings.Docker.json", optional: false, reloadOnChange: true);
}

var config = builder.Configuration;

builder.Services.AddDbContext<OrderContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

builder.Services.AddGrpcClient<ProductService.ProductService.ProductServiceClient>(options =>
{
    var productServiceUrl = config["GrpcSettings:ProductServiceUrl"];
    if (string.IsNullOrEmpty(productServiceUrl))
    {
        throw new InvalidOperationException("The ProductServiceUrl configuration is missing or empty.");
    }
    options.Address = new Uri(productServiceUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Docker"))
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddGrpcClient<UserService.UserService.UserServiceClient>(options =>
{
    var userServiceUrl = config["GrpcSettings:UserServiceUrl"];
    if (string.IsNullOrEmpty(userServiceUrl))
    {
        throw new InvalidOperationException("The UserServiceUrl configuration is missing or empty.");
    }
    options.Address = new Uri(userServiceUrl);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Docker"))
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService.Service.Implementations.OrderService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

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
            message = message
        };

        return new BadRequestObjectResult(result);
    };
});

var jwtKey = config["Jwt:Key"];
var jwtIssuer = config["Jwt:Issuer"];
var jwtAudience = config["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    errorCode = "HB40101",
                    message = "Token missing or invalid"
                };

                return context.Response.WriteAsJsonAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    errorCode = "HB40301",
                    message = "Permission denied"
                };

                return context.Response.WriteAsJsonAsync(result);
            }
        };
    });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("OrderService is running"))
    .AddAsyncCheck("database", async cancellationToken =>
    {
        try
        {
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
            await dbContext.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection successful");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Database connection failed: {ex.Message}");
        }
    })
    .AddCheck("grpc-dependencies", () =>
    {
        try
        {
            var productServiceUrl = config["GrpcSettings:ProductServiceUrl"];
            var userServiceUrl = config["GrpcSettings:UserServiceUrl"];

            if (string.IsNullOrEmpty(productServiceUrl) || string.IsNullOrEmpty(userServiceUrl))
            {
                return HealthCheckResult.Unhealthy("gRPC service URLs not configured");
            }

            return HealthCheckResult.Healthy("gRPC dependencies configured");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"gRPC configuration error: {ex.Message}");
        }
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
    try
    {
        var retryCount = 0;
        while (retryCount < 10)
        {
            try
            {
                dbContext.Database.CanConnect();
                break;
            }
            catch
            {
                retryCount++;
                await Task.Delay(5000);
            }
        }
        dbContext.Database.Migrate();
        Console.WriteLine("Order database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the Order database: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<OrderGrpcService>();

app.UseMiddleware<ErrorHandlerMiddleware>();

if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            service = "OrderService",
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.Run();