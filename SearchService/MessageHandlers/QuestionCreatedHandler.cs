using System.Text.RegularExpressions;
using Contracts;
using SearchService.Models;
using Typesense;

namespace SearchService.MessageHandlers;

public class QuestionCreatedHandler(ITypesenseClient client)
{
    public async Task Handle(QuestionCreated message)
    {
        var created = new DateTimeOffset(message.Created).ToUnixTimeSeconds();

        var doc = new SearchQuestion
        {
            Id = message.QuestionId,
            Title = message.Title,
            Content = StripHtml(message.Content),
            Tags = message.Tags.ToArray(),
            CreatedAt = created
        };

        await client.CreateDocument("questions", doc);

        Console.WriteLine($"Created Question with id {message.QuestionId}");
    }

    private static string StripHtml(string content)
    {
        return Regex.Replace(content, "<.*?>", string.Empty);
    }
}