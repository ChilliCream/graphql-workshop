namespace ConferencePlanner.GraphQL.Speakers;

public sealed record AddSpeakerInput(
    string Name,
    string? Bio,
    string? Website);
