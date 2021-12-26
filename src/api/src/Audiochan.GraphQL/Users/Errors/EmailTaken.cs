using Audiochan.Application.Features.Users.Exceptions;
using Audiochan.GraphQL.Common.Errors;

namespace Audiochan.GraphQL.Users.Errors;

public class EmailTaken : GraphQlError
{
    public string Email { get; }
    
    public EmailTaken(EmailTakenException ex) : base($"{ex.Email} is already taken.")
    {
        Email = ex.Email;
    }
}