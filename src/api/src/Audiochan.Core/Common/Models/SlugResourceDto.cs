using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Common.Models
{
    public abstract record SlugResourceDto : IResourceDto<long>
    {
        public long Id { get; init; }
        public string Slug => HashIdHelper.EncodeLong(Id);
    }
}