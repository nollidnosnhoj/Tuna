using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Dtos;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Security;

public class AuthService : IAuthService, IAsyncDisposable
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IPasswordHasher passwordHasher, IUnitOfWork unitOfWork, IDbContextFactory<ApplicationDbContext> dbContextFactory, IHttpContextAccessor httpContextAccessor)
    {
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContextFactory.CreateDbContext();
    }

    public async Task<UserDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetUserWithLogin(request.Login, cancellationToken);

        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var currentUser = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Picture = user.Picture
        };

        return currentUser;
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }
}