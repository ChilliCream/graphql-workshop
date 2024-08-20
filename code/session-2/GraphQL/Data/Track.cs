using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Track
{
    public int Id { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    public ICollection<Session> Sessions { get; set; } =
        new List<Session>();
}
