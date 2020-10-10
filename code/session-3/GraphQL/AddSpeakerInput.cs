namespace ConferencePlanner.GraphQL
{
    public record AddSpeakerInput(
        string Name,
        string? Bio,
        string? WebSite);
}