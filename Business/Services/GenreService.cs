using Business.Models;
using Business.Results;
using Business.Results.Bases;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Business.Services
{
    public interface IGenreService
    {
        IQueryable<GenreModel> Query();
        Result Add(GenreModel model);
        Result Update(GenreModel model);
        Result Delete(int id);

        // extra method definitions can be added to use in the related controller and
        // to implement to the related classes
        List<GenreModel> GetList();
        GenreModel GetItem(int id);
    }

    public class GenreService : IGenreService
    {
        private readonly Db _db;

        public GenreService(Db db)
        {
            _db = db;
        }

        public IQueryable<GenreModel> Query()
        {
            return _db.Genres.Include(r => r.MovieGenres).Select(r => new GenreModel()
            {
                Name = r.Name,
                MovieCountOutput = r.MovieGenres.Count,

                // querying over many to many relationship
                MovieNamesOutput = string.Join("<br />", r.MovieGenres.Select(ur => ur.Movie.Name)), // to show movie names in details operation
                MovieIdsInput = r.MovieGenres.Select(ur => ur.MovieId).ToList() // to set selected MovieIds in edit operation
            }).OrderByDescending(r => r.Name);

        }

        public Result Add(GenreModel model)
        {
            var entity = new Genre()
            {
                // ? can be used if the value of a property can be null,
                // if model.Content is null, Content is set to null, else Content is set to
                // model.Content's trimmed value
                Name = model.Name?.Trim(),

                // inserting many to many relational entity,
                // ? must be used with MovieIdsInput if there is a possibility that it can be null
                MovieGenres = model.MovieIdsInput?.Select(movieId => new MovieGenre()
                {
                    MovieId = movieId
                }).ToList()
            };

            _db.Genres.Add(entity);
            _db.SaveChanges();

            return new SuccessResult("Genre added successfully.");
        }

        public Result Update(GenreModel model)
        {

            // deleting many to many relational entity
            var existingEntity = _db.Genres.Include(r => r.MovieGenres).SingleOrDefault(r => r.Id == model.Id);
            if (existingEntity is not null && existingEntity.MovieGenres is not null)
                _db.MovieGenres.RemoveRange(existingEntity.MovieGenres);

            // existingEntity queried from the database must be updated since we got the existingEntity
            // first as above, therefore changes of the existing entity are being tracked by Entity Framework,
            // if disabling of change tracking is required, AsNoTracking method must be used after the DbSet,
            // for example _db.Resources.AsNoTracking()
            existingEntity.Name = model.Name?.Trim();

            // inserting many to many relational entity
            existingEntity.MovieGenres = model.MovieIdsInput?.Select(movieId => new MovieGenre()
            {
                MovieId = movieId
            }).ToList();

            _db.Genres.Update(existingEntity);
            _db.SaveChanges(); // changes in all DbSets are commited to the database by Unit of Work

            return new SuccessResult("Gnere updated successfully.");
        }

        public Result Delete(int id)
        {
            var entity = _db.Genres.Include(r => r.MovieGenres).SingleOrDefault(r => r.Id == id);
            if (entity is null)
                return new ErrorResult("Genre not found!");

            // deleting many to many relational entity:
            // deleting relational MovieGenre entities of the genre entity first
            _db.MovieGenres.RemoveRange(entity.MovieGenres);

            // then deleting the Genre entity
            _db.Genres.Remove(entity);

            _db.SaveChanges();

            return new SuccessResult("Genre deleted successfully.");
        }

        public GenreModel GetItem(int id) => Query().SingleOrDefault(r => r.Id == id);

        public List<GenreModel> GetList()
        {
            // since we wrote the Query method above, we should call it
            // and return the result as a list by calling ToList method
            return Query().ToList();
        }



       
    }

}
