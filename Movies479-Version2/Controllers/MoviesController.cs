#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business.Models;
using Business.Services;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using DataAccess.Entities;
using Business;
using Business.Results.Bases;

//Generated from Custom Template.
namespace Movies479_Version2.Controllers
{
    public class MoviesController : Controller
    {
        // Add service injections here
        #region Moive and Director Service Constructor Injections
        private readonly IMovieService _movieService;
        private readonly IDirectorService _directorService;

        public MoviesController(IMovieService movieService, IDirectorService directorService)
        {
            _movieService = movieService;
            _directorService = directorService;
        }
        #endregion

        // GET: Users/GetList
        public IActionResult GetList()
        {
            // A query is executed and the result is stored in the collection
            // when ToList method is called.
            List<MovieModel> movieList = _movieService.Query().ToList();

            return View("List", movieList); 
        }

        // Returning user list in JSON format:
        // GET: Users/GetListJson
        public JsonResult GetListJson()
        {
            var movieList = _movieService.Query().ToList();
            return Json(movieList);
        }

        // GET: Movies
        public IActionResult Index()
        {
            List<MovieModel> movieList = new List<MovieModel>(); // TODO: Add get list service logic here
            return View(movieList);
        }

        // GET: Movies/Details/5
        public IActionResult Details(int id)
        {
            MovieModel movie = _movieService.Query().SingleOrDefault(u => u.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewBag.Directors = new SelectList(_directorService.Query().ToList(), "Id", "Name");
           
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovieModel movie)
        {
            if (ModelState.IsValid)
            {

                Result result = _movieService.Add(movie); // result referenced object can be of type SuccessResult or ErrorResult
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message; // if there is a redirection, the data should be carried with TempData to the redirected action's view
                    return RedirectToAction(nameof(GetList)); // redirection to the action specified of this controller to get the updated list from database
                }

                ModelState.AddModelError("", result.Message);
            }

            ViewBag.Directors = new SelectList(_directorService.Query().ToList(), "Id", "Name");
            return View(movie);
        }

        // GET: Movies/Edit/5
        public IActionResult Edit(int id)
        {
            MovieModel user = _movieService.Query().SingleOrDefault(u => u.Id == id); // getting the model from the service
            if (user == null)
            {
                return NotFound(); // 404 HTTP Status Code
            }
            // Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewBag.RoleId = new SelectList(_directorService.Query().ToList(), "Id", "Name"); // filling the roles
            return View(user); // returning the model to the view so that user can see the data to edit
        }

        // POST: Movies/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MovieModel movie)
        {
            if (ModelState.IsValid)
            {
                var result = _movieService.Update(movie);
                if (result.IsSuccessful)
                {

                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(GetList));
                }
                ModelState.AddModelError("", result.Message);
            }

            ViewBag.DirectorId = new SelectList(_directorService.Query().ToList(), "Id", "Name");


            return View(movie);
        }

        // GET: Movies/Delete/5
        public IActionResult Delete(int id)
        {
            MovieModel movie = _movieService.Query().SingleOrDefault(u => u.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _movieService.DeleteMovie(id);

            // carrying the service result message to the List view through GetList action
            TempData["Message"] = result.Message;

            return RedirectToAction(nameof(GetList));
        }
	}
}
