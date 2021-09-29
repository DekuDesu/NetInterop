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
            IWorkerPool pool = new DefaultWorkPool();

            Assert.Equal(Environment.ProcessorCount,pool.WorkerLimit);
        }

        [Fact]
        public void Test_ShrinkPool_Limit()
        {
            IWorkerPool pool = new DefaultWorkPool();

            pool.ShrinkPool(1);

            Assert.Equal(Environment.ProcessorCount-1,pool.WorkerLimit);

            Assert.Throws<ArgumentOutOfRangeException>(()=>pool.ShrinkPool(-1));

            int previous = pool.WorkerLimit;

            pool.ShrinkPool(0);

            Assert.Equal(previous, pool.WorkerLimit);
        }

        [Fact]
        public void Test_ExpandPool_Limit()
        {
            IWorkerPool pool = new DefaultWorkPool() { 
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
            IWorkerPool pool = new DefaultWorkPool()
            {
                WorkerLimit = count
            };
            Assert.Equal(count, pool.WorkerLimit);
            Assert.Equal(0, pool.WorkerCount);
            Assert.False(pool.PoolStarted);

            pool.StartPool();

            Assert.Equal(count,pool.WorkerCount);

            pool.StopPool();

            Assert.Equal(0, pool.WorkerCount);
        }

        [Fact]
        public void Test_PerformsWork()
        {
            IWorkerPool pool = new DefaultWorkPool()
            {
                WorkerLimit = 1
            };

            int count = 0;

            Barrier barrier = new(2);

            IWork work = new TestWork(()=> {
                barrier.SignalAndWait();
                count++;
            });

            pool.StartPool();

            Assert.Equal(0, pool.WaitingWork);

            pool.AddWork(work);

            Assert.Equal(1, pool.WaitingWork);

            Assert.Equal(0,count);

            barrier.SignalAndWait();

            Assert.Equal(0,pool.WaitingWork);

            Assert.Equal(1,count);

            pool.StopPool();

            Assert.Equal(0,pool.WorkerCount);

            Assert.False(pool.PoolStarted);
        }
        private class TestWork : IWork
        {
            private readonly Action work;

            public TestWork(Action work)
            {
                this.work = work;
            }

            public void PerformWork()
            {
                work?.Invoke();
            }
        }
    }
}
