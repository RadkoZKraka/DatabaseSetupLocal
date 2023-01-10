using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseSetupLocal.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string UserName { get; set; }
    public List<Race> Race { get; set; }
}

public class Race
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public List<Shot> Shot { get; set; }
    public string? RaceLocation { get; set; }    
    public int RaceYear { get; set; }
    public int RaceNo { get; set; }
    public string? Rand { get; set; }
    public string? PolePosition { get; set; }
}

public class Shot
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Driver { get; set; }

}