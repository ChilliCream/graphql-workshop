using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL;

public class Mutation
{
    public async Task<Speaker> AddSpeakerAsync(
        string name,
        string? bio,
        string? webSite,
        ApplicationDbContext context)
    {
        var speaker = new Speaker
        {
            Name = name,
            Bio = bio,
            WebSite = webSite
        };
        context.Speakers.Add(speaker);
        await context.SaveChangesAsync();
        return speaker;
    }
}