
The focus of this project was a simple implementation of **Minimal Api** with **Sql Lite** and **Entity Framework**.

![#](https://github.com/HenrySaldanha/AspNet.MinimalApi/blob/main/images/api.png?raw=true)

## Packages
    EntityFramework
    Flunt
    Microsoft.EntityFrameworkCore
    Microsoft.EntityFrameworkCore.Design
    Microsoft.EntityFrameworkCore.Sqlite
    Microsoft.EntityFrameworkCore.Tools
    Swashbuckle.AspNetCore

## Config
In the Program class was configured a database, json input and output, sql lite repository and swagger.

	//builder config
	var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddDbContext<MovieDbContext>(op =>
	    op.UseSqlite("DataSource=app.db;Cache=Shared"));
	builder.Services.Configure<JsonOptions>(opt =>
	    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
	builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();
	builder.Services.AddScoped<MovieRepository>();

	//app config
	var app = builder.Build();
	app.UseSwagger().UseSwaggerUI();

    
## EndPoints
End points were implemented in the Program class.

	app.MapGet("/movie", (MovieRepository repository) =>
	{
	    var movies = repository.Get();
	    return Results.Ok(movies.Select(m => (MovieResponse)m));
	}).Produces<IEnumerable<Movie>>(StatusCodes.Status200OK);

	app.MapGet("/movie/{id}", (MovieRepository repository, Guid id) =>
	{
	    var movie = repository.Get(id);
	    if (movie is null)
	        return Results.NotFound();

	    return Results.Ok((MovieResponse)movie);
	}).Produces<Movie>(StatusCodes.Status200OK)
	.Produces(StatusCodes.Status404NotFound);

	app.MapPost("/movie", (MovieRepository repository, CreateMovieRequest request) =>
	{
	    Movie movie = request;
	    if (!request.IsValid)
	        return Results.BadRequest(request.Notifications);

	    repository.Create(movie);

	    return Results.Created("/movie", (MovieResponse)movie);
	}).Produces<Movie>(StatusCodes.Status201Created)
	.Produces(StatusCodes.Status400BadRequest);

	app.MapDelete("/movie/{id}", (MovieRepository repository, Guid id) =>
	{
	    var movie = repository.Get(id);
	    if (movie is null)
	        return Results.NotFound();

	    repository.Delete(movie);

	    return Results.Accepted();
	}).Produces(StatusCodes.Status202Accepted)
	.Produces(StatusCodes.Status404NotFound);

After implementing the end points, we need to run the application.
		
	app.Run();

## Request Validation
The request validation was done with the Flunt package as follows:

	public class CreateMovieRequest : Notifiable<Notification>
	{
	    public string Name { get; set; }
	    public DateTime Released { get; set; }
	    [JsonConverter(typeof(JsonStringEnumConverter))]
	    public Genre Genre { get; set; }

	    public static implicit operator Movie(CreateMovieRequest request)
	    {
	        var contract = new Contract<Notification>()
	            .Requires()
	            .IsNotNull(request.Name, "Name is null")
	            .IsLowerOrEqualsThan(request.Released, DateTime.UtcNow, "Invalid date");

	        request.AddNotifications(contract);

	        return new Movie(Guid.NewGuid(), request.Name, request.Released, request.Genre);
	    }
	}

## SqlLite Implementation

Sql Lite implementation was done with Entity Framework Code First Approach.

DbContext implementation:

	public class MovieDbContext : DbContext
	{
	    public DbSet<Movie> Movies { get; set; }

	    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }
	}

Database usage implementation:

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

## Entity Framework Commands
The migration was created and applied in the package manager console with the following commands

	 Add-Migration InitialMigration
	 Update-Database

## Give a Star 
If you found this Implementation helpful or used it in your Projects, do give it a star. Thanks!

## This project was built with
* [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [Entity Framework](https://docs.microsoft.com/pt-br/ef/)
* [Swagger](https://swagger.io/)
* [Flunt](https://github.com/andrebaltieri/Flunt)
* [Sql Lite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/#dependencies-body-tab)

## My contacts
* [LinkedIn](https://www.linkedin.com/in/henry-saldanha-3b930b98/)