﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace HttpClientServices.Core.UnitTests.AutoData
{
    public class AutoNSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoNSubstituteDataAttribute() : base(() =>
            new Fixture().Customize(new CompositeCustomization(new AutoNSubstituteCustomization
            {
                ConfigureMembers = true
            })))
        {
        }
    }
}
