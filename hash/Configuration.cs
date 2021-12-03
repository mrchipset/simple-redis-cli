using System;
using Microsoft.Extensions.Configuration;

namespace hash
{
    public static class AppConfiguration
    {
        public static IConfiguration Configuration { get; private set; }
        private static bool _build;
        private static object _lkObj = new object();
        public static IConfiguration Build()
        {
            lock (_lkObj)
            {
                if (!_build)
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json");
                    Configuration = builder.Build();
                    _build = true;
                }
            }
            return Configuration;
        }
    }
}
