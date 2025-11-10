using AspireSample.Extensions.Neo4j;

var builder = DistributedApplication.CreateBuilder(args);

var neo4jPassword = builder.AddParameter("Neo4jPassword");

var neo4j = builder.AddNeo4j("neo4j", neo4jPassword.Resource)
    .WithVolumeStorage("data")
    .WithLifetime(ContainerLifetime.Persistent);

neo4jPassword.WithParentRelationship(neo4j);

//var redis = builder.AddContainer("redis", "redis:6")
//    .WithContainerName("redis")
//    .WithHttpEndpoint(6379, 6379);

//var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice", "https")
//    .WithEndpointsInEnvironment(x => x.UriScheme == "https")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.AspireSample_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();

