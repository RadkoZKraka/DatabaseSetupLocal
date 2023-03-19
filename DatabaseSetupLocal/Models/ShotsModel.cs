using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseSetupLocal.Models;

public class UserShots
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string UserName { get; set; }
    public string? OwnerId { get; set; }
    public List<Race> Race { get; set; }
}

public class Race
{
    [Key] 
    public int Id { get; set; }

    public List<Shot> Shot { get; set; }
    public string? RaceLocation { get; set; }
    public int RaceYear { get; set; }
    public int RaceNo { get; set; }
    public int Points { get; set; }
    public string? Rand { get; set; }
    public string? PolePosition { get; set; }
    public bool Locked { get; set; }
}

public class Shot
{
    [Key] 
    public int Id { get; set; }
    public string? OwnerId { get; set; }
    public string? UsersShotDriver { get; set; }
    public string? ResultDriver { get; set; }
    public int Points { get; set; }
    
}