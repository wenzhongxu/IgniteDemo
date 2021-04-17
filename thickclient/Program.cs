using System;
using Apache.Ignite.Core;

namespace IgniteDemo
{
    class Program
    {
        static void Main(string[] args)
        {
          var ignite = Ignition.Start();
          var cache = ignite.GetOrCreateCache<int, string>("my-cache");
          cache.Put(1, "Hello, World");
          Console.WriteLine(cache.Get(1));
        }
    }
}
