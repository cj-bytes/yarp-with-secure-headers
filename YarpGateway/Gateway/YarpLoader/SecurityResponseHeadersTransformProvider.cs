using Gateway.YarpExtensions;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

public class SecurityResponseHeadersTransformProvider : ITransformProvider
{
    private readonly YarpResponseTransformConfig _transformsHelper;
    private readonly ILogger<SecurityResponseHeadersTransformProvider> _logger;

    public SecurityResponseHeadersTransformProvider(IOptions<YarpResponseTransformConfig> transformsHelper, ILogger<SecurityResponseHeadersTransformProvider> logger)
    {
        _transformsHelper = transformsHelper.Value;
        _logger = logger;
    }

    public void Apply(TransformBuilderContext context)
    {
        if (context.Route.Metadata?.TryGetValue("ResponseHeaderTransformKey", out var transformKey) ?? false)
        {
            if (_transformsHelper.TryGetValue(transformKey, out var transform))
            {
                ApplyResponseTransformToRoute(context, transform);

                _logger.LogInformation("ResponseTransform '{transformKey}' applied to '{routeName}'", transformKey, context.Route.RouteId);
            }
            else
            {
                _logger.LogWarning("A metadata for ResponseHeaderTransformKey but '{transformKey}' has no registered configuration", transformKey);
            }
        }
    }

    private static void ApplyResponseTransformToRoute(TransformBuilderContext context, ResponseHeadersTransform transform)
    {
        foreach (var headerName in transform.RemoveHeaders)
        {
            context.AddResponseHeaderRemove(headerName, ResponseCondition.Always);
        }

        foreach (var header in transform.AddHeaders)
        {
            context.AddResponseHeader(header.Key, header.Value, false, ResponseCondition.Always);
        }
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }
}