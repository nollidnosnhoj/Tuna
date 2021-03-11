using System;
using Audiochan.Core.Common.Models;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public class AudioViewModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public bool IsPublic { get; init; }
        public int Duration { get; init; }
        public string Picture { get; init; }
        public int FavoriteCount { get; init; }
        public bool IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public GenreDto Genre { get; init; }
        public UserDto User { get; init; }
    }
}