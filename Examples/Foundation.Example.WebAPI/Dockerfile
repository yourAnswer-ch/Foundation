#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Foundation.Example.WebAPI/Foundation.Example.WebAPI.csproj", "Foundation.Example.WebAPI/"]
RUN dotnet restore "Foundation.Example.WebAPI/Foundation.Example.WebAPI.csproj"
COPY . .
WORKDIR "/src/Foundation.Example.WebAPI"
RUN dotnet build "Foundation.Example.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Foundation.Example.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Foundation.Example.WebAPI.dll"]