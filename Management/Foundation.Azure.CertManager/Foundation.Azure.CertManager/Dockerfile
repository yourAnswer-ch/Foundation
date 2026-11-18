#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# The image is built for linux/amd64 and linux/arm64 (azure-pipelines.yml, platforms). Nothing is
# emulated: the SDK runs on the architecture of the build machine ($BUILDPLATFORM) and
# compiles for the architecture of the image ($TARGETARCH). The final stage only copies the
# result, so it must not get a RUN instruction.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG NUGET_KEY
ARG NUGET_SOURCE
ARG TARGETARCH

WORKDIR /src
COPY ["Foundation.Azure.CertManager/Foundation.Azure.CertManager.csproj", "Foundation.Azure.CertManager/"]

RUN dotnet nuget add source --valid-authentication-types basic --store-password-in-clear-text --username Docker --password $NUGET_KEY --name FlairFindr $NUGET_SOURCE
RUN dotnet restore "Foundation.Azure.CertManager/Foundation.Azure.CertManager.csproj" -a $TARGETARCH

COPY . .
WORKDIR "/src/Foundation.Azure.CertManager"

RUN dotnet build "Foundation.Azure.CertManager.csproj" -c Release -a $TARGETARCH -o /app/build

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Foundation.Azure.CertManager.csproj" -c Release -a $TARGETARCH -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Foundation.Azure.CertManager.dll"]