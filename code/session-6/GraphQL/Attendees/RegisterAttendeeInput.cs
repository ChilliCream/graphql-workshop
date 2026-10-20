namespace ConferencePlanner.GraphQL.Attendees;

public sealed record RegisterAttendeeInput(
    string FirstName,
    string LastName,
    string Username,
    string EmailAddress);
