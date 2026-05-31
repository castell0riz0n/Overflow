using Typesense;

namespace SearchService.Data;

public static class SearchInitializer
{
    public static async Task EnsureIndexExists(ITypesenseClient client)
    {
        const string schemaName = "questions";
        bool exists = false;

        try
        {
            await client.RetrieveCollection(schemaName);
            Console.WriteLine($"Collection {schemaName} already exists");
            exists = true;
        }
        catch (TypesenseApiException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            // The official client sometimes wraps the 404 in a generic API exception
            Console.WriteLine($"Collection {schemaName} does not exist yet");
        }
        catch (Exception ex)
        {
            // Fallback for any other exception types thrown by the library
            Console.WriteLine($"Collection {schemaName} look-up failed or not found: {ex.Message}");
        }

        // Exit early if it already exists
        if (exists) return;
        
        var schema = new Schema(schemaName, new List<Field>
        {
            new("id", FieldType.String),
            new("title", FieldType.String),
            new("content", FieldType.String),
            new("tags", FieldType.StringArray),
            new("createdAt", FieldType.Int64),
            new("hasAcceptedAnswer", FieldType.Bool),
            new("answerCount", FieldType.Int32)
        })
        {
            DefaultSortingField = "createdAt"
        };

        await client.CreateCollection(schema);
        Console.WriteLine($"Collection {schemaName} created");
    }
}