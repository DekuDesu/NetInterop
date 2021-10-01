using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Abstractions.Runtime;
using NetInterop.Transport.Core.Runtime;
using Xunit;

namespace NetInterop.Tests.Runtime
{
    public class WorkerPoolTests
    {
        [Fact]
        public void Test_DefaultWorkerLimit()
        {
            IWorkPool pool = new DefaultWorkPool();

            Assert.Equal(Environment.ProcessorCount, pool.WorkerLimit);
        }

        [Fact]
        public void Test_ShrinkPool_Limit()
        {
            IWorkPool pool = new DefaultWorkPool();

            pool.ShrinkPool(1);

            Assert.Equal(Environment.ProcessorCount - 1, pool.WorkerLimit);

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.ShrinkPool(-1));

            int previous = pool.WorkerLimit;

            pool.ShrinkPool(0);

            Assert.Equal(previous, pool.WorkerLimit);
        }

        [Fact]
        public void Test_ExpandPool_Limit()
        {
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = Environment.ProcessorCount - 1
            };

            pool.ExpandPool(1);

            Assert.Equal(Environment.ProcessorCount, pool.WorkerLimit);

            Assert.Throws<ArgumentOutOfRangeException>(() => pool.ExpandPool(-1));

            int previous = pool.WorkerLimit;

            pool.ExpandPool(0);

            Assert.Equal(previous, pool.WorkerLimit);
        }
        [Fact]
        public void Test_StartWorkers()
        {
            int count = 2;
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = count
            };
            Assert.Equal(count, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);
            Assert.False(pool.PoolStarted);

            pool.StartPool();

            Assert.Equal(count, pool.WorkerCount);

            pool.StopPool();

            Assert.Equal(0, pool.WorkerCount);
        }

        [Fact]
        public void Test_PerformsWork()
        {
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = 1
            };

            int count = 0;

            Barrier barrier = new(2);

            IWork work = new TestWork(() =>
            {
                barrier.SignalAndWait();
                count++;
            });

            pool.StartPool();

            Assert.Equal(0, pool.WaitingWork);

            pool.AddWork(work);

            Assert.Equal(1, pool.WaitingWork);

            Assert.Equal(0, count);

            barrier.SignalAndWait();

            Assert.Equal(0, pool.WaitingWork);

            Assert.Equal(1, count);

            pool.StopPool();

            Assert.Equal(0, pool.WorkerCount);

            Assert.False(pool.PoolStarted);
        }

        [Fact]
        public void Test_Race_Start()
        {
            int count = Environment.ProcessorCount;
            // when we start the pool we should never exceed WorkerLimit even if we call start multiple times
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = Environment.ProcessorCount
            };

            Assert.Equal(count, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);

            pool.StartPool();

            Assert.Equal(count, pool.WorkerCount);

            pool.StopPool();

            Assert.Equal(0, pool.WorkerCount);

            // begin race condition testing
            int taskCount = 4;

            Barrier barrier = new(taskCount);

            void Worker()
            {
                barrier.SignalAndWait();
                pool.StartPool();
            }


            List<Task> tasks = new List<Task>();

            for (int i = 0; i < taskCount; i++)
            {
                tasks.Add(Task.Run(Worker));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.Equal(count, pool.WorkerCount);
        }

        [Fact]
        public void Test_Expand()
        {
            // when we start the pool we should never exceed WorkerLimit even if we call start multiple times
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = 2
            };

            Assert.Equal(2, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);

            pool.StartPool();

            Assert.Equal(2, pool.WorkerCount);

            pool.ExpandPool(1);
            Assert.Equal(3, pool.WorkerLimit);
            Assert.Equal(3, pool.WorkerCount);

            pool.StopPool();

            Assert.Equal(3, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);
        }

        [Fact]
        public void Test_Shrink()
        {
            // when we start the pool we should never exceed WorkerLimit even if we call start multiple times
            IWorkPool pool = new DefaultWorkPool()
            {
                WorkerLimit = 3
            };

            Assert.Equal(3, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);

            pool.StartPool();

            Assert.Equal(3, pool.WorkerCount);

            pool.ShrinkPool(1);
            Assert.Equal(2, pool.WorkerLimit);
            Assert.Equal(2, pool.WorkerCount);

            pool.StopPool();

            Assert.Equal(2, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);
        }

        private class TestWork : IWork
        {
            private readonly Action work;

            public TestWork(Action work)
            {
                this.work = work;
            }

            public void PerformWork(CancellationToken token)
            {
                work?.Invoke();
            }
        }
    }
}
