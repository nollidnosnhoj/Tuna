﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;

namespace Audiochan.Core.Common.Interfaces
{
    public interface ISearchService
    {
        Task<PagedList<AudioViewModel>> SearchAudios(string searchTerm, string[] filteredTags, int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default);
    }
}