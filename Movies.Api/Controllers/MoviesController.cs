using Microsoft.AspNetCore.Mvc;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovie request)
        {
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Title = request.Title,
                Generes = request.Genres.ToList(),
                YearOfRelease = request.YearOfRelease,
            };
            await _movieService.CreateAsync(movie);
            var response = new MovieResponse
            {
                Id = movie.Id,
                Description = movie.Description,
                Genres = movie.Generes.ToList(),
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
            };
            return Created($"api/movies/{movie.Id}", response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMovieById([FromRoute] Guid id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie is null)
            {
                return NotFound();
            }
            var response = new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genres = movie.Generes.ToList(),
                YearOfRelease = movie.YearOfRelease,
            };
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetAllAsync();
            var response = movies.Select(movie => new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Genres = movie.Generes.ToList(),
                YearOfRelease = movie.YearOfRelease,
            });
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMovie([FromRoute] Guid id, [FromBody] UpdateMovie request)
        {
            var movie = new Movie
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                YearOfRelease = request.YearOfRelease,
                Generes = request.Genres.ToList()
            };

            var UpdatedMovie = await _movieService.UpdateAsync(movie);

            if(!UpdatedMovie)
            {
                return NotFound();
            }
            var response = new MovieResponse 
            {
                Id = movie.Id,
                Genres = movie.Generes.ToList(),
                Description = movie.Description,
                Title = movie.Title,
                YearOfRelease = movie.YearOfRelease,
            };

            return Ok(response);


        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMovie([FromRoute]Guid id)
        {
            var isDeleted = await _movieService.DeleteAsync(id);
            if (!isDeleted)
            {
                return NotFound(id);
            }

            return Ok();
        }

    }
}
