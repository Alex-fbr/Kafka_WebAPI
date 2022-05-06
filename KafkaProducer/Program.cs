using KafkaProducer.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(nameof(KafkaOptions)));

builder.Services.AddLogging(logBuilder =>
    logBuilder.AddDebug()
              .AddConsole()
              .AddConfiguration(builder.Configuration.GetSection("Logging"))
              .SetMinimumLevel(LogLevel.Information));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
