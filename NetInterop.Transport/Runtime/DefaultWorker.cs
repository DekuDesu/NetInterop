using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Abstractions.Runtime;

namespace NetInterop.Transport.Core.Runtime
{
    public class DefaultWorker : IWorker
    {
        private readonly Task task;
        private readonly CancellationTokenSource token;

        public DefaultWorker(Action<CancellationToken> worker)
        {
            this.token = new CancellationTokenSource();
            this.task = Task.Run(() => worker(token.Token), token.Token);
        }

        public void Cancel()
        {
            token.Cancel();
            token.Dispose();
        }

        public void CancelAndWait()
        {
            Cancel();
            Task.WaitAll(task);
        }
    }
}
