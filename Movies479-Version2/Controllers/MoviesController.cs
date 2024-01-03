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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Data;

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
        [Authorize]
        public IActionResult GetList()
        {
            // A query is executed and the result is stored in the collection
            // when ToList method is called.
            List<MovieModel> movieList = _movieService.Query().ToList();

            return View("List", movieList); 
        }

        // Returning user list in JSON format:
        // GET: Users/GetListJson
        [Authorize]
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

            if (!User.Identity.IsAuthenticated || !User.IsInRole("admin")) // setting default user model values for registering new users operation
            {
                movie.DirectorId = 1;

            }

            if (ModelState.IsValid)
            {

                Result result = _movieService.Add(movie); // result referenced object can be of type SuccessResult or ErrorResult
                if (result.IsSuccessful)
                {

                    if (!User.Identity.IsAuthenticated || !User.IsInRole("admin")) // if register operation is successful, redirect to the "Account/Login" route
                        return Redirect("Account/Login"); // custom route redirection

                    TempData["Message"] = result.Message; // if there is a redirection, the data should be carried with TempData to the redirected action's view
                    return RedirectToAction(nameof(GetList)); // redirection to the action specified of this controller to get the updated list from database
                }

                ModelState.AddModelError("", result.Message);
            }

            ViewBag.Directors = new SelectList(_directorService.Query().ToList(), "Id", "Name");
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
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
        [Authorize]
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


        #region Movie Authentication
        // Way 3: we can also change the route template in the HttpGet action method
        [HttpGet("Account/{action}")]
        public IActionResult Login()
        {
            return View(); // returning the Login view to the user for entering the movie name
        }

        // Way 1: changing the route by using Route attribute
        //[Route("Account/{action}")]
        // Way 2: changing the route by using the HttpPost action method
        [HttpPost("Account/{action}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MovieModel movie)
        {
            // checking the active user from the database table by the user name
            var existingUser = _movieService.Query().SingleOrDefault(u => u.Name == movie.Name);
            if (existingUser is null) // if an active user with the entered user name and password can't be found in the database table
            {
                ModelState.AddModelError("", "Invalid movie name"); // send the invalid message to the view's validation summary 
                return View(); // returning the Login view
            }

            // Creating the claim list that will be hashed in the authentication cookie which will be sent with each request to the web application.
            // Only non-critical user data, which will be generally used in the web application such as user name to show in the views or user role
            // to check if the user is authorized to perform specific actions, should be put in the claim list.
            // Critical data such as password must never be put in the claim list!
            List<Claim> movieClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, existingUser.Name),
                new Claim(ClaimTypes.Role, existingUser.DirectorNameOutput),
            };

            // creating an identity by the claim list and default cookie authentication
            var movieIdentity = new ClaimsIdentity(movieClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            // creating a principal by the identity
            var moviePrincipal = new ClaimsPrincipal(movieIdentity);

            // signing the user in to the MVC web application and returning the hashed authentication cookie to the client
            await HttpContext.SignInAsync(moviePrincipal);
            // Methods ending with "Async" should be used with the "await" (asynchronous wait) operator therefore
            // the execution of the task run by the asynchronous method can be waited to complete and the
            // result of the method can be used. If the "await" operator is used in a method, the method definition
            // must be changed by adding "async" keyword before the return type and the return type must be written 
            // in "Task". If the method is void, only "Task" should be written.

            // redirecting user to the home page
            return RedirectToAction("Index", "Home");
        }

        // ~/Account/Logout
        [HttpGet("Account/{action}")]
        public async Task<IActionResult> Logout()
        {
            // signing out the user by removing the authentication cookie from the client
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // redirecting user to the home page
            return RedirectToAction("Index", "Home");
        }

        // ~/Account/AccessDenied
        [HttpGet("Account/{action}")]
        public IActionResult AccessDenied()
        {
            // returning the partial view "_Error" by sending the message of type string as model
            return View("_Error", "You don't have access to this operation!");
        }
        #endregion
    }

}
