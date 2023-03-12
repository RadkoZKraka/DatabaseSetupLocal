namespace DatabaseSetupLocal.Models;

public class F1Grid
{
    public int Year { get; set; }
    public List<Driver> Drivers { get; set; }
}

public class Driver
{
    public string Number { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Abbreviation { get; set; }
    public string Team { get; set; }
}