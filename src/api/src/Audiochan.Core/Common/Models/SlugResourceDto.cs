using Audiochan.Core.Common.Helpers;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Common.Models
{
    public abstract record SlugResourceDto : IHasId<long>
    {
        public long Id { get; init; }
        public string Slug => HashIdHelper.EncodeLong(Id);
    }
}