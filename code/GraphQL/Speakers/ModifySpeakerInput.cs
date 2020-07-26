using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Speakers
{
    public class ModifySpeakerInput : InputBase
    {
        public ModifySpeakerInput(
            int id,
            Optional<string?> name,
            Optional<string?> bio,
            Optional<string?> webSite,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Id = id;
            Name = name;
            Bio = bio;
            WebSite = webSite;
        }

        [ID(nameof(Speaker))] 
        public int Id { get; }

        public Optional<string?> Name { get; }

        public Optional<string?> Bio { get; }

        public Optional<string?> WebSite { get; }
    }
}