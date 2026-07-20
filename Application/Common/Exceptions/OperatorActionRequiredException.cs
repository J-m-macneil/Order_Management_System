namespace Application.Common.Exceptions;

public class OperatorActionRequiredException : Exception
{
    public OperatorActionRequiredException(string message)
        : base(message)
    {
    }
}
