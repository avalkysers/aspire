using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AspireSample.Extensions.Neo4j;

public class Neo4jResource(string name, ParameterResource? password = null)
: ContainerResource(name)
{
    public const string DefaultRegistry = "docker.io";
    public const string DefaultImage = "neo4j";
    public const string DefaultTag = "2025.07.1-community-bullseye";

    public const string BoltEndpointName = "bolt";
    public const string AdminEndpointName = "adminUI";

    public string Username => "neo4j";
    public ParameterResource PasswordParameter { get; }
        = password ?? new ParameterResource("Neo4jPassword", x => "test1234", true);

}