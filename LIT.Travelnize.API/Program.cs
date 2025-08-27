using LIT.Travelnize.API.Endpoints;
using LIT.Travelnize.Domain;
using LIT.Travelnize.Infrastructure;
using LIT.Travelnize.Infrastructure.Persistence;
using LIT.Travelnize.ServiceDefaults;
using LIT.Travelnize.UseCases;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateSlimBuilder(args); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.AddServiceDefaults();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddUseCasesServices();

var app = builder.Build();

app.MapDefaultEndpoints();
app.RegisterAuthEndpoints();
app.RegisterUsersEndpoints();

//app.UseCors("AllowLocalhost");
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

app.Run();
