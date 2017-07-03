// <copyright file="20170702114621_Initial.cs" company="natsnudasoft">
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
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Entity Framework migration.
    /// </summary>
    /// <seealso cref="Migration" />
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Initial : Migration
    {
        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062",
            Justification = "Validated not null.")]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sequence",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SequenceIndex = table.Column<byte>(nullable: false),
                    SequenceName = table.Column<string>(maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequence", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SequenceStep",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SequenceId = table.Column<int>(nullable: false),
                    StepDelay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SequenceStep_Sequence_SequenceId",
                        column: x => x.SequenceId,
                        principalTable: "Sequence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ColorString",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Index = table.Column<int>(nullable: false),
                    SequenceStepId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorString", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorString_SequenceStep_SequenceStepId",
                        column: x => x.SequenceStepId,
                        principalTable: "SequenceStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorString_SequenceStepId",
                table: "ColorString",
                column: "SequenceStepId");

            migrationBuilder.CreateIndex(
                name: "IX_SequenceStep_SequenceId",
                table: "SequenceStep",
                column: "SequenceId");
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1062",
            Justification = "Validated not null.")]
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorString");

            migrationBuilder.DropTable(
                name: "SequenceStep");

            migrationBuilder.DropTable(
                name: "Sequence");
        }
    }
}
