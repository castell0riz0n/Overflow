#pragma warning disable ASPIRECERTIFICATES001
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data")
    .WithPgAdmin();

var typesenseApiKey = builder.AddParameter("typesense-api-key", secret: true);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "30.2")
    .WithArgs("--data-dir", "/data", "--api-key", typesenseApiKey, "--enable-cors", "true")
    .WithVolume("typesense-data", "/data")
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typesenseContainer = typesense.GetEndpoint("typesense");

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithoutHttpsCertificate()
    .WithDataVolume("keycloak-data");

var questionDb = postgres.AddDatabase("questionDb");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(port: 15672);

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(questionDb)
    .WithReference(keycloak)
    .WithReference(rabbitmq)
    .WaitFor(questionDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithEnvironment("typesense-api-key", typesenseApiKey)
    .WithReference(typesenseContainer)
    .WithReference(rabbitmq)
    .WaitFor(typesense)
    .WaitFor(rabbitmq);
    
builder.Build().Run();