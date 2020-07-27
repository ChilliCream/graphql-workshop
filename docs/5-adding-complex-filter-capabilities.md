# Adding complex filter capabilities

So far, our GraphQL server only exposes plain lists that would, at some point, grow so large that our server would time out. Moreover, we miss some filter capabilities for our session list so that the application using our backend can filter for tracks, titles, or search the abstract for topics.

# Add paging to your lists

Let us start by implementing the last Relay server specification we are still missing in our server by adding Relay compliant paging to our lists. In general, you should avoid plain lists wherever lists grow or are very large. Relay describes a curser based paging where you can navigate between edges through their cursors. Cursor based paging is ideal whenever you implement infinite scrolling solutions. In contrast to offset-pagination, you cannot jump to a specific page, but you can jump to a particular curser and navigate from there.

> Many database drivers or databases do not support `skip while`, so Hot Chocolate will under the hood use positions instead of proper IDs for cursers in theses cases. Meaning, you can always use cursor-based pagination, and Hot Chocolate will handle the rest underneath.

1. Head over to the `Tracks`directory and replace the `GetTracksAsync` resolver in the `TrackQueries.cs` with the following code.

   ```csharp
   [UseApplicationDbContext]
   [UsePaging]
   public IQueryable<Track> GetTracks(
       [ScopedService] ApplicationDbContext context) =>
       context.Tracks.OrderBy(t => t.Name);
   ```

   > The new resolver will instead of executing the database query return an ` IQueryable``. The `IQueryable`is like a query builder. By applying the`UsePaging` middleware, we are rewriting the database query to only fetch the items that we need for our data-set.

   The resolver pipeline for our field now looks like the following:

   ![Query speaker names](images/22-pagination.png)

1. Start your GraphQL server.

   ```console
   dotnet run --project GraphQL
   ```

1. Open Banana Cake Pop and refresh the schema.

   ![Query speaker names](images/23-bcp-schema.png)

1. Head into the schema browser, and let us have a look at how our API structure has changed.

   ![Query speaker names](images/24-bcp-schema.png)

1. Define a simple query to fetch the first track.

   ```graphql
   query GetFirstTrack {
     tracks(first: 1) {
       edges {
         node {
           id
           name
         }
         cursor
       }
       pageInfo {
         startCursor
         endCursor
         hasNextPage
         hasPreviousPage
       }
     }
   }
   ```

   ![Query speaker names](images/25-bcp-GetFirstTrack.png)

1. Take the curser from this item and add a second argument after and feed in the cursor.

   ```graphql
   query GetNextItem {
     tracks(first: 1, after: "MA==") {
       edges {
         node {
           id
           name
         }
         cursor
       }
       pageInfo {
         startCursor
         endCursor
         hasNextPage
         hasPreviousPage
       }
     }
   }
   ```

   ![Query speaker names](images/26-bcp-GetNextTrack.png)

1. Head over to the `SpeakerQueries.cs` which are located in the `Speakers` directory and replace the `GetSpeakersAsync` resolver with the following code:

   ```csharp
   [UseApplicationDbContext]
   [UsePaging]
   public IQueryable<Speaker> GetSpeakers(
       [ScopedService] ApplicationDbContext context) =>
       context.Speakers.OrderBy(t => t.Name);
   ```

1. Next, go to the `SessionQueries.cs` in the `Sessions` directory and replace the `GetSessionsAsync` with the following code:

   ```csharp
   [UseApplicationDbContext]
   [UsePaging]
   public IQueryable<Session> GetSessions(
       [ScopedService] ApplicationDbContext context) =>
       context.Sessions;
   ```

1. Last replace the `GetAttendeesAsync` resolver which is located in `Attendees/AttendeeQueries.cs` with the following code:

   ```csharp
   [UseApplicationDbContext]
   [UsePaging]
   public IQueryable<Attendee> GetAttendeesAsync(
       [ScopedService] ApplicationDbContext context) =>
       context.Attendees;
   ```

We have now replaced all the root level list fields and are now using our pagination middleware. There are still more lists left where we should apply pagination like the 

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
