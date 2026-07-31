using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService,TokenService>(); //or AddTransient or AddSingleton
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(option =>
{
    var tokenKey=builder.Configuration["TokenKey"]?? throw new Exception("Token key not found -Program.cs");
option.TokenValidationParameters=new TokenValidationParameters
{
    
    ValidateIssuerSigningKey=true,
    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
    ValidateIssuer=false,
    ValidateAudience=false
};
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
