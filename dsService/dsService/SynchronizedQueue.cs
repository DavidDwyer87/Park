using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace dsService
{
    public class SynchronizedQueue<T>
    {
        Queue<T> queue = new Queue<T>();
        ManualResetEvent waitHandle = new ManualResetEvent(false);

        public SynchronizedQueue() { }

        public void SynchronizedEnqueue(T item)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(item);
                this.waitHandle.Set();
            }
        }

        public T SynchronizedDequeue(TimeSpan timeout)
        {
            T result = default(T);
            if (this.waitHandle.WaitOne(timeout))
            {
                lock (this.queue)
                {
                    if (this.queue.Count == 1)
                    {
                        result = this.queue.Dequeue();
                        this.waitHandle.Reset();
                    }
                    else if (this.queue.Count > 1)
                    {
                        result = this.queue.Dequeue();
                    }
                }
            }

            return result;
        }
    }
}