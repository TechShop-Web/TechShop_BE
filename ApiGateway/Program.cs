using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
var ocelotFileName = environment == "Docker" ? "ocelot.Docker.json" : "ocelot.json";
if (builder.Environment.IsEnvironment("Docker"))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8080);
    });
}
builder.Configuration.AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true);
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured"))),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("JWT Authentication failed: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogDebug("JWT Token validated successfully for user: {User}",
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TechShop API Gateway",
        Version = "1.0.0",
        Description = "API Gateway for TechShop Microservices using Ocelot"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token in format: Bearer {your-token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id = "Bearer",
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOcelot()
    .AddPolly();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechShop API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAllOrigins");

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    if (!context.Request.Path.StartsWithSegments("/swagger") &&
        !context.Request.Path.StartsWithSegments("/_framework"))
    {
        logger.LogInformation("{Method} {Path} from {RemoteIP}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);
    }

    await next();

    stopwatch.Stop();

    if (!context.Request.Path.StartsWithSegments("/swagger") &&
        !context.Request.Path.StartsWithSegments("/_framework"))
    {
        var statusIcon = context.Response.StatusCode < 400 ? "SUCCESSFULLY" : "FAILED";
        logger.LogInformation("{Icon} {StatusCode} in {ElapsedMs}ms",
            statusIcon, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    service = "TechShop API Gateway",
    version = "1.0.0",
    environment = environment,
    ocelotConfig = ocelotFileName,
    endpoints = new
    {
        swagger = "/swagger"
    },
    gatewayRoutes = new
    {
        userService = "/gateway/user/*",
        productService = "/gateway/product/*",
        categoryService = "/gateway/category/*",
        cartService = "/gateway/carts/*",
        orderService = "/gateway/orders/*",
        paymentService = "/gateway/payments/*"
    },
    timestamp = DateTime.UtcNow
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "TechShop API Gateway",
    environment = environment,
    ocelotConfig = ocelotFileName,
    timestamp = DateTime.UtcNow
}));

app.MapControllers();
await app.UseOcelot();


app.Run();