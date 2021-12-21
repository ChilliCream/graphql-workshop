using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Speakers
{
    public record UploadSpeakerPhotoInput(int Id, IFile Photo);
}