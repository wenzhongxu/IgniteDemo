using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Multicast;
using Apache.Ignite.Core.Log;
using Models;

namespace ServerNode
{
    public class Program
    {
        private static readonly IgniteConfiguration igniteConfiguration = new IgniteConfiguration
        {
            Localhost = "127.0.0.1",
            DiscoverySpi = new TcpDiscoverySpi
            { 
                IpFinder = new TcpDiscoveryMulticastIpFinder
                { 
                    Endpoints = new[]
                    {
                        "127.0.0.1:47500..47502"
                    }
                }
            },
            JvmOptions = new[]
            {
                "-DIGNITE_QUIET=true",
                "-DIGNITE_PERFORMANCE_SUGGESTIONS_DISABLED=true"
            },
            Logger = new ConsoleLogger
            { 
                MinLevel = LogLevel.Error
            },
            PeerAssemblyLoadingMode = Apache.Ignite.Core.Deployment.PeerAssemblyLoadingMode.CurrentAppDomain
        };


        [STAThread]
        public static void Main(string[] args)
        {
            //using var ignite = Ignition.StartFromApplicationConfiguration(); //根据配置文件启动服务
            using var ignite = Ignition.Start(igniteConfiguration);

            Console.WriteLine(">>> ignite services started.");
            Console.WriteLine();

            //部署一个单节点服务。 ignite保证每个节点上总有一个服务实例在运行，只要底层集群组中启动了新的节点，ignite就会在每个新节点上自动部署一个服务实例
            Console.WriteLine(">>> deploying service to all nodes...");
            ignite.GetServices().DeployNodeSingleton("default-map-service", new MapService<int, string>());

            var prx = ignite.GetServices().GetServiceProxy<IMapService<int, string>>("default-map-service", true);

            for (int i = 0; i < 10; i++)
            {
                prx.Put(i, i.ToString());
            }

            var mapSize = prx.Size;

            Console.WriteLine(">>> Map service size is " + mapSize);

            ignite.GetServices().CancelAll();

            Console.ReadKey();
        }
    }
}
