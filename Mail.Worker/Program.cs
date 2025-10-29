using Mail.Service;
using Mail.Service.Commons.Interface;
using Mail.Service.Commons.Settings;
using Mail.Service.Strategies;
using Mail.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("Application"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

// Strategies
builder.Services.AddScoped<IEmailStrategy, ConfirmEmail>();
builder.Services.AddScoped<IEmailStrategy, Welcome>();
builder.Services.AddScoped<IEmailStrategy, ForgotPassword>();

builder.Services.AddHostedService<Worker>();

builder.Build().Run();
