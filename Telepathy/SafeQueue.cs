// replaces ConcurrentQueue which is not available in .NET 3.5 yet.
using System.Collections.Generic;
using System.Threading;

namespace Telepathy
{
    public class SafeQueue<T>
    {
        Queue<T> queue = new Queue<T>();

        // for statistics. don't call Count and assume that it's the same after the
        // call.
        public int Count
        {
            get
            {
                lock(queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock(queue)
            {
                queue.Enqueue(item);
            }
        }

        // can't check .Count before doing Dequeue because it might change inbetween,
        // so we need a TryDequeue
        public bool TryDequeue(out T result)
        {
            lock(queue)
            {
                result = default(T);
                if (queue.Count > 0)
                {
                    result = queue.Dequeue();
                    return true;
                }
                return false;
            }
        }

        // for when we want to dequeue and remove all of them at once without
        // locking every single TryDequeue.
        public bool TryDequeueAll(out T[] result)
        {
            lock(queue)
            {
                result = queue.ToArray();
                queue.Clear();
                return result.Length > 0;
            }
        }

        public void Clear()
        {
            lock(queue)
            {
                queue.Clear();
            }
        }
    }
}