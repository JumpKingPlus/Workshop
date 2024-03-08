using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates
{
    public class ConcurrentDropoutQueue<T> : ConcurrentQueue<T>
    {
        public ConcurrentDropoutQueue(int capacity, params T[] values) : base(values)
        {
            Capacity = capacity;
        }

        public int Capacity { get; }

        public new void Enqueue(T item)
        {
            while (Count >= Capacity)
            {
                base.TryDequeue(out T _);
            }

            base.Enqueue(item);
        }

        public new bool TryDequeue(out T item)
        {
            // keep always one fella in the list
            if (Count <= 1)
            {
                TryPeek(out item);
                return false;
            }

            base.TryDequeue(out item);
            return true;
        }
    }
}
