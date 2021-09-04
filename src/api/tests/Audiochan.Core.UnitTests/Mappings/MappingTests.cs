using System;
using System.Runtime.Serialization;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.GetFollowers;
using Audiochan.Core.Features.Users.GetFollowings;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Domain.Entities;
using AutoMapper;
using Xunit;

namespace Audiochan.Core.UnitTests.Mappings
{
    // https://github.com/jasontaylordev/CleanArchitecture/blob/main/tests/Application.UnitTests/Common/Mappings/MappingTests.cs
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(Audio), typeof(AudioDto))]
        [InlineData(typeof(Playlist), typeof(PlaylistDto))]
        [InlineData(typeof(User), typeof(UserDto))]
        [InlineData(typeof(User), typeof(ProfileDto))]
        [InlineData(typeof(User), typeof(CurrentUserDto))]
        [InlineData(typeof(FollowedUser), typeof(FollowingViewModel))]
        [InlineData(typeof(FollowedUser), typeof(FollowerViewModel))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = GetInstanceOf(source);
            _mapper.Map(instance, source, destination);
        }

        private object? GetInstanceOf(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null 
                ? Activator.CreateInstance(type) 
                : FormatterServices.GetUninitializedObject(type);
        }
    }
}