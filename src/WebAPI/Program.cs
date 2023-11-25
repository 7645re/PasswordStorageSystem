using Cassandra.Mapping;
using Domain;
using Domain.Migrations;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;

var builder = WebApplication.CreateBuilder(args);

var cassandraOptions = new CassandraOptions();
builder.Configuration.GetSection(nameof(CassandraOptions)).Bind(cassandraOptions);
builder.Services.Configure<CassandraOptions>(options =>
    builder.Configuration.GetSection(nameof(CassandraOptions)).Bind(options));
builder.Services.AddSingleton<ICassandraOptions, CassandraOptions>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordStorageRepository, PasswordStorageRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPasswordManagerService, PasswordManagerService>();

MappingConfiguration.Global.Define(new UserMapping());
MappingConfiguration.Global.Define(new PasswordStorageMapping());

var cassandraMigrationService = new CassandraMigrationService(cassandraOptions);
using (cassandraMigrationService)
    cassandraMigrationService.ApplyMigrations();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();