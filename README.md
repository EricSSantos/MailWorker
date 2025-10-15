# 📬 MailWorker

[![.NET](https://img.shields.io/badge/.NET-9-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/) [![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Enabled-orange?logo=rabbitmq)](https://www.rabbitmq.com/) [![SMTP](https://img.shields.io/badge/SMTP-Supported-success?logo=gmail)](https://en.wikipedia.org/wiki/Simple_Mail_Transfer_Protocol) [![Docker](https://img.shields.io/badge/Docker-Supported-2496ED?logo=docker)](https://www.docker.com/)

O **MailWorker** é responsável por processar mensagens da fila RabbitMQ e enviar e-mails transacionais utilizando um servidor SMTP configurável (ex: Gmail, Hostinger, Outlook, etc).

---

## ⚙️ Principais recursos

- Envio **assíncrono** de e-mails com RabbitMQ  
- **Retry automático** em falhas temporárias  
- Fila de **Dead Letter** para mensagens não entregues  
- Templates HTML reutilizáveis
- Configuração simples via `appsettings.json` ou variáveis de ambiente `docker-compose.yml`

---

## 📈 Como funciona

1. Um serviço externo publica uma mensagem JSON na fila `email_queue`.  
2. O MailWorker consome a mensagem e identifica o tipo de e-mail com base no campo type, utilizando um enum compartilhado entre os serviços para garantir consistência entre quem envia e quem processa.
3. O serviço seleciona a **estratégia de template** correspondente.  
4. O conteúdo é processado e enviado pelo **provedor SMTP**.  
5. Se falhar:
   - Tenta reenviar até **3 vezes**;
   - Depois, envia para a **Dead Letter Queue**.

---

## 💌 Estrutura das mensagens

### Exemplo: `Welcome`
```json
{
  "id": "6f02a1b2-9c1f-4f9c-a2f2-d6e5d54b2c67",
  "type": 0,
  "to": "usuario@dominio.com.br",
  "fullName": "Usuário Exemplo",
  "payload": {}
}
```

---

## 🔧 Configuração Locais (`appsettings.json`)

Edite o arquivo com suas credenciais de SMTP e RabbitMQ:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Application": {
    "Name": "<APPLICATION_NAME>"
  },
  "Smtp": {
    "Host": "<SMTP_HOST>",
    "Port": 587,
    "User": "<SMTP_USER>",
    "Password": "<SMTP_PASSWORD>",
    "FromEmail": "<SMTP_FROM_EMAIL>",
    "FromName": "<SMTP_FROM_NAME>",
    "EnableSsl": true
  },
  "RabbitMQ": {
    "Host": "<RABBITMQ_HOST>",
    "Port": 5672,
    "User": "<RABBITMQ_USER>",
    "Password": "<RABBITMQ_PASSWORD>",
    "EmailQueue": "<RABBITMQ_EMAIL_QUEUE>",
    "DeadLetterQueue": "<RABBITMQ_DEADLETTER_QUEUE>"
  }
}

```

---

## 🌎 Variáveis de ambiente

Quando executado em **Docker** é possível configurar tudo via variáveis:

```bash
APPLICATION__NAME=<APPLICATION_NAME>

SMTP__HOST=<SMTP_HOST>
SMTP__PORT=<SMTP_PORT>
SMTP__USER=<SMTP_USER>
SMTP__PASSWORD=<SMTP_PASSWORD>
SMTP__FROMEMAIL=<SMTP_FROM_EMAIL>
SMTP__FROMNAME=<SMTP_FROM_NAME>
SMTP__ENABLESSL=<SMTP_ENABLE_SSL>

RABBITMQ__HOST=<RABBITMQ_HOST>
RABBITMQ__PORT=<RABBITMQ_PORT>
RABBITMQ__USER=<RABBITMQ_USER>
RABBITMQ__PASSWORD=<RABBITMQ_PASSWORD>
RABBITMQ__EMAILQUEUE=<RABBITMQ_EMAIL_QUEUE>
RABBITMQ__DEADLETTERQUEUE=<RABBITMQ_DEADLETTER_QUEUE>
```

---

## 🐋 Execução com Docker

### Subir os serviços:

```bash
docker compose up --build
```

O **Docker Compose** criará:
- Container do **RabbitMQ** (com painel em `http://localhost:15672`)
- Container do **MailWorker**
- Volumes persistentes para dados das filas
