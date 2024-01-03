using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business
{
    public interface IMovieService
    {
        IQueryable<MovieModel> Query();

        Result Add(MovieModel model);
        Result Update(MovieModel model);

        [Obsolete("Do not use this method anymore, use DeleteMovie method instead!")]
        Result Delete(int id);

        Result DeleteMovie(int id);
    }

    public class MovieService : IMovieService
    {
       
        private readonly Db _db;

        public MovieService(Db db)
        {
            _db = db;
        }

        public IQueryable<MovieModel> Query()
        {
            return _db.Movies.Include(e => e.Director).OrderByDescending(e => e.DirectorId)
           .ThenBy(e => e.Name)
           .Select(e => new MovieModel()
           {
               // model - entity property assignments
               Id = e.Id,
               Name = e.Name,
               Year = e.Year,
               Revenue = e.Revenue,
               DirectorId = e.DirectorId,
                
               DirectorNameOutput = e.Director.Name
           });
        }

        public Result Add(MovieModel model)
        {
            List<Movie> existingMovies = _db.Movies.ToList();
            if (existingMovies.Any(u => u.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return new ErrorResult("Movie with the same movie name already exists!");

            // entity creation from the model
            Movie movieEntity = new Movie()
            {
                Year = model.Year,
                Name = model.Name.Trim(),
                Revenue = model.Revenue,
                DirectorId = model.DirectorId ?? 0
            
            };
 
            _db.Movies.Add(movieEntity);
            
            _db.SaveChanges();

            return new SuccessResult("Movie added successfully.");
        }

        public Result Update(MovieModel model)
        {
            var existingMovies = _db.Movies.Where(u => u.Id != model.Id).ToList();
            if (existingMovies.Any(u => u.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return new ErrorResult("Movie with the same movie name already exists!");

       
            var movieEntity = _db.Movies.SingleOrDefault(u => u.Id == model.Id);

      
            if (movieEntity is null)
                return new ErrorResult("User not found!");

 
            movieEntity.Year = model.Year;
            movieEntity.Name = model.Name.Trim();
            movieEntity.Revenue = model.Revenue;
            movieEntity.DirectorId = model.DirectorId ?? 0;


            _db.Movies.Update(movieEntity);

    
            _db.SaveChanges();

            return new SuccessResult("Movie updated successfully.");
        }

        public Result Delete(int id)
        {
            var movieGenreEntities = _db.MovieGenres.Where(mv => mv.MovieId == id).ToList();
            
            _db.MovieGenres.RemoveRange(movieGenreEntities);

            // 2) deleting the user record:
            var movieEntity = _db.Movies.SingleOrDefault(u => u.Id == id);
            if (movieEntity is null)
                return new ErrorResult("Movie not found!");
            _db.Movies.Remove(movieEntity);

            _db.SaveChanges();

            return new SuccessResult("Movie deleted successfully.");
        }

        public Result DeleteMovie(int id)
        {
            // getting the user record joined with the user resources records
            var movieEntity = _db.Movies.Include(u => u.MovieGenres).SingleOrDefault(u => u.Id == id);
            if (movieEntity is null)
                return new ErrorResult("Movie not found!");

            _db.MovieGenres.RemoveRange(movieEntity.MovieGenres);
  
            _db.Movies.Remove(movieEntity);

            _db.SaveChanges(); 

            return new SuccessResult("Movie deleted successfully.");
        }




    }


}
