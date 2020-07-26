
# Schema Design

## Reorganize mutation types

1. Create a new folder `Common`.

   ```console
   mkdir GraphQL/Speakers
   ```

1. Create a new base class `InputBase` in the `Common` directory with the following code:

   ```csharp
   namespace ConferencePlanner.GraphQL.Common
   {
       public class InputBase
       {
           public InputBase(string? clientMutationId)
           {
               ClientMutationId = clientMutationId;
           }

           public string? ClientMutationId { get; }
       }
   }
   ```

1. Create a field `PayloadBase.cs` in the `Common` directory with the following code:

   ```csharp
   using System;
   using System.Collections.Generic;

   namespace ConferencePlanner.GraphQL.Common
   {
       public class PayloadBase
       {
           protected PayloadBase(string? clientMutationId)
           {
               Errors = Array.Empty<UserError>();;
               ClientMutationId = clientMutationId;
           }

           protected PayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
           {
               Errors = errors;
               ClientMutationId = clientMutationId;
           }

           public IReadOnlyList<UserError> Errors { get; }

           public string? ClientMutationId { get; }
       }
   }
   ```

1. Next, we create a new class `UserError` that is also located in the `Common` directory with the following code:

   ```csharp
   namespace ConferencePlanner.GraphQL.Common
   {
       public class UserError
       {
           public UserError(string message, string code)
           {
               Message = message;
               Code = code;
        }

        public string Message { get; }

        public string Code { get; }
       }
   }
   ```

Now, that we have some base classes for our mutation types let us start to reorganize the mutation type.

1. Create a new folder `Speakers`.

   ```console
   mkdir GraphQL/Speakers
   ```

1. Move the `Mutation.cs` to the `Speakers` folder and rename it to `SpeakerMutations`.

1. Now, annotate the renamed class with the `ExtendObjectTypeAttribute.` The class should look like this now:

   ```csharp
   using System.Threading;
   using System.Threading.Tasks;
   using ConferencePlanner.GraphQL.Common;
   using ConferencePlanner.GraphQL.Data;
   using HotChocolate;
   using HotChocolate.Types;

   namespace ConferencePlanner.GraphQL.Speakers
   {
       [ExtendObjectType(Name = "Mutation")]
       public class SpeakerMutations
       {
           [UseApplicationDbContext]
           public async Task<AddSpeakerPayload> AddSpeakerAsync(
               AddSpeakerInput input,
               [ScopedService] ApplicationDbContext context,
               CancellationToken cancellationToken)
           {
               var speaker = new Speaker
               {
                   Name = input.Name,
                   Bio = input.Bio,
                   WebSite = input.WebSite
               };

               context.Speakers.Add(speaker);
               await context.SaveChangesAsync(cancellationToken);

               return new AddSpeakerPayload(speaker, input.ClientMutationId);
           }
       }
   }
   ```

1. Move the `AddSpeakerInput.cs` into the `Speakers` directory.

1. Inherit `AddSpeakerInput` from our newly create `InputBase`. The reworked class should look like the following:

   ```csharp
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
   ```

1. Next, create a new class `SpeakerPayloadBase` with the following code:

   ```csharp
   using System.Collections.Generic;
   using ConferencePlanner.GraphQL.Common;
   using ConferencePlanner.GraphQL.Data;

   namespace ConferencePlanner.GraphQL.Speakers
   {
       public class SpeakerPayloadBase : PayloadBase
       {
           public SpeakerPayloadBase(Speaker speaker, string? clientMutationId)
               : base(clientMutationId)
           {
               Speaker = speaker;
           }

           public SpeakerPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
               : base(errors, clientMutationId)
           {
           }

           public Speaker? Speaker { get; }
       }
   }
   ```

1. Now, move the `AddSpeakerPayload` and base it on the new `SpeakerPayloadBase`. The code should now look like the following:

   ```csharp
   using System.Collections.Generic;
   using ConferencePlanner.GraphQL.Common;
   using ConferencePlanner.GraphQL.Data;

   namespace ConferencePlanner.GraphQL.Speakers
   {
       public class AddSpeakerPayload : SpeakerPayloadBase
       {
           public AddSpeakerPayload(Speaker speaker, string? clientMutationId)
               : base(speaker, clientMutationId)
           {
           }

           public AddSpeakerPayload(IReadOnlyList<UserError> errors, string? clientMutationId)
               : base(errors, clientMutationId)
           {
           }
       }
   }
   ```

1. Change the schema builder configurations so that we can merge the various `Mutation` class that we will have into one. For that replace the schema builder configuration with the following code in the `Startup.cs`:

   ```csharp
   services.AddGraphQL(
       SchemaBuilder.New()
           .AddQueryType<Query>()
           .AddMutationType(d => d.Name("Mutation"))
               .AddType<SpeakerMutations>()
           .AddType<SpeakerType>());
   ```

## Enable Relay support

1. Enable relay support for the schema.

   ```csharp
   services.AddGraphQL(
       SchemaBuilder.New()
           .AddQueryType<Query>()
           .AddMutationType(d => d.Name("Mutation"))
               .AddType<SpeakerMutations>()
           .AddType<SpeakerType>()
           .EnableRelaySupport());
   ```

2. Configure the speaker entity to implement the `Node` interface by adding the node configuration to the `SpeakerType`.

   ```csharp
   using System.Collections.Generic;
   using System.Threading;
   using System.Threading.Tasks;
   using ConferencePlanner.GraphQL.Data;
   using ConferencePlanner.GraphQL.DataLoader;
   using HotChocolate.Resolvers;
   using HotChocolate.Types;
   using HotChocolate.Types.Relay;

   namespace ConferencePlanner.GraphQL.Types
   {
       public class SpeakerType : ObjectType<Speaker>
       {
           protected override void Configure(IObjectTypeDescriptor<Speaker> descriptor)
           {
               descriptor
                   .AsNode()
                   .IdField(t => t.Id)
                   .NodeResolver((ctx, id) =>
                       ctx.DataLoader<SpeakerByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

               descriptor
                   .Field(t => t.SessionSpeakers)
                   .ResolveWith<SpeakerResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                   .Name("sessions");
           }

           private class SpeakerResolvers
           {
               public async Task<IEnumerable<Session>> GetSessionsAsync(
                   Speaker speaker,
                   SessionBySpeakerIdDataLoader sessionBySpeakerId,
                   CancellationToken cancellationToken) =>
                   await sessionBySpeakerId.LoadAsync(speaker.Id, cancellationToken);
           }
       }
   }
   ```

1. Head over to the `Query.cs` and annotate the `id` argument of `GetSpeaker` with the `ID` attribute.

   ```csharp
   public Task<Speaker> GetSpeakerAsync(
       [ID(nameof(Speaker))]int id,
       SpeakerByIdDataLoader dataLoader,
       CancellationToken cancellationToken) =>
       dataLoader.LoadAsync(id, cancellationToken);
   ```

1. Start the GraphQL server.

   ```console
   dotent run --project GraphQL
   ```

1. Head to Banana Cakepop and refresh the schema.

   ![Connect to GraphQL server with Banana Cakepop](images/12_bcp_speaker_query.png)
