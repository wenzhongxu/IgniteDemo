using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IMapService<TK, TV>
    {
        void Put(TK key, TV value);

        TV Get(TK key);

        void Clear();

        int Size { get; }
    }
}
