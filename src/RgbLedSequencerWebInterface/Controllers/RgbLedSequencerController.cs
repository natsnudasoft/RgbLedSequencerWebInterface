// <copyright file="RgbLedSequencerController.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Natsnudasoft.NatsnudaLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Natsnudasoft.RgbLedSequencerWebInterface.Services;

    /// <summary>
    /// Defines a controller for RGB LED Sequencer operations.
    /// </summary>
    /// <seealso cref="Controller" />
    public sealed class RgbLedSequencerController : Controller
    {
        private const int DefaultResultsPerPage = 10;
        private readonly IRgbLedSequencerService rgbLedSequencerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RgbLedSequencerController"/> class.
        /// </summary>
        /// <param name="rgbLedSequencerService">The RGB LED Sequencer service.</param>
        public RgbLedSequencerController(IRgbLedSequencerService rgbLedSequencerService)
        {
            ParameterValidation.IsNotNull(rgbLedSequencerService, nameof(rgbLedSequencerService));

            this.rgbLedSequencerService = rgbLedSequencerService;
        }

        /// <summary>
        /// (GET /RgbLedSequencer/Index)
        /// Gets a view representing the index page.
        /// </summary>
        /// <returns>The index page view.</returns>
        public IActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// (POST /RgbLedSequencer/EnqueueSequence)
        /// Enqueues the specified sequence to be eventually sent to the RGB LED Sequencer.
        /// </summary>
        /// <param name="sequenceItem">The model describing the sequence to enqueue.</param>
        /// <returns>A task representing the asynchronous operation. Upon completion, the result of
        /// the task will contain the result of the enqueue operation.</returns>
        [HttpPost]
        [ActionName("EnqueueSequence")]
        public async Task<IActionResult> EnqueueSequenceAsync(
            [FromBody]SequenceItem sequenceItem)
        {
            if (this.ModelState.IsValid)
            {
                sequenceItem.Timestamp = DateTimeOffset.UtcNow;
                await this.rgbLedSequencerService.EnqueueSequenceAsync(sequenceItem);
                return this.StatusCode(201);
            }

            return this.BadRequest(this.ModelState);
        }

        /// <summary>
        /// (GET /RgbLedSequencer/SequenceQueue)
        /// Retrieves a page from the list of enqueued sequences based on the specified page number
        /// and number of results per page.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="resultsPerPage">The number of results that are displayed per page.</param>
        /// <returns>A task representing the asynchronous operation. Upon completion, the result of
        /// the task will contain the list of enqueued sequences for the specified page number as a
        /// <see cref="JsonResult"/>.</returns>
        [HttpGet]
        [ActionName("SequenceQueue")]
        public async Task<IActionResult> SequenceQueueAsync(
            int pageNumber,
            int resultsPerPage = DefaultResultsPerPage)
        {
            var sequenceQueue = await this.rgbLedSequencerService
                .GetSequenceQueueAsync(pageNumber, resultsPerPage);
            return this.Json(new
            {
                PageNumber = sequenceQueue.PageNumber,
                TotalItemCount = sequenceQueue.TotalItemCount,
                PageSequenceQueue = sequenceQueue.PageSequenceQueue.Select(s => new
                {
                    Id = s.Id,
                    SequenceName = s.SequenceName,
                    SequenceIndex = s.SequenceIndex,
                    UnixTimeMilliseconds = s.Timestamp.ToUnixTimeMilliseconds()
                })
            });
        }

        /// <summary>
        /// (Post /RgbLedSequencer/SendNextSequence)
        /// Sends the next sequence in the sequence queue to the RGB LED Sequencer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. Upon completion, the result of
        /// the task will contain the result of the send next sequence operation.</returns>
        [HttpPost]
        [ActionName("SendNextSequence")]
        public async Task<IActionResult> SendNextSequenceAsync()
        {
            await this.rgbLedSequencerService.SendNextSequenceAsync();
            return this.StatusCode(200);
        }
    }
}
