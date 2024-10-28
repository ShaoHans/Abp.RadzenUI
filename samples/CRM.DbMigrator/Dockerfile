#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["samples/CRM.DbMigrator/CRM.DbMigrator.csproj", "samples/CRM.DbMigrator/"]
COPY ["samples/CRM.EntityFrameworkCore/CRM.EntityFrameworkCore.csproj", "samples/CRM.EntityFrameworkCore/"]
COPY ["samples/CRM.Domain/CRM.Domain.csproj", "samples/CRM.Domain/"]
COPY ["samples/CRM.Domain.Shared/CRM.Domain.Shared.csproj", "samples/CRM.Domain.Shared/"]
COPY ["samples/CRM.Application.Contracts/CRM.Application.Contracts.csproj", "samples/CRM.Application.Contracts/"]
RUN dotnet restore "./samples/CRM.DbMigrator/CRM.DbMigrator.csproj"
COPY . .
WORKDIR "/src/samples/CRM.DbMigrator"
RUN dotnet build "./CRM.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CRM.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.DbMigrator.dll"]