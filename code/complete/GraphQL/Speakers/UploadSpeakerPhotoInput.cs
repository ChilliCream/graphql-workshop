using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Speakers
{
    public record UploadSpeakerPhotoInput([ID(nameof(Speaker))]int Id, IFile Photo);
}