# ScrapyardApp

ScrapyardApp is a web application built with ASP.NET Core Razor Pages and .NET 8. It provides management features for a scrapyard business, including inventory, customer management, sales, purchases, and audit logging.

## Features

- **Inventory Management:** Track scrap items, categories, and price history.
- **Customer Management:** Store customer details and manage transactions.
- **Sales & Purchases:** Record sales and purchases with full audit logging.
- **User Authentication:** Secure login and user management using ASP.NET Core Identity.
- **Responsive UI:** Built with Razor Pages for fast and modern web experiences.

## Technologies Used

- ASP.NET Core Razor Pages (.NET 8)
- Entity Framework Core (SQL Server)
- ASP.NET Core Identity
- C#

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote)

### Setup

1. **Clone the repository:**

2. **Configure the database:**
   - Update the connection string `DbConn` in `appsettings.json` to point to your SQL Server instance.

3. **Apply migrations:**
   - Run the following command in the terminal:
	 ```bash
	 dotnet ef database update
	 ```
4. **Run the application:**
5. **Access the app:**
   - Open your browser and navigate to `https://localhost:5001` (or the port shown in the console).

## Project Structure

- `Data/` - Entity Framework Core DbContext and data models
- `Models/` - Application domain models
- `Pages/` - Razor Pages for UI
- `Views/` - Shared views and tag helpers
- `Program.cs` - Application startup and configuration

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License.