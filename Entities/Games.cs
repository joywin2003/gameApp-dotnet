using System;

namespace GameStore.Entities;

public class Games
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public Genre? Genre { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
