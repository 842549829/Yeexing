using Autofac;
using Autofac.Configuration;
using Bms.Domain.Interface;
using Bms.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFac
{
    /// <summary>
    /// http://autofac.readthedocs.io/en/latest/getting-started/index.html
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            InjectByAPI();

            WriteDate();


            Console.WriteLine();
            Console.ReadLine();
        }

        public static void WriteDate()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var writer = scope.Resolve<IAdminRepository>();
                writer.Write("XX");
            }
        }

        static IContainer Container;

        /// <summary>
        /// 采用API注入
        /// </summary>
        static void InjectByAPI()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AdminRepository>().As<IAdminRepository>();
            Container = builder.Build();
        }

        /// <summary>
        /// 采用配置文件注入
        /// </summary>
        static void InjectByJsonConfig()
        {
            ContainerBuilder builder = new ContainerBuilder();
            IConfigurationBuilder config = new ConfigurationBuilder();
            IConfigurationSource autofacJsonConfigSource = new JsonConfigurationSource()
            {
                Path = "/autofac.json",
                Optional = false,//boolean,默认就是false,可不写
                ReloadOnChange = false,//同上
            };
            config.Add(autofacJsonConfigSource);
            var module = new ConfigurationModule(config.Build());
            builder.RegisterModule(module);
            Container = builder.Build();

            //var config = new ConfigurationBuilder();
            //config.AddJsonFile("autofac.json");
            //var module = new ConfigurationModule(config.Build());
            //IContainer builder = new ContainerBuilder();
            //builder.RegisterModule(module);
            //Container = builder.Build();
        }
    }
}