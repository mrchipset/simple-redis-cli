using System;
using StackExchange.Redis;

namespace lk
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
            Console.WriteLine("Hello! This is lk.exe. A simple Redis CLI tool for distributed lock");

            if (args.Length < 3)
            {
                Console.Error.WriteLine("Error!\nUsage:\nlk.exe LOCK/RELEASE [key] [Token]");
                return -1;
            } else
            {
                var conn = ConnectionMultiplexer.Connect($"{host}:{port}");
                var db = conn.GetDatabase();
                int milliseconds = 5000;
                string key = args[1];
                string token = args[2];

                if (args.Length >= 4)
                {
                    int.TryParse(args[3], out milliseconds);
                }

                if (args[0] == "LOCK")
                {
                    if (db.LockTake(key, token, TimeSpan.FromMilliseconds(milliseconds)))
                    {
                        Console.WriteLine("Get the lock.");
                        return 1;
                    } else
                    {
                        Console.Error.WriteLine("Could not get the lock.");
                        return 0;
                    }
                } else
                {
                    if (db.LockRelease(key, token))
                    {
                        Console.WriteLine("Release the lock.");
                        return 1;
                    } else
                    {
                        Console.Error.WriteLine("Could not release the lock.");
                        return 0;
                    }
                }
            }
        }
    }
}
