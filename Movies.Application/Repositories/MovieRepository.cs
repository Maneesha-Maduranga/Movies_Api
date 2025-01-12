using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = new();
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateAsync(Movie movie)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            var movieQuery = @"
            INSERT INTO Movies (Id, Title, Description, YearOfRelease)
            VALUES (@Id, @Title, @Description, @YearOfRelease)";

            var result = await connection.ExecuteAsync(movieQuery, movie, transaction);

            if (result > 0)
            {
                var genreQuery = @"
                INSERT INTO Genres (MovieId, Name)
                VALUES (@MovieId, @Name)";

                foreach (var genre in movie.Generes)
                {
                    var genreModel = new
                    {
                        MovieId = movie.Id,
                        Name = genre
                    };
                    await connection.ExecuteAsync(genreQuery, genreModel, transaction);
                }
            }

            transaction.Commit();
            return result > 0;

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();
            // Delete genres associated with the movie
            var deleteGenresQuery = "DELETE FROM Genres WHERE MovieId = @MovieId;";
            await connection.ExecuteAsync(deleteGenresQuery, new { MovieId = id },transaction);

            // Delete the movie itself
            var deleteMovieQuery = "DELETE FROM Movies WHERE Id = @Id;";
            var rowsAffected = await connection.ExecuteAsync(deleteMovieQuery, new { Id = id },transaction);
            transaction.Commit();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            var movieQuery = @"
            SELECT M.*, STRING_AGG(G.Name, ',') AS genres
            FROM Movies M
            LEFT JOIN Genres G ON M.Id = G.MovieId
            GROUP BY M.Id, M.Title, M.Description, M.YearOfRelease;
            ";

            var result = await connection.QueryAsync(movieQuery);
            return result.Select(x => new Movie
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                YearOfRelease = x.YearOfRelease,
                Generes = Enumerable.ToList(x.genres.Split(','))
            });
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();

            var movieQuery = @"
            SELECT M.*, STRING_AGG(G.Name, ',') AS genres
            FROM Movies M
            LEFT JOIN Genres G ON M.Id = G.MovieId
            WHERE M.Id = @Id
            GROUP BY M.Id, M.Title, M.Description, M.YearOfRelease
            ";
            var result = await connection.QuerySingleOrDefaultAsync(movieQuery, new { Id = id });
            if(result is null)
            {
                return null;
            }
            return new Movie
            {
                Id = result.Id,
                Description = result.Description,
                Generes = result.genres != null ? Enumerable.ToList(result.genres.Split(',')) : new List<string>(),
                Title = result.Title,
                YearOfRelease = result.YearOfRelease,
            };
        }

        public async Task<bool> UpdateAsync(Movie movie)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();


            // Delete existing genres for the movie
            var deleteGenresQuery = "DELETE FROM Genres WHERE MovieId = @MovieId;";
            await connection.ExecuteAsync(deleteGenresQuery, new { MovieId = movie.Id }, transaction);

            // Insert updated genres
            var insertGenresQuery = @"
            INSERT INTO Genres (MovieId, Name)
            VALUES (@MovieId, @Name);
        ";
            var genresParameters = movie.Generes.Select(genre => new { MovieId = movie.Id, Name = genre });
            await connection.ExecuteAsync(insertGenresQuery, genresParameters, transaction);

            // Update the Movies table
            var updateMovieQuery = @"
            UPDATE Movies
            SET Title = @Title, Description = @Description, YearOfRelease = @YearOfRelease
            WHERE Id = @Id;
        ";
            var rowsAffected = await connection.ExecuteAsync(updateMovieQuery, movie, transaction);

            if (rowsAffected == 0)
            {
                transaction.Rollback();
                return false;
            }

            // Commit transaction
            transaction.Commit();
            return true;
        }
    }
}
