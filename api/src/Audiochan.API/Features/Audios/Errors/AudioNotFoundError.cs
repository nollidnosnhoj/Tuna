using Audiochan.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Domain.Entities;
using HotChocolate.Types.Relay;

namespace Audiochan.API.Features.Audios.Errors;

public record AudioNotFoundError([ID(nameof(AudioViewModel))] long AudioId) 
    : UserError("Audio was not found.", nameof(AudioNotFoundError));