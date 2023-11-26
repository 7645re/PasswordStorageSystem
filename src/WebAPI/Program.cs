using System;
using System.Diagnostics;
using Cassandra;
using Cassandra.Mapping;
using Cassandra.Mapping.TypeConversion;
using Domain;
using Domain.Migrations;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var cassandraOptions = new CassandraOptions();
builder.Configuration.GetSection(nameof(CassandraOptions)).Bind(cassandraOptions);
builder.Services.Configure<CassandraOptions>(options =>
    builder.Configuration.GetSection(nameof(CassandraOptions)).Bind(options));
builder.Services.AddSingleton<ICassandraOptions, CassandraOptions>();

var cassandraMigrationService = new CassandraMigrationService(cassandraOptions);
using (cassandraMigrationService)
    cassandraMigrationService.ApplyMigrations();

builder.Logging.AddConsole();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IResourceCredentialRepository, ResourceCredentialRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ICredentialManagerService, CredentialManagerService>();

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