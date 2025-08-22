using LIT.Travelnize.API.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
var app = builder.Build();
app.MapDefaultEndpoints();
app.RegisterUsersEndpoints();

app.Run();
