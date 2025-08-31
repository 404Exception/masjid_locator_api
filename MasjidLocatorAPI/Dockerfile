FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY published/ .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MasjidLocatorAPI.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MasjidLocatorAPI.csproj", "."]
RUN dotnet restore "MasjidLocatorAPI.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage  
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "MasjidLocatorAPI.dll"]
