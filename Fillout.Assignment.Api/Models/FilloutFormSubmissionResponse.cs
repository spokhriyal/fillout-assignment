namespace Fillout.Assignment.Api.Models;

public class FilloutFormSubmissionResponse
{
    public List<Response>? Responses { get; set; }

    public int? TotalResponses { get; set; }

    public int? PageCount { get; set; }
}

public class Response
{
    public List<Question>? Questions { get; set; }

    public List<Calculation>? Calculations { get; set; }

    public List<UrlParameters>? UrlParameters { get; set; }

    // optional depending on if form is configured to be a quiz
    public Quiz? Quiz { get; set; }

    public string? SubmissionId { get; set; }

    public string? SubmissionTime { get; set; }
}

public class Quiz
{
    public int Score { get; set; }

    public int MaxScore { get; set; }
}

public class Question
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Value { get; set; }
}

public class Calculation
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Value { get; set; }
}

public class UrlParameters
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Value { get; set; }
}