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
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddUseCasesServices();

var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterAuthEndpoints();
app.RegisterUsersEndpoints();

app.InitialiseDatabaseAsync().GetAwaiter().GetResult();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.Run();
