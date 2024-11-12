using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks;

[MutationType]
public static class TrackMutations
{
    public static async Task<Track> AddTrackAsync(
        AddTrackInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var track = new Track { Name = input.Name };

        dbContext.Tracks.Add(track);

        await dbContext.SaveChangesAsync(cancellationToken);

        return track;
    }

    [Error<TrackNotFoundException>]
    public static async Task<Track> RenameTrackAsync(
        RenameTrackInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var track = await dbContext.Tracks.FindAsync([input.Id], cancellationToken);

        if (track is null)
        {
            throw new TrackNotFoundException();
        }

        track.Name = input.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return track;
    }
}
