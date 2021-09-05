﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IUserRepository : IEntityRepository<User>
    {
        Task<long[]> GetObserverFollowingIds(long observerId, CancellationToken ct = default);
    }
}