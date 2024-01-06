using Domain.Options;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CassandraOptions>(builder.Configuration.GetRequiredSection("Cassandra"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("Jwt"));

builder.Logging.AddConsole();
builder.Services.AddMemoryCache();
builder.Services.AddValidators();
builder.Services.AddRepositories();
builder.Services.AddFactories();
builder.Services.AddServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithErrorBehavior();
builder.Services.AddSwagger();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddCORS();

var app = builder.Build();
app.UseCors("AllowOrigin");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();