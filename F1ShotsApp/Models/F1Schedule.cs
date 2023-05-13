namespace DatabaseSetupLocal.Models;

public class F1Schedule
{
    public int Year { get; set; }
    public List<RaceSchedule> Races { get; set; }
}

public class RaceSchedule
{
    public string RaceName { get; set; }
    public List<F1Event> F1Events { get; set; }
    public DateTime StartOfQualifing { get; set; }
}

public class F1Event
{
    public string EventName { get; set; }
    public DateTime EventDateAndTime { get; set; }
}