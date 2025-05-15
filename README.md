# HackerNewsApi

## Description

HackerNewsApi is a .NET 8 Web API designed to retrieve top stories from Hacker News. It includes features like caching, rate limiting using Polly, request logging middleware, and interactive API documentation via Swagger UI.

## Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8 SDK**: Required for local development and testing. Download it from [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/8.0).
- **Docker**: Required for containerized deployment. Download it from [Docker's website](https://www.docker.com/get-started).

## Running the Application

### Using Docker (Recommended)

1. **Build the Docker Image**  
   From the root directory of the project, run:
```sh
	docker build -t hackernewsapi .
```
2. **Run the Docker Container**  
   After building the image, run the container:
```sh
	docker run -p 8080:80 hackernewsapi
```
3. **Access the API**  
   Open your browser and navigate to:

   http://localhost:8080/swagger

This will open the Swagger UI for testing the API.

### Running Locally

1. **Install .NET 8 SDK**  
   Ensure the .NET 8 SDK is installed on your machine.

2. **Restore Dependencies**  
   Navigate to the project directory and run:
```sh
   dotnet restore
   dotnet build
```
3. **Run the Application**  
   Start the application by running:
```sh
   dotnet run --project HackerNewsApi
```
4. **Access the API**  
   By default, the API will be available at:

   http://localhost:5000/swagger

## Running Tests

Tests are automatically executed during the Docker build process. To run tests manually:

1. Navigate to the `HackerNewsTests` directory:
```sh
	cd HackerNewsTests
```
2. Run the tests:
```sh
	dotnet test
```
## Notes

- The Docker build process includes running tests. If any test fails, the build will stop.
- Swagger UI is available in both Docker and local environments for easy API testing.
- Rate limiting is implemented using Polly to handle API request limits gracefully.

---

## Suggestions for Improvement

Here are some ideas to further enhance the HackerNewsApi project:

1. **Implement Integration Tests**
   - Add integration tests to verify the behavior of the application when interacting with external APIs (e.g., Hacker News API).
   - Use a mock server to simulate external API responses.
   
2. **Add Database Support**
   - Store fetched stories in a database (e.g., PostgreSQL) for historical analysis or offline access.
   - Use Entity Framework Core for database interactions.

3. **Add a Sync Service**
   - Add a `HackerNewsSync` service that periodically (e.g., once per hour) fetches stories from the Hacker News API and stores them in the database.
   - This will reduce the load on the external API and improve response times for clients.

4. **Modernize Solution Structure**
   - Refactor the solution to improve code organization and readability.
   - Separate the code into projects that correspond to its architectural layers (e.g., `HackerNewsApi.Core`, `HackerNewsApi.Infrastructure`, `HackerNewsApi.Application`, etc.).
   - Follow clean architecture principles to ensure better maintainability and scalability.

5. **Add Distributed Caching**
   - Replace the in-memory cache with a distributed caching solution like Redis to support scaling across multiple instances.
   
6. **Containerize with Docker Compose**
   - Create a `docker-compose.yml` file to orchestrate multiple services (e.g., API, Redis for caching).
   - Simplify the setup for local development and testing.

7. **Add Rate Limiting at the API Gateway**
   - While Polly handles rate limiting for outgoing requests, consider adding rate limiting for incoming requests.

8. **Optimize Performance**
   - Profile the application using tools like `dotnet-trace` or `dotnet-counters` to identify bottlenecks.

9. **Add Metrics and Monitoring**
   - Expose metrics using Prometheus and visualize them in Grafana.
   - Monitor API performance, request counts, and error rates.
   
9. **Add Unit Tests for Edge Cases**
   - Expand the test coverage to include edge cases, such as handling empty responses, invalid data, or API timeouts.
   - Use tools to measure code coverage and ensure critical paths are tested.

10. **Improve Error Handling**
   - Create a global exception handler middleware to standardize error responses.
   - Log detailed error information while returning user-friendly error messages.


By implementing these improvements, the project can become more robust, scalable, and production-ready.
