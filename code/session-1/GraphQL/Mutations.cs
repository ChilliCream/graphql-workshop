using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL;

public static class Mutations
{
    [Mutation]
    public static async Task<AddSpeakerPayload> AddSpeakerAsync(
        AddSpeakerInput input,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var speaker = new Speaker
        {
            Name = input.Name,
            Bio = input.Bio,
            Website = input.Website
        };

        dbContext.Speakers.Add(speaker);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddSpeakerPayload(speaker);
    }
}
