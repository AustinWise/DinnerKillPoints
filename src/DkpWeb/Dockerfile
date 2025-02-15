# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN rm -f /etc/apt/apt.conf.d/docker-clean; \
    echo 'Binary::apt::APT::Keep-Downloaded-Packages "true";' > /etc/apt/apt.conf.d/keep-cache
RUN --mount=type=cache,target=/var/cache/apt,sharing=locked \
    --mount=type=cache,target=/var/lib/apt,sharing=locked \
        apt-get update \
        && apt-get upgrade -y \
        && apt-get install -y --no-install-recommends graphviz

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DkpWeb/DkpWeb.csproj", "src/DkpWeb/"]
RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet restore "src/DkpWeb/DkpWeb.csproj" \
        -r linux-x64
COPY . .
WORKDIR "/src/src/DkpWeb"
# TODO: figure out how to add --no-restore to this. Then the cache would not be needed.
RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet publish "DkpWeb.csproj" \
        -c Release \
        -o /app/publish \
        /p:UseAppHost=false \
        -r linux-x64 \
        --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "DkpWeb.dll"]
