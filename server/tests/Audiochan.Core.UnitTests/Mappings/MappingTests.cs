using System;
using System.Runtime.Serialization;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using Audiochan.Core.Features.Genres.ListGenre;
using Audiochan.Core.Features.Users.GetCurrentUser;
using Audiochan.Core.Features.Users.GetUser;
using AutoMapper;
using Xunit;

namespace Audiochan.Core.UnitTests.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AudioMappingProfile>();
                cfg.AddProfile<GetFollowingsMappingProfile>();
                cfg.AddProfile<GetFollowersMappingProfile>();
                cfg.AddProfile<CurrentUserMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<ListGenreMappingProfile>();
            });
            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(Audio), typeof(AudioViewModel))]
        [InlineData(typeof(FollowedUser), typeof(FollowerViewModel))]
        [InlineData(typeof(FollowedUser), typeof(FollowingViewModel))]
        [InlineData(typeof(User), typeof(CurrentUserViewModel))]
        [InlineData(typeof(User), typeof(UserViewModel))]
        [InlineData(typeof(Genre), typeof(GenreViewModel))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = GetInstanceOf(source);
            _mapper.Map(instance, source, destination);
        }

        private object? GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
                return Activator.CreateInstance(type);

            // Type without parameterless constructor
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}