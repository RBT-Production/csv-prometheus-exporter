using System.Collections.Generic;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace csv_prometheus_exporter.Scraper.Config
{
    public class ConnectionSettings
    {
        [YamlMember(Alias = "file", ApplyNamingConventions = false)]
        public string File { get; set; } = null;


        [YamlMember(Alias = "user", ApplyNamingConventions = false)]
        public string User { get; set; } = null;


        [YamlMember(Alias = "password", ApplyNamingConventions = false)]
        public string Password { get; set; } = null;


        [YamlMember(Alias = "pkey", ApplyNamingConventions = false)]
        public string PKey { get; set; } = null;

        [YamlMember(Alias = "connect-timeout", ApplyNamingConventions = false)]
        public int? ConnectTimeout { get; set; } = null;

        [YamlMember(Alias = "read-timeout-ms", ApplyNamingConventions = false)]
        public int? ReadTimeoutMs { get; set; } = null;
    }

    public class Environment
    {
        [YamlMember(Alias = "hosts", ApplyNamingConventions = false)]
        public List<string> Hosts { get; set; }

        [YamlMember(Alias = "connection", ApplyNamingConventions = false)]
        public ConnectionSettings ConnectionSettings { get; set; }
    }

    public class SSH
    {
        [YamlMember(Alias = "environments", ApplyNamingConventions = false)]
        public Dictionary<string, Environment> Environments { get; set; }

        [YamlMember(Alias = "connection", ApplyNamingConventions = false)]
        public ConnectionSettings ConnectionSettings { get; set; }
    }

    public class Global
    {
        [YamlMember(Alias = "ttl", ApplyNamingConventions = false)]
        public int TTL { get; set; } = 60;

        [YamlMember(Alias = "background-resilience", ApplyNamingConventions = false)]
        public int BackgroundResilience { get; set; } = 1;

        [YamlMember(Alias = "long-term-resilience", ApplyNamingConventions = false)]
        public int LongTermResilience { get; set; } = 10;

        [YamlMember(Alias = "prefix", ApplyNamingConventions = false)]
        public string Prefix { get; set; } = null;

        [YamlMember(Alias = "histograms", ApplyNamingConventions = false)]
        public Dictionary<string, List<double>> Histograms { get; set; }

        [YamlMember(Alias = "format", ApplyNamingConventions = false)]
        public List<Dictionary<string, string>> Format { get; set; }
    }

    public class ScraperConfig
    {
        [YamlMember(Alias = "global", ApplyNamingConventions = false)]
        public Global Global { get; set; }

        [YamlMember(Alias = "ssh", ApplyNamingConventions = false)]
        public SSH SSH { get; set; }

        [YamlMember(Alias = "script", ApplyNamingConventions = false)]
        public string Script { get; set; } = null;

        [YamlMember(Alias = "reload-interval", ApplyNamingConventions = false)]
        public int? ReloadInterval { get; set; } = null;
    }
}