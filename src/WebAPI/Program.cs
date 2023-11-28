using System.Text;
using Domain.Middlewares;
using Domain.Options;
using Domain.Repositories;
using Domain.Services;
using Domain.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CassandraOptions>(builder.Configuration.GetRequiredSection("Cassandra"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("Jwt"));


builder.Logging.AddConsole();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICredentialValidator, CredentialValidator>();
builder.Services.AddSingleton<IUserValidator, UserValidator>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICredentialHistoryRepository, CredentialHistoryRepository>();
builder.Services.AddSingleton<ICredentialRepository, CredentialRepository>();
builder.Services.AddSingleton<ITokenBlackListRepository, TokenBlackListRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICredentialService, CredentialService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowOrigin");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<TokenBlackListMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();