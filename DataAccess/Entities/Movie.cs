#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Movie: Record
{
    [Required]
    public string Name { get; set; }

    public short Year { get; set; }

    public double Revenue { get; set; }

    public Director Director { get; set; }

    public int DirectorId { get; set; }

    public List<MovieGenre> MovieGenres { get; set; }
}
