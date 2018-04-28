﻿using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Scm.Web;
using SimpleLiveData.App.Presentation.SignalR;
using Xunit;

namespace SimpleLiveData.Tests
{
    public class LiveHubTests : TestSourceBasedTests
    {
        [Fact]
        public async Task ObservingThoughApiWorks()
        {
            TestSource.Prepare(3, 3);
            var builder = new HubConnectionBuilder()
                .WithUrl("http://test/signalr/livedata")
                .WithMessageHandler(_ => Server.CreateHandler())
                .WithConsoleLogger()
                .WithTransport(TransportType.LongPolling);
            var hubConnection = builder.Build();
            await hubConnection.StartAsync();
            var obs = hubConnection.Observe<HubData>("Observe");
            var obsTask = obs
                .Do(x => Debug.WriteLine(x))
                .Timestamp().Take(10).ToList()
                .ToTask(CancellationToken);
            await obs.Ready.FirstAsync(x => x > 0);
            // var obs = TestSource.Data.Observe(x => x.Take(10).Timestamp().ToList()).ToTask(CancellationToken);
            var incoming = TestSource.ProduceData().Take(10).ToList();
            var results = await obsTask;
            results.Select(x => x.Value).Should().BeEquivalentTo(incoming,
                cfg => cfg.Excluding(x => x.Installation).Excluding(x => x.Signal));
            // TODO: Test the timing
        }
    }
}