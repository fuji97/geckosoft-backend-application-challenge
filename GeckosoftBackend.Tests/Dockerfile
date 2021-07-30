FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GeckosoftBackend/GeckosoftBackend.csproj", "GeckosoftBackend/"]
RUN dotnet restore "GeckosoftBackend/GeckosoftBackend.csproj"
COPY ["GeckosoftBackend.Tests/GeckosoftBackend.Tests.csproj", "GeckosoftBackend.Tests/"]
RUN dotnet restore "GeckosoftBackend.Tests/GeckosoftBackend.Tests.csproj"

COPY . .
WORKDIR "/src/GeckosoftBackend.Tests"

ENTRYPOINT ["dotnet", "test"]
