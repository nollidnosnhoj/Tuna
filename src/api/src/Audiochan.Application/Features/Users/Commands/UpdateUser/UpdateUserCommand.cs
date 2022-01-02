using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Exceptions;
using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Application.Persistence;
using Audiochan.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(long UserId, string? Username, string? Email) : ICommandRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, ApplicationDbContext dbContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindAsync(request.UserId, cancellationToken);

        if (user is null) throw new UnauthorizedException();

        if (!string.IsNullOrEmpty(request.Username))
            await UpdateUsernameAsync(user, request.Username, cancellationToken);

        if (!string.IsNullOrEmpty(request.Email))
            await UpdateEmailAsync(user, request.Email, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserDto>(user);
    }

    private async Task UpdateUsernameAsync(User user, string username, CancellationToken cancellationToken = default)
    {
        // check if username already exists
        var usernameExists =
            await _dbContext.Users.AnyAsync(u => u.UserName == username, cancellationToken);
        
        if (usernameExists)
            throw new UsernameTakenException(username);
        
        user.UserName = username;
    }

    private async Task UpdateEmailAsync(User user, string email, CancellationToken cancellationToken = default)
    {
        var emailExists = await _dbContext.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
        
        if (emailExists)
            throw new EmailTakenException(email);
        
        user.Email = email;
    }
}