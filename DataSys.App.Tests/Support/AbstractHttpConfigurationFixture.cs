﻿using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace DataSys.App.Tests.Support
{
    public abstract class AbstractHttpConfigurationFixture : IDisposable
    {
        public virtual IWebHostBuilder Builder =>
            _builder ?? (_builder = ConfigureBuilder(new WebHostBuilder()));

        public virtual TestServer Server => _server ?? (_server = MakeServer());
        public virtual HttpClient Client => _client ?? (_client = MakeClient());
        public Guid Id { get; } = Guid.NewGuid();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract IWebHostBuilder ConfigureBuilder(IWebHostBuilder builder);

        protected virtual TestServer MakeServer()
        {
            var srv = new TestServer(Builder);
            return srv;
        }

        protected virtual HttpClient MakeClient()
            => Server.CreateClient();


        ~AbstractHttpConfigurationFixture()
        {
            Dispose(false);
        }

        #region Internal variables

        private IWebHostBuilder _builder;
        private TestServer _server;
        private HttpClient _client;

        private static long _undisposedCount;
        private static readonly ISubject<Unit> MissingDisposeSubject = new Subject<Unit>();
        public static IObservable<Unit> MissingDispose => MissingDisposeSubject.AsObservable();

        private static void NotifyMissingDispose()
        {
            Interlocked.Increment(ref _undisposedCount);
            MissingDisposeSubject.OnNext(Unit.Default);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                NotifyMissingDispose();
            try
            {
                _client?.Dispose();
            }
            finally
            {
                _server?.Dispose();
            }
        }

        #endregion
    }
}