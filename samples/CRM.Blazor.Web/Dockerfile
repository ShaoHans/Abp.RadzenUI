FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["samples/CRM.Blazor.Web/CRM.Blazor.Web.csproj", "samples/CRM.Blazor.Web/"]
COPY ["src/Abp.Blazor.Server.RadzenUI/Abp.Blazor.Server.RadzenUI.csproj", "src/Abp.Blazor.Server.RadzenUI/"]
COPY ["samples/CRM.Application.Contracts/CRM.Application.Contracts.csproj", "samples/CRM.Application.Contracts/"]
COPY ["samples/CRM.Domain.Shared/CRM.Domain.Shared.csproj", "samples/CRM.Domain.Shared/"]
COPY ["samples/CRM.Application/CRM.Application.csproj", "samples/CRM.Application/"]
COPY ["samples/CRM.Domain/CRM.Domain.csproj", "samples/CRM.Domain/"]
COPY ["samples/CRM.Aspire.ServiceDefaults/CRM.Aspire.ServiceDefaults.csproj", "samples/CRM.Aspire.ServiceDefaults/"]
COPY ["samples/CRM.HttpApi/CRM.HttpApi.csproj", "samples/CRM.HttpApi/"]
COPY ["samples/CRM.EntityFrameworkCore/CRM.EntityFrameworkCore.csproj", "samples/CRM.EntityFrameworkCore/"]
RUN dotnet restore "samples/CRM.Blazor.Web/CRM.Blazor.Web.csproj"
COPY . .
WORKDIR "/src/samples/CRM.Blazor.Web"
RUN dotnet build "CRM.Blazor.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CRM.Blazor.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRM.Blazor.Web.dll"]
