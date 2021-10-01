using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Abstractions.Runtime
{
    /// <summary>
    /// Object that is responsible for consuming and handling exceptions encountered by <see cref="IWorker"/>s inside of an <see cref="IWorkPool"/>
    /// </summary>
    public interface IWorkExceptionHandler
    {
        /// <summary>
        /// Handles an exception that is encountered inside of a work job
        /// </summary>
        /// <param name="work">The job that caused the exception</param>
        /// <param name="exception">The exception that was encountered</param>
        void Handle(IWork work, Exception exception);
    }
}
