using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Speakers
{
    public class AddSpeakerInput : InputBase
    {
        public AddSpeakerInput(
            string name, 
            string? bio, 
            string? webSite, 
            string? clientMutationId)
            : base(clientMutationId)
        {
            Name = name;
            Bio = bio;
            WebSite = webSite;
        }

        public string Name { get; }

        public string? Bio { get; }

        public string? WebSite { get; }
    }
}