// <copyright file="TestAsyncEnumerator.cs" company="natsnudasoft">
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
    using System.Threading;
    using System.Threading.Tasks;

    public class TestAsyncEnumerator<TEntity> : IAsyncEnumerator<TEntity>
    {
        private readonly IEnumerator<TEntity> innerEnumerator;

        public TestAsyncEnumerator(IEnumerator<TEntity> innerEnumerator)
        {
            this.innerEnumerator = innerEnumerator;
        }

        public TEntity Current => this.innerEnumerator.Current;

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.innerEnumerator.MoveNext());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerEnumerator.Dispose();
            }
        }
    }
}
