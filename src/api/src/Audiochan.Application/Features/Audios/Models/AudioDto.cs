using System;
using Audiochan.Application.Commons.Helpers;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Features.Users.Models;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Features.Audios.Models
{
    public record AudioDto : IHasId<long>, IMapFrom<Audio>
    {
        public long Id { get; set; }
        public string Slug => HashIdHelper.EncodeLong(this.Id);
        public DateTime Created { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public decimal Duration { get; set; }
        public string File { get; set; } = string.Empty;
        public long Size { get; set; }
        public string? Picture { get; set; }
        public long UserId { get; set; }
        public UserDto User { get; set; } = null!;
        
    }
}