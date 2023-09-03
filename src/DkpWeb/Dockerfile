#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENV \
    # UID of the non-root user 'app'
    # TODO: remove when upgrading to .NET 8
    APP_UID=1654

RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --no-install-recommends graphviz \
    && rm -rf /var/lib/apt/lists/*

# Create a non-root user and group
# TODO: remove when upgrading to .NET 8
RUN groupadd \
        --gid=$APP_UID \
        app \
    && useradd -l \
        --uid=$APP_UID \
        --gid=$APP_UID \
        --create-home \
        app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DkpWeb/DkpWeb.csproj", "src/DkpWeb/"]
RUN dotnet restore "src/DkpWeb/DkpWeb.csproj" -r linux-x64
COPY . .
WORKDIR "/src/src/DkpWeb"
RUN dotnet publish "DkpWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false -r linux-x64 --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "DkpWeb.dll"]
