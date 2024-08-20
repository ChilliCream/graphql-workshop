namespace ConferencePlanner.GraphQL.Data;

public sealed class SessionSpeaker
{
    public int SessionId { get; set; }

    public Session? Session { get; set; }

    public int SpeakerId { get; set; }

    public Speaker? Speaker { get; set; }
}
