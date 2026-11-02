namespace SportMatch.API.Exceptions;

public class SportFieldDataException : Exception
{
	public SportFieldDataException(string message, Exception? inner = null)
		: base(message, inner)
	{
	}
}