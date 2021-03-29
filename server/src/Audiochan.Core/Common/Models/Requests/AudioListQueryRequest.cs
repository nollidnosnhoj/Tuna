
namespace Audiochan.Core.Common.Models.Requests
{
    public abstract record AudioListQueryRequest : PaginationQueryRequest
    {
        public string Sort { get; init; } = string.Empty;
    }
}