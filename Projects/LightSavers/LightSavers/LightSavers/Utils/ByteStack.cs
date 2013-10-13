using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Utils
{
    public class ByteStack
    {
        byte[] queue;
        int cursor;

        public ByteStack(int size)
        {
            queue = new byte[size];
            cursor = 0;
        }

        public void Reset()
        {
            cursor = 0;
        }

        public void Push(byte v)
        {
            queue[cursor] = v;
            cursor++;
        }

        public byte Pop()
        {
            return queue[--cursor];
        }

        public bool IsEmpty()
        {
            return cursor == 0;
        }

    }
}
