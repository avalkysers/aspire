using System.Reflection;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

var builder = DistributedApplication.CreateBuilder(args);

var neo4j = builder.AddContainer("neo4j", "neo4j")
    .WithEnvironment("NEO4J_AUTH", "neo4j/test1234")
    .WithEndpoint(targetPort: 7687, port: 7687, name: "bolt", scheme: "neo4j")
    .WithEndpoint(7474, 7474, name: "adminUI", scheme: "http")
    .WithHttpHealthCheck("/", endpointName: "adminUI")
    .WithUrlForEndpoint("adminUI", x =>
    {
        x.DisplayLocation = UrlDisplayLocation.SummaryAndDetails;
        x.DisplayText = "Admin UI";
    })
    .WithUrlForEndpoint("bolt", x =>
    {
        x.DisplayLocation = UrlDisplayLocation.DetailsOnly;
        x.DisplayText = "Bolt Endpoint";
    })
    .WithVolume("data", "/data");

var redis = builder.AddContainer("redis", "redis:6")
    .WithContainerName("redis")
    .WithHttpEndpoint(6379, 6379);

var apiService = builder.AddProject<Projects.AspireSample_ApiService>("apiservice", "https")
    .WithEndpointsInEnvironment(x => x.UriScheme == "https")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

