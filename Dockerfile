# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY HackerNewsApi/*.csproj ./HackerNewsApi/
#COPY HackerNewsTests/*.csproj ./HackerNewsTests/
COPY HackerNewsApi/* ./HackerNewsApi/
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Run tests
#WORKDIR /src/HackerNewsTests
#RUN dotnet test --no-restore --logger:"trx;LogFileName=test_results.trx"

# Build the application
WORKDIR /src/HackerNewsApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 8 ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set environment variables if needed
# ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "HackerNewsApi.dll"]