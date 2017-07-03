// <copyright file="SequenceStepItemTests.cs" company="natsnudasoft">
// Copyright (c) Adrian John Dunstan. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace Natsnudasoft.RgbLedSequencerWebInterfaceTests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Xunit;
    using SutAlias = RgbLedSequencerWebInterface.Models.SequenceStepItem;

    public class SequenceStepItemTests
    {
        private static readonly Type SutType = typeof(SutAlias);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void PropertiesCorrectlyWritable(
            Fixture fixture,
            WritablePropertyAssertion assertion)
        {
            fixture.ApplyDefaultModelCustomization();
            assertion.Verify(SutType.GetProperties());
        }

        [Theory]
        [AutoMoqData]
        public void IndexedColorsReturnsColorsSortedByIndex()
        {
            var colors = CreateTestColors();
            var expected = colors.OrderBy(c => c.Index).ToArray();
            var sut = new SutAlias
            {
                Colors = colors
            };

            Assert.Equal(expected, sut.IndexedColors);
        }

        private static ICollection<ColorStringItem> CreateTestColors()
        {
            return new[]
            {
                new ColorStringItem
                {
                    Index = 3,
                    Value = "#FF0000"
                },
                new ColorStringItem
                {
                    Index = 0,
                    Value = "#0000FF"
                },
                new ColorStringItem
                {
                    Index = 2,
                    Value = "#000000"
                },
                new ColorStringItem
                {
                    Index = 1,
                    Value = "#00FF00"
                }
            };
        }
    }
}
