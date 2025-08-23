using LIT.Travelnize.API.Endpoints;
using LIT.Travelnize.ServiceDefaults;
using LIT.Travelnize.Infrastructure;
using LIT.Travelnize.Domain;
using LIT.Travelnize.UseCases;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddUseCasesServices();

var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterUsersEndpoints();

app.Run();
