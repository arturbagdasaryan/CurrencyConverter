# Currency Converter API

This project offers a RESTful API service for converting currencies, fetching the latest exchange rates, and retrieving historical exchange rate data. The API supports authentication, rate limiting, caching, resilience, and provides conversion functionality excluding certain currencies.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- External dependency: [Frankfurter API](https://www.frankfurter.app/docs/)

## Features

- **Currency Conversion**: Convert amounts between various currencies, excluding specific currencies (TRY, PLN, THB, MXN).
- **Latest Exchange Rates**: Retrieve the latest exchange rates for a given base currency.
- **Historical Exchange Rates**: Fetch historical exchange rates for a specified date range with pagination support.
- **JWT Authentication**: Secure API access using JSON Web Tokens (JWT) with role-based access control (RBAC).
- **Rate Limiting**: Prevent abuse by enforcing API rate limits.
- **Logging & Monitoring**: Structured logging with Serilog and distributed tracing via OpenTelemetry for easy debugging and performance tracking.
- **Resilience**: Implements Polly for retry policies, circuit breakers.
- **Secure Login**: Provides a login endpoint to authenticate users and issue JWT tokens.

## Technology Stack

- **Backend Framework**: ASP.NET Core
- **Programming Language**: C#
- **Authentication**: JWT (JSON Web Token) with RBAC
- **API Rate Limiting**: ASP.NET Core Rate Limiting
- **Caching**: In-memory Caching (MemoryCache)
- **Resilience**: Polly (Retry, Circuit Breaker)
- **Testing**: (Unit & Integration Testing)
- **CI/CD**: GitHub Actions, Docker
- **Version Control**: Git, GitHub
- **Deployment**: Docker (Azure/AWS optional)


### Steps to Run Locally

1. **Clone the repository**

   ```bash
   git clone https://github.com/arturbagdasaryan/CurrencyConverter.git
   cd CurrencyConverter
   
2. **Restore the dependencies:**

   ```sh
   dotnet restore
   ```

3. **Build the project:**

   ```sh
   dotnet build
   ```

4. **Run the application:**

   ```sh
   dotnet run
   ```
## Endpoints

### 1. **Retrieve Latest Exchange Rates**
   - **Endpoint**: `/api/v1/rates/latest`
   - **Method**: `GET`
   - **Description**: Fetches the latest exchange rates for a specified base currency.
   - **Query Parameters**:
     - `baseCurrency` (required): The base currency for which exchange rates are to be retrieved (e.g., EUR, USD).
   - **Authentication**: Requires JWT token for access.
   - **Response**:
     - `200 OK`: Returns the latest exchange rates for the specified base currency.
     - `400 Bad Request`: Invalid or unsupported base currency.

### 2. **Currency Conversion**
   - **Endpoint**: `/api/v1/rates/convert`
   - **Method**: `POST`
   - **Description**: Converts an amount from one currency to another.
   - **Request Body**:
     ```json
     {
       "sourceCurrency": "USD",
       "targetCurrency": "EUR",
       "amount": 100
     }
     ```
   - **Authentication**: Requires JWT token with `Admin` role.
   - **Response**:
     - `200 OK`: Returns the converted amount.
     - `400 Bad Request`: If the source or target currency is one of the excluded currencies (e.g., TRY, PLN, THB, MXN).
     - `404 Not Found`: Invalid or unsupported currency.

### 3. **Retrieve Historical Exchange Rates**
   - **Endpoint**: `/api/v1/rates/historical`
   - **Method**: `GET`
   - **Description**: Retrieves historical exchange rates for a given date range with pagination.
   - **Query Parameters**:
     - `baseCurrency` (required): The base currency for the historical exchange rates (e.g., EUR).
     - `startDate` (required): The start date in `YYYY-MM-DD` format.
     - `endDate` (required): The end date in `YYYY-MM-DD` format.
     - `pageNumber` (optional): The page number for paginated results (default is 1).
     - `pageSize` (optional): The number of results per page (default is 10).
   - **Authentication**: Requires JWT token for access.
   - **Response**:
     - `200 OK`: Returns historical exchange rates for the specified date range, with pagination.
     - `400 Bad Request`: Invalid date format or unsupported base currency.
     - `404 Not Found`: No data available for the specified date range.

### 4. **User Login & Token Generation**
   - **Endpoint**: `/api/auth/login`
   - **Method**: `POST`
   - **Description**: Authenticates a user and returns a JWT token.
   - **Request Body**:
     ```json
     {
       "email": "admin@currencyapi.com",
       "password": "P@ssw0rd"
     }
     ```
   - **Response**:
     - `200 OK`: Returns a JWT token for the authenticated user.
     - `401 Unauthorized`: Invalid credentials.

## Troubleshooting
If you run into any issues, consider the following:
- Confirm that the .NET 8.0 SDK is installed and properly configured on your system.
- Verify that your internet connection is stable for connecting to the Frankfurter API.

