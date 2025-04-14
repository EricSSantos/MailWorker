using Mail.Service;
using Mail.Service.Interface;
using Mail.Worker;
using Mail.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMailService, MailService>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
