# BackgroundJobs101

This project demonstrates the usage of different types of hosted services in an ASP.NET Core application. It includes examples of timed hosted services, scoped processing services, and queue processing using channels.

## Project Structure

- **API/Program.cs**: Configures the web application and registers the hosted services.
- **API/BackgroundProcessors/HostedService.cs**: A timed hosted service that logs a message at regular intervals.
- **API/BackgroundProcessors/ScopedProcessingService.cs**: A scoped processing service that performs work in a loop until cancellation.
- **API/BackgroundProcessors/ConsumeScopedServiceHostedService.cs**: A background service that consumes the scoped processing service.
- **API/BackgroundProcessors/QueueProcessor.cs**: A background service that processes messages from a channel.

## How to Run

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/backgroundJobs101.git
    cd backgroundJobs101/src/API
    ```

2. Build and run the application:
    ```sh
    dotnet build
    dotnet run
    ```

3. Use the following endpoint to push messages to the queue:
    ```sh
    GET /push
    ```

## Services

### HostedService

A simple timed hosted service that logs a message every 5 seconds.

### ScopedProcessingService

A scoped service that performs work in a loop until the cancellation token is triggered.

### ConsumeScopedServiceHostedService

A background service that creates a scope and consumes the `ScopedProcessingService`.

### QueueProcessor

A background service that reads messages from a channel and processes them.

## License

This project is licensed under the MIT License.
