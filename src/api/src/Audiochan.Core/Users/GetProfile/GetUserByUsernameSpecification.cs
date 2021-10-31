﻿using Ardalis.Specification;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Users
{
    public sealed class GetUserByUsernameSpecification : Specification<User>
    {
        public GetUserByUsernameSpecification(string username)
        {
            Query.AsNoTracking();
            Query.Where(u => u.UserName == username);
        }
    }
}