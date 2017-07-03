// <copyright file="RgbLedSequencerService.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Natsnudasoft.NatsnudaLibrary;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Data;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models.RgbLedSequencerViewModels;
    using Nito.AsyncEx;

    /// <summary>
    /// Provides a class defining operations relating to an RGB LED Sequencer.
    /// </summary>
    /// <seealso cref="IRgbLedSequencerService" />
    public sealed class RgbLedSequencerService : IRgbLedSequencerService
    {
        private const string CurrentSequenceKey = nameof(CurrentSequenceKey);
        private static readonly AsyncLock RgbLedSequencerLock = new AsyncLock();
        private static readonly TimeSpan RgbLedSequencerTimeout = TimeSpan.FromSeconds(3);

        private readonly ApplicationDbContext dbContext;
        private readonly IRgbLedSequencer rgbLedSequencer;
        private readonly ISerialPortAdapter serialPortAdapter;
        private readonly IRgbLedSequencerConfiguration rgbLedSequencerConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencerService"/> class.
        /// </summary>
        /// <param name="dbContext">The database context to use for database operations.</param>
        /// <param name="rgbLedSequencer">The RGB led sequencer to communicate with.</param>
        /// <param name="serialPortAdapter">The serial port used by the RGB LED Sequencer.</param>
        /// <param name="rgbLedSequencerOptions">The RGB LED Sequencer options.</param>
        public RgbLedSequencerService(
            ApplicationDbContext dbContext,
            IRgbLedSequencer rgbLedSequencer,
            ISerialPortAdapter serialPortAdapter,
            IOptions<RgbLedSequencerConfiguration> rgbLedSequencerOptions)
        {
            ParameterValidation.IsNotNull(dbContext, nameof(dbContext));
            ParameterValidation.IsNotNull(rgbLedSequencer, nameof(rgbLedSequencer));
            ParameterValidation.IsNotNull(serialPortAdapter, nameof(serialPortAdapter));
            ParameterValidation.IsNotNull(rgbLedSequencerOptions, nameof(rgbLedSequencerOptions));

            this.dbContext = dbContext;
            this.rgbLedSequencer = rgbLedSequencer;
            this.serialPortAdapter = serialPortAdapter;
            this.rgbLedSequencerConfig = rgbLedSequencerOptions.Value;
        }

        /// <inheritdoc/>
        public async Task EnqueueSequenceAsync(SequenceItem sequenceItem)
        {
            ParameterValidation.IsNotNull(sequenceItem, nameof(sequenceItem));

            this.dbContext.Sequences.Add(sequenceItem);
            await this.dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task SendNextSequenceAsync()
        {
            try
            {
                using (var cts = new CancellationTokenSource(RgbLedSequencerTimeout))
                using (await RgbLedSequencerLock.LockAsync(cts.Token))
                {
                    var sequenceItem = await this.dbContext.Sequences
                        .OrderBy(s => s.Timestamp)
                        .Include(s => s.Steps)
                            .ThenInclude(st => st.Colors)
                        .FirstOrDefaultAsync();
                    if (sequenceItem != null)
                    {
                        var sequenceData = this.CreateSeqenceData(sequenceItem.Steps);
                        if (!this.serialPortAdapter.IsOpen)
                        {
                            this.serialPortAdapter.Open();
                        }

                        await this.rgbLedSequencer
                            .SaveSequenceAsync(sequenceItem.SequenceIndex, sequenceData);
                        this.dbContext.Sequences.Remove(sequenceItem);
                        await this.dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException("The RGB LED Sequencer is busy.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<SequenceQueueViewModel> GetSequenceQueueAsync(
            int pageNumber,
            int resultsPerPage)
        {
            var itemCount = await this.dbContext.Sequences.CountAsync();
            var pageSequences = await this.dbContext.Sequences
                .OrderBy(s => s.Timestamp)
                .Skip((pageNumber - 1) * resultsPerPage)
                .Take(resultsPerPage)
                .ToListAsync();

            return new SequenceQueueViewModel
            {
                PageNumber = pageNumber,
                TotalItemCount = itemCount,
                PageSequenceQueue = pageSequences
            };
        }

        private SequenceData CreateSeqenceData(IEnumerable<SequenceStepItem> sequenceStepItems)
        {
            var sequenceSteps = new List<SequenceStep>();
            foreach (var sequenceStepItem in sequenceStepItems)
            {
                var ledGrayscales = new List<LedGrayscale>();
                foreach (var colorString in sequenceStepItem.IndexedColors)
                {
                    var color = ColorTranslator.FromHtml(colorString.Value);
                    var ledGrayscale = new LedGrayscale(
                        this.rgbLedSequencerConfig,
                        color.R,
                        color.G,
                        color.B);
                    ledGrayscales.Add(ledGrayscale);
                }

                var grayscaleData = new GrayscaleData(this.rgbLedSequencerConfig, ledGrayscales);
                var sequenceStep = new SequenceStep(
                    this.rgbLedSequencerConfig,
                    grayscaleData,
                    sequenceStepItem.StepDelay);
                sequenceSteps.Add(sequenceStep);
            }

            return new SequenceData(this.rgbLedSequencerConfig, sequenceSteps);
        }
    }
}
