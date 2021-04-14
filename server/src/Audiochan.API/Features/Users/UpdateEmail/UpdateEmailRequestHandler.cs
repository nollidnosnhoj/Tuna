using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Audiochan.Core.Extensions;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.API.Features.Users.UpdateEmail
{
    public class UpdateEmailRequestHandler : IRequestHandler<UpdateEmailRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdateEmailRequestHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdateEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);

            // TEMPORARY UNTIL EMAIL CONFIRMATION IS SETUP
            var result = await _userManager.SetEmailAsync(user, request.NewEmail);
            if (!result.Succeeded) return result.ToResult();
            await _userManager.UpdateNormalizedEmailAsync(user);
            return Result<bool>.Success(true);
        }
    }
}