
namespace Movies.Contracts.Responses
{
    public class MovieResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int YearOfRelease { get; set; }
        public IEnumerable<string> Genres { get; set; } = Enumerable.Empty<string>();
    }
}
