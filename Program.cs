using System.Text.Json.Serialization;
using InsuranceDomain;
using InsuranceDomain.Services;

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

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();