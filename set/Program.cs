using System;
using StackExchange.Redis;

namespace set
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
            Console.WriteLine("Hello! This is set.exe. A simple Redis CLI tool.");

            if (args.Length < 2)
            {
                Console.Error.WriteLine("Error!\nUsage:\nset.exe PUSH/POP [set name] [value]");
                return -1;
            }
            else
            {
                var conn = ConnectionMultiplexer.Connect($"{host}:{port}");
                var db = conn.GetDatabase();
                string key = args[1];

                if (args[0] == "PUSH")
                {
                    if (args.Length < 3)
                    {
                        Console.Error.WriteLine("Error!\nUsage:\nset.exe PUSH/POP [set name] [value]");
                        return -1;
                    }
                    string value = args[2];
                    if (db.SetAdd(key, value))
                    {
                        long length = db.SetLength(key);
                        Console.WriteLine($"push: {key}\t value: {value};\nLength: {length}");
                        return 1;
                    } else
                    {
                        Console.WriteLine($"the value {value} is already existed.");
                        return 0;
                    }

                }
                else
                {
                    string value = db.SetPop(key);
                    long length = db.SetLength(key);
                    Console.WriteLine($"POP Key: {key}\tValue:{value};\nLength: {length}");
                    return 1;
                }
            }
        }
    }
}
