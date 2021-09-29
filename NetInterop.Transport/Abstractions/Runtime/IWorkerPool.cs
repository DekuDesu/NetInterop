using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Abstractions.Runtime
{
    /// <summary>
    /// Represents a group of threads, tasks responsible for consuming <see cref="IWork"/> from the pool, processing it,
    /// and waiting on standby.
    /// </summary>
    public interface IWorkerPool
    {
        /// <summary>
        /// The amount of work that is waiting to be performed.
        /// </summary>
        int WaitingWork { get; }
        /// <summary>
        /// The number of workers assigned to the pool
        /// </summary>
        int WorkerCount { get; }
        /// <summary>
        /// The maximum number of workers this pool should create
        /// </summary>
        int WorkerLimit { get; set; }
        /// <summary>
        /// Whether or not this pool has current workers that are working, or waiting for work
        /// </summary>
        bool PoolStarted { get; }

        /// <summary>
        /// Removes n <paramref name="count"/> workers from the pool
        /// </summary>
        /// <param name="count"></param>
        void ShrinkPool(int count);
        /// <summary>
        /// Adds n <paramref name="count"/> or WorkerLimit, whichever is less, workers to the pool
        /// </summary>
        /// <param name="count"></param>
        void ExpandPool(int count);
        /// <summary>
        /// Adds a fire-and-forget task to the pool to be completed.
        /// </summary>
        /// <param name="work"></param>
        void AddWork(IWork work);
        /// <summary>
        /// Starts workers that begin consuming work immediately
        /// </summary>
        void StartPool();
        /// <summary>
        /// Cancels all workers gracefully and releases all managed resources
        /// </summary>
        void StopPool();
    }
}
