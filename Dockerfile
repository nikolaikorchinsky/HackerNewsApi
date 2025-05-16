# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish HackerNewsApi.csproj -c Release -o /app/publish

# Use the official .NET 8 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set environment variables if needed
 ENV ASPNETCORE_URLS=http://+:80
 ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "HackerNewsApi.dll"]