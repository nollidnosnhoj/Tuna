using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Models;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.API.Features.Users.UpdateUser
{
    public record UpdateUserDetailsRequest : IRequest<Result<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string? DisplayName { get; init; }
        public string? About { get; init; }
        public string? Website { get; init; }
    }

    public class UpdateUserDetailsRequestHandler : IRequestHandler<UpdateUserDetailsRequest, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserDetailsRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateUserDetailsRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return Result<bool>.Fail(ResultError.NotFound);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            user.UpdateDisplayName(request.DisplayName);
            user.UpdateAbout(request.About);
            user.UpdateWebsite(request.Website);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}