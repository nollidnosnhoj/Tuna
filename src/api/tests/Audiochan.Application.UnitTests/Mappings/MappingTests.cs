using Audiochan.Application.Features.Audios.Mappings;
using Audiochan.Application.Features.Auth.Mappings;
using Audiochan.Application.Features.Users.Mappings;
using AutoMapper;
using Xunit;

namespace Audiochan.Application.UnitTests.Mappings
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