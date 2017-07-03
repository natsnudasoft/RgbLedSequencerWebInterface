// <copyright file="TestDbContextBuilder.cs" company="natsnudasoft">
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

namespace Natsnudasoft.RgbLedSequencerWebInterfaceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Moq;

    public sealed class TestDbContextBuilder<T>
        where T : DbContext
    {
        private readonly Mock<T> dbContextMock;

        public TestDbContextBuilder()
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseInMemoryDatabase();
            this.dbContextMock = new Mock<T>(optionsBuilder.Options);
        }

        public Mock<T> Build()
        {
            return this.dbContextMock;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000",
            Justification = "Disposable members are exposed.")]
        public TestDbContextBuilder<T> WithDbSetData<TEntity>(
            Expression<Func<T, DbSet<TEntity>>> dbSetExpression,
            IQueryable<TEntity> data)
            where TEntity : class
        {
            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetEnumerator())
#pragma warning disable CC0022 // Should dispose object
                    .Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));
#pragma warning restore CC0022 // Should dispose object
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<TEntity>(data.Provider));
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.GetEnumerator());
            this.dbContextMock.Setup(dbSetExpression).Returns(mockSet.Object);

            return this;
        }
    }
}
