﻿using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users.Queries
{
    public sealed class GetUserByUsernameSpecification : Specification<Artist>
    {
        public GetUserByUsernameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.UserName == username);
        }
    }
}