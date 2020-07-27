# Adding real-time functionality with subscriptions

For the last few parts of our journey through GraphQL, we have dealt with queries and mutations. In many APIs, this is all people need or want, but GraphQL also offers us real-time capabilities where we can formulate what data we want to receive when a specific event happens.

For our conference API, we would like to introduce two events a user can subscribe to. So, whenever a session is scheduled, we want to be notified. An `onSessionScheduled` event would allow us to send the user notifications whenever a new session is available or whenever a schedule for a specific session has changed.

The second case that we have for subscriptions is whenever a user checks in to a session we want to raise a subscription so that we can notify users that the space in a session is running low or even have some analytics tool subscribe to this event.

## Refactor GraphQL API

Before we can start with introducing our new subscriptions we need to first bring in some new types and add some more packages.

1. Add a reference to the NuGet package package `HotChocolate.Subscriptions.InMemory` version `10.5.0`.
   1. `dotnet add GraphQL package HotChocolate.Subscriptions.InMemory --version 10.5.0`

   > This brings an in-memory subscription bus which is enough if you have just one server. If you want to use multiple GraphQL servers with a strong pub/sub system like Redis you can use `HotChocolate.Subscriptions.Redis` for instance.

1. Add a new directory `Attendees`.

   ```console
   mkdir GraphQL/Attendees
   ```

1. Create a new class `AttendeeQueries` located in the `Attendees` directory with the following content:

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using System.Linq;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Query")]
    public class AttendeeQueries
    {
        [UseApplicationDbContext]
        [UsePaging]
        public IQueryable<Attendee> GetAttendees(
            [ScopedService] ApplicationDbContext context) => 
            context.Attendees;

        public Task<Attendee> GetAttendeeByIdAsync(
            [ID(nameof(Attendee))]int id,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) => 
            attendeeById.LoadAsync(id, cancellationToken);

        public async Task<IEnumerable<Attendee>> GetAttendeesByIdAsync(
            [ID(nameof(Attendee))]int[] ids,
            AttendeeByIdDataLoader attendeeById,
            CancellationToken cancellationToken) => 
            await attendeeById.LoadAsync(ids, cancellationToken);
    }
}
```

```csharp
using System.Collections.Generic;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class AttendeePayloadBase : PayloadBase
    {
        public AttendeePayloadBase(Attendee attendee, string? clientMutationId)
            : base(clientMutationId)
        {
            Attendee = attendee;
        }

        public AttendeePayloadBase(IReadOnlyList<UserError> errors, string? clientMutationId)
            : base(errors, clientMutationId)
        {
        }

        public Attendee? Attendee { get; }
    }
}
```

```csharp
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate.Types.Relay;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class CheckInAttendeeInput : InputBase
    {
        public CheckInAttendeeInput(
            int sessionId,
            int attendeeId,
            string? clientMutationId)
            : base(clientMutationId)
        {
            SessionId = sessionId;
            AttendeeId = attendeeId;
        }

        [ID(nameof(Session))]
        public int SessionId { get; }

        [ID(nameof(Attendee))]
        public int AttendeeId { get; }
    }
}
```

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.DataLoader;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class CheckInAttendeePayload : AttendeePayloadBase
    {
        private int? _sessionId;

        public CheckInAttendeePayload(Attendee attendee, int sessionId, string? clientMutationId)
            : base(attendee, clientMutationId)
        {
            _sessionId = sessionId;
        }

        public CheckInAttendeePayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }

        public async Task<Session?> GetAttendeeAsync(
            SessionByIdDataLoader sessionById,
            CancellationToken cancellationToken)
        {
            if (_sessionId.HasValue)
            {
                return await sessionById.LoadAsync(_sessionId.Value, cancellationToken);
            }

            return null;
        }
    }
}
```


```csharp
using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class RegisterAttendeeInput : InputBase
    {
        public RegisterAttendeeInput(
            string firstName,
            string lastName,
            string userName,
            string emailAddress,
            string? clientMutationId)
            : base(clientMutationId)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            EmailAddress = emailAddress;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string UserName { get; }

        public string EmailAddress { get; }
    }
}
```

```csharp
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Attendees
{
    public class RegisterAttendeePayload : AttendeePayloadBase
    {
        public RegisterAttendeePayload(Attendee attendee, string? clientMutationId)
            : base(attendee, clientMutationId)
        {
        }

        public RegisterAttendeePayload(UserError error, string? clientMutationId)
            : base(new[] { error }, clientMutationId)
        {
        }
    }
}
```

```csharp
using System.Threading;
using System.Threading.Tasks;
using ConferencePlanner.GraphQL.Common;
using ConferencePlanner.GraphQL.Data;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Attendees
{
    [ExtendObjectType(Name = "Mutation")]
    public class AttendeeMutations
    {
        [UseApplicationDbContext]
        public async Task<RegisterAttendeePayload> RegisterAttendeeAsync(
            RegisterAttendeeInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            var attendee = new Attendee
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                UserName = input.UserName,
                EmailAddress = input.EmailAddress
            };

            context.Attendees.Add(attendee);

            await context.SaveChangesAsync(cancellationToken);

            return new RegisterAttendeePayload(attendee, input.ClientMutationId);
        }

        [UseApplicationDbContext]
        public async Task<CheckInAttendeePayload> CheckInAttendeeAsync(
            CheckInAttendeeInput input,
            [ScopedService] ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            Attendee attendee = await context.Attendees.FindAsync(input.AttendeeId, cancellationToken);

            if (attendee is null)
            {
                return new CheckInAttendeePayload(
                    new UserError("Attendee not found.", "ATTENDEE_NOT_FOUND"),
                    input.ClientMutationId);
            }

            attendee.SessionsAttendees.Add(
                new SessionAttendee
                {
                    SessionId = input.SessionId
                });

            await context.SaveChangesAsync(cancellationToken);

            return new CheckInAttendeePayload(attendee, input.SessionId, input.ClientMutationId);
        }
    }
}
```

```csharp

```

```csharp

```