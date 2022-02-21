using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort)
        {
            var movies = from movie in _dbContext.Movies
                select new
                {
                    Id = movie.Id,
                    Name = movie.Name,
                    Duration = movie.Duration,
                    Language = movie.Language,
                    Rating = movie.Rating,
                    Genre = movie.Genre,
                    ImageUrl = movie.ImageUrl
                };

            switch (sort)
            {
                case "desc":
                    return Ok(movies.OrderByDescending(m => m.Rating));
                case "asc":
                    return Ok(movies.OrderBy(m => m.Rating));
                default:
                    return Ok(movies);
            }
        }

        //api/movies/moviedetail/{id}
        //[Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        // POST api/<MoviesController>
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }

            movieObj.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT api/<MoviesController>/5
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found against this id");
            }

            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
                movie.ImageUrl = filePath.Remove(0, 7);
            }

            movie.Name = movieObj.Name;
            movie.Language = movieObj.Language;
            movie.Duration = movieObj.Duration;
            movie.PlayingTime = movieObj.PlayingTime;
            movie.PlayingDate = movieObj.PlayingDate;
            movie.TicketPrice = movieObj.TicketPrice;
            movie.Rating = movieObj.Rating;
            movie.Genre = movieObj.Genre;
            movie.TrailerUrl = movieObj.TrailerUrl;
            movie.ImageUrl = movieObj.ImageUrl;
            _dbContext.SaveChanges();
            return Ok("Record updated successfully");
        }

        // DELETE api/<MoviesController>/5
        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found against this id");
            }
            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();
            return Ok("Record deleted");
        }
    }
}
