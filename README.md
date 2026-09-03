# InsureApi

A .NET 10 ASP.NET Core Web API implementing an insurance domain with household and buy-to-let policy management.

## Prerequisites

- .NET 10 SDK
- JetBrains Rider or another .NET-compatible IDE

## Building the Solution

```bash
dotnet restore InsureApi.sln
dotnet build InsureApi.sln
```

## Running the API

```bash
dotnet run --project InsureApi.csproj --launch-profile http
```

The API will be available at `http://localhost:5078`

## Running Tests

```bash
dotnet test InsureApi.sln
```

## Using the HTTP Flows

The solution includes independent HTTP scenario files in the `HttpFlows` directory for testing the API endpoints. These files use JetBrains Rider's built-in HTTP Client.

### Running HTTP Scenarios

1. Start the API with:
   ```bash
   dotnet run --project InsureApi.csproj --launch-profile http
   ```
2. Open one scenario file in Rider
3. Use "Run All Requests in File" to execute the complete scenario
4. Each scenario is independent and contains all prerequisite requests

### Scenario Files

- **01-household-sale-and-retrieval.http** - Creates a customer, address, sells a household policy, and retrieves related data
- **02-buytolet-sale-and-cancellation.http** - Creates a customer, address, sells a buy-to-let policy, calculates cancellation, and cancels
- **03-policy-renewal.http** - Renews the development-seeded policy (POL-RENEW-DEMO)
- **04-informative-errors.http** - Demonstrates error responses for invalid requests

### Important Notes

- Restart the API before re-running the renewal scenario because persistence is deliberately in memory
- The cancellation scenario creates its own policy and should be run from the beginning when repeated
- Each scenario is self-contained and does not depend on variables from other files

## API Endpoints

### Customer Management

- `POST /api/customers` - Create a new customer
- `GET /api/customers/{customerId}` - Retrieve a customer
- `GET /api/customers/{customerId}/policies` - Retrieve a customer's policies

### Address Management

- `POST /api/addresses` - Create a new address
- `GET /api/addresses/{addressId}` - Retrieve an address

### Policy Sales

- `POST /api/policies/households` - Sell a household policy
- `POST /api/policies/buytolet` - Sell a buy-to-let policy

### Policy Management

- `GET /api/policies/{uniqueReference}` - Retrieve a policy
- `GET /api/policies/{uniqueReference}/payments` - Retrieve payments for a policy
- `GET /api/policies/{uniqueReference}/refunds` - Retrieve refunds for a policy

### Policy Administration

- `POST /api/policies/{uniqueReference}/cancellation-quotes` - Calculate cancellation cost
- `PUT /api/policies/{uniqueReference}/cancellation` - Cancel a policy
- `POST /api/policies/{uniqueReference}/renewals` - Renew a policy

## Use Cases

### Selling a Policy

1. Create a customer via `POST /api/customers`
2. Create an address via `POST /api/addresses`
3. Sell a household or buy-to-let policy via `POST /api/policies/households` or `POST /api/policies/buytolet`

### Retrieving Policy Information

- Get policy details via `GET /api/policies/{uniqueReference}`
- View payments via `GET /api/policies/{uniqueReference}/payments`
- View refunds via `GET /api/policies/{uniqueReference}/refunds`

### Cancelling a Policy

1. Calculate cancellation cost via `POST /api/policies/{uniqueReference}/cancellation-quotes`
2. Cancel the policy via `PUT /api/policies/{uniqueReference}/cancellation`

### Renewing a Policy

- Renew a policy via `POST /api/policies/{uniqueReference}/renewals`

## Domain Rules

- Policies must be exactly one year in length
- Policies cannot start in the past or more than 60 days in advance
- All customers on a policy must be over 16 on the policy start date
- A policy must have between 1 and 3 customers
- Cancellation during the 14-day cooling-off period produces a full refund
- Cancellation after the cooling-off period produces a pro-rata refund
- Policies can only be renewed within 30 days of their end date
- Renewed policies must be exactly one year in length

## Architecture

- **InsuranceDomain** - Domain entities, aggregates, and use-case services
- **InsureApi** - Web API with controllers, contracts, and mapping
- **InsureApi.Tests** - xUnit tests for domain rules and services

The implementation follows domain-driven design principles with:
- In-memory store (no database)
- No repositoryPattern
- No AutoMapper
- No MediatR
- No authentication or messaging
