﻿using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Auth.Refresh
{
    public record RefreshTokenRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string RefreshToken { get; init; }
    }
}