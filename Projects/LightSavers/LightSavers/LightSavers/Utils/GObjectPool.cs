using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Author: Benjamin Meier (benmeier42@gmail.com)
// Date: 06/10/2013

namespace ObjectPool
{
    /// <summary>
    /// This class is a basic generic object pool. It allows one to use a fixed set of instances in such a way
    /// that they can be reused without triggering Garbage Collection.
    /// 
    /// The idea is that Provision() will return an instance that can be used and references. When it is not 
    /// needed, it can be Disposed() in order to free it up for a future use. 
    /// 
    /// IPoolable is a very simple interface:
    /// '''''''''''''''''''''''''''''''''
    /// public interface IPoolable
    /// {
    ///     int PoolIndex { get; set; }
    /// }
    /// '''''''''''''''''''''''''''''''''
    /// 
    /// Use:
    /// ''''''''''''''''''''''''''''''''''
    /// // construct a pool of 10 instances
    /// ObjectPool<SomeComplexPoolable> op = new GObjectPool<SomeComplexPoolable>(10);
    /// 
    /// // get a new instance and construct it with some values
    /// SomeComplexPoolable A = op.Provide();
    /// A.Construct(value arguments to set on the instance);
    /// 
    /// // and when you don't need it anymore
    /// op.Dispose(A);
    /// 
    /// // this also supports looping through the Object Pool:
    /// int i = op.GetFirst();
    /// while (i > -1)
    /// {
    ///    SomeComplexPoolable T = op.GetByIndex(i);
    ///    System.Diagnostics.Debug.WriteLine(T.value);
    ///    i = op.NextIndex(T);
    /// }
    /// 
    /// '''''''''''''''''''''''''''''''''''
    /// 
    /// Behind the scenes its an array backed linked list thing!
    /// 
    /// WARNING: 
    ///   Still has bugs:
    ///   - disposing from empty pool
    ///   - exceeding capacity
    ///   - random OutOfRange issues
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GObjectPool<T> where T : IPoolable, new()
    {

        // backing array
        private T[] pool;

        // array (stack thing) of available indices
        private int[] availables;

        // arrays of pointers to next and previous items. This gives fast lookups and 
        // avoid storing the pointers on the IPoolable which should make it a bit more
        // hidden/abstract
        private int[] previouses;
        private int[] nexts;

        // the next available pool object
        private int cursor;

        // maximum size
        private int size;

        // pointers to the last and first elements, default = -1
        private int first;
        private int last;

        public GObjectPool(int size)
        {
            // set size, this is required all over the place
            this.size = size;

            // create pool of nulls
            this.pool = new T[size];

            // empty so therefore -1
            this.first = -1;
            this.last = -1;

            // create stack of indices; 0, 1, 2, 3, 4, 5, n
            this.availables = new int[size];
            for (int i = 0; i < size; i++) availables[i] = i;

            //the first available index is obviously the first
            this.cursor = 0;

            //setup next and previous things
            previouses = new int[size];
            nexts = new int[size];
        }

        /// <summary>
        /// Return and Provision a pooled object
        /// </summary>
        /// <returns></returns>
        public T Provide()
        {
            // 1 : cursor is at the next available index, so get it.
            int a = availables[cursor];

            // 2 : increment cursor 
            cursor++;

            // 3 : the object in the pool may never have been created
            if (pool[a] == null)
            {
                pool[a] = new T();
                pool[a].PoolIndex = a;
            }

            // this is the newest item so link it to the previous last 
            nexts[a] = -1;
            previouses[a] = last;

            // IF this is the first item, make it so
            if (first == -1) 
                first = a;
            // otherwise link it to the last one
            else
                nexts[last] = a;

            // NOW this one is the last item
            last = a;

            // 4 : return object
            return pool[a];
        }

        /// <summary>
        /// Free up the specified index so that something else can use it
        /// </summary>
        public void Dispose(int i)
        {
            // out of range
            if (i == -1 || i >= size) return;

            // decrement the cursor
            cursor--;

            // mark this instance as available
            availables[cursor] = i;

            // get links
            int n = nexts[i];
            int p = previouses[i];
            
            // if this is the head, update the head
            if (first == i) first = n;
            // if this is the tail, update the tail
            if (last == i) last = p;

            // if the links exist, update the next and previous links to skip this one
            if (n != -1) previouses[n] = p;
            if (p != -1) nexts[p] = n;

        }

        /// <summary>
        /// Dispose by the pool index
        /// </summary>
        /// <param name="t">The Poolable object to dispose from this pool</param>
        public void Dispose(T t)
        {
            Dispose(t.PoolIndex);
        }

        /// <summary>
        /// Get the index of the first item
        /// </summary>
        public int GetFirst()
        {
            return first;
        }

        /// <summary>
        /// Get the IPoolable object stored at this pool index
        /// </summary>
        public T GetByIndex(int i)
        {
            return pool[i];
        }

        /// <summary>
        /// Return the index of the item that follows the given one.
        /// Order is provision order
        /// </summary>
        /// TODO: range checks
        public int NextIndex(T t)
        {
            return nexts[t.PoolIndex];
        }

        /// <summary>
        /// Return the index of the item that precedes the given one.
        /// Order is provision order
        /// </summary>
        /// TODO: range checks
        public int PreviousIndex(T t)
        {
            return previouses[t.PoolIndex];
        }

    }
}
