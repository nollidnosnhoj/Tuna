using Audiochan.Core.Commons.Interfaces;
using AutoMapper;

namespace Audiochan.Core.Commons.Extensions;

public static class MappingExtensions
{
    public static IMappingExpression<TSource, TDestination> CreateStrictMap<TSource, TDestination>(this Profile profile)
        where TDestination : IMapFrom<TSource>
    {
        return profile.CreateMap<TSource, TDestination>();
    }
}