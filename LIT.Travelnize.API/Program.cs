using LIT.Travelnize.Api.External.Wikipedia;
using LIT.Travelnize.API.Endpoints;
using LIT.Travelnize.Domain;
using LIT.Travelnize.Infrastructure;
using LIT.Travelnize.Infrastructure.Persistence;
using LIT.Travelnize.ServiceDefaults;
using LIT.Travelnize.UseCases;
using Microsoft.AspNetCore.Routing.Constraints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.AddServiceDefaults();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddUseCasesServices();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IWikipediaClient, WikipediaClient>();

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("CorsPolicy");

// Alle API-Endpunkte unter /api
var api = app.MapGroup("/api");
app.MapHomeEndpoints();
api.MapAuthEndpoints();
api.MapTripsEndpoints();
api.MapWikipediaEndpoints();
app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.InitialiseDatabaseAsync().GetAwaiter().GetResult();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

await app.RunAsync();
