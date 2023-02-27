namespace Audiochan.API.Features.Users.Inputs;

public record UpdatePasswordInput(string CurrentPassword, string NewPassword);