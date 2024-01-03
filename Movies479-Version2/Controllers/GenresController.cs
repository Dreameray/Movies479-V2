#nullable disable
using Business;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// Generated from Custom Template.
namespace MVC.Controllers
{
    [Authorize]
    public class GenresController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IMovieService _movieService;

        public GenresController(IGenreService genreService, IMovieService movieService)
        {
            _genreService = genreService;
            _movieService = movieService;
        }

        // GET: Genres
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<GenreModel> genreList = _genreService.GetList();
            return View(genreList);
        }

        // GET: Genres/Details/5
        public IActionResult Details(int id)
        {
            GenreModel genre = _genreService.GetItem(id);
            if (genre == null)
            {
                return View("_Error", "Genre not found!");
            }
            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            ViewBag.MovieId = new MultiSelectList(_movieService.Query().ToList(), "Id", "MovieName");
            return View();
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                var result = _genreService.Add(genre);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.UserId = new MultiSelectList(_movieService.Query().ToList(), "Id", "MovieName");
            return View(genre);
        }

        // GET: Genres/Edit/5
        public IActionResult Edit(int id)
        {
            GenreModel genre = _genreService.GetItem(id);
            if (genre == null)
            {
                return View("_Error", "Genre not found!");
            }
            ViewBag.UserId = new MultiSelectList(_movieService.Query().ToList(), "Id", "MovieName");
            return View(genre);
        }

        // POST: Genres/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                var result = _genreService.Update(genre);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = genre.Id });
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.UserId = new MultiSelectList(_movieService.Query().ToList(), "Id", "MovieName");
            return View(genre);
        }

        // GET: Genres/Delete/5
        public IActionResult Delete(int id)
        {
            var result = _genreService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
