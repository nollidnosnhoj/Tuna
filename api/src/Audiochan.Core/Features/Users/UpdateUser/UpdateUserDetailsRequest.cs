using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.UpdateUser
{
    public record UpdateUserDetailsRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; } = string.Empty;
        public string? DisplayName { get; init; }
        public string? About { get; init; }
        public string? Website { get; init; }
    }

    public class UpdateUserDetailsRequestHandler : IRequestHandler<UpdateUserDetailsRequest, IResult<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserDetailsRequestHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<bool>> Handle(UpdateUserDetailsRequest request, CancellationToken cancellationToken)
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