var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("database")
    .WithPgAdmin()
    .WithDataVolume();

var databaseTravelnize = database.AddDatabase("travelnize");
var databaseIdentity = database.AddDatabase("identity");

var api = builder.AddProject<Projects.LIT_Travelnize_API>("backend")
    .WithReference(databaseTravelnize)
    .WithReference(databaseIdentity)
    .WaitFor(databaseTravelnize)
    .WaitFor(databaseIdentity);

builder.AddProject<Projects.LIT_Travelnize>("frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
