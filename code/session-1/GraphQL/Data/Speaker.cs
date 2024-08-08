using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.GraphQL.Data;

public sealed class Speaker
{
    public int Id { get; set; }

    [StringLength(200)]
    public required string Name { get; set; }

    [StringLength(4000)]
    public string? Bio { get; set; }

    [StringLength(1000)]
    public string? Website { get; set; }
}
