using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Repository.Interfaces
{
    public interface IRepository<T, in TKey>
    {
        T Get(TKey Id);
        T Get(string Name);
    }
}
