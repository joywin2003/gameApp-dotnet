using GameStore.Data;
using GameStore.dtos;
using GameStore.Entities;
using GameStore.Mapper;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        group.MapGet("/", (GameStoreContext dbContext) =>
            dbContext.Games
            .Include(game => game.Genre)
            .Select(game => game.ToSummaryDto())
            .AsNoTracking()
        );

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

        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {
            dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDelete();
            return Results.NoContent();
        });
        return group;
    }
}
