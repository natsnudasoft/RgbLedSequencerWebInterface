// <copyright file="ColorStringItem.cs" company="natsnudasoft">
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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines a model for a hexadecimal color string.
    /// </summary>
    [Table("ColorString")]
    public class ColorStringItem
    {
        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        [Key]
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the index of this color string in the color string list.
        /// </summary>
        [Required]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the value of this color string.
        /// </summary>
        [Required]
        [StringLength(7)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the id of the <see cref="SequenceStepItem"/> that owns this color string.
        /// </summary>
        public int SequenceStepId { get; set; }

        /// <summary>
        /// Gets or sets the the <see cref="SequenceStepItem"/> that owns this color string.
        /// </summary>
        [ForeignKey(nameof(SequenceStepId))]
        public virtual SequenceStepItem SequenceStep { get; set; }
    }
}
