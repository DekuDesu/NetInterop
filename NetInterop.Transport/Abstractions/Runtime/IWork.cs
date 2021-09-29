namespace NetInterop.Transport.Core.Abstractions.Runtime
{
    /// <summary>
    /// Represents some arbitrary work that needs to be completed by a <see cref="IWorkerPool"/>
    /// </summary>
    public interface IWork
    {
        /// <summary>
        /// Performs the work in the context any random thread or task
        /// </summary>
        void PerformWork();
    }
}