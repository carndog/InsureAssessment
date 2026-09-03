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

The solution includes HTTP flow files in the `HttpFlows` directory for testing the API endpoints. These files use JetBrains Rider's built-in HTTP Client.

### Flow Dependencies

Some flows depend on variables set by previous flows. Run them in this order:

1. **customer-management.http** - Creates and retrieves customers
2. **address-management.http** - Creates and retrieves addresses
3. **sell-household-policy.http** - Creates a customer, address, and sells a household policy (sets `policyReference`)
4. **sell-buytolet-policy.http** - Creates a customer, address, and sells a buy-to-let policy (sets `policyReference`)
5. **policy-management.http** - Retrieves policy details, payments, and refunds (requires `policyReference`)
6. **policy-cancellation.http** - Calculates cancellation quotes and cancels policies (requires `policyReference`)
7. **policy-renewal.http** - Renews policies independently using the development policy `POL-RENEW-DEMO`

### Running HTTP Flows

1. Keep the API running using the HTTP launch profile
2. Open an HTTP flow file in Rider
3. Click the run icon next to each request to execute it
4. Variables are automatically extracted from responses and used in subsequent requests

### Flow Sequence

- Run either selling flow (sell-household-policy.http or sell-buytolet-policy.http) to create a policy and set the `policyReference` variable
- Run policy management or cancellation using the generated `policyReference`
- Run renewal independently against the development policy `POL-RENEW-DEMO` (does not depend on a selling flow)

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
