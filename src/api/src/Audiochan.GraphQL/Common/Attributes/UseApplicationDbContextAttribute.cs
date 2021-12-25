using Audiochan.Application.Persistence;

namespace Audiochan.GraphQL.Common.Attributes;

public class UseApplicationDbContextAttribute : UseDbContextAttribute
{
    public UseApplicationDbContextAttribute() : base(typeof(ApplicationDbContext))
    {
    }
}