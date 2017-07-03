// <copyright file="ApplicationDbContextModelSnapshot.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterface.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Natsnudasoft.RgbLedSequencerWebInterface.Data;

    /// <summary>
    /// Entity Framework Snapshot.
    /// </summary>
    /// <seealso cref="ModelSnapshot" />
    [DbContext(typeof(ApplicationDbContext))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062",
            Justification = "Validated not null.")]
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Natsnudasoft.RgbLedSequencerWebInterface.Models.ColorStringItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Index");

                    b.Property<int>("SequenceStepId");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(7);

                    b.HasKey("Id");

                    b.HasIndex("SequenceStepId");

                    b.ToTable("ColorString");
                });

            modelBuilder.Entity("Natsnudasoft.RgbLedSequencerWebInterface.Models.SequenceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("SequenceIndex");

                    b.Property<string>("SequenceName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTimeOffset>("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("Sequence");
                });

            modelBuilder.Entity("Natsnudasoft.RgbLedSequencerWebInterface.Models.SequenceStepItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("SequenceId");

                    b.Property<int>("StepDelay");

                    b.HasKey("Id");

                    b.HasIndex("SequenceId");

                    b.ToTable("SequenceStep");
                });

            modelBuilder.Entity("Natsnudasoft.RgbLedSequencerWebInterface.Models.ColorStringItem", b =>
                {
                    b.HasOne("Natsnudasoft.RgbLedSequencerWebInterface.Models.SequenceStepItem", "SequenceStep")
                        .WithMany("Colors")
                        .HasForeignKey("SequenceStepId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Natsnudasoft.RgbLedSequencerWebInterface.Models.SequenceStepItem", b =>
                {
                    b.HasOne("Natsnudasoft.RgbLedSequencerWebInterface.Models.SequenceItem", "Sequence")
                        .WithMany("Steps")
                        .HasForeignKey("SequenceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
