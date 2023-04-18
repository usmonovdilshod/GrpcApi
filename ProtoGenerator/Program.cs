using ProtoBuf.Meta;
using ProtoBuf.Grpc.Reflection;
using System.Reflection;

var generator = new SchemaGenerator { ProtoSyntax = ProtoSyntax.Proto3 };

var assemblyPath = @"C:\Users\User\Desktop\GrpcApi\Shared\bin\Debug\net7.0\Shared.dll";
var sharedAssembly = Assembly.LoadFrom(assemblyPath);

var schemas = sharedAssembly
    .GetTypes()
    .Where(t => t.IsInterface && t.Name.StartsWith("I"))
    .ToDictionary(
        iface => iface.Name.Replace("I", ""),
        iface => generator.GetSchema(iface)
    );

var outputPath = Path.Combine("..", "..", "..", "Protos");

if (!Directory.Exists(outputPath))
{
    Directory.CreateDirectory(outputPath);
}

foreach (var schema in schemas)
{
    var fileName = $"{schema.Key}.proto";
    var filePath = Path.Combine(outputPath, fileName);
    await File.WriteAllTextAsync(filePath, schema.Value);
}