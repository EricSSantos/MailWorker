# Fase base (runtime do .NET para execução final)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# Fase de build (SDK do .NET para compilar os projetos)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia apenas os arquivos de projeto (para melhor aproveitamento do cache no restore)
COPY ["MailWorker/Mail.Worker/Mail.Worker.csproj", "MailWorker/Mail.Worker/"]
COPY ["MailWorker/Mail.Domain/Mail.Domain.csproj", "MailWorker/Mail.Domain/"]
COPY ["MailWorker/Mail.Service/Mail.Service.csproj", "MailWorker/Mail.Service/"]

# Restaura as dependências com base nos .csproj
RUN dotnet restore "MailWorker/Mail.Worker/Mail.Worker.csproj"

# Copia apenas os diretórios relevantes (evita copiar o projeto inteiro e quebrar o cache)
COPY MailWorker/Mail.Worker/. MailWorker/Mail.Worker/
COPY MailWorker/Mail.Domain/. MailWorker/Mail.Domain/
COPY MailWorker/Mail.Service/. MailWorker/Mail.Service/

# Compila o projeto principal
WORKDIR "/src/MailWorker/Mail.Worker"
RUN dotnet build "Mail.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica o projeto compilado para a pasta final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mail.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final com apenas o runtime e os arquivos publicados
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mail.Worker.dll"]
