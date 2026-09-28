namespace NotificationService.Application.Exceptions;

public class DuplicateProcessedEventException : Exception
{
    public DuplicateProcessedEventException(string message, Exception innerException) : base(message, innerException)
    {
    }
}