
namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {
        public Guid Id { get; }
        public string Title { get;}
        public string Description { get; }
        public int YearOfRelease { get; }
        public IEnumerable<string> Genres { get; } = Enumerable.Empty<string>();
    }
}
