using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemoteInvoke.Net;
using RemoteInvoke;
using RemoteInvoke.Net.Server;
using System.Threading;

namespace RemoteInvokeTests.Server
{
    public class IServerTests
    {
        private static readonly IClientProvider<DummyClient> s_clientProvider = new DummyProvider();

        [Fact]
        public void Test_Start()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            Assert.False(server.IsStarted);

            server.Start();

            Assert.True(server.IsStarted);
        }

        [Fact]
        public void Test_AcceptClient()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            Assert.NotNull(server.AcceptClient());
        }

        [Fact]
        public void Test_ConnectedClientsCount()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            server.AcceptClient();

            Assert.Single(server.ConnectedClients);
        }

        [Fact]
        public void Test_ConnectedClientsMatches()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            var client = server.AcceptClient();

            Assert.Equal(client, server.ConnectedClients.FirstOrDefault());
        }

        [Fact]
        public void Test_Port()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            Assert.Equal(-1, server.Port);
        }

        [Fact]
        public void Test_PortChange()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            Assert.Equal(-1, server.Port);

            server.SetPort(2);

            Assert.Equal(2, server.Port);
        }

        [Fact]
        public void Test_ServerStartedException()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);
            Assert.Throws<InvalidOperationException>(() =>
            {
                server.AcceptClient();
            });
            Assert.Throws<InvalidOperationException>(() =>
            {
                server.BeginAcceptingClients(null, CancellationToken.None);
            });
        }

        [Fact]
        public void Test_AcceptMultipleClients()
        {
            var provider = new DummyProvider();

            var server = new DefaultServer<DummyClient>(-1, provider);

            server.Start();

            Assert.Empty(server.ConnectedClients);

            using CancellationTokenSource tokenSource = new();

            int expected = 10;

            int current = 0;

            void ClientDispatcher(DummyClient client)
            {
                if (++current >= expected)
                {
                    tokenSource.Cancel();
                }
            }

            Task task = Task.Run(() => server.BeginAcceptingClients(ClientDispatcher, tokenSource.Token));

            // the polling rate is 10ms so expect 100-200ms for this test
            task.Wait(200);

            tokenSource.Cancel();

            Assert.Equal(expected, server.ConnectedClients.Count);
        }

        [Fact]
        public void Test_Cancel()
        {
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            Barrier barrier = new(2);

            using CancellationTokenSource tokenSource = new();

            void ClientDispatcher(DummyClient client)
            {
                if (barrier.SignalAndWait(100) is false)
                {
                    throw new OperationCanceledException("Failed to get barrier");
                }
            }

            Task task = Task.Run(() =>
            {
                server.BeginAcceptingClients(ClientDispatcher, tokenSource.Token);
            });

            try
            {
                if (barrier.SignalAndWait(100) is false)
                {
                    throw new OperationCanceledException("Failed to get barrier");
                }

                Assert.True(server.IsAcceptingClients);

                server.Cancel();

                Assert.False(server.IsAcceptingClients);
            }
            finally
            {
                tokenSource.Cancel();
                task.Wait();
            }
        }

        [Fact]
        public void Test_CloseClients()
        {

            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            List<DummyClient> clients = new()
            {
                server.AcceptClient(),
                server.AcceptClient(),
                server.AcceptClient(),
                server.AcceptClient(),
                server.AcceptClient(),
            };

            foreach (var item in clients)
            {
                Assert.False(item.IsDisposed);
            }

            server.CloseConnections();

            foreach (var item in clients)
            {
                Assert.True(item.IsDisposed);
            }
        }

        [Fact]
        public void Test_BreakingChangeRaceCondition()
        {
            // we are ensuring that if we make a breaking change everything stops gracefully
            var server = new DefaultServer<DummyClient>(-1, s_clientProvider);

            server.Start();

            Barrier barrier = new(2);

            using CancellationTokenSource tokenSource = new();

            void ClientDispatcher(DummyClient client)
            {
                if (barrier.SignalAndWait(100) is false)
                {
                    throw new OperationCanceledException("Failed to get barrier");
                }
            }

            Task task = Task.Run(() =>
            {
                server.BeginAcceptingClients(ClientDispatcher, tokenSource.Token);
            });

            // we should try to change the port(a breaking change since it requires initilizing a new TCPserver) 
            // at the same time we should accept new clients
            try
            {
                if (barrier.SignalAndWait(100) is false)
                {
                    throw new OperationCanceledException("Failed to get barrier");
                }

                // we should expect it to be on and activey looping
                Assert.True(server.IsStarted);
                Assert.True(server.IsAcceptingClients);

                // when we change the port it should stop the loop and change it
                server.SetPort(2);

                // make sure it restarted the server
                Assert.True(server.IsStarted);

                // make sure it isnt accepting clients yet
                Assert.False(server.IsAcceptingClients);

                // make sure the port was actually changed
                Assert.Equal(2, server.Port);

                // make sure the loop actually exited and the senitenel didn't just get set to true
                Assert.True(task.Wait(100));
            }
            finally
            {
                tokenSource?.Cancel();
            }
        }

        private class DummyClient : IDisposable
        {
            public bool IsDisposed { get; private set; } = false;
            public void Dispose()
            {
                IsDisposed = true;
            }
        }
        private class DummyProvider : IClientProvider<DummyClient>
        {
            public bool ShouldProvide { get; set; } = true;
            public bool Started { get; private set; }
            public bool ShouldPend { get; set; } = true;
            public DummyClient AcceptClient()
            {
                if (ShouldProvide)
                {
                    return new DummyClient();
                }
                return null;
            }

            public bool Pending()
            {
                return ShouldPend;
            }

            public void SetPort(int port) { }


            public void Start()
            {
                Started = true;
            }

            public void Stop()
            {
                Started = false;
            }
        }
    }
}
