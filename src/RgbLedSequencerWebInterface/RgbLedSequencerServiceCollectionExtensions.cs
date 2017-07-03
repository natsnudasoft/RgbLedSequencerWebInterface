// <copyright file="RgbLedSequencerServiceCollectionExtensions.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Natsnudasoft.NatsnudaLibrary;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;

    /// <summary>
    /// Provides extension methods for RGB LED Sequencer operations on ASP.NET Startup.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class RgbLedSequencerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the RGB LED Sequencer configuration to the service collection.
        /// </summary>
        /// <param name="services">The services collection to add to.</param>
        /// <param name="rgbLedSequencerConfigSection">The configuration section containing the
        /// configuration for the RGB LED Sequencer.</param>
        /// <returns>A bound instance of the RGB LED Sequencer configuration.</returns>
        public static IRgbLedSequencerConfiguration AddRgbLedSequencerConfiguration(
            this IServiceCollection services,
            IConfiguration rgbLedSequencerConfigSection)
        {
            services.Configure<RgbLedSequencerConfiguration>(rgbLedSequencerConfigSection);
            var rgbLedSequencerConfig = new RgbLedSequencerConfiguration();
            rgbLedSequencerConfigSection.Bind(rgbLedSequencerConfig);
            return rgbLedSequencerConfig;
        }

        /// <summary>
        /// Adds the RGB LED Sequencer items to the service collection.
        /// </summary>
        /// <param name="services">The services collection to add to.</param>
        /// <param name="rgbLedSequencerConfig">The RGB LED Sequencer configuration.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000",
            Justification = "Disposable members are added to the services collection.")]
        public static void AddRgbLedSequencer(
            this IServiceCollection services,
            IRgbLedSequencerConfiguration rgbLedSequencerConfig)
        {
            ParameterValidation.IsNotNull(services, nameof(services));
            ParameterValidation.IsNotNull(rgbLedSequencerConfig, nameof(rgbLedSequencerConfig));

            var serialPortAdapter = new SerialPortAdapter(rgbLedSequencerConfig);
            var picaxeCommandInterface = new PicaxeCommandInterface(serialPortAdapter);
            var progress = new Progress<CommandProgress>();
            var rgbLedSequencer = new RgbLedSequencer(
                rgbLedSequencerConfig,
                picaxeCommandInterface,
                progress);

            services.AddSingleton<ISerialPortAdapter>(serialPortAdapter);
            services.AddSingleton<IPicaxeCommandInterface>(picaxeCommandInterface);
            services.AddSingleton<IRgbLedSequencer>(rgbLedSequencer);
            services.AddSingleton(progress);
            services.AddSingleton<IProgress<CommandProgress>>(progress);
        }
    }
}
