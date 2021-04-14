using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Extensions;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.API.Features.Auth.Register
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