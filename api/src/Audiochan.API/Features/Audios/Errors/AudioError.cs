using Audiochan.Common.Models;
using HotChocolate.Types;

namespace Audiochan.API.Features.Audios.Errors;

[UnionType("AudioError")]
public interface IAudioError : IUserError
{
    
}