#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Genre: Record
{
    [Required]
    public string Name { get; set; }

    public List<MovieGenre> MovieGenres { get; set; }
}
