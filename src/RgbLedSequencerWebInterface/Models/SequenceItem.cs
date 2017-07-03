// <copyright file="SequenceItem.cs" company="natsnudasoft">
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines a model for a queued sequence.
    /// </summary>
    [Table("Sequence")]
    public class SequenceItem
    {
        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        [Key]
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the time that this sequence was enqueued.
        /// </summary>
        [Required]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string SequenceName { get; set; }

        /// <summary>
        /// Gets or sets the index to store this sequence at on the RGB LED Sequencer.
        /// </summary>
        [Required]
        public byte SequenceIndex { get; set; }

        /// <summary>
        /// Gets or sets the collection of steps in this sequence.
        /// </summary>
        [Required]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2227",
            Justification = "Set required by Entity Framework.")]
        public virtual ICollection<SequenceStepItem> Steps { get; set; }
    }
}
