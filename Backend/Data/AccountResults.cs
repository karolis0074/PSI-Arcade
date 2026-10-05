namespace Backend.Data;

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