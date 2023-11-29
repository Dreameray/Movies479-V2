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
    public interface IDirectorService
    {
        IQueryable<DirectorModel> Query();
        Result Add(DirectorModel model);
        Result Update(DirectorModel model);
        Result Delete(int id);
    }

    public class DirectorService : IDirectorService
    {
        private readonly Db _db;

        public DirectorService(Db db)
        {
            _db = db;
        }

        public IQueryable<DirectorModel> Query()
        {
            return _db.Directors.Include(r => r.Movies).OrderBy(r => r.Name).Select(r => new DirectorModel()
            {
                // model - entity property assignments
                Id = r.Id,
                Name = r.Name,

                // modified model - entity property assignments for displaying in views
                MovieCountOutput = r.Movies.Count // display the user count for each role
            });
        }

        public Result Add(DirectorModel model)
        {
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim());

            var query = _db.Directors.FromSqlRaw("select * from Directors where UPPER(Name) = UPPER(@name)", nameSqlParameter);
            if (query.Any()) // if there are any results for the query above
                return new ErrorResult("Director with the same name already exists!");

            var entity = new Director()
            {
                Name = model.Name.Trim()
            };
            _db.Directors.Add(entity);
            _db.SaveChanges();
            return new SuccessResult("Director added successfully.");

        }

        public Result Update(DirectorModel model)
        {
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim()); // using a parameter prevents SQL Injection
            var idSqlParameter = new SqlParameter("id", model.Id);

            var query = _db.Directors.FromSqlRaw("select * from Directors where UPPER(Name) = UPPER(@name) and Id != @id", nameSqlParameter, idSqlParameter);
            if (query.Any()) // if there are any results for the query above
                return new ErrorResult("Director with the same name already exists!");

            var entity = new Director()
            {
                Id = model.Id, // must be set
                Name = model.Name.Trim()
            };

            // then updating the entity in the related database table
            _db.Directors.Update(entity);
            _db.SaveChanges();
            return new SuccessResult("Director updated successfully.");
        }

        public Result Delete(int id)
        {
            var existingEntity = _db.Directors.Include(r => r.Movies).SingleOrDefault(r => r.Id == id);
            if (existingEntity is null)
                return new ErrorResult("Direcotr not found!");

            if (existingEntity.Movies.Any())
                return new ErrorResult("Director can't be deleted because it has users!");

            // since there is no relational user entities of the role entity, we can delete it
            _db.Directors.Remove(existingEntity);
            _db.SaveChanges();
            return new SuccessResult("Director deleted successfully.");
        }

        
    }


}
