using System.Text.Json.Serialization;
using InsuranceDomain;
using InsuranceDomain.Services;
using InsureApi.Middleware;
using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<InsuranceStore>();

builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<AddressService>();
builder.Services.AddSingleton<SellPolicyService>();
builder.Services.AddSingleton<RetrievePolicyService>();
builder.Services.AddSingleton<CalculateCancellationCostService>();
builder.Services.AddSingleton<CancelPolicyService>();
builder.Services.AddSingleton<RenewPolicyService>();

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    InsuranceStore store = app.Services.GetRequiredService<InsuranceStore>();
    TimeProvider timeProvider = app.Services.GetRequiredService<TimeProvider>();
    store.SeedDevelopmentData(timeProvider);
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();