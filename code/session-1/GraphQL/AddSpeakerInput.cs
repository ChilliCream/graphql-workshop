namespace ConferencePlanner.GraphQL;

public sealed record AddSpeakerInput(
    string Name,
    string? Bio,
    string? Website);
