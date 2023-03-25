using Gateway.YarpExtensions;
using System.Text.Json;

public static class WebApplicationBuilderYarpExtensions
{
    /// <summary>
    /// Register yarp configurations
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="path">Path where yarp related json configurations are stored</param>
    public static void AutoConfigReverseProxy(this WebApplicationBuilder builder, string path)
    {
        var configuration = builder.Configuration;

        builder.Services.Configure<YarpResponseTransformConfig>(
             builder.Configuration.GetSection(YarpResponseTransformConfig.ConfigurationKey));

        var proxyConfigNames = BuildAndLoadProxyConfigurations(builder, path);

        var yarp = builder.Services
            .AddReverseProxy();

        foreach (var item in proxyConfigNames)
        {
            yarp.LoadFromConfig(configuration.GetSection(item));
        }

        yarp.AddTransforms<SecurityResponseHeadersTransformProvider>();
    }

    private static List<string> BuildAndLoadProxyConfigurations(WebApplicationBuilder builder, string path)
    {
        var files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        var proxyConfigNames = new List<string>();
        foreach (var file in files)
        {
            var jsonText = File.ReadAllText(file);
            var deserializedJson = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(jsonText);
            proxyConfigNames.Add(deserializedJson.Keys.FirstOrDefault());

            builder.Configuration.AddJsonFile(file);
        }
        builder.Configuration.AddEnvironmentVariables();
        return proxyConfigNames;
    }
}