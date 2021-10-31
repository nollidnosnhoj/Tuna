﻿namespace Audiochan.Core.Users
{
    public record UpdateProfileRequest
    {
        public string? DisplayName { get; init; }
        public string? About { get; init; }
        public string? Website { get; init; }
    }
}