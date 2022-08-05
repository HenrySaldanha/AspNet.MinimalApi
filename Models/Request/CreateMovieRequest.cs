using Flunt.Notifications;
using Flunt.Validations;
using System.Text.Json.Serialization;

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
