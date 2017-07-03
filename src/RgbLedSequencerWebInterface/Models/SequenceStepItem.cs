// <copyright file="SequenceStepItem.cs" company="natsnudasoft">
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    /// <summary>
    /// Defines a model for a step in a sequence.
    /// </summary>
    [Table("SequenceStep")]
    public class SequenceStepItem
    {
        private ICollection<ColorStringItem> colors;

        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        [Key]
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the unordered collection of colors representing the RGB LEDs in this step.
        /// </summary>
        [Required]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2227",
            Justification = "Set required by Entity Framework.")]
        public virtual ICollection<ColorStringItem> Colors
        {
            get => this.colors;
            set
            {
                this.colors = value;
                this.IndexedColors = this.colors.OrderBy(c => c.Index).ToArray();
            }
        }

        /// <summary>
        /// Gets the ordered collection of colors representing the RGB LEDs in this step in.
        /// </summary>
        [NotMapped]
        public IEnumerable<ColorStringItem> IndexedColors { get; private set; }

        /// <summary>
        /// Gets or sets the step delay of this step.
        /// </summary>
        [Required]
        public int StepDelay { get; set; }

        /// <summary>
        /// Gets or sets the id of the sequence that owns this step.
        /// </summary>
        public int SequenceId { get; set; }

        /// <summary>
        /// Gets or sets the sequence that owns this step.
        /// </summary>
        [ForeignKey(nameof(SequenceId))]
        public virtual SequenceItem Sequence { get; set; }
    }
}
