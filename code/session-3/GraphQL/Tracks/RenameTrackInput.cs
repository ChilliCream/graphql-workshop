using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks;

public sealed record RenameTrackInput([property: ID<Track>] int Id, string Name);
