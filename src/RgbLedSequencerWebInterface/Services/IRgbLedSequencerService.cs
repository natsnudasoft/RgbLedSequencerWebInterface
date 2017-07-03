// <copyright file="IRgbLedSequencerService.cs" company="natsnudasoft">
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
    using System.Threading.Tasks;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models.RgbLedSequencerViewModels;

    /// <summary>
    /// Provides an interface describing a service for an RGB LED Sequencer.
    /// </summary>
    public interface IRgbLedSequencerService
    {
        /// <summary>
        /// Enqueues the specified sequence to be eventually sent to the RGB LED Sequencer.
        /// </summary>
        /// <param name="sequenceItem">The model describing the sequence to enqueue.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task EnqueueSequenceAsync(SequenceItem sequenceItem);

        /// <summary>
        /// Sends the next sequence in the sequence queue to the RGB LED Sequencer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendNextSequenceAsync();

        /// <summary>
        /// Retrieves a page from the list of enqueued sequences based on the specified page number
        /// and number of results per page.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="resultsPerPage">The number of results that are displayed per page.</param>
        /// <returns>A task representing the asynchronous operation. Upon completion, the result of
        /// the task will contain a <see cref="SequenceQueueViewModel"/> describing the the list of
        /// enqueued sequences for the specified page number.</returns>
        Task<SequenceQueueViewModel> GetSequenceQueueAsync(int pageNumber, int resultsPerPage);
    }
}
