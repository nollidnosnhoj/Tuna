﻿using System.Threading;
using System.Threading.Tasks;
using Tuna.Domain.Entities;

namespace Tuna.Application.Persistence.Repositories;

public interface IUserRepository : IEntityRepository<User>
{
    Task<User?> LoadUserWithFollowers(long targetId, long observerId, CancellationToken cancellationToken = default);
}