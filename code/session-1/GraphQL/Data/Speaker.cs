using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Speaker
{
    public int Id { get; init; }

    [StringLength(200)]
    public required string Name { get; init; }

    [StringLength(4000)]
    public string? Bio { get; init; }

    [StringLength(1000)]
    public string? Website { get; init; }
}
