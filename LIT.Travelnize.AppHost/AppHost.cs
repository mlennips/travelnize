var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("database")
    .WithPgAdmin()
    .WithDataVolume()
    .AddDatabase("travelnize");

var api = builder.AddProject<Projects.LIT_Travelnize_API>("backend")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.LIT_Travelnize>("frontend")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
