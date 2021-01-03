using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Profiles.Models;

namespace Audiochan.Core.Interfaces
{
    public interface IProfileService
    {
        Task<IResult<ProfileViewModel>> GetProfile(string username, CancellationToken cancellationToken = default);
    }
}