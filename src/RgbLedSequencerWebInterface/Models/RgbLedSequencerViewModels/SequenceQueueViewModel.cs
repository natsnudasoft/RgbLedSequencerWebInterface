// <copyright file="SequenceQueueViewModel.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface.Models.RgbLedSequencerViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a view model for a paged list of sequences from the sequence queue.
    /// </summary>
    public sealed class SequenceQueueViewModel
    {
        /// <summary>
        /// Gets or sets the page number of the sequence list.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of sequences in the sequence queue.
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Gets or sets the list of sequences on the page.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2227",
            Justification = "Set required by Entity Framework.")]
        public IList<SequenceItem> PageSequenceQueue { get; set; }
    }
}
