using Castle.Core.Configuration;
using GrpcApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProtoBuf.Grpc.Server;
using Shared;
using System;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddCodeFirstGrpc(config =>
{
    config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
});

services.AddGrpc();

services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithHeaders("Access-Control-Allow-Origin")
                          .WithExposedHeaders("Access-Control-Allow-Origin", "Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")
                          .WithOrigins("http://localhost:1234");
                      });
});

services.AddSingleton<CounterStateStorageService>();

services.AddDbContextFactory<MyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => { x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); }));

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGrpcService<CounterService>();

var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<MyDbContext>>();
await using var dbContext = dbContextFactory.CreateDbContext();
//await dbContext.Database.MigrateAsync();
// await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

app.Run();
