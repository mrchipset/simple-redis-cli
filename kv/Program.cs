using System;
using System.Linq;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace kv
{
    class Program
    {

        static string host;
        static int port;

        static void GetConfiguration()
        {
            AppConfiguration.Build();
            host = AppConfiguration.Configuration.GetSection("host").Value;
            if (string.IsNullOrEmpty(host))
            {
                host = "127.0.0.1";
            }

            if (!int.TryParse(AppConfiguration.Configuration.GetSection("port").Value, out port))
            {
                port = 6379;
            }
        }

        static void Main(string[] args)
        {
            Task.Run(() => {
                Worker(args);
            }).Wait();
        }

        static async void Worker(string[] args)
        {
            GetConfiguration();
            Console.WriteLine("Hello! This is kv.exe. A simple Redis CLI tool");

            if (args.Length < 1)
            {
                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                Console.Error.Write("Usage:\nkv.exe GET/SET/LIST/DEL [Key Value]");
            }
            else
            {
                try
                {
                    var conn = ConnectionMultiplexer.Connect($"{host}:{port}");
                    var db = conn.GetDatabase();

                    string value = null;
                    string key = null;
                    IServer server;
                    switch(args[0])
                    {
                        case "LIST":
                            server = conn.GetServer(host, port);
                            var keys = server.Keys();
                            Console.WriteLine($"Key Count: {keys.Count()}");
                            foreach(var _key in keys)
                            {
                                Console.WriteLine(_key);
                            }
                            break;
                        case "GET":
                            if (args.Length < 2)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nkv.exe GET Key");
                                return;
                            }
                            key = args[1];
                            value = db.StringGet(key);
                            Console.Write($"GET key: {key}, value: {value}");
                            break;
                        case "SET":
                            if (args.Length < 3)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nkv.exe SET Key Value");
                                return;
                            }
                            key = args[1];
                            value = args[2];
                            db.StringSet(key, value);
                            Console.Write($"SET key: {key}, value: {value}");
                            break;
                        case "DEL":
                            if (args.Length < 2)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nkv.exe DEL Key");
                                return;
                            }
                            key = args[1];
                            db.KeyDelete(key);
                            break;
                        case "CLEAR":
                            server = conn.GetServer(host, port);
                            foreach (var _key in server.Keys())
                            {
                                await db.KeyDeleteAsync(_key);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine("Please check the connection configuration in appsettings.json.");
                }
            }
        }
    }
}
