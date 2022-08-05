using System.Text.Json.Serialization;

public class MovieResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Released { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Genre Genre { get; set; }

    public static implicit operator MovieResponse(Movie movie)
    {
        if (movie is null) 
            return null;

        return new MovieResponse
        {
            Id = movie.Id,
            Name = movie.Name,
            Released = movie.Released,
            Genre = movie.Genre,
        };
    }
}
