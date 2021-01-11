namespace Audiochan.Core.Features.Genres.Models
{
    public enum ListGenresSort
    {
        Alphabetically,
        Popularity
    }
    
    public record ListGenresQueryParams(ListGenresSort Sort = default)
    {
    }
}