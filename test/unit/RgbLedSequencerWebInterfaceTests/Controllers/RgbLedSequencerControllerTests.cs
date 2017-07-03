// <copyright file="RgbLedSequencerControllerTests.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterfaceTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Natsnudasoft.NatsnudaLibrary.TestExtensions;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models;
    using Natsnudasoft.RgbLedSequencerWebInterface.Models.RgbLedSequencerViewModels;
    using Natsnudasoft.RgbLedSequencerWebInterface.Services;
    using Ploeh.AutoFixture.Idioms;
    using Ploeh.AutoFixture.Xunit2;
    using Xunit;
    using SutAlias = RgbLedSequencerWebInterface.Controllers.RgbLedSequencerController;

    public class RgbLedSequencerControllerTests
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
        public void IndexReturnsAViewResult(
            [Frozen]Mock<IRgbLedSequencerService> rgbLedSequencerServiceMock)
        {
            IActionResult actual;
            using (var sut = new SutAlias(rgbLedSequencerServiceMock.Object))
            {
                actual = sut.Index();
            }

            Assert.IsType<ViewResult>(actual);
        }

        [Theory]
        [AutoMoqData]
        public async Task EnqueueSequenceAsyncWithValidModelReturnsStatusCode(
            [Frozen]Mock<IRgbLedSequencerService> rgbLedSequencerServiceMock)
        {
            IActionResult actual;
            var sequenceItem = new SequenceItem();
            using (var sut = new SutAlias(rgbLedSequencerServiceMock.Object))
            {
                actual = await sut.EnqueueSequenceAsync(sequenceItem);
            }

            rgbLedSequencerServiceMock.Verify(
                r => r.EnqueueSequenceAsync(sequenceItem),
                Times.Once());
            var statusResult = Assert.IsType<StatusCodeResult>(actual);
            Assert.Equal(201, statusResult.StatusCode);
        }

        [Theory]
        [AutoMoqData]
        public async Task EnqueueSequenceAsyncWithInvalidModelReturnsBadRequest(
            [Frozen]Mock<IRgbLedSequencerService> rgbLedSequencerServiceMock)
        {
            IActionResult actual;
            var sequenceItem = new SequenceItem();
            using (var sut = new SutAlias(rgbLedSequencerServiceMock.Object))
            {
                sut.ModelState.AddModelError("Id", "Required");

                actual = await sut.EnqueueSequenceAsync(sequenceItem);
            }

            rgbLedSequencerServiceMock.Verify(
                r => r.EnqueueSequenceAsync(It.IsAny<SequenceItem>()),
                Times.Never());
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.Equal(400, badRequestObjectResult.StatusCode);
        }

        [Theory]
        [AutoMoqData]
        public async Task SequenceQueueAsyncReturnsJsonResultWithSequenceQueue(
            [Frozen]Mock<IRgbLedSequencerService> rgbLedSequencerServiceMock)
        {
            const int PageNumber = 2;
            const int ResultsPerPage = 10;
            IActionResult actual;
            var expected = CreateSequenceQueueViewModel(PageNumber);
            rgbLedSequencerServiceMock
                .Setup(r => r.GetSequenceQueueAsync(PageNumber, ResultsPerPage))
                .Returns(Task.FromResult(expected));
            using (var sut = new SutAlias(rgbLedSequencerServiceMock.Object))
            {
                actual = await sut.SequenceQueueAsync(PageNumber, ResultsPerPage);
            }

            var jsonResult = Assert.IsType<JsonResult>(actual);
            dynamic jsonValue = jsonResult.Value;
            Assert.Equal(expected.PageNumber, jsonValue.PageNumber);
            Assert.Equal(expected.TotalItemCount, jsonValue.TotalItemCount);
            var pageSequenceQueue = ((IEnumerable<dynamic>)jsonValue.PageSequenceQueue).ToList();
            Assert.Equal(expected.PageSequenceQueue.Count, pageSequenceQueue.Count);
            for (int i = 0; i < expected.PageSequenceQueue.Count; ++i)
            {
                Assert.Equal(
                    expected.PageSequenceQueue[i].SequenceIndex,
                    pageSequenceQueue[i].SequenceIndex);
                Assert.Equal(
                    expected.PageSequenceQueue[i].SequenceName,
                    pageSequenceQueue[i].SequenceName);
            }
        }

        [Theory]
        [AutoMoqData]
        public async Task SendNextSequenceAsyncReturnsStatusCode(
            [Frozen]Mock<IRgbLedSequencerService> rgbLedSequencerServiceMock)
        {
            IActionResult actual;
            using (var sut = new SutAlias(rgbLedSequencerServiceMock.Object))
            {
                actual = await sut.SendNextSequenceAsync();
            }

            rgbLedSequencerServiceMock.Verify(r => r.SendNextSequenceAsync(), Times.Once());
            var statusResult = Assert.IsType<StatusCodeResult>(actual);
            Assert.Equal(200, statusResult.StatusCode);
        }

        private static SequenceQueueViewModel CreateSequenceQueueViewModel(int pageNumber)
        {
            return new SequenceQueueViewModel
            {
                PageNumber = pageNumber,
                TotalItemCount = 2,
                PageSequenceQueue = new[]
                {
                    new SequenceItem
                    {
                        SequenceIndex = 1,
                        SequenceName = "Sequence One"
                    },
                    new SequenceItem
                    {
                        SequenceIndex = 7,
                        SequenceName = "Sequence Two"
                    }
                }
            };
        }
    }
}
