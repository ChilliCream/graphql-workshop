using ConferencePlanner.GraphQL.Common;

namespace ConferencePlanner.GraphQL.Attendees
{
    public record RegisterAttendeeInput(
        string FirstName,
        string LastName,
        string UserName,
        string EmailAddress,
        string? ClientMutationId) 
        : Input(ClientMutationId);
    
}