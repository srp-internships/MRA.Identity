#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MRA.Identity.Api/MRA.Identity.Api.csproj", "MRA.Identity.Api/"]
COPY ["MRA.Identity.Infrastructure/MRA.Identity.Infrastructure.csproj", "MRA.Identity.Infrastructure/"]
COPY ["MRA.Identity.Application/MRA.Identity.Application.csproj", "MRA.Identity.Application/"]
COPY ["MRA.Identity.Application.Contract/MRA.Identity.Application.Contract.csproj", "MRA.Identity.Application.Contract/"]
COPY ["MRA.Identity.Domain/MRA.Identity.Domain.csproj", "MRA.Identity.Domain/"]
RUN dotnet restore "./MRA.Identity.Api/./MRA.Identity.Api.csproj"
COPY . .
WORKDIR "/src/MRA.Identity.Api"
RUN dotnet build "./MRA.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MRA.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MRA.Identity.Api.dll"]