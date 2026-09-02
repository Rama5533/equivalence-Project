using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Middleware;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using API.SinglR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors(Options =>
    {
        Options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });

builder.Services.AddScoped<ITokenService, TokenService>(); //or AddTransient or AddSingleton
// builder.Services.AddScoped<IUnitOfWork,UnitOfWorks>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();// حاليا هاد نفس الي فوق

// builder.Services.AddScoped<LogUserActivity>();
// builder.Services.AddSignalR();

builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(Options =>
{
    var tokenKey = builder.Configuration["TokenKey"]
        ?? throw new Exception("Token key not found -Program.cs");
    Options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    // Options.Events = new JwtBearerEvents
    // {
    //     OnMessageReceived = context =>
    //     {
    //         var accessToken=context.Request.Query["access_token"];

    //         var path=context.HttpContext.Request.Path;
    //         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
    //         {
    //             context.Token=accessToken;
    //         }
    //         return Task.CompletedTask;
    //     }


    // };

});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequierAdminRole", policy => policy.RequireRole("ADMIN"))
    .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("ADMIN", "MODERATOR"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection(nameof(MailSettings)));

builder.Services.AddTransient<IMailService, MailService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();


app.UseHttpsRedirection();//اذا اجاها طلبhttp بتحوله لhttps


app.UseCors("AllowReactApp");

//هاي لتشبيك الفرونت اند بالباك اند 
// app.UseCors(x =>
//     x.AllowAnyHeader()
//     .AllowAnyMethod()
//     .AllowCredentials()
//     .WithOrigins("http://localhost:4200", "http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// app.MapHub<PersenceHub>("hubs/presence");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager);

}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");

}

app.Run();
