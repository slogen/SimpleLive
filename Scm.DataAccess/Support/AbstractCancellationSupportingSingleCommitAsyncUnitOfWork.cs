﻿using System.Threading;

namespace Scm.DataAccess.Support
{
    public abstract class
        AbstractCancellationSupportingSingleCommitAsyncUnitOfWork : AbstractSingleCommitAsyncUnitOfWork
    {
        private readonly CancellationTokenSource _disposed = new CancellationTokenSource();
        protected CancellationToken DisposeToken => _disposed.Token;

        protected override void Dispose(bool disposing)
        {
            if (_disposed?.IsCancellationRequested ?? true)
                return;
            _disposed.Cancel();
            if (disposing)
                _disposed.Dispose();
        }
    }
}