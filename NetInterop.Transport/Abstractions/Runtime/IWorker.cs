namespace NetInterop.Transport.Core.Abstractions.Runtime
{
    public interface IWorker
    {
        void Cancel();
        void CancelAndWait();
    }
}