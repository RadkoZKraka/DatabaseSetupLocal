namespace DatabaseSetupLocal.Exceptions;

public class RaceDoesntExistException : Exception
{
    public RaceDoesntExistException() : base("User doesnt exist.") { }
    public RaceDoesntExistException(string message) : base(message) { }
    public RaceDoesntExistException(string message, Exception inner) : base(message, inner) { }
}
public class YearListDoesntExistException : Exception
{
    public YearListDoesntExistException() : base("Year list doesnt exist.") { }
    public YearListDoesntExistException(string message) : base(message) { }
    public YearListDoesntExistException(string message, Exception inner) : base(message, inner) { }
}

public class AppUserDoesntExistException : Exception
{
    public AppUserDoesntExistException() : base("App user doesnt exist.") { }
    public AppUserDoesntExistException(string message) : base(message) { }
    public AppUserDoesntExistException(string message, Exception inner) : base(message, inner) { }
}