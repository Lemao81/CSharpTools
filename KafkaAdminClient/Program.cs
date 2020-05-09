using System;
using System.Linq;
using CommandLine;
using Confluent.Kafka;

namespace KafkaAdminClient
{
    class Options
    {
        [Option('t', "topics", HelpText = "List Topics")]
        public bool IsTopics { get; set; }

        [Option('g', "groups", HelpText = "Show groups")]
        public bool IsGroups { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(options =>
            {
                var config = new AdminClientConfig
                {
                    BootstrapServers = "localhost:9092"
                };
                using var adminClient = new AdminClientBuilder(config).Build();

                Metadata metaData = null;
                try
                {
                    metaData = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                }
                catch (KafkaException)
                {
                    Console.WriteLine("Timed out");
                    Environment.Exit(1);
                }

                if (options.IsTopics)
                {
                    Console.WriteLine("Topics:");
                    Console.WriteLine();
                    foreach (var topic in metaData.Topics)
                    {
                        Console.WriteLine(topic.Topic);
                    }
                }
                else if (options.IsGroups)
                {
                    Console.WriteLine("Groups:");
                    Console.WriteLine();
                    foreach (var group in adminClient.ListGroups(TimeSpan.FromSeconds(10)))
                    {
                        Console.WriteLine(group.Group);
                    }
                }
            });
        }
    }
}