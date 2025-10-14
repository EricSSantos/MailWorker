using Mail.Service;
using Mail.Service.Helpers.Settings;
using Mail.Service.Interface;
using Mail.Worker;

var builder = Host.CreateApplicationBuilder(args);

//Inclui variáveis de ambiente (Dokploy, Docker,etc)
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
