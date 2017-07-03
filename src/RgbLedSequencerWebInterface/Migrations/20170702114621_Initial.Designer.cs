using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Natsnudasoft.RgbLedSequencerWebInterface.Data;

namespace Natsnudasoft.RgbLedSequencerWebInterface.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20170702114621_Initial")]
    partial class Initial
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062",
            Justification = "Validated not null.")]
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
