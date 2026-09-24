namespace ConferencePlanner.GraphQL.Sessions;

public sealed class EndTimeInvalidException() : Exception("EndTime must be after StartTime.");

public sealed class NoSpeakerException() : Exception("No speaker assigned.");

public sealed class SessionNotFoundException() : Exception("Session not found.");

public sealed class TitleEmptyException() : Exception("The title cannot be empty.");
