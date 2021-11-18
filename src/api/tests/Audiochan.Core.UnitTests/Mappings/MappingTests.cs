﻿using Audiochan.Core.Common.Mappings;
using AutoMapper;
using NUnit.Framework;

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
                cfg.AddProfile<AutoMapperProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }
    }
}