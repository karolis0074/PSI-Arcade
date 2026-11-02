namespace SportMatch.API.Exceptions;

public class InvalidAccountException : Exception
{
	public InvalidAccountException(string message) : base(message)
	{
	}
}