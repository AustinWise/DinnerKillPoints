#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN apt-get update && apt-get upgrade -y && apt-get install -y --no-install-recommends graphviz

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DkpWeb/DkpWeb.csproj", "src/DkpWeb/"]
RUN dotnet restore "src/DkpWeb/DkpWeb.csproj" -r linux-x64
COPY . .
WORKDIR "/src/src/DkpWeb"
RUN dotnet publish "DkpWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false -r linux-x64 --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DkpWeb.dll"]
