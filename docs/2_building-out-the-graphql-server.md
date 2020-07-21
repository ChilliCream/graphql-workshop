
# Build out the GraphQL server project

## Configure Nullability

The GraphQL type system distinguishes between nullable and non-nullable types. This helps the consumer of the API by providing guarantees when a field value can be trusted to never be null or when an input is not allowed to be null. The ability to rely on such type information simplifies the code of the null since we do not have to write a ton of null checks for things that will never be null.

1. Open the project file of your GraphQL server project `GraphQL.csproj` and add the following property:

   ```xml
   <Nullable>enable</Nullable>
   ```

   You project file now look like the following:

   ```xml
   <Project Sdk="Microsoft.NET.Sdk.Web">

     <PropertyGroup>
       <TargetFramework>netcoreapp3.1</TargetFramework>
       <Nullable>enable</Nullable>
     </PropertyGroup>

     <ItemGroup>
       <PackageReference Include="HotChocolate.AspNetCore" Version="10.5.0-rc.0" />
       <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="3.1.6" />
       <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="3.1.6">
         <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
         <PrivateAssets>all</PrivateAssets>
       </PackageReference>
     </ItemGroup>

   </Project>
   ```

1. Build your project.
   1. `dotnet build`

   > The compiler will now output a lot of warnings about properties that are now not nullable that are likely to be null. In GraphQL types are by default nullable whereas in C# types are per default not nullable.

1. The compiler is complaining that the `ApplicationDBContext` property `Speakers` might be null when the type is created. The Entity Framework is setting this field dynamically so the compiler can not see that this field will actually be set. So, in order to fix this lets tell the compiler not to worry about it by assigning `default!` to it:

   ```csharp
   public DbSet<Speaker> Speakers { get; set; } = default!;
   ```

1. Next update the speaker model by marking all the reference types as nullable.

   > The schema still will infer nullability correct since the schema understands the data annotations.

   ```csharp
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    namespace ConferencePlanner.GraphQL.Data
    {
        public class Speaker
        {
            public int Id { get; set; }

            [Required]
            [StringLength(200)]
            public string? Name { get; set; }

            [StringLength(4000)]
            public string? Bio { get; set; }

            [StringLength(1000)]
            public virtual string? WebSite { get; set; }
        }
    }
   ```

1. Now update the input and payload type by marking nullable fields.

   ```csharp
    namespace GraphQL
    {
        public class AddSpeakerInput
        {
            public AddSpeakerInput(string name, string? bio, string? webSite, string? clientMutationId)
            {
                Name = name;
                Bio = bio;
                WebSite = webSite;
                ClientMutationId = clientMutationId;
            }

            public string Name { get; }

            public string? Bio { get; }

            public string? WebSite { get; }

            public string? ClientMutationId { get; }
        }
    }
   ```

   ```csharp
    using ConferencePlanner.GraphQL.Data;

    namespace GraphQL
    {
        public class AddSpeakerPayload
        {
            public AddSpeakerPayload(Speaker speaker, string? clientMutationId)
            {
                Speaker = speaker;
                ClientMutationId = clientMutationId;
            }

            public Speaker Speaker { get; }

            public string? ClientMutationId { get; }
        }
    }
   ```

## Adding the remaining models to ConferenceDTO

We've got several more models to add, and unfortunately it's a little mechanical. You can copy the following classes manually, or open the completed solution which is shown at the end.

1. Create an `Attendee.cs` class in the *ConferenceDTO* project with the following code:
   ```csharp
   using System;
   using System.Collections.Generic;
   using System.ComponentModel.DataAnnotations;
   
   namespace ConferenceDTO
   {
       public class Attendee
       {
           public int Id { get; set; }
   
           [Required]
           [StringLength(200)]
           public virtual string FirstName { get; set; }
   
           [Required]
           [StringLength(200)]
           public virtual string LastName { get; set; }
   
           [Required]
           [StringLength(200)]
           public string UserName { get; set; }
           
           [StringLength(256)]
           public virtual string EmailAddress { get; set; }
       }
   }
   ```
1. Create a `Session.cs` class with the following code:
   ```csharp
   using System;
   using System.Collections;
   using System.Collections.Generic;
   using System.ComponentModel.DataAnnotations;
   
   namespace ConferenceDTO
   {
       public class Session
       {
           public int Id { get; set; }
   
           [Required]
           [StringLength(200)]
           public string Title { get; set; }
   
           [StringLength(4000)]
           public virtual string Abstract { get; set; }
   
           public virtual DateTimeOffset? StartTime { get; set; }
   
           public virtual DateTimeOffset? EndTime { get; set; }
   
           // Bonus points to those who can figure out why this is written this way
           public TimeSpan Duration => EndTime?.Subtract(StartTime ?? EndTime ?? DateTimeOffset.MinValue) ?? TimeSpan.Zero;
   
           public int? TrackId { get; set; }
       }
   }
   ```
1. Create a new `Track.cs` class with the following code:
   ```csharp
   using System;
   using System.Collections.Generic;
   using System.ComponentModel.DataAnnotations;
   
   namespace ConferenceDTO
   {
       public class Track
       {
           public int Id { get; set; }
   
           [Required]
           [StringLength(200)]
           public string Name { get; set; }
       }
   }
   ```