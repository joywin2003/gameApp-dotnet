using GameStore.dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var GetGameEndpointName = "GetGame";

List<GameDto> gameDtos = [
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

app.MapGet("/", () => "Hello World!");

app.MapGet("games", () => gameDtos);

app.MapGet("games/{id}", (int id) => gameDtos.Find(game => game.Id == id)).WithName(GetGameEndpointName);

app.MapPost("games", (CreateGameDto newGame) =>
{
      GameDto game = new(
            Id: gameDtos.Count + 1,
            newGame.Name,
            newGame.Genre,
            newGame.Price,
            newGame.ReleaseDate
      );
      gameDtos.Add(game);
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
});

app.MapPut("games/{id}",(int id, UpdateGameDto updateGame)=>{
      var index = gameDtos.FindIndex(game => game.Id == id);
      gameDtos[index] = new GameDto(
            id,
            updateGame.Name,
            updateGame.Genre,
            updateGame.Price,
            updateGame.ReleaseDate
      );
      return Results.Ok($"{id},{index}");
});

app.Run();
