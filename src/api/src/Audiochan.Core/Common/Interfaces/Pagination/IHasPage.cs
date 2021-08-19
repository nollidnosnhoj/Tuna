﻿namespace Audiochan.Core.Common.Interfaces.Pagination
{
    public interface IHasPage
    {
        public int Page { get; init; }
        public int Size { get; init; }
    }
}