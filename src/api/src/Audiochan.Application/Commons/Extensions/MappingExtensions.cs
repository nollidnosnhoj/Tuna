using Audiochan.Application.Commons.Interfaces;
using AutoMapper;

namespace Audiochan.Application.Commons.Extensions;

public static class MappingExtensions
{
    public static IMappingExpression<TSource, TDestination> CreateStrictMap<TSource, TDestination>(this Profile profile)
        where TDestination : IMapFrom<TSource>
    {
        return profile.CreateMap<TSource, TDestination>();
    }
}