using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Runtime
{
    public class DefaultWorkPool : IWorkerPool
    {
        private readonly IProducerConsumerCollection<IWork> waitingWork = new ConcurrentBag<IWork>();
        private readonly IProducerConsumerCollection<IWorker> workers = new ConcurrentBag<IWorker>();
        private readonly SemaphoreSlim synchronizationLock = new SemaphoreSlim(1, 1);

        public int WaitingWork => waitingWork.Count;

        public bool PoolStarted { get; private set; }

        public int WorkerCount => workers.Count;

        public int WorkerLimit { get; set; } = Environment.ProcessorCount;

        public void AddWork(IWork work)
        {
            while (waitingWork.TryAdd(work) is false) { }
        }

        public void ExpandPool(int count)
        {
            WorkerLimit = Math.Min(WorkerLimit + count, Environment.ProcessorCount);

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"Name: {nameof(count)}, expected positive iteger, got negative. Did you mean to call {nameof(ShrinkPool)} instead?");
            }

            while (PoolStarted && WorkerCount < WorkerLimit)
            {
                InvokeSynchronized(AddWorkerToPoolUnsafe);
            }
        }

        public void ShrinkPool(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException($"Name: {nameof(count)}, expected positive iteger, got negative. Did you mean to call {nameof(ExpandPool)} instead?");
            }
            WorkerLimit = Math.Max(WorkerLimit - count, 0);

            while (PoolStarted && WorkerCount > WorkerLimit)
            {
                InvokeSynchronized(RemoveWorkerFromPoolUnsafe);
            }
        }

        public void StartPool()
        {
            while (WorkerCount < WorkerLimit)
            {
                InvokeSynchronized(AddWorkerToPoolUnsafe);
            }

            // bools are atomic
            PoolStarted = true;
        }

        public void StopPool()
        {
            while (WorkerCount > 0)
            {
                InvokeSynchronized(RemoveWorkerFromPoolUnsafe);
            }

            // bools are atomic
            PoolStarted = false;
        }

        private void AddWorkerToPoolUnsafe()
        {
            if (workers.Count < WorkerLimit)
            {
                while (workers.TryAdd(CreateWorker()) is false) { }
            }
        }

        private void RemoveWorkerFromPoolUnsafe()
        {
            if (workers.Count > 0)
            {
                IWorker worker;

                while (workers.TryTake(out worker) is false) { }

                worker?.Cancel();
            }
        }

        private IWorker CreateWorker()
        {
            return new DefaultWorker(Worker);
        }

        private void Worker(CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                while (waitingWork.Count > 0)
                {
                    if (waitingWork.TryTake(out IWork work))
                    {
                        work.PerformWork();
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

                // could have used a goto: instead here, but this is cleaner although slower
                if (token.IsCancellationRequested)
                {
                    break;
                }

                // wait 1/60th of a second
                Thread.Sleep(1000 / 60);
            }
        }

        /// <summary>
        /// Invokes the provided action using this objects syncrhonization object to perform atomic state-changing operations on this object
        /// </summary>
        /// <param name="expression"></param>
        private void InvokeSynchronized(Action expression)
        {
            synchronizationLock.Wait();
            try
            {
                expression?.Invoke();
            }
            finally
            {
                synchronizationLock.Release();
            }
        }
    }
}
