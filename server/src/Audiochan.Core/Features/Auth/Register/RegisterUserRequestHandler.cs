using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth.Register
{
    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RegisterUserRequestHandler(UserManager<User> userManager, IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<bool>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User(request.Username.Trim().ToLower(), request.Email, _dateTimeProvider.Now);
            var identityResult = await _userManager.CreateAsync(user, request.Password);
            return identityResult.ToResult();
        }
    }
}