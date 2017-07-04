// <copyright file="TestAsyncEnumerable.cs" company="natsnudasoft">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Naming",
        "CA1710",
        Justification = "Name is accurate description of its purpose.")]
    public class TestAsyncEnumerable<TEntity> :
        EnumerableQuery<TEntity>,
        IAsyncEnumerable<TEntity>,
        IQueryable<TEntity>
    {
        public TestAsyncEnumerable(IEnumerable<TEntity> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IQueryProvider Provider => new TestAsyncQueryProvider<TEntity>(this);

        IQueryProvider IQueryable.Provider => this.Provider;

        public IAsyncEnumerator<TEntity> GetEnumerator()
        {
            return new TestAsyncEnumerator<TEntity>(this.AsEnumerable().GetEnumerator());
        }
    }
}
