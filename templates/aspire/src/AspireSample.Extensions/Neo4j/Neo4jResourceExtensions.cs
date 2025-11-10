using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AspireSample.Extensions.Neo4j;

public static class Neo4jResourceExtensions
{
    public static IResourceBuilder<Neo4jResource> WithVolumeStorage(
        this IResourceBuilder<Neo4jResource> builder,
        string? name = null)
        => builder.WithVolume(name ?? "data", "/data");
} 