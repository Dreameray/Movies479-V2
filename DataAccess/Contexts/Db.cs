using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Contexts;

public class Db: DbContext
{
    public DbSet<Director> Directors { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<MovieGenre> MovieGenres { get; set; }

    public DbSet<Movie> Movies { get; set; }

    public Db(DbContextOptions options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieGenre>().HasKey(e =>  new { e.MovieId, e.GenreId }); 
    }
}
