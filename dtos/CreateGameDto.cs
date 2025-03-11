namespace GameStore.dtos;

public record class CreateGameDto(
    string Name, 
    string Genre, 
    decimal Price, 
    DateOnly ReleaseDate
);
