using Domain.Factories;
using Domain.Options;
using Domain.Repositories.UserRepository;
using Domain.Services.TokenService;
using Domain.Services.UserService;
using Domain.Validators.UserValidator;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CassandraOptions>(builder.Configuration.GetRequiredSection("Cassandra"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("Jwt"));

builder.Logging.AddConsole();
builder.Services.AddSingleton<ICassandraSessionFactory, CassandraSessionFactory>();
builder.Services.AddSingleton<IUserValidator, UserValidator>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithErrorBehavior();
builder.Services.AddSwagger();
builder.Services.AddJwt(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("www-authenticate");
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();