using Audiochan.API.Features.Audios.Mappings;
using Audiochan.API.Features.Auth.Mappings;
using Audiochan.API.Features.Users.Mappings;
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
                cfg.AddProfile<AudioDtoMapping>();
                cfg.AddProfile<CurrentUserDtoMapping>();
                cfg.AddProfile<ProfileDtoMapping>();
                cfg.AddProfile<UserDtoMapping>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }
    }
}