# 📬 Mail Worker

[![.NET](https://img.shields.io/badge/.NET-9-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/)  [![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Enabled-orange?logo=rabbitmq)](https://www.rabbitmq.com/)  [![SendGrid](https://img.shields.io/badge/SendGrid-API-success?logo=sendgrid)](https://sendgrid.com/)  [![Docker](https://img.shields.io/badge/Docker-Supported-2496ED?logo=docker)](https://www.docker.com/)

Serviço de envio de e-mails assíncrono via RabbitMQ + SendGrid, desenvolvido com Worker Service em .NET 9.
- Envio assíncrono de e-mails via fila RabbitMQ  
- Suporte a múltiplos tipos de e-mail (ex: confirmação de conta, redefinição de senha)  
- Retry automático até 3 vezes  
- Fila de *Dead Letter* para mensagens com falha

---

### 📥 Clone o projeto

```bash
# Clone o repositório
git clone https://github.com/EricSSantos/MailWorker.git

# Acesse a pasta do projeto
cd MailWorker
```

### ⚙️ Configure o `appsettings.json`

Edite o arquivo `appsettings.json` com suas credenciais:

```json
{
  "Application": {
    "ResetPasswordUrl": "https://www.seudominio.com.br/reset-password?token="
  },
  "SendGrid": {
    "Key": "SUA_CHAVE_SENDGRID",
    "FromEmail": "email@dominio.com.br",
    "FromName": "Nome que aparecerá como remetente"
  },
  "RabbitMQ": {
    "Host": "rabbitmq",
    "Port": 5672,
    "User": "guest",
    "Password": "guest",
    "MailQueue": "mail_queue",
    "DeadLetterQueue": "deadletter_queue"
  }
}
```

### 🚀 Build e execução

```bash
# Faça o build do projeto
dotnet build

# Suba os serviços com Docker Compose
docker compose up --build
```

---

### 📌 O que é criado

- **RabbitMQ** com interface Web e configurações pré-definidas  
- **Filas:**
  - `mail_queue`
  - `deadletter_queue`
- **Rede isolada** entre containers  
- **Worker** que escuta e processa as mensagens  

---

## 📈 Como funciona

1. Uma aplicação externa publica uma `EmailMessage` na fila RabbitMQ.  
2. O Worker consome a mensagem.  
3. `MessageService` processa o conteúdo.  
4. `MailService` monta o HTML e envia com SendGrid.  
5. Em caso de erro:
   - Tenta **3 vezes**
   - Depois envia para **dead-letter queue**

![image](https://github.com/user-attachments/assets/7eb1fed9-e82b-4997-834c-63ae3aa36204)

---

## 📦 Estrutura das mensagens

### Exemplo: `AccountConfirmation`
```json
{
  "id": "f36c7b4f-1e1f-4c20-90a3-2e7e1e19d9fb",
  "type": "AccountConfirmation",
  "to": "johndoe@email.com",
  "fullName": "John Doe",
  "content": {
    "code": "ABC123"
  }
}
```

### Exemplo: `PasswordReset`
```json
{
  "id": "f36c7b4f-1e1f-4c20-90a3-2e7e1e19d9fb",
  "type": "PasswordReset",
  "to": "johndoe@email.com",
  "fullName": "John Doe",
  "content": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIx...."
  }
}

```
