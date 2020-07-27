# GraphQL schema design approaches

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

1. Configure the speaker entity to implement the `Node` interface by adding the node configuration to the `SpeakerType`.

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

## Build out the schema

This step will add more DataLoader and schema types, while this will be a bit mechanical, it will form the basis for our ventures in proper GraphQL schema design.

We will start by adding the rest of the DataLoader that we will need. Then we will add types for `Attendee`, `Track`, and `Session`. Last, we will reorganize our query type so that we can split this as well. Once we have all this in, we will start diving into some schema design rules and how to apply them.

1. Add missing DataLoader to the `DataLoader` directory.

`AttendeeByIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class AttendeeByIdDataLoader : BatchDataLoader<int, Attendee>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public AttendeeByIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<IReadOnlyDictionary<int, Attendee>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                return await dbContext.Attendees
                    .Where(s => keys.Contains(s.Id))
                    .ToDictionaryAsync(t => t.Id, cancellationToken);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`AttendeeBySessionIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class AttendeeBySessionIdDataLoader : GroupedDataLoader<int, Attendee>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public AttendeeBySessionIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Attendee>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                List<SessionAttendee> speakers = await dbContext.Sessions
                    .Where(session => keys.Contains(session.Id))
                    .Include(session => session.SessionAttendees)
                    .SelectMany(session => session.SessionAttendees)
                    .ToListAsync();

                return speakers.Where(t => t.Attendee is { }).ToLookup(t => t.SessionId, t => t.Attendee!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`SessionByAttendeeIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class SessionByAttendeeIdDataLoader : GroupedDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByAttendeeIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Session>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                List<SessionAttendee> speakers = await dbContext.Attendees
                    .Where(speaker => keys.Contains(speaker.Id))
                    .Include(speaker => speaker.SessionsAttendees)
                    .SelectMany(speaker => speaker.SessionsAttendees)
                    .Include(sessionSpeaker => sessionSpeaker.Session)
                    .ToListAsync();

                return speakers
                    .Where(t => t.Session is { })
                    .ToLookup(t => t.AttendeeId, t => t.Session!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`SessionByIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class SessionByIdDataLoader : BatchDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<IReadOnlyDictionary<int, Session>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                return await dbContext.Sessions
                    .Where(s => keys.Contains(s.Id))
                    .ToDictionaryAsync(t => t.Id, cancellationToken);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`SessionByTrackIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class SessionByTrackIdDataLoader : GroupedDataLoader<int, Session>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SessionByTrackIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Session>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                var sessions = await dbContext.Tracks
                    .Where(track => keys.Contains(track.Id))
                    .Include(track => track.Sessions)
                    .SelectMany(track => track.Sessions)
                    .ToListAsync();

                return sessions.ToLookup(t => t.TrackId!.Value);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`SpeakerBySessionIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class SpeakerBySessionIdDataLoader : GroupedDataLoader<int, Speaker>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public SpeakerBySessionIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<ILookup<int, Speaker>> LoadGroupedBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                List<SessionSpeaker> speakers = await dbContext.Sessions
                    .Where(session => keys.Contains(session.Id))
                    .Include(session => session.SessionSpeakers)
                    .SelectMany(session => session.SessionSpeakers)
                    .Include(sessionSpeaker => sessionSpeaker.Speaker)
                    .ToListAsync();

                return speakers
                    .Where(t => t.Speaker is { })
                    .ToLookup(t => t.SessionId, t => t.Speaker!);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

`TrackByIdDataLoader.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.DataLoader;

namespace ConferencePlanner.GraphQL.DataLoader
{
    public class TrackByIdDataLoader : BatchDataLoader<int, Track>
    {
        private readonly DbContextPool<ApplicationDbContext> _dbContextPool;

        public TrackByIdDataLoader(DbContextPool<ApplicationDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool ?? throw new ArgumentNullException(nameof(dbContextPool));
        }

        protected override async Task<IReadOnlyDictionary<int, Track>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            ApplicationDbContext dbContext = _dbContextPool.Rent();
            try
            {
                return await dbContext.Tracks
                    .Where(s => keys.Contains(s.Id))
                    .ToDictionaryAsync(t => t.Id, cancellationToken);
            }
            finally
            {
                _dbContextPool.Return(dbContext);
            }
        }
    }
}
```

1. Now, add the missing type classes, `AttendeeType`, `TrackType`, and `SessionType` to the `Types` directory.

`AttendeeType`

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
    public class AttendeeType : ObjectType<Attendee>
    {
        protected override void Configure(IObjectTypeDescriptor<Attendee> descriptor)
        {
            descriptor
                .AsNode()
                .IdField(t => t.Id)
                .NodeResolver((ctx, id) =>
                    ctx.DataLoader<AttendeeByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.SessionsAttendees)
                .ResolveWith<AttendeeResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                .Name("sessions");
        }

        private class AttendeeResolvers
        {
            public async Task<IEnumerable<Session>> GetSessionsAsync(
                Attendee attendee,
                SessionByAttendeeIdDataLoader sessionByAttendeeId,
                CancellationToken cancellationToken) =>
                await sessionByAttendeeId.LoadAsync(attendee.Id, cancellationToken);
        }
    }
}
```

`SessionType.cs`

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
    public class SessionType : ObjectType<Session>
    {
        protected override void Configure(IObjectTypeDescriptor<Session> descriptor)
        {
            descriptor
                .AsNode()
                .IdField(t => t.Id)
                .NodeResolver((ctx, id) =>
                    ctx.DataLoader<SessionByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.SessionSpeakers)
                .ResolveWith<SessionResolvers>(t => t.GetSpeakersAsync(default!, default!, default))
                .Name("speakers");

            descriptor
                .Field(t => t.SessionAttendees)
                .ResolveWith<SessionResolvers>(t => t.GetAttendeesAsync(default!, default!, default))
                .Name("attendees");

            descriptor
                .Field(t => t.Track)
                .ResolveWith<SessionResolvers>(t => t.GetTrackAsync(default!, default!, default));
        }

        private class SessionResolvers
        {
            public async Task<IEnumerable<Speaker>> GetSpeakersAsync(
                Session session,
                SpeakerBySessionIdDataLoader speakerBySessionId,
                CancellationToken cancellationToken) =>
                await speakerBySessionId.LoadAsync(session.Id, cancellationToken);

            public async Task<IEnumerable<Attendee>> GetAttendeesAsync(
                Session session,
                AttendeeBySessionIdDataLoader attendeeBySessionId,
                CancellationToken cancellationToken) =>
                await attendeeBySessionId.LoadAsync(session.Id, cancellationToken);

            public async Task<Track?> GetTrackAsync(
                Session session,
                TrackByIdDataLoader trackById,
                CancellationToken cancellationToken)
            {
                if (session.TrackId is null)
                {
                    return null;
                }

                return await trackById.LoadAsync(session.TrackId.Value, cancellationToken);
            }

        }
    }
}
```

`TrackType.cs`

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
    public class TrackType : ObjectType<Track>
    {
        protected override void Configure(IObjectTypeDescriptor<Track> descriptor)
        {
            descriptor
                .AsNode()
                .IdField(t => t.Id)
                .NodeResolver((ctx, id) =>
                    ctx.DataLoader<TrackByIdDataLoader>().LoadAsync(id, ctx.RequestAborted));

            descriptor
                .Field(t => t.Sessions)
                .ResolveWith<TrackResolvers>(t => t.GetSessionsAsync(default!, default!, default))
                .Name("sessions");
        }

        private class TrackResolvers
        {
            public async Task<IEnumerable<Session>> GetSessionsAsync(
                Track track,
                SessionByTrackIdDataLoader sessionByTrackId,
                CancellationToken cancellationToken) =>
                await sessionByTrackId.LoadAsync(track.Id, cancellationToken);
        }
    }
}
```

1. Now, move the `Query.cs` to the `Speakers` directory and rename it to `SpeakerQueries.cs`.

1. Next, add the `[ExtendObjectType(Name = "Query")]` on top of our `SpeakerQueries` class. The code should no look like the following.

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class SpeakerQueries
    {
        [UseApplicationDbContext]
        public Task<List<Speaker>> GetSpeakersAsync(
            [ScopedService] ApplicationDbContext context) =>
            context.Speakers.ToListAsync();

        public Task<Speaker> GetSpeakerAsync(
            [ID(nameof(Speaker))]int id,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);
    }
}
```

1. Head over to the `Startup.cs` and lets reconfigure the schema builder like we did with the `Mutation` type. The new schema configuration should look like the following:

```csharp
services.AddGraphQL(
    SchemaBuilder.New()
        .AddQueryType(d => d.Name("Mutation"))
            .AddType<SpeakerQueries>()
        .AddMutationType(d => d.Name("Mutation"))
            .AddType<SpeakerMutations>()
        .AddType<SpeakerType>()
        .EnableRelaySupport());
```

1. Register the `AttendeeType`, `TrackType`, and `SessionType` with the schema builder.

```csharp
services.AddGraphQL(
    SchemaBuilder.New()
        .AddQueryType(d => d.Name("Mutation"))
            .AddType<SpeakerQueries>()
        .AddMutationType(d => d.Name("Mutation"))
            .AddType<SpeakerMutations>()
        .AddType<AttendeeType>()
        .AddType<SessionType>()
        .AddType<SpeakerType>()
        .AddType<TrackType>()
        .EnableRelaySupport());
```

Great, we now have our base schema and are ready to dive into some schema design topics. Although GraphQL has a single root query type, a single root mutation type, and a single root subscription type, Hot Chocolate allows splitting the root types into multiple classes, which will enable us to organize our schema around topics rather than divide it along its root types.

### Think beyond CRUD

GraphQL represents a much better way to expose APIs over HTTP. GraphQL wants us to think beyond standard CRUD APIs. By using action or behavior specific fields and mutations, we can create a more human-readable API that helps clients use our API.

In this chapter, we will design our mutation API by really thinking about the use-cases of our conference API. We do not just want to expose our database model to the user; we want to create an understandable and easy-to-use API driven by use-cases rather than the raw data structures.

First, we will focus on the sessions. The session is the primary data model we are interacting with. People want to lookup sessions, schedule sessions, search for sessions, or even file new sessions.

Conferences typically first ask for papers; after some time, they will accept some of the proposed talks. After more time, they will build from these sessions the schedule. Often the program is divided into tracks. A talk will also often be moved around until the conference starts, but even at this point, schedule changes might happen.

This reflection on our subject at hand leads us to two mutations that we need. First, we need to be able to add new sessions; then, we need to be able to schedule sessions on a specific track and time slot.

1. Create a new directory called `Sessions`

```console
mkdir GraphQL/Sessions
```

1. Add a new class `SessionPayloadBase` in the `Sessions` directory with the following code:

> The `SessionPayloadBase` will be the base for all of our session payloads.

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class SessionPayloadBase : PayloadBase
    {
        public SessionPayloadBase(Session session, string? clientMutationId)
            : base(clientMutationId)
        {
            Session = session;
        }

        public SessionPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Session? Session { get; }
    }
}
```

1. Next add a new class `AddSessionInput` in the `Sessions` directory with the following code:

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class AddSessionInput : InputBase
    {
        public AddSessionInput(
            string title,
            string? @abstract,
            IReadOnlyList<int> speakerIds,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Title = title;
            Abstract = @abstract;
            SpeakerIds = speakerIds;
        }

        public string Title { get; }

        public string? Abstract { get; }

        [ID(nameof(Speaker))]
        public IReadOnlyList<int> SpeakerIds { get; }
    }
}
```

1. Add a new class `AddSessionPayload` in the `Sessions` directory with the following code:

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class AddSessionPayload : PayloadBase
    {
        public AddSessionPayload(Session session, string? clientMutationId)
            : base(clientMutationId)
        {
            Session = session;
        }

        public AddSessionPayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }

        public AddSessionPayload(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Session? Session { get; }
    }
}
```

1. Now, add a new class `SessionMutations` into the `Sessions` directory with the following code:

```csharp
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Sessions
{
    [ExtendObjectType(Name = "Mutation")]
    public class SessionMutations
    {
        [UseApplicationDbContext]
        public async Task<AddSessionPayload> AddSessionAsync(
            AddSessionInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(input.Title))
            {
                return new AddSessionPayload(
                    new UserError("The title cannot be empty.", "TITLE_EMPTY"),
                    input.ClientMutationId);
            }

            if (input.SpeakerIds.Count == 0)
            {
                return new AddSessionPayload(
                    new UserError("No speaker assigned.", "NO_SPEAKER"),
                    input.ClientMutationId);
            }

            var session = new Session
            {
                Title = input.Title,
                Abstract = input.Abstract,
            };

            foreach (int speakerId in input.SpeakerIds)
            {
                session.SessionSpeakers.Add(new SessionSpeaker
                {
                    SpeakerId = speakerId
                });
            }

            context.Sessions.Add(session);
            await context.SaveChangesAsync(cancellationToken);

            return new AddSessionPayload(session, input.ClientMutationId);
        }
    }
}
```

> Our `addSession` mutation will only let you specify the title, the abstract and the speakers.

1. Head back to the `Startup.cs` and add the `SessionMutations` to the schema builder.

```csharp
services.AddGraphQL(
    SchemaBuilder.New()
        .AddQueryType(d => d.Name("Mutation"))
            .AddType<SpeakerQueries>()
        .AddMutationType(d => d.Name("Mutation"))
            .AddType<SessionMutations>()
            .AddType<SpeakerMutations>()
        .AddType<AttendeeType>()
        .AddType<SessionType>()
        .AddType<SpeakerType>()
        .AddType<TrackType>()
        .EnableRelaySupport());
```

1. Next, add the `ScheduleSessionInput` to our `Sessions` directory with the following code:

```csharp
using System;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class ScheduleSessionInput : InputBase
    {
        public ScheduleSessionInput(
            int sessionId,
            int trackId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string? clientMutationId)
            : base(clientMutationId)
        {
            SessionId = sessionId;
            TrackId = trackId;
            StartTime = startTime;
            EndTime = endTime;
        }

        [ID(nameof(Session))]
        public int SessionId { get; }

        [ID(nameof(Track))]
        public int TrackId { get; }

        public DateTimeOffset StartTime { get; }

        public DateTimeOffset EndTime { get; }
    }
}
```

1. Add the `ScheduleSessionPayload` class to with the following code:

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

namespace ConferencePlanner.GraphQL.Sessions
{
    public class ScheduleSessionPayload : SessionPayloadBase
    {
        public ScheduleSessionPayload(Session session, string? clientMutationId)
            : base(session, clientMutationId)
        {
        }

        public ScheduleSessionPayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }

        public async Task<Track?> GetTrackAsync(
            TrackByIdDataLoader trackById,
            CancellationToken cancellationToken)
        {
            if (Session is null)
            {
                return null;
            }

            return await trackById.LoadAsync(Session.Id, cancellationToken);
        }

        public async Task<IEnumerable<Speaker>?> GetSpeakersAsync(
            SpeakerBySessionIdDataLoader speakerBySessionId,
            CancellationToken cancellationToken)
        {
            if (Session is null)
            {
                return null;
            }

            return await speakerBySessionId.LoadAsync(Session.Id, cancellationToken);
        }
    }
}
```

1. Now, insert the following `scheduleSession` mutation to the `SessionMutations` class:

```csharp
[UseApplicationDbContext]
public async Task<ScheduleSessionPayload> ScheduleSessionAsync(
    ScheduleSessionInput input,
    [ScopedService] ApplicationDbContext context,
    [Service]ITopicEventSender eventSender)
{
    if (input.EndTime < input.StartTime)
    {
        return new ScheduleSessionPayload(
            new UserError("endTime has to be larger than startTime.", "END_TIME_INVALID"),
            input.ClientMutationId);
    }

    Session session = await context.Sessions.FindAsync(input.SessionId);
    int? initialTrackId = session.TrackId;

    if (session is null)
    {
        return new ScheduleSessionPayload(
            new UserError("Session not found.", "SESSION_NOT_FOUND"),
            input.ClientMutationId);
    }

    session.TrackId = input.TrackId;
    session.StartTime = input.StartTime;
    session.EndTime = input.EndTime;

    await context.SaveChangesAsync();

    await eventSender.SendAsync(
        nameof(SessionSubscriptions.OnSessionScheduledAsync),
        session.Id);

    return new ScheduleSessionPayload(session, input.ClientMutationId);
}
```

While we now are able to add sessions and then schedule them, we still need some mutations to create a track or rename a track.

1. Create a new directory called `Tracks`

```console
mkdir GraphQL/Tracks
```

1. Add a class `TrackPayloadBase` to the `Tracks` directory with the following code:

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class TrackPayloadBase : PayloadBase
    {
        public TrackPayloadBase(Track track, string? clientMutationId)
            : base(clientMutationId)
        {
            Track = track;
        }

        public TrackPayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Track? Track { get; }
    }
}
```

1. Add a class `AddTrackInput` to the `Tracks` directory with the following code:

```csharp
using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class AddTrackInput : InputBase
    {
        public AddTrackInput(
            string name,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
```

1. Next, add the `AddTrackPayload` payload class with the following code:

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class AddTrackPayload : TrackPayloadBase
    {
        public AddTrackPayload(Track track, string? clientMutationId)
            : base(track, clientMutationId)
        {
        }

        public AddTrackPayload(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }
    }
}
```

1. Now that you have the payload and input files in create a new class `TracksMutations` with the following code:

```csharp
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types;

namespace ConferencePlanner.GraphQL.Tracks
{
    [ExtendObjectType(Name = "Mutation")]
    public class TrackMutations
    {
        [UseApplicationDbContext]
        public async Task<AddTrackPayload> AddTrackAsync(
            AddTrackInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var track = new Track { Name = input.Name };
            context.Tracks.Add(track);

            await context.SaveChangesAsync(cancellationToken);

            return new AddTrackPayload(track, input.ClientMutationId);
        }
    }
}
```

1. Head back to the `Startup.cs` and add the `TrackMutations` to the schema builder.

```csharp
services.AddGraphQL(
    SchemaBuilder.New()
        .AddQueryType(d => d.Name("Mutation"))
            .AddType<SpeakerQueries>()
        .AddMutationType(d => d.Name("Mutation"))
            .AddType<SessionMutations>()
            .AddType<SpeakerMutations>()
            .AddType<TrackMutations>()
        .AddType<AttendeeType>()
        .AddType<SessionType>()
        .AddType<SpeakerType>()
        .AddType<TrackType>()
        .EnableRelaySupport());
```

1. Next, we need to get our `renameTrack` mutation in. For this create a new class `RenameTrackInput` and place it in the `Tracks` directory.

```csharp
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class RenameTrackInput : InputBase
    {
        public RenameTrackInput(
            int id,
            string name,
            string? clientMutationId)
            : base(clientMutationId)
        {
            Id = id;
            Name = name;
        }

        [ID(nameof(Track))]
        public int Id { get; }

        public string Name { get; }
    }
}
```

1. Add a class `RenameTrackPayload` and put it into the `Tracks` directory.

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Tracks
{
    public class RenameTrackPayload : TrackPayloadBase
    {
        public RenameTrackPayload(Track track, string? clientMutationId) 
            : base(track, clientMutationId)
        {
        }

        public RenameTrackPayload(IReadOnlyList<UserError> errors, string? clientMutationId) 
            : base(errors, clientMutationId)
        {
        }
    }
}
```

1. Last, we will add the `renameTrack` mutation to our `TrackMutations` class.

```csharp
[UseApplicationDbContext]
public async Task<RenameTrackPayload> RenameTrackAsync(
    RenameTrackInput input,
    [ScopedService] ApplicationDbContext context,
    CancellationToken cancellationToken)
{
    Track track = await context.Tracks.FindAsync(input.Id);
    track.Name = input.Name;

    await context.SaveChangesAsync(cancellationToken);

    return new RenameTrackPayload(track, input.ClientMutationId);
}
```

1. Start your GraphQL server and verify that your `Mutations` work by adding some sessions, creating tracks and scheduling the sessions to the tracks.

   > The DateTime format in GraphQL is specified by RFC3399 and looks like the following: `2020-05-24T15:00:00`. More about the GraphQL `DateTime` scalar can be found here: https://www.graphql-scalars.com/date-time/

### Offer plural versions fields and be precise about field names

With GraphQL, we want to think about efficiency a lot. For instance, we offer mutations with one `input` argument so that clients can assign this from one variable. Almost every little aspect in GraphQL is done so that you can request data more efficiently. That is why we also should design our schema in such a way that we allow users of our API to fetch multiple entities in one go.

Sure, we technically can do that already.

```graphql
{
    speaker1: speaker(id: 1) {
        name
    }
    speaker2: speaker(id: 2) {
        name
    }
}
```

But with plural versions, we can specify a variable of ids and pass that into a query without modifying the query text itself. By doing that, we can use static queries on our client and also let the query engine of the server optimize this static query for execution. Further, we can write a resolver that is optimized to fetch data in one go. Offering plural fields allows for more flexibility and better performance.

The second aspect here is also to be more specific about our fields. The name `speaker` is quite unspecific, and we are already starting to get a problem with this once we introduce a plural version of it called `speakers` since we already have a field `speakers` that is the list of speakers. A good choice in `GraphQL` would be to name the fields `speakerById` and the second one, `speakersById`.

In this section, we will optimize our `Query` type by bringing in more query and restructuring them to offer plural fields.

1. Head over to your `SpeakerQueries` class and update the `speaker` field to be named `speakerById`.

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class SpeakerQueries
    {
        [UseApplicationDbContext]
        public Task<List<Speaker>> GetSpeakersAsync(
            [ScopedService] ApplicationDbContext context) =>
            context.Speakers.ToListAsync();

        public Task<Speaker> GetSpeakerByIdAsync(
            [ID(nameof(Speaker))]int id,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);
    }
}
```

1. Next, introduce a new `GetSpeakersByIdAsync` method as our plural version. 

> Note that the `DataLoader` can also fetch multiples for us.

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class SpeakerQueries
    {
        [UseApplicationDbContext]
        public Task<List<Speaker>> GetSpeakersAsync(
            [ScopedService] ApplicationDbContext context) =>
            context.Speakers.ToListAsync();

        public Task<Speaker> GetSpeakerByIdAsync(
            [ID(nameof(Speaker))]int id,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            dataLoader.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Speaker>> GetSpeakersByIdAsync(
            [ID(nameof(Speaker))]int[] ids,
            SpeakerByIdDataLoader dataLoader,
            CancellationToken cancellationToken) =>
            await dataLoader.LoadAsync(ids, cancellationToken);
    }
}
```




```csharp

```

```csharp

```


```csharp

```

```csharp

```


```csharp

```

```csharp

```


```csharp

```

```csharp

```


```csharp

```

```csharp

```


```csharp

```

```csharp

```


```csharp

```

