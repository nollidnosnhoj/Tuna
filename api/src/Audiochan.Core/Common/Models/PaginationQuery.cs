namespace Audiochan.Core.Common.Models
{
    public class PaginationQuery
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 15;
    }
}