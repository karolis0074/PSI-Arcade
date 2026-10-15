namespace SportMatch.API.Results;

public enum RegisterResult
{
    Success,
    UsernameTaken,
    InvalidInput
}

public enum AccountResult
{
    Success,
    NotFound,
    InvalidInput
}