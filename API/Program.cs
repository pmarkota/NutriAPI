using System.Text;
using API.BLL.Services;
using API.DAL;
using API.DAL.Models;
using API.DAL.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add CORS configuration

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173") // Add your frontend URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials() // Add this line
                .SetIsOriginAllowed(origin => true) // Be careful with this in production
                .WithExposedHeaders("Content-Disposition") // Add if needed
                .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));
        }
    );
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Register your DbContext using the connection string from appsettings.json

builder.Services.AddDbContext<API.DAL.Models.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.Scan(scan =>
    scan.FromAssemblyOf<IRepository>()
        .AddClasses(classes => classes.AssignableTo<IRepository>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services.Scan(scan =>
    scan.FromAssemblyOf<IService>()
        .AddClasses(classes => classes.AssignableTo<IService>())
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services.AddSwaggerGen();

// Add JWT Authentication - Simplified version
builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])
            ),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
