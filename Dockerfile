# Backend .NET 9 - TATA.BACKEND.PROYECTO1 (contexto de build: raiz del repo)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY TATA.BACKEND.PROYECTO1.sln ./
COPY TATA.BACKEND.PROYECTO1.API/TATA.BACKEND.PROYECTO1.API.csproj TATA.BACKEND.PROYECTO1.API/
COPY TATA.BACKEND.PROYECTO1.CORE/TATA.BACKEND.PROYECTO1.CORE.csproj TATA.BACKEND.PROYECTO1.CORE/
RUN dotnet restore TATA.BACKEND.PROYECTO1.API/TATA.BACKEND.PROYECTO1.API.csproj
COPY . .
RUN dotnet publish TATA.BACKEND.PROYECTO1.API/TATA.BACKEND.PROYECTO1.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app ./
# Solo escucha en local: Nginx (mismo task de ECS) es el unico que entra por el puerto 80
ENV ASPNETCORE_URLS=http://127.0.0.1:5000
USER app
ENTRYPOINT ["dotnet", "TATA.BACKEND.PROYECTO1.API.dll"]
