namespace Audiochan.API.Features.Users.Inputs;

public record UpdateUsernameInput(long UserId, string NewUsername);