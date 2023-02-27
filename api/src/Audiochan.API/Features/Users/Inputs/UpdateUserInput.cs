namespace Audiochan.API.Features.Users.Inputs;

public record UpdateUserInput(long UserId, string? DisplayName);