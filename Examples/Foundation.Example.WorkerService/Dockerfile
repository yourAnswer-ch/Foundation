#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Foundation.Example.Host.csproj", "."]
RUN dotnet restore "./Foundation.Example.Host.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Foundation.Example.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Foundation.Example.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Foundation.Example.Host.dll"]