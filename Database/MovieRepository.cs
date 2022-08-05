public class MovieRepository
{
    public MovieDbContext _dbContext { get; set; }

    public MovieRepository(MovieDbContext movieDbContext)
    {
        _dbContext = movieDbContext;
    }

    public IEnumerable<Movie> Get() =>
        _dbContext.Movies.ToArray();

    public Movie Get(Guid id) =>
        _dbContext.Movies.FirstOrDefault(c => c.Id == id);

    public Movie Create(Movie movie)
    {
        _dbContext.Movies.Add(movie);
        _dbContext.SaveChanges();
        return movie;
    }

    public void Delete(Movie movie)
    {
        _dbContext.Movies.Remove(movie);
        _dbContext.SaveChanges();
    }
}
