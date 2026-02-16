# Estágio de Runtime (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
# O Railway usa a variável PORT, mas internamente o container ouve na 8080
EXPOSE 8080
EXPOSE 8081

# Estágio de Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copia os arquivos de projeto primeiro para aproveitar o cache de camadas do Docker
# Note que mantemos a estrutura de pastas para que as referências entre projetos não quebrem
COPY ["src/DesafioMinervaFoods.API/DesafioMinervaFoods.API.csproj", "DesafioMinervaFoods.API/"]
COPY ["src/DesafioMinervaFoods.Application/DesafioMinervaFoods.Application.csproj", "DesafioMinervaFoods.Application/"]
COPY ["src/DesafioMinervaFoods.Domain/DesafioMinervaFoods.Domain.csproj", "DesafioMinervaFoods.Domain/"]
COPY ["src/DesafioMinervaFoods.Infrastructure/DesafioMinervaFoods.Infrastructure.csproj", "DesafioMinervaFoods.Infrastructure/"]

# 2. Restaura as dependências
RUN dotnet restore "DesafioMinervaFoods.API/DesafioMinervaFoods.API.csproj"

# 3. Copia todo o restante do código fonte
COPY . .

# 4. Define o diretório de trabalho para o projeto da API e compila
WORKDIR "/src/DesafioMinervaFoods.API"
RUN dotnet build "DesafioMinervaFoods.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DesafioMinervaFoods.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio Final (Imagem enxuta apenas com o necessário para rodar)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Variáveis de ambiente para garantir que o ASP.NET ouça na porta correta
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DesafioMinervaFoods.API.dll"]