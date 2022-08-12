using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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

//end points
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

//start app
app.Run();
