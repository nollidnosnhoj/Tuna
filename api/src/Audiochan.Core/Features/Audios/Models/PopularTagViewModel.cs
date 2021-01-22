﻿namespace Audiochan.Core.Features.Audios.Models
{
    /// <summary>
    /// Used to return data about a tag
    /// </summary>
    public class PopularTagViewModel
    {
        public string Tag { get; init; } = null!;
        public int Count { get; init; }
    }
}