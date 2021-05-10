using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;
using Persistencia.Entidades;
using Persistencia.Repositorio;

namespace MovieWeb.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;

        public MoviesController()
        {
            _context = new MovieContext();
        }

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            // Use LINQ to get list of genres.
            IQueryable<String> genreQuery = from m in _context.Movies
                                            orderby m.Genre.Name
                                            select m.Genre.Name;

            var movies = from m in _context.Movies.Include("Genre")
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre.Name.Equals(movieGenre));
            }

            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };

            return View(movieGenreVM);
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }



        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "GenreId");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Director,ReleaseDate,Gross,Rating,GenreID")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "GenreId", movie.GenreID);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "Name", movie.GenreID);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieToUpdate = await _context.Movies.Include(i => i.Genre).FirstOrDefaultAsync(m => m.MovieId == id);

            if (movieToUpdate == null)
            {
                Movie deletedMovie = new Movie();
                await TryUpdateModelAsync(deletedMovie);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", deletedMovie.GenreID);
                return View(deletedMovie);
            }

            _context.Entry(movieToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Movie>(
                movieToUpdate,
                "",
                s => s.Title, s => s.Director, s => s.ReleaseDate, s => s.Gross, s => s.Rating, s => s.GenreID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Movie)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The movie was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Movie)databaseEntry.ToObject();

                        if (databaseValues.Title != clientValues.Title)
                        {
                            ModelState.AddModelError("Title", $"Current value: {databaseValues.Title}");
                        }
                        if (databaseValues.Director != clientValues.Director)
                        {
                            ModelState.AddModelError("Director", $"Current value: {databaseValues.Director:c}");
                        }
                        if (databaseValues.ReleaseDate != clientValues.ReleaseDate)
                        {
                            ModelState.AddModelError("ReleaseDate", $"Current value: {databaseValues.ReleaseDate:d}");
                        }
                        if (databaseValues.Gross != clientValues.Gross)
                        {
                            ModelState.AddModelError("Gross", $"Current value: {databaseValues.Gross:e}");
                        }
                        if (databaseValues.Rating != clientValues.Rating)
                        {
                            ModelState.AddModelError("Rating", $"Current value: {databaseValues.Rating:e}");
                        }
                        if (databaseValues.GenreID != clientValues.GenreID)
                        {
                            Genre databaseGenre = await _context.Genres.FirstOrDefaultAsync(i => i.GenreId == databaseValues.GenreID);
                            ModelState.AddModelError("Genre", $"Current value: {databaseGenre?.Name}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        movieToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", movieToUpdate.GenreID);
            return View(movieToUpdate);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Movie movie)
        {
            try
            {
                if (await _context.Movies.AnyAsync(m => m.MovieId == movie.MovieId))
                {
                    _context.Movies.Remove(movie);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = movie.MovieId });
            }
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
