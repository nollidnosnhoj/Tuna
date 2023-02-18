using System;
using System.Collections.Generic;
using Audiochan.Core.Features.Users.Dtos;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Features.Audios.Dtos
{
    public record AudioDto : IHasId<long>
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal Duration { get; set; }
        public long Size { get; set; }
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string ObjectKey { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}