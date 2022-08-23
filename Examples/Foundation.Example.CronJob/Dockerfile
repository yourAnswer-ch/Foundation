#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG NUGET_KEY
ARG NUGET_SOURCE

WORKDIR /src
COPY ["Foundation.Example.CronJob/Foundation.Example.CronJob.csproj", "Foundation.Example.CronJob/"]

RUN dotnet nuget add source --valid-authentication-types basic --store-password-in-clear-text --username Docker --password $NUGET_KEY --name FlairFindr $NUGET_SOURCE
RUN dotnet restore "Foundation.Example.CronJob/Foundation.Example.CronJob.csproj"

COPY . .
WORKDIR "/src/Foundation.Example.CronJob"

RUN dotnet build "Foundation.Example.CronJob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Foundation.Example.CronJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Foundation.Example.CronJob.dll"]