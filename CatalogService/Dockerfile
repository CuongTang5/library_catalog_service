FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["CatalogService.csproj", "./"]
RUN dotnet restore "./CatalogService.csproj"

# copy everything and publish
COPY . .
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5185
ENTRYPOINT ["dotnet", "CatalogService.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CatalogService.csproj", "./"]
RUN dotnet restore "CatalogService.csproj"

COPY . .
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5185
ENTRYPOINT ["dotnet", "CatalogService.dll"]
