using KafkaConsumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddLogging(logBuilder =>
          logBuilder
              .AddDebug()
              .AddConsole()
              .AddConfiguration(builder.Configuration.GetSection("Logging"))
              .SetMinimumLevel(LogLevel.Information));

        services.Configure<KafkaOptions>(builder.Configuration.GetSection(nameof(KafkaOptions)));
        services.AddHostedService<Consumer>();
    })
    .Build();

await host.RunAsync();
