using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPool
{
    public interface IPoolable
    {
        int PoolIndex { get; set; }
    }
}
