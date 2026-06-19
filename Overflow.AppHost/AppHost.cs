using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// var compose = builder.AddDockerComposeEnvironment("production")
//     .WithDashboard(dash => dash.WithHostPort(8080));
//
// var keycloak = builder.AddKeycloak("keycloak", 6001)
//     .WithDataVolume("keycloak-data")
//     .WithRealmImport("../infra/realms")
//     .WithEnvironment("KC_HTTP_ENABLED", "true")
//     .WithEnvironment("KC_HOSTNAME_STRICT", "false")
//     .WithEnvironment("KC_HOSTNAME_STRICT_HTTPS", "false")
//     // Tell Keycloak to strictly use this domain name for all issuer validations
//     .WithEnvironment("KC_HOSTNAME", "id.overflow.local") 
//     .WithEnvironment("VIRTUAL_HOST", "id.overflow.local")
//     .WithEnvironment("VIRTUAL_PORT", "8080")
//     .WithEndpoint(6001, 8080, name: "keycloak", scheme: "http", isExternal: true);

var compose = builder.AddDockerComposeEnvironment("production")
    .WithDashboard(dashboard => dashboard.WithHostPort(8080));

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithDataVolume("keycloak-data")
    .WithRealmImport("../infra/realms")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithEnvironment("VIRTUAL_HOST", "id.overflow.local")
    .WithEnvironment("VIRTUAL_PORT", "8080");

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data")
    .WithPgAdmin();

// var typesenseApiKey = builder.AddParameter("typesense-api-key", secret: true);

var typesenseApiKey = builder.Environment.IsDevelopment()
    ? builder.Configuration["Parameters:typesense-api-key"]
      ?? throw new InvalidOperationException("Missing parameters:typesense-api-key")
    : "${TYPESENSE_API_KEY}";

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithVolume("typesense-data", "/data")
    .WithEnvironment("TYPESENSE_DATA_DIR", "/data")
    .WithEnvironment("TYPESENSE_ENABLE_CORS", "true")
    .WithEnvironment("TYPESENSE_API_KEY", typesenseApiKey)
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typeSenseContainer = typesense.GetEndpoint("typesense");

var questionDb = postgres.AddDatabase("questionDb");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port: 15672);

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WithReference(rabbitmq)
    .WaitFor(keycloak)
    .WaitFor(questionDb)
    .WaitFor(rabbitmq);

var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithEnvironment("typesense-api-key", typesenseApiKey)
    .WithReference(typeSenseContainer)
    .WithReference(rabbitmq)
    .WaitFor(typesense)
    .WaitFor(rabbitmq);

var yarp = builder.AddYarp("gateway")
    .WithConfiguration(yarpBuilder =>
    {
        yarpBuilder.AddRoute("/questions/{**catch-all}", questionService);
        yarpBuilder.AddRoute("/tags/{**catch-all}", questionService);
        yarpBuilder.AddRoute("/search/{**catch-all}", searchService);
    })
    // 1. Override default "http" endpoint parameters
    .WithEndpoint("http", endpoint =>
    {
        endpoint.Port = 8001;       // Host Port
        endpoint.TargetPort = 8001; // Container Port
    })
    // 2. Override default "https" endpoint parameters (Prevents random HTTPS ports)
    .WithEndpoint("https", endpoint =>
    {
        endpoint.Port = 8002;       // Host Port
        endpoint.TargetPort = 8002; // Container Port
    })
    // 3. Keep your custom ASPNETCORE_URLS matching the targetPort
    .WithEnvironment("ASPNETCORE_URLS", "http://*:8001")
    .WithEnvironment("VIRTUAL_HOST", "api.overflow.local")
    .WithEnvironment("VIRTUAL_PORT", "8001");


var webapp = builder.AddJavaScriptApp("webapp", "../webapp")
    .WithReference(keycloak)
    .WithReference(yarp)
    .WithHttpEndpoint(env: "PORT", port: 3000);


if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddContainer("nginx-proxy", "nginxproxy/nginx-proxy", "1.11")
        .WithEndpoint(80, 80, name: "nginx", scheme: "http", isExternal: true)
        .WithBindMount("/var/run/docker.sock", "/tmp/docker.sock", true);
}

builder.Build().Run();