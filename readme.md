1.  **Prerequisites**: Ensure you have the [.NET 8 sdk] (check version using `dotnet --version`).
2.  **Clone the repository**: `git clone https://github.com/SdSadat/WorkflowEngine`
3.  **Navigate to the project directory**: `cd Infonetica.WorkflowEngine`
4.  **Run the application**:
    ```bash
    dotnet run
    ```
5.  The service will start and listen on the HTTPS port (e.g `https://localhost:5020`).
6.  A Swagger UI will be available at `/swagger` (`https://localhost:5020/swagger`)
7.  The Default Environment is setup to `Development` This can be changed in `launchSettings.json`