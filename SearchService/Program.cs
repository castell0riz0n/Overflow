using System.Text.RegularExpressions;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SearchService.Data;
using SearchService.Models;
using Typesense;
using Typesense.Setup;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry().WithTracing(tracing =>
{
    tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName))
        .AddSource("Wolverine");
});
builder.Host.UseWolverine(options =>
{
    options.UseRabbitMqUsingNamedConnection("messaging")
        .AutoProvision();
    options.ListenToRabbitQueue("questions.search", cfg =>
    {
        cfg.BindExchange("Questions");
    });
});

var typesenseURI = builder.Configuration["services:typesense:typesense:0"];
if (string.IsNullOrEmpty(typesenseURI))
{
    throw new InvalidOperationException("Typesense URI is not set");   
}

var typesenseApiKey = builder.Configuration["typesense-api-key"];

if (string.IsNullOrEmpty(typesenseApiKey))
    throw new InvalidOperationException("Typesense API key is not set");

var uri = new Uri(typesenseURI);
builder.Services.AddTypesenseClient(cfg =>
{
    cfg.ApiKey = typesenseApiKey;
    cfg.Nodes = new List<Node>
    {
        new(uri.Host, uri.Port.ToString(), uri.Scheme)
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapGet("/search", async (string query, TypesenseClient client) =>
{
    string? tag = null;
    var tagMatch = Regex.Match(query, @"\[(.*?)\]");
    if (tagMatch.Success)
    {
        tag = tagMatch.Groups[1].Value;
        query = query.Replace(tagMatch.Value, "").Trim();
    }
    
    var searchParams = new SearchParameters(query, "title,content");
    if (!string.IsNullOrWhiteSpace(tag))
    {
        searchParams.FilterBy = $"tags:[{tag}]";
    }

    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);
        
        return Results.Ok(result.Hits.Select(a => a.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Typesense search failed", e.Message);
    }
    
});

app.MapGet("/search/similar-titles", async (string query, ITypesenseClient client) =>
{
    var searchParams = new SearchParameters(query, "title");
    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);

        return Results.Ok(result.Hits.Select(a => a.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Typesense search failed", e.Message);
    }
});

var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchInitializer.EnsureIndexExists(client);


app.Run();
