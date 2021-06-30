using System;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IFavorited
    {
        public DateTime FavoriteDate { get; set; }
        public DateTime? UnfavoriteDate { get; set; }
    }
}