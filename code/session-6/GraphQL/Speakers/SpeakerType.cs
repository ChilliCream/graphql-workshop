using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Speakers;

[ObjectType<Speaker>]
public static partial class SpeakerType
{
    [BindMember(nameof(Speaker.SessionSpeakers))]
    public static async Task<IEnumerable<Session>> GetSessionsAsync(
        [Parent] Speaker speaker,
        SessionsBySpeakerIdDataLoader sessionsBySpeakerId,
        CancellationToken cancellationToken)
    {
        return await sessionsBySpeakerId.LoadRequiredAsync(speaker.Id, cancellationToken);
    }
}