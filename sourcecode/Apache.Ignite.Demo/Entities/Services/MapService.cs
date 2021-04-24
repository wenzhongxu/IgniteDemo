using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Resource;
using Apache.Ignite.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MapService<TK, TV> : IService
    {
        //注册实例
        [InstanceResource] private readonly IIgnite _ignite;

        private ICache<TK, TV> _cache;

        public void Init(IServiceContext context)
        {
            _cache = _ignite.GetOrCreateCache<TK, TV>("MapService_" + context.Name);

            Console.WriteLine("Service initialized:" + context.Name);
        }

        public void Cancel(IServiceContext context)
        {
            Console.WriteLine("Service cancelled:" + context.Name);
        }

        public void Execute(IServiceContext context)
        {
            Console.WriteLine("Service start:" + context.Name);
        }

        public void Put(TK key, TV value)
        {
            _cache.Put(key, value);
        }

        public TV Get(TK key)
        {
            return _cache.Get(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public int Size
        {
            get
            {
                return _cache.GetSize();
            }
        }
    }
}
