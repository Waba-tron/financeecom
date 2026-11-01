using EcomBackend.Application;
using EcomBackend.Application.Common.Interfaces;
using EcomBackend.Application.Payments;
using EcomBackend.Domain.Entities;
using EcomBackend.Infrastructure;
using EcomBackend.Infrastructure.Config;
using EcomBackend.Infrastructure.Data;
using EcomBackend.Infrastructure.Middleware;
using EcomBackend.Infrastructure.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var scheme = new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste ONLY the raw JWT (no 'Bearer ' prefix — Swagger adds it).",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
    options.AddSecurityRequirement(doc => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, doc),
            new List<string>()
        }
    });
});

builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();


var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            RoleClaimType = "role",
            NameClaimType = "sub"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    var seedUsers = new (string Email, string DisplayName, string Role, string Password)[]
    {
        ("admin@com",    "Administrator", "Admin",    "Admin1234"),
       ("customer@com", "Test Customer", "Customer", "PasswordPassword123")
    };

    var existingEmails = context.Users
        .Where(u => seedUsers.Select(s => s.Email).Contains(u.Email))
        .Select(u => u.Email)
        .ToHashSet();

    var missing = seedUsers.Where(s => !existingEmails.Contains(s.Email)).ToList();
    if (missing.Count > 0)
    {
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        context.Users.AddRange(missing.Select(s => new User
        {
            Email = s.Email,
            DisplayName = s.DisplayName,
            Role = s.Role,
            PasswordHash = passwordHasher.Hash(s.Password),
            CreatedDate = DateTime.UtcNow
        }));
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DEV"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
