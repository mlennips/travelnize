using LIT.Travelnize.API.Endpoints;
using LIT.Travelnize.ServiceDefaults;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterUsersEndpoints();

app.Run();
