using System.Text;
using API.Configuration;
using API.Data;
using API.Entities;
using API.Interfaces;
using API.Middleware;
using API.Services;
using API.SinglR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddDbContext<InstitutionDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IEquivalencyApplicationService, EquivalencyApplicationService>();

builder.Services.AddScoped<IApplicantRepository,ApplicantRepository>();

builder.Services.AddSignalR();

// Identity
builder.Services
    .AddIdentityCore<AppUser>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        var tokenKey =
            builder.Configuration["TokenKey"]
            ?? throw new Exception(
                "Token key not found - Program.cs"
            );

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tokenKey)
                    ),

                ValidateIssuer = false,

                ValidateAudience = false
            };

        // Allow SignalR to receive JWT from query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken =
                context.Request.Query["access_token"];

                var path =
                context.HttpContext.Request.Path;

                if (
                    !string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs")
                )
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services
    .AddAuthorizationBuilder()

    .AddPolicy(
        "RequierAdminRole",
        policy =>
            policy.RequireRole("ADMIN")
    )

    .AddPolicy(
        "ManagePhotoRole",
        policy =>
            policy.RequireRole(
                "ADMIN",
                "MANAGER"
            )
    );

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mail settings
builder.Services.Configure<MailSettings>(
    builder.Configuration
        .GetSection(nameof(MailSettings))
);

builder.Services.AddTransient<
    IMailService,
    MailService
>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Exception middleware
app.UseMiddleware<ExceptionMiddleware>();

// HTTPS redirection disabled حاليا
// app.UseHttpsRedirection();

// CORS
app.UseCors("AllowReactApp");

// Serve files from wwwroot
// مثل الصور الموجودة داخل:
// wwwroot/uploads/profiles
app.UseStaticFiles();

// Authentication لازم يكون قبل Authorization
app.UseAuthentication();

// Debug middleware:
// يطبع roles الموجودة داخل JWT
app.Use(async (context, next) =>
{
    var roles =
        context.User.Claims
            .Where(
                c =>
                    c.Type ==
                    System.Security.Claims
                        .ClaimTypes.Role
            )
            .Select(c => c.Value)
            .ToList();

    Console.WriteLine(
        "Roles: " +
        string.Join(", ", roles)
    );

    await next();
});

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

//SingleR
app.MapHub<PersenceHub>("/hubs/presence");

// Apply migrations + seed database
using var scope =
    app.Services.CreateScope();

var services =
    scope.ServiceProvider;

try
{
    var context =
        services
            .GetRequiredService<AppDbContext>();

    var userManager =
        services
            .GetRequiredService<
                UserManager<AppUser>
            >();

    await context.Database.MigrateAsync();

    await Seed.SeedUsers(userManager);
}
catch (Exception ex)
{
    var logger =
        services
            .GetRequiredService<
                ILogger<Program>
            >();

    logger.LogError(
        ex,
        "An error occurred during migration"
    );
}

app.Run();