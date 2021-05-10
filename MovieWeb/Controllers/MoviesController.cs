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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            //completar edit
            var movie2 = await _context.Movies.Include(i => i.Genre).FirstOrDefaultAsync(m => m.MovieId == id);
            //var movie = await _context.Movies.FindAsync(id);

            if (movie2 == null)
            {
                Movie deletedMovie = new Movie();
                await TryUpdateModelAsync(deletedMovie);
                ModelState.AddModelError(string.Empty,
                    "Não foi possível salvar alterações. O Filme foi deletado por outra usuário.");
                ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "Name", movie2.GenreID);
                return View(deletedMovie); //ver onde esse view é usada
            }
            _context.Entry(movie2).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Movie>(
                movie2,
                "", 
               s => s.Title, s => s.ReleaseDate,  s => s.Gross,  s => s.MovieId))
            { 
                    try 
                    {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // confirmar se é index a ser utilizado
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var exceptionEntry = ex.Entries.Single();
                        var clientValues = (Movie)exceptionEntry.Entity;
                        var databaseEntry = exceptionEntry.GetDatabaseValues();
                        if (databaseEntry == null)
                        {
                            ModelState.AddModelError(string.Empty,
                                "Unable to save changes. The department was deleted by another user.");
                        }
                        else
                        {
                            var databaseValues = (Movie)databaseEntry.ToObject();

                            if (databaseValues.Title != clientValues.Title)
                            {
                                ModelState.AddModelError("Title", $"Current value: {databaseValues.Title}");
                            }
                            if (databaseValues.Gross != clientValues.Gross)
                            {
                                ModelState.AddModelError("Faturamento", $"Current value: {databaseValues.Gross:c}");
                            }
                            if (databaseValues.ReleaseDate != clientValues.ReleaseDate)
                            {
                                ModelState.AddModelError("StartDate", $"Current value: {databaseValues.ReleaseDate:d}");
                            }
                            if (databaseValues.GenreID != clientValues.GenreID) // verificar se é o GenreID a ser usado
                            {
                                Genre databaseInstructor = await _context.Genres.FirstOrDefaultAsync(i => i.GenreId == databaseValues.GenreID); // verificar a comparação 
                                ModelState.AddModelError("GenreID", $"Current value: {databaseInstructor?.GenreId}"); //Qual valor colocar
                            }

                            ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled and the current values in the database "
                                    + "have been displayed. If you still want to edit this record, click "
                                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                            movie2.RowVersion = (byte[])databaseValues.RowVersion;
                            ModelState.Remove("RowVersion");
                        }
                    }
                }

                ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "Name", movie2.GenreID);
                return View(movie2);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("MovieId,Title,Director,ReleaseDate,Gross,Rating,GenreID")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreID"] = new SelectList(_context.Genres, "GenreId", "GenreId", movie.GenreID);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
