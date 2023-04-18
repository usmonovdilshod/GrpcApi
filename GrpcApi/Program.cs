using GrpcApi;
using ProtoBuf.Grpc.Server;
using Shared;

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
var app = builder.Build();

app.UseRouting();
app.UseCors();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGrpcService<CounterService>();

app.Run();
