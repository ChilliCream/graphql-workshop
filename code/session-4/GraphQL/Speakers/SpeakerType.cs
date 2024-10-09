using ConferencePlanner.GraphQL.Data;
using GreenDonut.Projections;
using HotChocolate.Execution.Processing;

namespace ConferencePlanner.GraphQL.Speakers;

[ObjectType<Speaker>]
public static partial class SpeakerType
{
    [BindMember(nameof(Speaker.SessionSpeakers))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Speaker speaker,
        ISessionsBySpeakerIdDataLoader sessionsBySpeakerId,
        ISelection selection,
        CancellationToken cancellationToken)
    {
        return await sessionsBySpeakerId
            .Select(selection)
            .LoadRequiredAsync(speaker.Id, cancellationToken);
    }
}
