using CartService.API.Middleware;
using CartService.Repository.Implementations;
using CartService.Repository.Interfaces;
using CartService.Repository.Models;
using CartService.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Repository.Interfaces;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddGrpcClient<ProductService.ProductService.ProductServiceClient>(options =>
{
    var productServiceUrl = config["GrpcSettings:ProductServiceUrl"];
    if (string.IsNullOrEmpty(productServiceUrl))
    {
        throw new InvalidOperationException("The ProductServiceUrl configuration is missing or empty.");
    }
    options.Address = new Uri(productServiceUrl);
});
builder.Services.AddGrpcClient<UserService.UserService.UserServiceClient>(options =>
{
    var userServiceUrl = config["GrpcSettings:UserServiceUrl"];
    if (string.IsNullOrEmpty(userServiceUrl))
    {
        throw new InvalidOperationException("The UserServiceUrl configuration is missing or empty.");
    }
    options.Address = new Uri(userServiceUrl);
});

// Database context
builder.Services.AddDbContext<TechShopCartServiceDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// Dependency injection
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICartService, CartService.Service.Implementations.CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();//builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controllers
builder.Services.AddControllers();


// API behavior
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

// Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = config["JWT:Issuer"],
//            ValidAudience = config["JWT:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
//        };

//        options.Events = new JwtBearerEvents
//        {
//            OnChallenge = context =>
//            {
//                context.HandleResponse();
//                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                context.Response.ContentType = "application/json";

//                var result = new
//                {
//                    errorCode = "HB40101",
//                    message = "Token missing or invalid"
//                };

//                return context.Response.WriteAsJsonAsync(result);
//            },
//            OnForbidden = context =>
//            {
//                context.Response.StatusCode = 403;
//                context.Response.ContentType = "application/json";

//                var result = new
//                {
//                    errorCode = "HB40301",
//                    message = "Permission denied"
//                };

//                return context.Response.WriteAsJsonAsync(result);
//            }
//        };
//    });
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
// Swagger
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "G-Coffee API", Version = "v1" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter JWT with Bearer into field",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

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


var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();