FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["GeckosoftBackend/GeckosoftBackend.csproj", "GeckosoftBackend/"]
RUN dotnet restore "GeckosoftBackend/GeckosoftBackend.csproj"
COPY . .
WORKDIR "/src/GeckosoftBackend"
RUN dotnet build "GeckosoftBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeckosoftBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckosoftBackend.dll"]
