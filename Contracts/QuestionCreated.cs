namespace Contracts;

public record QuestionCreated(string QuestionId, string Title, string Content, DateTime Created, List<string> Tags);
public record QuestionUpdated(string QuestionId, string Title, string Content, string[] Tags);
public record QuestionDeleted(string QuestionId);