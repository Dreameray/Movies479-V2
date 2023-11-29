#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Director: Record
{
    [Required]
    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTime BirthDate { get; set; }

    public bool IsRetired { get; set; }

    public List<Movie> Movies { get; set; }
}
