// <copyright file="SerialPortConfiguration.cs" company="natsnudasoft">
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
    using System.IO.Ports;
    using Natsnudasoft.RgbLedSequencerLibrary;

    /// <summary>
    /// Defines the configuration for the serial port settings for the RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="ISerialPortConfiguration" />
    public sealed class SerialPortConfiguration : ISerialPortConfiguration
    {
        /// <inheritdoc/>
        public string PortName { get; set; }

        /// <inheritdoc/>
        public int BaudRate { get; set; }

        /// <inheritdoc/>
        public Parity Parity { get; set; }

        /// <inheritdoc/>
        public int DataBits { get; set; }

        /// <inheritdoc/>
        public StopBits StopBits { get; set; }

        /// <inheritdoc/>
        public int ReadTimeout { get; set; }

        /// <inheritdoc/>
        public int WriteTimeout { get; set; }
    }
}
