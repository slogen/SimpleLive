﻿using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DataSys.App.DataModel;
using DataSys.App.Presentation.SignalR;
using DataSys.App.Tests.Support.Trace;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Scm.DataAccess;
using Scm.Rx;
using Scm.Web;
using Xunit;
using Xunit.Abstractions;

namespace DataSys.App.Tests.Test
{
    public class LiveHubTests : StandardClientServer33
    {
        public LiveHubTests(ITestOutputHelper output) {
            Output = output;
        }

        public ITestOutputHelper Output { get; }

        [DataContract]
        public class ChangeData : IChange<Data>
        {
            [DataMember] public Data Entity { get; set; }

            [DataMember] public EntityChange Change { get; set; }

            [JsonIgnore] object IChange.Entity => Entity;
        }

        [Fact]
        public async Task ObservingThoughApiWorks()
        {
            await Prepared.ConfigureAwait(false);
            var builder = new HubConnectionBuilder()
                    .WithUrl($"http://test{LiveHub.Route}",
                        cfg => cfg.HttpMessageHandlerFactory = _ => Server.CreateHandler())
                ;
            var hubConnection = builder.Build();
            await hubConnection.StartAsync();
            var obs = hubConnection.Observe<ChangeData>(nameof(LiveHub.Observe));
            const int takeCount = 10;
            var sem = new SemaphoreSlim(2); // Coordinate progress between produced data and listener

            var obsTask = obs
                .TraceTest(Output)
                .Take(takeCount)
                .Select((v, idx) => new {v, idx})
                // Start a new producer whenever we have received 5 items
                .Do(x =>
                {
                    if (x.idx % 5 == 0) sem.Release();
                })
                .TraceTest(Output)
                .Select(x => x.v)
                .ToListAsync(CancellationToken);

            var pushTask = TestSource.ProduceData(
                    // Wait for sem for each batch of data
                    sem.ObserveRelease().Take(2).Select(_ => DateTime.UtcNow)
                        .TraceTest(Output)
                )
                .TraceTest(Output)
                .SelectMany(x => x)
                .TraceTest(Output)
                .ToListAsync(CancellationToken);

            // All ready, start pusing
            sem.Release();

            var observedChanges = await obsTask.ConfigureAwait(false);
            var observed = observedChanges.Select(c => c.Entity).ToList();
            var pushed = await pushTask.ConfigureAwait(false);

            var expect = pushed.Take(takeCount);
            observed.Should().BeEquivalentTo(expect,
                cfg => cfg.Excluding(x => x.Installation).Excluding(x => x.Signal));
            // Reception should contained interleaved data
            observed.Select(x => x.InstallationId)
                .Should().NotBeAscendingInOrder()
                .And.NotBeDescendingInOrder();
        }
    }
}