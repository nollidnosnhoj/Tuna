using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Auth.Commands
{
    public sealed class LoadUserByNameOrEmailSpecification : Specification<User>
    {
        public LoadUserByNameOrEmailSpecification(string login)
        {
            Query.Where(u => u.UserName == login || u.Email == login);
        }
    }
}