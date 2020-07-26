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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }
    }
}