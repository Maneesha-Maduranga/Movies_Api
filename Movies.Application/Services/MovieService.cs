﻿using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;  
        }
        public Task<bool> CreateAsync(Movie movie)
        {
            return _movieRepository.CreateAsync(movie);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
           return _movieRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Movie>> GetAllAsync()
        {
          return _movieRepository.GetAllAsync();
        }

        public Task<Movie?> GetByIdAsync(Guid id)
        {
          return _movieRepository.GetByIdAsync(id);
        }

        public Task<bool> UpdateAsync(Movie movie)
        {
            return _movieRepository.UpdateAsync(movie);
        }
    }
}
