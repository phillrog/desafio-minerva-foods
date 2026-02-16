# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copia os projetos (Caminhos relativos à RAIZ do repositório)
COPY ["src/DesafioMinervaFoods.API/DesafioMinervaFoods.API.csproj", "DesafioMinervaFoods.API/"]
COPY ["src/DesafioMinervaFoods.Application/DesafioMinervaFoods.Application.csproj", "DesafioMinervaFoods.Application/"]
COPY ["src/DesafioMinervaFoods.Domain/DesafioMinervaFoods.Domain.csproj", "DesafioMinervaFoods.Domain/"]
COPY ["src/DesafioMinervaFoods.Infrastructure/DesafioMinervaFoods.Infrastructure.csproj", "DesafioMinervaFoods.Infrastructure/"]

# 2. Restaura
RUN dotnet restore "DesafioMinervaFoods.API/DesafioMinervaFoods.API.csproj"

# 3. Copia o código todo (O .dockerignore vai filtrar a pasta .front)
COPY . .

# 4. Compila
WORKDIR "/src/src/DesafioMinervaFoods.API"
RUN dotnet build "DesafioMinervaFoods.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Estágio de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DesafioMinervaFoods.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Estágio Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DesafioMinervaFoods.API.dll"]