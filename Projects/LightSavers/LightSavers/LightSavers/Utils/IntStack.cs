using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils
{
    public class IntStack
    {
        int[] queue;
        int cursor;

        public IntStack(int size)
        {
            queue = new int[size];
            cursor = 0;
        }

        public void Reset()
        {
            cursor = 0;
        }

        public void Push(int v)
        {
            queue[cursor] = v;
            cursor++;
        }

        public int Pop()
        {
            return queue[--cursor];
        }

        public bool IsEmpty()
        {
            return cursor == 0;
        }

    }
}
