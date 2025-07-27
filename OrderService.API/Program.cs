using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.API.Middleware;
using OrderService.Repository.ApplicationContext;
using OrderService.Repository.Implementations;
using OrderService.Repository.Interfaces;
using OrderService.Service.Interfaces;
using OrderService.Service.Mapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService.Service.Implementations.OrderService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

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
          ValidIssuer = config["JWT:Issuer"],
          ValidAudience = config["JWT:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
      },
      Array.Empty < string > ()
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();