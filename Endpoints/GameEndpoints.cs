using GameStore.Data;
using GameStore.dtos;
using GameStore.Entities;
using GameStore.Mapper;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameSummaryDto> gameDtos = [
        new GameSummaryDto(1, "The Legend of Zelda: Breath of the Wild", "Adventure", 59.99m, new DateOnly(2017, 3, 3)),
        new GameSummaryDto(2, "Elden Ring", "Action RPG", 69.99m, new DateOnly(2022, 2, 25)),
        new GameSummaryDto(3, "Minecraft", "Sandbox", 29.99m, new DateOnly(2011, 11, 18)),
        new GameSummaryDto(4, "Cyberpunk 2077", "Action RPG", 49.99m, new DateOnly(2020, 12, 10)),
        new GameSummaryDto(5, "Stardew Valley", "Simulation", 14.99m, new DateOnly(2016, 2, 26)),
        new GameSummaryDto(6, "God of War: Ragnarok", "Action Adventure", 69.99m, new DateOnly(2022, 11, 9)),
        new GameSummaryDto(7, "Super Mario Odyssey", "Platformer", 59.99m, new DateOnly(2017, 10, 27)),
        new GameSummaryDto(8, "Hollow Knight", "Metroidvania", 14.99m, new DateOnly(2017, 2, 24)),
        new GameSummaryDto(9, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new GameSummaryDto(10, "Among Us", "Party", 4.99m, new DateOnly(2018, 11, 16))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();


        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {
            Game? game = dbContext.Games.Find(id);

            return game == null ? Results.NotFound() : Results.Ok(game);
        })
        .WithName(GetGameEndpointName);

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            game.Genre = dbContext.Genre.Find(newGame.GenreId);
            dbContext.Games.Add(game);
            dbContext.SaveChanges();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToDetailsDto());
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            Game? existingGame = dbContext.Games.Find(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }
            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updateGame.ToEntity(id));
            dbContext.SaveChanges();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id) =>
        {
            gameDtos.RemoveAll((game) => game.Id == id);
            return Results.NoContent();
        });
        return group;
    }
}
