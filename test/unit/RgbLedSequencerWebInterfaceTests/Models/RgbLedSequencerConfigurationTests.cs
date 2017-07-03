// <copyright file="RgbLedSequencerConfigurationTests.cs" company="natsnudasoft">
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
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Ploeh.AutoFixture.Idioms;
    using Xunit;
    using SutAlias = RgbLedSequencerWebInterface.Models.RgbLedSequencerConfiguration;

    public class RgbLedSequencerConfigurationTests
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
        public void PropertiesCorrectlyWritable(WritablePropertyAssertion assertion)
        {
            assertion.Verify(SutType.GetProperties());
        }

        [Theory]
        [AutoMoqData]
        public void InterfaceSerialPortConfigurationReturnsUnderlyingSerialPortConfiguration(
            SerialPortConfiguration serialPortConfig)
        {
            var sut = new SutAlias
            {
                SerialPort = serialPortConfig
            };

            var interfaceSut = (IRgbLedSequencerConfiguration)sut;
            Assert.Equal(serialPortConfig, interfaceSut.SerialPort);
        }
    }
}
