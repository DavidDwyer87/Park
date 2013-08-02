using Microsoft.ServiceModel.PollingDuplex.Scalable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace dsService
{
    // This is where this in-memory sample goes synchronous, while a real world implementation would not
    public class DequeueAsyncResult : IAsyncResult
    {
        ManualResetEvent waitHandle = new ManualResetEvent(true);

        public object AsyncState { get; set; }
        public WaitHandle AsyncWaitHandle { get { return this.waitHandle; } }
        public bool CompletedSynchronously { get { return true; } }
        public bool IsCompleted { get { return true; } }

        public Notification Notification { get; set; }
        public MakeConnection Poll { get; set; }

        public DequeueAsyncResult(MakeConnection poll, TimeSpan timeout, SynchronizedQueue<Notification> queue, AsyncCallback callback, object state)
        {
            this.Poll = poll;
            this.AsyncState = state;

            if (queue != null)
            {
                // This is where the scaled-out implementation would make a truly async call to 
                // fetch a notification from a scaled-out store, e.g. Azure Queues
                this.Notification = queue.SynchronizedDequeue(timeout);
            }

            if (callback != null)
            {
                callback(this);
            }
        }
    }
}