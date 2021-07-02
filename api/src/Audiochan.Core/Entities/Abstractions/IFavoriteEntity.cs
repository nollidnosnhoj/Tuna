using System;

namespace Audiochan.Core.Entities.Abstractions
{
    public interface IFavoriteEntity
    {
        public DateTime FavoriteDate { get; set; }
        public DateTime? UnfavoriteDate { get; set; }
    }
}