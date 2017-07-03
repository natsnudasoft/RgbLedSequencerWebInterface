// <copyright file="RgbLedSequencerServiceTests.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterfaceTests.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerLibrary;
    using Natsnudasoft.RgbLedSequencerWebInterface.Data;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using Xunit;
    using SutAlias = RgbLedSequencerWebInterface.Services.RgbLedSequencerService;

    public class RgbLedSequencerServiceTests
    {
        private static readonly Type SutType = typeof(SutAlias);

        [Theory]
        [AutoMoqData]
        public void ConstructorHasCorrectGuardClauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public void ConstructorDoesNotThrow(DoesNotThrowAssertion assertion)
        {
            assertion.Verify(SutType.GetConstructors());
        }

        [Theory]
        [AutoMoqData]
        public async Task EnqueueSequenceAsyncAddsItem(Fixture fixture)
        {
            var sequences = new SequenceItem[0];
            var dbContextMock = new TestDbContextBuilder<ApplicationDbContext>()
                .WithDbSetData(d => d.Sequences, sequences.AsQueryable())
                .Build();
            fixture.Inject(dbContextMock.Object);
            fixture.ApplyDefaultModelCustomization();
            var sequenceItem = fixture.Create<SequenceItem>();
            var sut = fixture.Create<SutAlias>();

            await sut.EnqueueSequenceAsync(sequenceItem);

            var sequencesMock = Mock.Get(dbContextMock.Object.Sequences);
            sequencesMock.Verify(m => m.Add(sequenceItem), Times.Once());
            dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task SendNextSequenceAsyncWithNoSequencesInQueueDoesNothing(
            Fixture fixture,
            [Frozen]Mock<IRgbLedSequencer> rgbLedSequencerMock)
        {
            fixture.ApplyDefaultModelCustomization();
            var sequences = new SequenceItem[0];
            var dbContextMock = new TestDbContextBuilder<ApplicationDbContext>()
                .WithDbSetData(d => d.Sequences, sequences.AsQueryable())
                .Build();
            fixture.Inject(dbContextMock.Object);
            var sut = fixture.Create<SutAlias>();

            await sut.SendNextSequenceAsync();

            rgbLedSequencerMock.Verify(
                m => m.SaveSequenceAsync(It.IsAny<byte>(), It.IsAny<SequenceData>()),
                Times.Never());
            var sequencesMock = Mock.Get(dbContextMock.Object.Sequences);
            sequencesMock.Verify(m => m.Remove(It.IsAny<SequenceItem>()), Times.Never());
            dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Theory]
        [AutoMoqData]
        public async Task SendNextSequenceAsyncSavesSequenceAndUpdatesDatabase(
            Fixture fixture,
            [Frozen]Mock<IRgbLedSequencer> rgbLedSequencerMock)
        {
            const int SequenceCount = 5;
            fixture.ApplyDefaultModelCustomization();
            var sequences = fixture.CreateMany<SequenceItem>(SequenceCount).ToArray();
            var dbContextMock = new TestDbContextBuilder<ApplicationDbContext>()
                .WithDbSetData(d => d.Sequences, sequences.AsQueryable())
                .Build();
            fixture.Inject(dbContextMock.Object);
            var sut = fixture.Create<SutAlias>();

            await sut.SendNextSequenceAsync();

            rgbLedSequencerMock.Verify(
                m => m.SaveSequenceAsync(It.IsAny<byte>(), It.IsAny<SequenceData>()),
                Times.Once());
            var sequencesMock = Mock.Get(dbContextMock.Object.Sequences);
            sequencesMock.Verify(m => m.Remove(It.IsAny<SequenceItem>()), Times.Once());
            dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Theory]
        [AutoMoqData]
        public async Task GetSequenceQueueAsyncGetsSequenceQueue(Fixture fixture)
        {
            const int PageNumber = 2;
            const int ResultsPerPage = 5;
            const int SequenceCount = 26;
            fixture.ApplyDefaultModelCustomization();
            var sequences = fixture.CreateMany<SequenceItem>(SequenceCount).ToArray();
            var expected = sequences
                .OrderBy(s => s.Timestamp)
                .Skip((PageNumber - 1) * ResultsPerPage)
                .Take(ResultsPerPage)
                .ToArray();
            var dbContextMock = new TestDbContextBuilder<ApplicationDbContext>()
                .WithDbSetData(d => d.Sequences, sequences.AsQueryable())
                .Build();
            fixture.Inject(dbContextMock.Object);
            var sut = fixture.Create<SutAlias>();

            var sequenceQueue = await sut.GetSequenceQueueAsync(PageNumber, ResultsPerPage);

            Assert.Equal(PageNumber, sequenceQueue.PageNumber);
            Assert.Equal(SequenceCount, sequenceQueue.TotalItemCount);
            Assert.Equal(expected, sequenceQueue.PageSequenceQueue);
        }
    }
}
