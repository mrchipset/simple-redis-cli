using System;
using StackExchange.Redis;

namespace hash
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

        static int Main(string[] args)
        {
            GetConfiguration();
            Console.WriteLine("Hello! This is hash.exe. A simple Redis CLI tool.");

            if (args.Length < 2)
            {
                Console.Error.WriteLine("Error!\nUsage:\nhash.exe SET/GET/DEL/LIST [dict name] [key] [value]");
                return -1;
            }
            else
            {
                try
                {
                    var conn = ConnectionMultiplexer.Connect($"{host}:{port}");
                    var db = conn.GetDatabase();

                    string value = null;
                    string key = null;
                    string dict = null;
                    switch(args[0])
                    {
                        case "LIST":
                            var keys = db.HashKeys(args[1]);
                            foreach(var _key in keys)
                            {
                                Console.WriteLine(_key);
                            }
                            break;
                        case "GET":
                            if (args.Length < 3)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nhash.exe GET Dict Key");
                                return -1;
                            }
                            dict = args[1];
                            key = args[2];
                            value = db.HashGet(dict, key);
                            Console.Write($"GET key: {key}, value: {value}");
                            break;
                        case "SET":
                            if (args.Length < 4)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nhash.exe SET Dict Key Value");
                                return -1;
                            }
                            dict = args[1];
                            key = args[2];
                            value = args[3];
                            db.HashSet(dict, key, value);
                            Console.Write($"SET key: {key}, value: {value}");
                            break;
                        case "DEL":
                            if (args.Length < 3)
                            {
                                Console.Error.WriteLine($"The arguement length {args.Length} is error.");
                                Console.Error.Write("Usage:\nhash.exe DEL Dict Key");
                                return -1;
                            }
                            dict = args[1];
                            key = args[2];
                            db.HashDelete(dict, key);
                            break;
                    }
                    return 1;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine("Please check the connection configuration in appsettings.json.");
                    return -1;
                }
            }
        }
    }

            
}
