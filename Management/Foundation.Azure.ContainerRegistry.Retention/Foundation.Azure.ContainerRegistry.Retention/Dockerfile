#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Foundation.Azure.ContainerRegistry.Retention/Foundation.Azure.ContainerRegistry.Retention.csproj", "Foundation.Azure.ContainerRegistry.Retention/"]
RUN dotnet restore "Foundation.Azure.ContainerRegistry.Retention/Foundation.Azure.ContainerRegistry.Retention.csproj"
COPY . .
WORKDIR "/src/Foundation.Azure.ContainerRegistry.Retention"
RUN dotnet build "Foundation.Azure.ContainerRegistry.Retention.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Foundation.Azure.ContainerRegistry.Retention.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Foundation.Azure.ContainerRegistry.Retention.dll"]