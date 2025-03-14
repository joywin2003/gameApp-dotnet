using GameStore.Data;
using GameStore.dtos;
using GameStore.Entities;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> gameDtos = [
        new GameDto(1, "The Legend of Zelda: Breath of the Wild", "Adventure", 59.99m, new DateOnly(2017, 3, 3)),
        new GameDto(2, "Elden Ring", "Action RPG", 69.99m, new DateOnly(2022, 2, 25)),
        new GameDto(3, "Minecraft", "Sandbox", 29.99m, new DateOnly(2011, 11, 18)),
        new GameDto(4, "Cyberpunk 2077", "Action RPG", 49.99m, new DateOnly(2020, 12, 10)),
        new GameDto(5, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26)),
        new GameDto(6, "God of War: Ragnarok", "Action Adventure", 69.99m, new DateOnly(2022, 11, 9)),
        new GameDto(7, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
        new GameDto(8, "Hollow Knight", "Metroidvania", 14.99m, new DateOnly(2017, 2, 24)),
        new GameDto(9, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new GameDto(10, "Among Us", "Party", 4.99m, new DateOnly(2018, 11, 16))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/", () => gameDtos);

        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = gameDtos.Find(game => game.Id == id);
            return game == null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new() {
                Name = newGame.Name,
                Genre = dbContext.Genre.Find(newGame.GenreId),
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            dbContext.SaveChanges();

            GameDto gameDto = new(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            );
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto);
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updateGame) =>
        {
            var index = gameDtos.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }
            gameDtos[index] = new GameDto(
                id,
                updateGame.Name,
                updateGame.Genre,
                updateGame.Price,
                updateGame.ReleaseDate
            );
            return Results.Ok($"{id},{index}");
        });

        group.MapDelete("/{id}", (int id) =>
        {
            gameDtos.RemoveAll((game) => game.Id == id);
            return Results.NoContent();
        });
        return group;
    }
}
