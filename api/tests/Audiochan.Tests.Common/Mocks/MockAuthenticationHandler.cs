using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Audiochan.Common.Services;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Audiochan.Tests.Common.Mocks;

public class MockAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public long DefaultUserId { get; set; }
    public string DefaultUserName { get; set; } = null!;
}

public class MockAuthenticationHandler : AuthenticationHandler<MockAuthenticationHandlerOptions>
{
    public const string UserIdHeader = "UserId";
    public const string UserNameHeader = "UserName";
    public const string AuthenticationScheme = "Test";

    private readonly long _defaultUserId;
    private readonly string _defaultUserName;

    public MockAuthenticationHandler(
        IOptionsMonitor<MockAuthenticationHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _defaultUserId = options.CurrentValue.DefaultUserId;
        _defaultUserName = options.CurrentValue.DefaultUserName;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var userId = _defaultUserId;
        string userName = _defaultUserName;
        var claims = new List<Claim>();

        if (Context.Request.Headers.TryGetValue(UserIdHeader, out var userIdValue))
        {
            var userIdString = userIdValue[0];
            if (long.TryParse(userIdString, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            Logger.LogWarning("Unable to parse userId in header. Using default user id.");
        }

        if (Context.Request.Headers.TryGetValue(UserNameHeader, out var userNameValue))
        {
            if (!string.IsNullOrEmpty(userNameValue[0])) userName = userNameValue[0]!;
        }

        var user = await GetOrAddUser(userId, userName);
        
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);
        var result = AuthenticateResult.Success(ticket);
        return result;
    }

    private async Task<User> GetOrAddUser(long userId, string userName)
    {
        var dbContext = Context.RequestServices.GetRequiredService<ApplicationDbContext>();

        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId, Context.RequestAborted);
        if (user is null)
        {
            var passwordHasher = Context.RequestServices.GetRequiredService<IPasswordHasher>();
            var password = passwordHasher.Hash(Guid.NewGuid().ToString());
            user = new User(userName, userName + "@example.com", password);
            await dbContext.Users.AddAsync(user, Context.RequestAborted);
        }

        return user;
    }
}