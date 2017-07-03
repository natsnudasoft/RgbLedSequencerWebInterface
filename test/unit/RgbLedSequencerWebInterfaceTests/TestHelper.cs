// <copyright file="TestHelper.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterfaceTests
{
    using System.Drawing;
    using System.IO.Ports;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using Moq;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Ploeh.AutoFixture;
    using static System.FormattableString;

    public static class TestHelper
    {
        public static void ApplyDefaultModelCustomization(this IFixture fixture)
        {
            var optionsMock = new Mock<IOptions<RgbLedSequencerConfiguration>>();
            var rgbLedSequencerConfig = new RgbLedSequencerConfiguration
            {
                MaxDotCorrection = 63,
                MaxGrayscale = 255,
                MaxStepCount = 770,
                MaxStepDelay = 65535,
                RgbLedCount = 5,
                SequenceCount = 10,
                SerialPort = new SerialPortConfiguration
                {
                    PortName = "COM1",
                    BaudRate = 38400,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    ReadTimeout = 3000,
                    WriteTimeout = 3000
                }
            };
            optionsMock.Setup(m => m.Value).Returns(rgbLedSequencerConfig);
            fixture.Inject<IRgbLedSequencerConfiguration>(rgbLedSequencerConfig);
            fixture.Inject(optionsMock.Object);

            fixture.Customize<ColorStringItem>(c => c
                .With(co => co.Value, ColorToHexString(fixture.Create<Color>()))
                .Without(o => o.SequenceStep));
            fixture.Customize<SequenceStepItem>(c => c
                .With(
                    o => o.Colors,
                    fixture.CreateMany<ColorStringItem>(rgbLedSequencerConfig.RgbLedCount).ToArray())
                .With(o => o.StepDelay, 100)
                .Without(o => o.Sequence));
            fixture.Customize<SequenceItem>(c => c
                .With(
                    o => o.Steps,
                    fixture.CreateMany<SequenceStepItem>(rgbLedSequencerConfig.MaxStepCount).ToArray()));
        }

        private static string ColorToHexString(Color color)
        {
            return Invariant($"#{color.R:X2}{color.G:X2}{color.B:X2}");
        }
    }
}
