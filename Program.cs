using System.Text.Json.Serialization;
using InsuranceDomain;
using InsuranceDomain.DataLayer;
using InsuranceDomain.Services;
using InsureApi.Middleware;

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

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<SellPolicyService>();
builder.Services.AddScoped<RetrievePolicyService>();
builder.Services.AddScoped<CalculateCancellationCostService>();
builder.Services.AddScoped<CancelPolicyService>();
builder.Services.AddScoped<RenewPolicyService>();

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
    DataSeeder dataSeeder = new DataSeeder(store);
    dataSeeder.SeedDevelopmentData(timeProvider);
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();