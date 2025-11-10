using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Allow CORS for local frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

var personnel = new List<Person>();

app.MapGet("/api/personnel", () => personnel);

app.MapPost("/api/personnel", (Person person) =>
{
    person.Id = Guid.NewGuid();
    personnel.Add(person);
    return Results.Created($"/api/personnel/{person.Id}", person);
});

app.MapPut("/api/personnel/{id}/status", (Guid id, StatusUpdate update) =>
{
    var person = personnel.FirstOrDefault(p => p.Id == id);
    if (person == null) return Results.NotFound();
    person.Status = update.Status;
    return Results.Ok(person);
});

app.Run();

record Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string Status { get; set; } = "Checked Out";
}

record StatusUpdate(string Status);