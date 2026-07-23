FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

USER root
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

ARG GITHUB_USERNAME
ARG GITHUB_PASSWORD
ENV GITHUB_USERNAME=$GITHUB_USERNAME
ENV GITHUB_PASSWORD=$GITHUB_PASSWORD

WORKDIR /src

COPY ["AppointmentsAPI.Presentation/AppointmentsAPI.Presentation.csproj", "AppointmentsAPI.Presentation/"]
COPY ["AppontmentsAPI.Application/AppointmentsAPI.Application.csproj", "AppontmentsAPI.Application/"]
COPY ["AppointmentsAPI.Infrastructure/AppointmentsAPI.Infrastructure.csproj", "AppointmentsAPI.Infrastructure/"]
COPY ["AppointmentsAPI.Domain/AppointmentsAPI.Domain.csproj", "AppointmentsAPI.Domain/"]
COPY ["nuget.config", "./"]

RUN dotnet nuget update source "https://nuget.pkg.github.com/XNeedX/index.json" \
    --name "github" \
    --username "$GITHUB_USERNAME" \
    --password "$GITHUB_PASSWORD" \
    --store-password-in-clear-text \
    --configfile nuget.config

RUN dotnet restore "./AppointmentsAPI.Presentation/AppointmentsAPI.Presentation.csproj"

COPY . .
WORKDIR "/src/AppointmentsAPI.Presentation"

RUN dotnet build "./AppointmentsAPI.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AppointmentsAPI.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AppointmentsAPI.Presentation.dll"] 