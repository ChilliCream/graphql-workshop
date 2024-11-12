using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Track
{
    public int Id { get; init; }

    [StringLength(200)]
    public required string Name { get; set; }

    public ICollection<Session> Sessions { get; init; } =
        new List<Session>();
}
