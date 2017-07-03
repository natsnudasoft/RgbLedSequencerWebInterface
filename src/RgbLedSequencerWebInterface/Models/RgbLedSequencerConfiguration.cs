// <copyright file="RgbLedSequencerConfiguration.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface.Models
{
    using Natsnudasoft.RgbLedSequencerLibrary;

    /// <summary>
    /// Defines the configuration for the RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="IRgbLedSequencerConfiguration" />
    public sealed class RgbLedSequencerConfiguration : IRgbLedSequencerConfiguration
    {
        /// <inheritdoc/>
        public int MaxDotCorrection { get; set; }

        /// <inheritdoc/>
        public int MaxGrayscale { get; set; }

        /// <inheritdoc/>
        public int MaxStepCount { get; set; }

        /// <inheritdoc/>
        public int MaxStepDelay { get; set; }

        /// <inheritdoc/>
        public int RgbLedCount { get; set; }

        /// <inheritdoc/>
        public int SequenceCount { get; set; }

        /// <inheritdoc/>
        ISerialPortConfiguration IRgbLedSequencerConfiguration.SerialPort => this.SerialPort;

        /// <summary>
        /// Gets or sets the configuration of the serial port used to communicate with the RGB LED
        /// Sequencer.
        /// </summary>
        public SerialPortConfiguration SerialPort { get; set; }
    }
}
