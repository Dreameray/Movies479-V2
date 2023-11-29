#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class MovieGenre
{
    [Key] 
    public int MovieId { get; set; }

    public Movie Movie { get; set; }

    [Key] 
    public int GenreId { get; set; }

    public Genre Genre { get; set; }
}
